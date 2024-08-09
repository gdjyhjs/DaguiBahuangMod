using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Cave.Patch;
using Cave;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.IO;

namespace Cave.BuildFunction
{
    /*
    采阴： 随机采集当前大境界1% - 3% 修为。 不足时下降一个境界。 
    50% 精力 - 1
    每相差一个大境界影响10%减少精力概率。
    仇恨 + 6-9
    精力为0强行退出。

    收服：征服度随机 + 1%-3%  满足进度随机+6%-9%
    50% 精力 - 1
    每相差一个大境界影响10%减少精力概率。
    亲密度 + 6-9
    精力为0强行退出。  亲密度-100    征服度-50*/
    // 地牢
    public class DungeonBuild : ClassBase
    {
        const float addMinExpRate = 1; // 增加修为百分比最小值
        const float addMaxExpRate = 3; // 增加修为百分比最大值
        const float cutEnergyRate = 25; // 减少精力概率
        const float mofEnergy = 10; // 差一个大境界影响的减少精力概率
        const float addHateMin = 6; // 增加仇恨最小值
        const float addHateMax = 9; // 增加仇恨最大值

        const float addConquerMin = 1; // 增加征服度最小值
        const float addConquerMax = 3; // 增加征服度最大值
        const float addSatisfyMin = 6; // 增加满足最小值
        const float addSatisfyMax = 9; // 增加满足度最大值
        const float addLoveMin = 6; // 增加亲密最小值
        const float addLoveMax = 9; // 增加亲密最大值
        const float extCutLove = 100; // 强行退出降低好感值
        const float extCutConquer = 50; // 强行退出降低征服值
        const float extAddLove = 100; // 满足好感值
        const float extAddConquer = 25; // 满足增加征服值

        const float actionCdMin = 3.5f; // 采阴/收服触发间隔最小值
        const float actionCdMax = 6.5f; // 采阴/收服触发间隔最大值

        class DialogueData
        {
            public string[] loverDialogue = new string[0];
            public string[] runDialogue = new string[0];
            public string[] loveDialogue1 = new string[0];
            public string[] loveDialogue2 = new string[0];
            public string[] loveDialogue3 = new string[0];
            public string[] loveDialogue1End = new string[0];
            public string[] loveDialogue2End = new string[0];
            public string[] loveDialogue3End = new string[0];
        }
        DialogueData dialogueData;

        public const int prisonerLuckId = 123010217; // 囚犯气运ID

        List<UnitCtrlHuman> units = new List<UnitCtrlHuman>(); // 被关押的单位
        UIBattleInfo uiBase;
        DataDungeon data;
        UnitCtrlHuman putUnit; // 抬起的单位
        GameObject unitOperateUI;
        TimerCoroutine unitOperateCor;

        private Vector2 playerBronPoint = new Vector2(2.6f, 9.2f); // 玩家出生点
        private Vector2 tranPoint = new Vector2(1.0f, 9.2f); // 传送阵点
        private DecorateMgr decorateMgr = new DecorateMgr();

        int operate = 0; // 1:布置摆件 2:移除摆件 0:无操作
        RectTransform listObj;
        float defVerticalNormalizedPosition = 1;
        List<int> allBarrierID; // 所有可创建的障碍物id
        GameObject barrierPrefab; // 要创建的障碍物预制件
        int barrierCreateID; // 要创建的障碍物ID
        GameObject clickDestroyBarrierb; // 点击将删除的障碍物
        GameObject destroyBarrierb; // 正准备删除的障碍物
        bool isEnterPanel; // 鼠标是否划入面板
        float createNextTime = 0; // 下一个装饰可创建时间

        float makeNextTime = 0; // 采阴收服cd
        int makeState = 0; // 1：采阴    2：收服    0：退出
        float makeSatisfy = 0; // 满足进度
        Vector2 makePosi;
        bool makeDire;
        int makeAnimID = 1; // 播放的动画ID
        float nearCameraSize = 2; // 拉近镜头的大小
        CameraCtrl.OrthoSizeData curSize;
        Il2CppSystem.Collections.Generic.List<CameraCtrl.OrthoSizeData> defSize = new Il2CppSystem.Collections.Generic.List<CameraCtrl.OrthoSizeData>();

        GameObject sliderBg;
        Image slider1;
        Image slider2;

        // 初始化地牢
        public override void Init(string param)
        {
            allBarrierID = new List<int>();
            foreach (var item in g.conf.dungeonSceneObject._allConfList)
            {
                if (!DecorateMgr.ignoreBarrierID.Contains(item.id))
                {
                    var list = CommonTool.StrSplitInt(item.decorationID, '|');
                    foreach (var id in list)
                    {
                        if (!allBarrierID.Contains(id))
                        {
                            allBarrierID.Add(id);
                        }
                    }
                }
            }


            MainCave.Close();
            DataCave dataCave = DataCave.ReadData();
            data = DataDungeon.ReadData();

            Action<ETypeData> onBattleStart = OnBattleStart;
            Action<ETypeData> onBattleEnd = OnBattleEnd;

            // 监听战斗开始一次
            g.events.On(EBattleType.BattleStart, onBattleStart, 1);
            // 监听战斗结束
            g.events.On(EBattleType.BattleEnd, onBattleEnd, 1);

            // 进入灵田副本
            g.world.battle.IntoBattle(new DataMap.MonstData() { id = 42685183, level = 1 });

            NewMethod();
        }

        private void NewMethod()
        {
            try
            {
                string dialogueStr;
                if (data.vip == 0)
                {
                    dialogueStr = File.ReadAllText(Cave.GetModDir() + "/DungeonDialogue.json");
                }
                else
                {
                    dialogueStr = File.ReadAllText(Cave.GetModDir() + "/DungeonDialogue_vip.json");
                }
                dialogueData = JsonConvert.DeserializeObject<DialogueData>(dialogueStr);
            }
            catch (Exception e)
            {
                Cave.LogWarning("读取气泡内容错误：" + e.Message + "\n" + e.StackTrace);
                dialogueData = new DialogueData();
            }
        }

        List<UnitCtrlBase> allUnits = new List<UnitCtrlBase>(); // 所有的关押单位
        private void OnBattleStart(ETypeData e)
        {
            uiBase = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
            //初始化灵田副本

            Action onIntoRoomEnd = () => OnIntoRoomEnd();
            SceneType.battle.battleMap.onIntoRoomEnd += onIntoRoomEnd;

            Action onStart = () =>
            {
                SceneType.battle.timer.Frame(new Action(OnUpdate), 1, true);
            };
            SceneType.battle.battleMap.AddStartBattleCall(onStart);

            // 自动点击开始战斗按钮
            TimerCoroutine autoStartCor = null;
            Action action = () =>
            {
                uiBase.uiStartBattle.btnStart.onClick.Invoke();
                if (!uiBase.uiStartBattle.goGroupRoot.activeSelf)
                {
                    SceneType.battle.timer.Stop(autoStartCor);
                    InitFarmUI();
                }
            };
            autoStartCor = SceneType.battle.timer.Frame(action, 1, true);

            InitDungeon();
        }

        bool clickF;
        bool clickG;

        // 初始化副本
        private void InitDungeon()
        {
            try
            {
                var fGo = CreateUI.NewButton(() =>
                {
                    clickF = true;
                });
                UITool.SetUILeft(g.ui.GetUI(UIType.BattleInfo), fGo.GetComponent<RectTransform>(), -20, "购买装饰(F)");

                var gGo = CreateUI.NewButton(() =>
                {
                    clickG = true;
                });
                UITool.SetUILeft(g.ui.GetUI(UIType.BattleInfo), gGo.GetComponent<RectTransform>(), -80, "拆除装饰(G)");
            }
            catch (Exception e)
            {
                Console.WriteLine("初始化按钮错误 " + e.ToString());
            }

            Action battleFrame = () =>
            {
                uiBase.startBattle.btnStart.onClick.Invoke();

                allUnits.Clear();

                Il2CppSystem.Collections.Generic.List<UnitCtrlBase> clearUnits = SceneType.battle.unit.GetAllUnit(true);
                foreach (UnitCtrlBase item in clearUnits)
                {
                    if (item.TryCast<UnitCtrlPlayer>() == null && !item.isDie && !item.isDestroy && item.gameObject != null)
                    {
                        item.Die(false);
                    }
                }

                //uiBase.info.goGroupRoot.SetActive(false);
                uiBase.info.goInfoRoot.SetActive(false);
                uiBase.info.goMonst.SetActive(false);
                uiBase.info.goHPEffect.SetActive(false);
                uiBase.info.goHitHPEffect.SetActive(false);
                uiBase.info.goUnitHPRoot.SetActive(false);
                uiBase.info.goUnitFocoRoot.SetActive(false);


                // 创建被关押的人
                int idx = 0;
                int line = 0;
                Vector2 startPos = playerBronPoint + new Vector2(2, 0);
                Vector2 pos = startPos;
                foreach (var unitId in data.units)
                {
                    WorldUnitBase unit = g.world.unit.GetUnit(unitId, true);
                    if (unit != null)
                    {
                        try
                        {
                            UnitCtrlHumanNPC humanNpc = SceneType.battle.unit.CreateUnitHuman<UnitCtrlHumanNPC>(unit.data, UnitType.PlayerNPC);
                            humanNpc.move.SetDire(true);

                            units.Add(humanNpc);
                            humanNpc.data.hp = humanNpc.data.maxHP.value;
                            BattleDialogueCtrl dialogueCtrl = UnityAPIEx.GetComponentOrAdd<BattleDialogueCtrl>(humanNpc.gameObject);
                            Action onCall = () =>
                            {
                                if (putUnit == null)
                                {
                                    humanNpc.move.SetDire(humanNpc.move.lastPosi.x > SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x);
                                    OperateUnit(humanNpc);
                                }
                            };
                            string key = g.conf.gameKeyCode.GetKey(g.data.globle.key.battleDialogue);
                            string topTips = "按" + key + "键进行操作";
                            dialogueCtrl.InitData(humanNpc.move.lastPosi, onCall, topTips, true, false);
                            dialogueCtrl.dialogueDis = 1.5f;

                            ClearPlayerEffects(humanNpc);
                            SceneType.battle.timer.Time(new Action(() => ClearPlayerEffects(humanNpc)), 1);

                            Vector2 getTipPosi()
                            {
                                return humanNpc.move.lastPosi;
                            }
                            dialogueCtrl.setTipPosiCall = (ReturnAction<Vector2>)getTipPosi;

                            bool isDialogue()
                            {
                                return operate == 0 && putUnit == null && curSize == null;
                            }
                            dialogueCtrl.isDialogueCall = (ReturnAction<bool>)isDialogue;

                            if (data.unitPosi.ContainsKey(unit.data.unitData.unitID))
                            {
                                var p = data.unitPosi[unit.data.unitData.unitID];
                                humanNpc.move.SetPosition(new Vector2(p[0], p[1]));
                                humanNpc.move.SetDire(p[2] == 0);
                            }
                            else
                            {
                                humanNpc.move.SetPosition(pos);
                                idx++;
                                if (idx >= 10)
                                {
                                    idx = 0;
                                    line++;
                                }
                                pos = new Vector2(startPos.x + 2.5f * idx, startPos.y + Mathf.CeilToInt(line * 0.5f) * (line % 2 == 1 ? 1 : -1) * 2.5f);
                            }
                            PlayCaveAnim(humanNpc, unit.data.unitData.propertyData.sex == UnitSexType.Man ? "kneel" : "kneel2");
                        }
                        catch (Exception e)
                        {
                            Cave.LogWarning(e.Message + "" + e.StackTrace);
                        }
                    }
                }

                ClearPlayerEffects(SceneType.battle.battleMap.playerUnitCtrl);
                // 固定移动速度
                SceneType.battle.battleMap.playerUnitCtrl.data.moveSpeed.baseValue = 800;

                Debug.Log("orthoSizes=" + SceneType.battle.cameraCtrl.orthoSizes.Count);

                InitFarmUI();
            };
            uiBase.AddCor(SceneType.battle.timer.Frame(battleFrame, 2));
        }

        private void CloseUnitOperate() // 关闭函数
        {
            GameObject.Destroy(unitOperateUI);
            unitOperateUI = null;
            SceneType.battle.timer.Stop(unitOperateCor);
        }

        /*
         * 采补（高境界限定）
        降低对方一个境界，给自己加满本小境界的经验
        记得删除npc的陷入瓶颈气运，以及npc当前境界已有的经验
        */
        // 弹出对单位的操作按钮选项： 查看、抱起、释放、采阴、处决
        private void OperateUnit(UnitCtrlHumanNPC humanNpc)
        {
            var scenePos = humanNpc.move.lastPosi;
            var uiPos = g.ui.uiCamera.ScreenToWorldPoint(SceneType.battle.camera.WorldToScreenPoint(scenePos));

            Action CloseUnitOperate = () => // 关闭函数
            {
                GameObject.Destroy(unitOperateUI);
                unitOperateUI = null;
                SceneType.battle.timer.Stop(unitOperateCor);
            };

            if (this.unitOperateUI != null)
            {
                CloseUnitOperate();
            }

            unitOperateCor = SceneType.battle.timer.Frame(new Action(() =>
            {
                if (unitOperateUI == null)
                {
                    SceneType.battle.timer.Stop(unitOperateCor);
                }
                else if (Vector3.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, humanNpc.move.lastPosi) > 2)
                {
                    CloseUnitOperate();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CloseUnitOperate();
                }
            }), 1, true);

            unitOperateUI = CreateUI.NewImage(SpriteTool.GetSprite("Common", "xiangqingguanxibg")); // 背景图
            unitOperateUI.GetComponent<Image>().type = Image.Type.Tiled;
            unitOperateUI.transform.SetParent(uiBase.transform, false);
            if (scenePos.x < 30)
            {
                unitOperateUI.GetComponent<RectTransform>().anchoredPosition = uiPos;
            }
            else
            {
                unitOperateUI.GetComponent<RectTransform>().anchoredPosition = uiPos;
            }
            var grid = unitOperateUI.AddComponent<GridLayoutGroup>();
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.cellSize = new Vector2(100, 50);
            grid.spacing = new Vector2(5, 5);

            GameObject tmpBtn, tmpText;
            // 查看
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 查看按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    var unitId = humanNpc.data.worldUnitData.unitData.unitID;
                    var unit = g.world.unit.GetUnit(unitId, true);
                    if (unit != null)
                    {
                        g.ui.OpenUI<UINPCInfo>(UIType.NPCInfo).InitData(unit, false);
                    }
                    CloseUnitOperate();
                }));

                tmpText = CreateUI.NewText("查看", new Vector2(80, 30)); // 查看按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }
            // 抬起
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 抬起按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    putUnit = humanNpc;
                    CloseUnitOperate();
                }));

                tmpText = CreateUI.NewText("抱起", new Vector2(80, 30)); // 抬起按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }
            // 释放
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 抬起按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    PlayDefAnim(humanNpc);
                    var unitId = humanNpc.data.worldUnitData.unitData.unitID;
                    new DramaFunction().ReleaseUnit(humanNpc, tranPoint);
                    GameObject.Destroy(humanNpc.gameObject.GetComponent<BattleDialogueCtrl>());
                    units.Remove(humanNpc);
                    data.units.Remove(unitId);
                    data.unitPosi.Remove(unitId);
                    data.unitConquer.Remove(unitId);
                    var unit = g.world.unit.GetUnit(unitId, true);
                    if (unit != null)
                    {
                        unit.CreateAction(new UnitActionLuckDel(prisonerLuckId));
                    }
                    CloseUnitOperate();
                    if (dialogueData.runDialogue.Length > 0)
                        Dialogue(humanNpc, dialogueData.runDialogue[CommonTool.Random(0, dialogueData.runDialogue.Length)]);
                }));

                tmpText = CreateUI.NewText("释放", new Vector2(80, 30)); // 抬起按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }
            // 收服
            if (humanNpc.data.worldUnitData.unitData.propertyData.sex != g.world.playerUnit.data.unitData.propertyData.sex)
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 抬起按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    MakeLove(humanNpc, CloseUnitOperate, 2);
                }));

                tmpText = CreateUI.NewText("调教", new Vector2(80, 30)); // 抬起按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }
            // 采阴
            if (humanNpc.data.worldUnitData.unitData.propertyData.sex != g.world.playerUnit.data.unitData.propertyData.sex)
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 抬起按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    MakeLove(humanNpc, CloseUnitOperate, 1);
                }));

                tmpText = CreateUI.NewText(g.world.playerUnit.data.unitData.propertyData.sex == UnitSexType.Man ? "采阴" : "采阳", new Vector2(80, 30)); // 抬起按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

            }
            // 处决
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 抬起按钮底图
                tmpBtn.transform.SetParent(unitOperateUI.transform, false);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    // 处决
                    PlayDefAnim(humanNpc);

                    var unitId = humanNpc.data.worldUnitData.unitData.unitID;
                    GameObject.Destroy(humanNpc.gameObject.GetComponent<BattleDialogueCtrl>());
                    units.Remove(humanNpc);
                    data.units.Remove(unitId);
                    data.unitPosi.Remove(unitId);
                    data.unitConquer.Remove(unitId);

                    var unit = g.world.unit.GetUnit(unitId, true);
                    if (unit != null)
                    {
                        g.conf.dramaDialogue.GetItem(1009037).nextDialogue = "0";
                        UICustomDramaDyn dramaDyn = new UICustomDramaDyn(1009037);
                        dramaDyn.dramaData.unitLeft = g.world.playerUnit;
                        dramaDyn.dramaData.unitRight = unit;
                        dramaDyn.dramaData.dialogueText[1009037] = "你竟然要杀我。。";
                        dramaDyn.dramaData.onDramaEndCall = new Action(() =>
                        {
                            // 爆出所有道具
                            var props = unit.data.unitData.propData.CloneAllProps();
                            foreach (var item in props)
                            {
                                unit.data.CostPropItem(item.propsID, item.propsCount);
                            }
                            uiBase.uiDrop.AddDrop(props, humanNpc.move.lastPosi);
                            // 真正死亡
                            humanNpc.Die();
                            unit.CreateAction(new UnitActionLuckDel(prisonerLuckId));
                            unit.CreateAction(new UnitActionRoleDie(unit));
                        });
                        dramaDyn.OpenUI();
                    }
                    else
                    {
                        humanNpc.Die();
                    }

                    CloseUnitOperate();
                }));

                tmpText = CreateUI.NewText("处决", new Vector2(80, 30)); // 抬起按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }



















            unitOperateUI.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            var csf = unitOperateUI.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // 弹出气泡
        private void Dialogue(UnitCtrlHuman humanNpc, string str)
        {
            uiBase.uiInfo.UnitDialogue(humanNpc, str, 5);
        }

        // 进入特殊动作 toMakeState：1采阴  2调教
        private void MakeLove(UnitCtrlHumanNPC humanNpc, Action CloseUnitOperate, int toMakeState)
        {
            if (!CheckEnergy())
            {
                UITipItem.AddTip("精力不足！");
                return;
            }
            try
            {
                InitUnitMakeLove();
            }
            catch (Exception e)
            {
                Cave.LogWarning("InitUnitMakeLove " + e.Message + " " + e.StackTrace);
            }

            CloseUnitOperate();

            try
            {
                InitMakeLoveAnim();
            }
            catch (Exception e)
            {
                Cave.LogWarning("InitMakeLoveAnim " + e.Message + " " + e.StackTrace);
            }

            sliderBg = CreateUI.New();
            sliderBg.transform.SetParent(uiBase.transform, false);
            sliderBg.transform.position = g.ui.uiCamera.ScreenToWorldPoint(SceneType.battle.camera.WorldToScreenPoint(makePosi));
            if (toMakeState == 2)
            {
                var go1 = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/CaveJinduBg"));
                go1.transform.SetParent(sliderBg.transform, false);
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -35);
                go1.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);

                var go2 = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/CaveJindu"));
                go2.transform.SetParent(go1.transform, false);
                go2.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);
                slider1 = go2.GetComponent<Image>();
                slider1.type = Image.Type.Filled;
                slider1.fillMethod = Image.FillMethod.Horizontal;
                slider1.fillAmount = 0;
                if (toMakeState == 1)
                {
                    slider1.color = new Color(0f, 0.54f, 0.42f);
                    slider1.fillAmount = 1;
                }
                else if (toMakeState == 2)
                {
                    slider1.color = new Color(0.79f, 0.15f, 0.65f);
                }
            }
            if(toMakeState == 2)
            {
                var go1 = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/CaveJinduBg"));
                go1.transform.SetParent(sliderBg.transform, false);
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);
                go1.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);

                var go2 = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/CaveJindu"));
                go2.transform.SetParent(go1.transform, false);
                go2.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 20);
                slider2 = go2.GetComponent<Image>();
                slider2.type = Image.Type.Filled;
                slider2.fillMethod = Image.FillMethod.Horizontal;
                slider2.color = new Color(0.79f, 0.43f, 0.15f);
                slider2.fillAmount = 0;
            }

            void InitUnitMakeLove()
            {
                Cave.Log("InitUnitMakeLove");
                makePosi = humanNpc.move.lastPosi;
                makeDire = humanNpc.move.isLockDire;
                makeSatisfy = 0;
                defSize.Clear();
                foreach (var item in SceneType.battle.cameraCtrl.orthoSizes)
                {
                    defSize.Add(item);
                }
                SceneType.battle.cameraCtrl.orthoSizes.Clear();
                curSize = new CameraCtrl.OrthoSizeData() { orthoSize = nearCameraSize };
                SceneType.battle.cameraCtrl.orthoSizes.Add(curSize);
                DG.Tweening.ShortcutExtensions.DOKill(SceneType.battle.camera);
                var tween = DG.Tweening.ShortcutExtensions.DOOrthoSize(SceneType.battle.camera, SceneType.battle.cameraCtrl.orthoSize, 0.1f);
                DG.Tweening.TweenSettingsExtensions.OnUpdate(tween, new Action(() => SceneType.battle.cameraCtrl.UpdateHalfPosi()));
                makeState = toMakeState;
                putUnit = humanNpc;
            }

            void InitMakeLoveAnim()
            {
                SceneType.battle.battleMap.playerUnitCtrl.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                putUnit.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                makeAnimID = CommonTool.Random(1, 3);
                if(data.vip == 0)
                {
                    makeAnimID = 3;
                    PlayCaveAnim(humanNpc, (humanNpc.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                    PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                }
                else
                {
                    PlayCaveAnim(humanNpc, (humanNpc.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID);
                    PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID);
                }
            }
        }

        // 初始化弹教学剧情
        private void InitFarmUI()
        {
            if (data.open == 0)
            {
                Cave.OpenDrama("第一次来地牢吧！我来给你简单介绍一下地牢的使用方法。<color=red>注意我只说一次！</color>", new Action(() =>
                {
                    Cave.OpenDrama("战斗击败敌人后可关押至地牢，可E键进行操作，抱起后E键放下，↑↑↓↓←←→→切换模式，调教时E键换动作，右键退出。", new Action(() =>
                    {
                        Cave.OpenDrama("按F键可以打开装饰列表，选择装饰后可以点击装饰到合适的位置。按G键可进入回收模式，点击装饰可拆除回收。", new Action(() =>
                        {
                            Cave.OpenDrama("当然所有装饰都是要收费的，拆除时按照80%价格回收。");
                        }));
                    }));
                }));
            }
            data.open++;
        }

        // 清空玩家效果 
        private void ClearPlayerEffects(UnitCtrlHuman unit)
        {
            // 清空玩家状态
            unit.AddState(UnitStateType.NotUserAllSkill, -1);
            // 清除蛟龙等效果
            new SchoolBigFight.UnitEffectSpecHandler().Close(unit);
            // 固定移动速度
            unit.data.moveSpeed.baseValue = 80;
            // 清除僚机
            unit.WingmanEnable(false);
            // 清除战斗效果
            var list = unit.effects;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                unit.DelEffect(list[i]);
            }
            var player = unit.TryCast<UnitCtrlPlayer>();
            if (player != null)
            {
                player.eyeSkillBase = null;
                player.piscesSkillBase = null;
            }
        }



        int vipKeyIdx = 0;
        KeyCode[] keyCodes = new KeyCode[]
        {
            KeyCode.UpArrow,
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.RightArrow,
        };

        private void OnUpdate()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(keyCodes[vipKeyIdx]))
                {
                    vipKeyIdx++;
                    if(vipKeyIdx >= keyCodes.Length)
                    {
                        vipKeyIdx = 0;
                        if (data.vip == 0)
                        {
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "是否启动隐藏内容？", 2, new Action(() =>
                            {
                                data.vip = 1;
                            }));
                        }
                        else
                        {
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "是否关闭隐藏内容？", 2, new Action(() =>
                            {
                                data.vip = 0;
                            }));
                        }
                    }
                }
                else
                {
                    vipKeyIdx = 0;
                }
            }


            try
            {
                if (curSize != null)
                {
                    MakeAction();

                    return;
                }
                if (putUnit != null)
                {
                    if (Input.GetKeyDown(g.data.globle.key.battleDialogue))
                    {
                        putUnit.move.SetPosition(SceneType.battle.battleMap.playerUnitCtrl.posiDown.position);
                        putUnit = null;
                    }
                    else
                    {
                        putUnit.move.SetPosition(SceneType.battle.battleMap.playerUnitCtrl.posiCenter.position);
                    }
                    return;
                }
                DecorateUpdate();
            }
            catch (Exception e)
            {
                Cave.LogWarning("DungeonOnUpdate:" + e.Message + "\n" + e.StackTrace);
                Debug.LogError("DungeonOnUpdate:"+e.Message+"\n"+e.StackTrace);
            }
        }

        // 采阴/收服行动
        private void MakeAction()
        {
            putUnit.move.SetDire(makeDire);
            SceneType.battle.battleMap.playerUnitCtrl.move.SetDire(makeDire);
            putUnit.move.SetPosition(makePosi);
            SceneType.battle.battleMap.playerUnitCtrl.move.SetPosition(makePosi);

            if (makeState != 0)
            {
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || !CheckEnergy())
                {
                    ExitMakeLove();
                }
                if (Input.GetKeyDown(g.data.globle.key.battleDialogue))
                {
                    makeAnimID = makeAnimID == 1 ? 2 : 1;
                    if (data.vip == 0)
                    {
                        PlayCaveAnim(putUnit, (putUnit.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                        PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                    }
                    else
                    {
                        PlayCaveAnim(putUnit, (putUnit.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID);
                        PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID);
                    }
                }
            }

            // makeNextTime
            if(Time.time > makeNextTime)
            {
                if (makeState == 1)
                {
                    // 处理吸取修为
                    AbsorbExp(g.world.playerUnit, g.world.unit.GetUnit(putUnit.data.worldUnitData.unitData.unitID, true), CommonTool.Random(addMinExpRate, addMaxExpRate));

                    // 消耗精力
                    if (IsCostEnergy(SceneType.battle.battleMap.playerUnitCtrl, putUnit))
                    {
                        CostEnergy(Mathf.Clamp(putUnit.data.grade.value + 1 - SceneType.battle.battleMap.playerUnitCtrl.data.grade.value, 1, 10));
                    }
                    // 增加仇恨
                    ModRelation(g.world.playerUnit, g.world.unit.GetUnit(putUnit.data.worldUnitData.unitData.unitID, true), -CommonTool.Random(addHateMin, addHateMax));
                }
                if (makeState == 2)
                {
                    // 增加征服度
                    data.AddConquer(putUnit.data.worldUnitData.unitData.unitID, CommonTool.Random(addConquerMin, addConquerMax));
                    makeSatisfy += CommonTool.Random(addSatisfyMin, addSatisfyMax);
                    // 消耗精力
                    if (IsCostEnergy(SceneType.battle.battleMap.playerUnitCtrl, putUnit))
                    {
                        CostEnergy(1);
                    }
                    // 增加好感
                    if (makeSatisfy >= 50 || data.GetConquer(putUnit.data.worldUnitData.unitData.unitID) >= 50)
                    {
                        ModRelation(g.world.playerUnit, g.world.unit.GetUnit(putUnit.data.worldUnitData.unitData.unitID, true), CommonTool.Random(addLoveMin, addLoveMax));
                    }
                    var str = dialogueData.loveDialogue1;
                    if(makeSatisfy >= 60)
                    {
                        str = dialogueData.loveDialogue2;
                    }
                    if(makeSatisfy >= 90)
                    {
                        str = dialogueData.loveDialogue3;
                    }
                    if (slider1 != null)
                        slider1.fillAmount = Mathf.Clamp(makeSatisfy / 100f, 0, 1);
                    if (slider2 != null)
                        slider2.fillAmount = Mathf.Clamp(data.GetConquer(putUnit.data.worldUnitData.unitData.unitID) / 100f, 0, 1);

                    if (str.Length > 0)
                        Dialogue(putUnit, str[CommonTool.Random(0, str.Length)]);
                }
                makeNextTime = Time.time + CommonTool.Random(actionCdMin, actionCdMax);
            }

            void ExitMakeLove()
            {
                if (sliderBg != null)
                    GameObject.Destroy(sliderBg);
                int delay = data.vip == 0 ? 1 : 5;
                if (makeState == 2)
                {
                    if (makeSatisfy < 60) // 满足判断
                    {
                        var unitid = putUnit.data.worldUnitData.unitData.unitID;
                        var unit = g.world.unit.GetUnit(unitid, true);
                        SceneType.battle.timer.Time(new Action(() =>
                        {
                            try
                            {
                                ModRelation(g.world.playerUnit, unit, -extCutLove);
                                data.AddConquer(unitid, -extCutConquer);
                            }
                            catch (Exception e)
                            {
                                Cave.LogWarning(e.Message + "\n" + e.StackTrace);
                            }
                        }), delay + 0.5f);
                        if (dialogueData.loveDialogue1End.Length < 0)
                            Dialogue(putUnit, dialogueData.loveDialogue1End[CommonTool.Random(0, dialogueData.loveDialogue1End.Length)]);
                    }
                    else if (makeSatisfy < 80)
                    {
                        if (dialogueData.loveDialogue2End.Length < 0)
                            Dialogue(putUnit, dialogueData.loveDialogue2End[CommonTool.Random(0, dialogueData.loveDialogue2End.Length)]);
                    }
                    else
                    {
                        var humanNpc = putUnit;
                        var unitid = putUnit.data.worldUnitData.unitData.unitID;
                        var unit = g.world.unit.GetUnit(unitid, true);
                        SceneType.battle.timer.Time(new Action(() =>
                        {
                            try
                            {
                                ModRelation(g.world.playerUnit, unit, extAddLove);
                                data.AddConquer(unitid, extAddConquer);
                            }
                            catch (Exception e)
                            {
                                Cave.LogWarning(e.Message + "\n" + e.StackTrace);
                            }
                            if (data.GetConquer(unitid) >= 100)
                            {
                            // 征服成功
                            UICustomDramaDyn dramaDyn = new UICustomDramaDyn(42685193);
                                dramaDyn.dramaData.onOptionsBackClickCall = new Action<ConfDramaOptionsItem>((v) =>
                                {
                                    Cave.Log("点击按钮 = " + v.id);
                                    if (v.id == 42685185) // 收为道侣
                                {
                                        PlayDefAnim(humanNpc);
                                        var unitId = humanNpc.data.worldUnitData.unitData.unitID;
                                        new DramaFunction().ReleaseUnit(humanNpc, tranPoint);
                                        GameObject.Destroy(humanNpc.gameObject.GetComponent<BattleDialogueCtrl>());
                                        units.Remove(humanNpc);
                                        data.units.Remove(unitId);
                                        data.unitPosi.Remove(unitId);
                                        data.unitConquer.Remove(unitId);
                                        ModRelation(g.world.playerUnit, unit, 1000);
                                        if (unit != null)
                                        {
                                            unit.CreateAction(new UnitActionLuckDel(prisonerLuckId));
                                        // 设置关系为道侣
                                        g.world.playerUnit.CreateAction(new UnitActionRelationSet(unit, UnitRelationType.Lover, 0));
                                            if (dialogueData.loverDialogue.Length > 0)
                                                Dialogue(humanNpc, dialogueData.loverDialogue[CommonTool.Random(0, dialogueData.loverDialogue.Length)]);
                                        }
                                    }
                                    if (v.id == 42685186) // 继续关押
                                {
                                        ModRelation(g.world.playerUnit, unit, -100);
                                        data.AddConquer(unitid, -100);
                                    }
                                });
                                dramaDyn.OpenUI();
                            }
                        }), delay);
                        if (dialogueData.loveDialogue3End.Length > 0)
                        {
                            Dialogue(putUnit, dialogueData.loveDialogue3End[CommonTool.Random(0, dialogueData.loveDialogue3End.Length)]);
                        }
                    }
                }
                makeState = 0;

                if (data.vip == 0)
                {
                    PlayCaveAnim(putUnit, (putUnit.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                    PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                }
                else
                {
                    PlayCaveAnim(putUnit, (putUnit.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                    PlayCaveAnim(SceneType.battle.battleMap.playerUnitCtrl, (SceneType.battle.battleMap.playerUnitCtrl.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "makeL1_" : "makeL2_") + makeAnimID + "end");
                }

                SceneType.battle.timer.Time(new Action(() =>
                {
                    SceneType.battle.battleMap.playerUnitCtrl.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                    putUnit.gameObject.GetComponent<BoxCollider2D>().enabled = true;

                    SceneType.battle.cameraCtrl.orthoSizes.Clear();
                    foreach (var item in defSize)
                    {
                        SceneType.battle.cameraCtrl.orthoSizes.Add(item);
                    }
                    defSize.Clear();
                    DG.Tweening.ShortcutExtensions.DOKill(SceneType.battle.camera);
                    var tween = DG.Tweening.ShortcutExtensions.DOOrthoSize(SceneType.battle.camera, SceneType.battle.cameraCtrl.orthoSize, 0.1f);
                    DG.Tweening.TweenSettingsExtensions.OnUpdate(tween, new Action(() => SceneType.battle.cameraCtrl.UpdateHalfPosi()));

                    PlayCaveAnim(putUnit, putUnit.data.worldUnitData.unitData.propertyData.sex == UnitSexType.Man ? "kneel" : "kneel2");
                    PlayDefAnim(SceneType.battle.battleMap.playerUnitCtrl);
                    putUnit = null;
                    curSize = null;
                }), delay);
            }
        }

        private void AddMakeNextTime()
        {
            makeNextTime = Time.time + CommonTool.Random(2, 8);
        }

        private void DecorateUpdate()
        {
            int op = operate;
            if (Input.GetKeyUp(KeyCode.F) || clickF)
            {
                clickF = false;
                op = op == 1 ? 0 : 1;
            }
            if (Input.GetKeyUp(KeyCode.G) || clickG)
            {
                clickG = false;
                op = op == 2 ? 0 : 2;
            }
            if (op != 0 && Input.GetMouseButtonDown(1))
            {
                op = 0;
            }
            if (op != operate)
            {
                operate = op;
                CreateOperateUI();
            }
            if (op != 1 && barrierPrefab != null)
            {
                GameObject.Destroy(barrierPrefab);
                GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
            }
            if (barrierPrefab != null)
            {
                Vector3 pos = SceneType.battle.camera.ScreenToWorldPoint(Input.mousePosition);
                barrierPrefab.transform.position = pos;
                bool a = (!isEnterPanel && Input.GetMouseButtonDown(0));
                bool b = (!isEnterPanel && Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && Time.time > createNextTime);
                if (a || b)
                {
                    // 判断价格 装饰物 b
                    int price = 1000;
                    if (DecorateMgr.decoratePrice.ContainsKey(barrierCreateID))
                    {
                        price = DecorateMgr.decoratePrice[barrierCreateID];
                    }
                    int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                    if (money < price)
                    {
                        UITipItem.AddTip("灵石不足！");
                    }
                    else
                    {
                        g.world.playerUnit.data.CostPropItem(PropsIDType.Money, price);
                        SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("灵石 -" + price);
                        createNextTime = Time.time + 0.1f;
                        var data = new DecorateMgr.DecorateData(barrierCreateID, pos.x, pos.y);
                        decorateMgr.decorateList.Add(data);
                        decorateMgr.CreateDecorate(data);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    createNextTime = 0;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    GameObject.Destroy(barrierPrefab);
                    GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
                }
            }
            if (op == 2)
            {
                if (clickDestroyBarrierb == null)
                {
                    Vector3 pos = SceneType.battle.camera.ScreenToWorldPoint(Input.mousePosition);
                    GameObject barrierb = null;
                    float dis = 100;
                    foreach (var item in decorateMgr.decorates.Values)
                    {
                        var d = Vector2.Distance(pos, item.transform.position);
                        if (d < dis && d < 1)
                        {
                            dis = d;
                            barrierb = item;
                        }
                    }
                    if (barrierb == null)
                    {
                        if (destroyBarrierb != null)
                        {
                            DecorateMgr.SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
                            destroyBarrierb = null;
                        }
                    }
                    else if (barrierb != destroyBarrierb)
                    {
                        if (destroyBarrierb != null)
                        {
                            DecorateMgr.SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
                        }
                        destroyBarrierb = barrierb;
                    }
                }

                // 销毁模式
                if (destroyBarrierb != null)
                {
                    float value = Time.time - ((int)Time.time);
                    if ((int)Time.time % 2 == 1)
                    {
                        value = 1 - value;
                    }
                    DecorateMgr.SetBarrierbColor(destroyBarrierb, Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 0, 0, 1), value));
                    //SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, value));
                }

                if (!isEnterPanel && Input.GetMouseButtonUp(0))
                {
                    if (destroyBarrierb != null)
                    {
                        decorateMgr.DestroyDecorate(destroyBarrierb);
                        destroyBarrierb = null;
                        clickDestroyBarrierb = null;
                        if (listObj == null)
                            CreateOperateUI();
                        else
                            CreateOperateUI();

                    }
                }
                if (Input.GetMouseButtonUp(1))
                {
                    if (clickDestroyBarrierb != null)
                    {
                        clickDestroyBarrierb = null;
                    }
                }
            }
        }

        // 创建操作装饰物UI
        private void CreateOperateUI()
        {
            if (destroyBarrierb != null)
            {
                DecorateMgr.SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
                destroyBarrierb = null;
            }
            if (listObj != null)
            {
                defVerticalNormalizedPosition = listObj.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition;
                GameObject.Destroy(listObj.gameObject);
                listObj = null;
            }
            GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
            switch (operate)
            {
                case 1:
                    {
                        if (uiBase != null)
                        {
                            var panel = new UIBarrierList(uiBase.transform, allBarrierID);
                            listObj = panel.bg.GetComponent<RectTransform>();
                            panel.clickCall = (go, id) =>
                            {
                                if (barrierPrefab != null) {
                                    GameObject.Destroy(barrierPrefab);
                                }

                                barrierPrefab = decorateMgr.CreateDecorate(go, id);
                                barrierCreateID = id;
                                GameTool.SetCursor("touqieshubiao", "touqieshubiao");
                            };
                        }
                    }
                    break;
                case 2:
                    {
                        GameTool.SetCursor("chaichuzhuangshi1", "chaichuzhuangshi");
                        if (uiBase != null)
                        {
                            var panel = new UIBarrierDestroy(uiBase.transform, decorateMgr.decorateList);
                            listObj = panel.bg.GetComponent<RectTransform>();
                            panel.clickCall = (go, barrierb) =>
                            {
                                if (decorateMgr.decorates.ContainsKey(barrierb.GetHashCode()))
                                {
                                    destroyBarrierb = decorateMgr.decorates[barrierb.GetHashCode()];
                                    if (destroyBarrierb != null)
                                    {
                                        decorateMgr.DestroyDecorate(destroyBarrierb);
                                        destroyBarrierb = null;
                                        clickDestroyBarrierb = null;
                                        if (listObj == null)
                                            CreateOperateUI();
                                        else
                                            CreateOperateUI();
                                    }
                                }
                            };
                        }
                    }
                    break;
            }
            isEnterPanel = false;
            if (listObj != null)
            {
                // 监听鼠标滑入
                var cg = listObj.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 1;
                var ui_event = listObj.gameObject.AddComponent<UIEventListener>();
                Action onEnter = () =>
                {
                    if (createNextTime == 0)
                    {
                        isEnterPanel = true;
                    }
                };
                Action onExit = () =>
                {
                    if (createNextTime == 0)
                    {
                        isEnterPanel = false;
                    }
                };
                ui_event.onMouseEnter.AddListener(onEnter);
                ui_event.onMouseExit.AddListener(onExit);

                defVerticalNormalizedPosition = Mathf.Clamp(defVerticalNormalizedPosition, 0.0001f, 0.9999f);
                listObj.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = defVerticalNormalizedPosition;
            }
        }


        // 进入房间结束
        private void OnIntoRoomEnd()
        {
            // 设置玩家初始位置
            SceneType.battle.battleMap.playerUnitCtrl.move.SetPosition(playerBronPoint);
            Cave.Log("设置玩家初始位置:" + SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x+","+ SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.y);

            float intoTime = 0;
            //初始化离开副本的法阵
            Action exitAction = () =>
            {
                if (Time.time < intoTime)
                {
                    return;
                }
                Action exitDungeon = () =>
                {
                    SceneType.battle.battleEnd.BattleEnd(true);
                    intoTime = Time.time + 1;
                };
                Action exitCancel = () =>
                {
                    intoTime = Time.time + 1;
                };
                intoTime = float.MaxValue;
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "是否确定离开地牢？", 2, exitDungeon, exitCancel);
            };
            // 设置离开传送法阵的位置
            SceneType.battle.battleMap.CreatePassLevelEffect(tranPoint, exitAction, null).isRepet = true;
            UIMask.GradientEffectToWhite();
            decorateMgr.Init(data.decorate);
        }

        private void OnBattleEnd(ETypeData e)
        {
            foreach (var item in units)
            {
                var unitid = item.data.worldUnitData.unitData.unitID;
                data.unitPosi[unitid] = new float[] { item.move.lastPosi.x, item.move.lastPosi.y, item.move.isLockDire ? 0 : 1 };
            }
            data.decorate = decorateMgr.GetData();
            DataDungeon.SaveData(data);
            g.events.On(EBattleType.BattleExit, new Action(()=>
            {
                GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
                new MainCave();
            }), 1, true);
            Cave.Log(data.decorate);
        }

        // 播放默认动画
        private void PlayDefAnim(UnitCtrlHuman human)
        {
            try
            {
                human.transform.Find("Root/Body").GetComponent<Animator>().enabled = true;
                var go = human.transform.Find("Root").gameObject;
                var com = UnityAPIEx.GetComponentOrAdd<Animator>(go);
                com.enabled = false;
            }
            catch (Exception e)
            {
                Cave.LogWarning(e.Message + "\n" + e.StackTrace);
            }
        }

        // 播放洞府动画
        private void PlayCaveAnim(UnitCtrlHuman human, string animName)
        {
            try
            {
                human.transform.Find("Root/Body").GetComponent<Animator>().enabled = false;
                var go = human.transform.Find("Root").gameObject;
                var com = UnityAPIEx.GetComponentOrAdd<Animator>(go);
                var anim = g.res.Load<RuntimeAnimatorController>("HumanAnim/" + animName);
                com.runtimeAnimatorController = anim;
                com.Play(animName);
                com.enabled = true;
            }
            catch (Exception e)
            {
                Cave.LogWarning(e.Message + "\n" + e.StackTrace);
            }
        }

        private bool CheckEnergy()
        {
            return g.world.playerUnit.data.unitData.propertyData.energy > 0;
        }

        private bool CostEnergy(int value)
        {
            g.world.playerUnit.data.unitData.propertyData.energy -= value;
            SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("精力 -" + value);
            return g.world.playerUnit.data.unitData.propertyData.energy > 0;
        }

        private bool IsCostEnergy(UnitCtrlHuman player, UnitCtrlHuman unit)
        {
            var playerGrade = player.data.grade.value;
            var unitGrade = player.data.grade.value;
            var rate = Mathf.Clamp(cutEnergyRate - ((playerGrade - unitGrade) * mofEnergy), 20, 80);
            return (CommonTool.Random(0, 100) < rate);
        }

        // 修正关系 value：增加好感值
        private void ModRelation(WorldUnitBase unit, WorldUnitBase toUnit, float value)
        {
            if (toUnit.data.GetRelationType(unit) == UnitBothRelationType.Hater)
            {
                value = -value;
                toUnit.data.unitData.relationData.AddHate(unit.data.unitData.unitID, value, 0, "", false);

                if (value > 0)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("仇恨 +" + value);
                }
                else
                {
                    SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("仇恨 " + value);
                }
            }
            else
            {
                toUnit.data.unitData.relationData.AddIntim(unit.data.unitData.unitID, value, 0, "", false);

                if (value > 0)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("好感 +" + value);
                }
                else
                {
                    SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("好感 " + value);
                }
            }
        }

        // 吸取修为
        private void AbsorbExp(WorldUnitBase absorbUnit, WorldUnitBase targetUnit, float rate)
        {
            var targetGradeItem = g.conf.roleGrade.GetItem(targetUnit.data.dynUnitData.gradeID.value);
            var targetGrade = targetGradeItem.grade;
            var targetPhase = targetGradeItem.phase;
            if (targetUnit.data.unitData.propertyData.exp == 0)
            {
                targetPhase -= 1;
                if(targetPhase == 0)
                {
                    targetPhase = 3;
                    targetGrade -= 1;
                }

                // 降低大境界
                targetUnit.data.dynUnitData.gradeID.baseValue = g.conf.roleGrade.GetGradeItem(targetGrade, targetPhase).id;
                targetUnit.data.dynUnitData.exp.baseValue = g.conf.roleGrade.GetGradeItem(targetGrade, targetPhase).exp;
                targetUnit.data.unitData.propertyData.exp = g.conf.roleGrade.GetGradeItem(targetGrade, targetPhase).exp;
            }

            int[] totalExp = g.conf.roleGrade.GetGradeTotalExp(targetUnit, targetGrade);
            int costExp = Mathf.CeilToInt(totalExp[1] * rate * 0.01f);
            targetUnit.data.unitData.propertyData.AddExp(-costExp);
            absorbUnit.data.unitData.propertyData.AddExp(costExp);
            SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("吸取修为 " + costExp);

            // 如果NPC有陷入瓶颈气运，则移除
            for (int i = 120; i < 139; i++)
            {
                WorldUnitLuckBase luck = targetUnit.GetLuck(i);
                if(luck != null)
                {
                    targetUnit.CreateAction(new UnitActionLuckDel(luck));
                }
            }
        }
    }
}
