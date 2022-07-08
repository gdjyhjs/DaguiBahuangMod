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

namespace Cave.BuildFunction
{
    // 灵田 
    public class BuildFarm : ClassBase
    {
        public const int prisonerLuckId = 123010217; // 囚犯气运ID
        List<WorldUnitBase> units = new List<WorldUnitBase>(); // 被关押的单位
        UIBattleInfo uiBase;
        DataFram data;

        private Vector2 playerBronPoint = new Vector2(2.6f, 9.2f); // 玩家出生点
        private Vector2 tranPoint = new Vector2(1.0f, 9.2f); // 传送阵点
        private DecorateMgr decorateMgr = new DecorateMgr();

        int operate = 0; // 1:布置摆件 2:移除摆件
        RectTransform listObj;
        float defVerticalNormalizedPosition = 0;
        List<int> allBarrierID;
        List<int> ignoreBarrierID = new List<int>()
        {
            10401,10402,10501,10502,10503,17801,10901,11001,11301,11501,11601,11701,20201,20301,20401,20501,20801,20802,20901,21701,21801,22101,22601,22701,22801,
            23001,30601,30701,30801,30901,31301,31401,31601,31701,31801,32101,32201,32401,32501,32701,32901,33001,33401,140504,141301,440101,440301,450001,467100,
            469100,470101,480101,490101,500801,520101,530101,420101,420201,430101,411301,330001,380101,350101,340101,400101,360501,360502,360503,360504,360505,240501,
            250101,260101,270101,270201,280101,280201,280301,280401,290101,290201,290301,290401,300101,300201,300301,300401,300501,300601,300701,300801,300901,301001,
            301101,301201,310101,310201,310301,310401,310501,310601,360101,360201,360301,360302,360401,360402,360403,360404,360405,232501,232401,240101,240201,240301,
            240401,200001,110101,120101,80401,17701,140501,140601,140701,190201,182101,211601,211602,212001,212002,510019,160101
        };
        private Vector2[] farmPoints = new Vector2[] // 所有灵田坑位的位置
        {
            new Vector2(7,1.8f),new Vector2(17,1.8f),new Vector2(27,1.8f),new Vector2(37,1.8f),
            new Vector2(11.8f,5),new Vector2(21.8f,5),new Vector2(31.8f,5),new Vector2(41.8f,5),
            new Vector2(7,13.4f),new Vector2(17,13.4f),new Vector2(27,13.4f),new Vector2(37,13.4f),
            new Vector2(11.8f,16.4f),new Vector2(21.8f,16.4f),new Vector2(31.8f,16.4f),new Vector2(41.8f,16.4f),
        };
        GameObject barrierPrefab; // 要创建的障碍物预制件
        int barrierCreateID; // 要创建的障碍物ID
        GameObject clickDestroyBarrierb; // 点击将删除的障碍物
        GameObject destroyBarrierb; // 正准备删除的障碍物
        bool isEnterPanel; // 鼠标是否划入面板
        float createNextTime = 0; // 下一个装饰可创建时间

        // 初始化灵田    
        public override void Init(string param)
        {
            allBarrierID = new List<int>();
            foreach (var item in g.conf.dungeonSceneObject._allConfList)
            {
                if (!ignoreBarrierID.Contains(item.id))
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

            Debug.Log("初始化灵田 " + param);
            MainCave.Close();
            DataCave dataCave = DataCave.ReadData();
            data = DataFram.ReadData();

            Action<ETypeData> onBattleStart = OnBattleStart;
            Action<ETypeData> onBattleEnd = OnBattleEnd;

            // 监听战斗开始一次
            g.events.On(EBattleType.BattleStart, onBattleStart, 1);
            // 监听战斗结束
            g.events.On(EBattleType.BattleEnd, onBattleEnd, 1);

            // 进入灵田副本
            g.world.battle.IntoBattle(new DataMap.MonstData() { id = 42685182, level = 1 });
        }

        List<GameObject> allFram = new List<GameObject>(); // 所以的灵田单位
        List<UnitCtrlBase> allUnits = new List<UnitCtrlBase>(); // 所以的关押单位
        private void OnBattleStart(ETypeData e)
        {
            uiBase = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
            //初始化灵田副本

            Action onIntoRoomEnd = () => OnIntoRoomEnd();
            SceneType.battle.battleMap.onIntoRoomEnd += onIntoRoomEnd;

            Action onStart = () =>
            {
                ClearPlayerEffects();
                SceneType.battle.timer.Frame(new Action(OnUpdate), 1, true);
            };
            SceneType.battle.battleMap.AddStartBattleCall(onStart);

            // 隐藏ui
            uiBase.uiInfo.goMonst.SetActive(false);

            // 自动点击开始战斗按钮
            TimerCoroutine autoStartCor = null;
            Action action = () =>
            {
                uiBase.uiStartBattle.btnStart.onClick.Invoke();
                if (!uiBase.uiStartBattle.goGroupRoot.activeSelf)
                {
                    SceneType.battle.timer.Stop(autoStartCor);
                }
            };
            autoStartCor = SceneType.battle.timer.Frame(action, 1, true);


            Cave.Log("初始化灵田A");
            InitFarm();
            Cave.Log("初始化灵田B");
        }

        private void InitFarmUI()
        {
            if (data.open == 0)
            {
                Cave.OpenDrama("第一次来灵田吧！我来给你简单介绍一下灵田的使用方法。", new Action(() =>
                {
                    Cave.OpenDrama("在灵田中种下灵果之后可成长为灵树，灵树没过一段时间可结出灵果，灵气越高需要的成熟结果时间越短。", new Action(() =>
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
        private void ClearPlayerEffects()
        {
            // 清空玩家状态
            var unit = SceneType.battle.battleMap.playerUnitCtrl;
            unit.AddState(UnitStateType.NotUserAllSkill, -1);

            // 清除蛟龙等效果
            new SchoolBigFight.UnitEffectSpecHandler().Close(unit);

            // 固定移动速度
            unit.data.moveSpeed.baseValue = 800;
        }

        private void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Patch_PointResourcesMgr_OnSchoolAction.Grow(data);
                UpdateAllCrop();
            }

            int op = operate;
            if (Input.GetKeyUp(KeyCode.F))
            {
                op = op == 1 ? 0 : 1;
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                op = op == 2 ? 0 : 2;
            }
            if (op == 2 && Input.GetMouseButtonDown(1))
            {
                op = 0;
            }
            if (op != operate)
            {
                operate = op;
                CreateOperateUI(0);
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
                if (!isEnterPanel && Input.GetMouseButton(0) && Time.time > createNextTime)
                {
                    // 判断价格 装饰物
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
                            SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
                            destroyBarrierb = null;
                        }
                    }
                    else if (barrierb != destroyBarrierb)
                    {
                        if (destroyBarrierb != null)
                        {
                            SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
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
                        value = 1-value;
                    }
                    SetBarrierbColor(destroyBarrierb, Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 0, 0, 1), value));
                    //SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, value));
                }

                if (!isEnterPanel && Input.GetMouseButtonUp(0))
                {
                    if (destroyBarrierb != null)
                    {
                        decorateMgr.DestroyDecorate(destroyBarrierb);
                        destroyBarrierb = null;
                        clickDestroyBarrierb = null;
                        if(listObj == null)
                            CreateOperateUI(defVerticalNormalizedPosition);
                        else
                            CreateOperateUI(listObj.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition);

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

        private void CreateOperateUI(float value)
        {
            if (destroyBarrierb != null)
            {
                SetBarrierbColor(destroyBarrierb, new Color(1, 1, 1, 1));
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
                        var ui = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
                        if (ui != null)
                        {
                            var panel = new UIBarrierList(ui.transform, allBarrierID);
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
            }
        }

        private void SetBarrierbColor(GameObject barrierb, Color color)
        {
            var sprites = barrierb.GetComponentsInChildren<SpriteRenderer>();
            foreach (var item in sprites)
            {
                item.color = color;
            }
        }

        // 初始化灵田
        private void InitFarm()
        {
            Cave.Log("延迟初始化灵田 "+ uiBase);
            Action battleFrame = () =>
            {
                Cave.Log("初始化灵田");
                g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo).startBattle.btnStart.onClick.Invoke();

                allUnits.Clear();

                // 玩家位置
                Vector2 startPoint = SceneType.battle.battleMap.roomCenterPosi + new Vector2(-30, -10);
                UnitCtrlPlayer player = SceneType.battle.battleMap.playerUnitCtrl;
                player.WingmanEnable(false);
                player.move.SetPosition(startPoint + new Vector2(-2, 0));
                Cave.Log("玩家位置：" + player.move.lastPosi.x+","+ player.move.lastPosi.y);
                Il2CppSystem.Collections.Generic.List<UnitCtrlBase> clearUnits = SceneType.battle.unit.GetAllUnit(true);
                foreach (UnitCtrlBase item in clearUnits)
                {
                    if (item.TryCast<UnitCtrlPlayer>() == null && !item.isDie && !item.isDestroy && item.gameObject != null)
                    {
                        Debug.Log("die " + item+" . "+ item.TryCast<UnitCtrlPlayer>());
                        item.Die(false);
                    }
                }

                int unitIdx = 0, max = 100;
                // 关押的单位
                Cave.Log("关押的NPC数量：" + units.Count);
                foreach (WorldUnitBase unit in units)
                {
                    MelonLoader.MelonLogger.Msg("关押的NPC " + unit.data.unitData.propertyData.GetName());
                    for (int i = 0; i < max; i++)
                    {
                        UnitCtrlHumanNPC humanNPC1 = SceneType.battle.unit.CreateUnitHuman<UnitCtrlHumanNPC>(unit.data, UnitType.NPC);
                        Vector2 resPoint = startPoint + new Vector2(unitIdx / 8, unitIdx % 16);
                        humanNPC1.move.SetPosition(SceneType.battle.battleMap.roomCenterPosi + new Vector2(2, 0));
                        humanNPC1.AddState(UnitStateType.LoseControl);
                        humanNPC1.moveBox.enabled = false;
                        humanNPC1.anim.Play("Idle");
                        humanNPC1.anim.enableAnim = false;

                        Action call = () => {
                            MelonLoader.MelonLogger.Msg("交互NPC");
                        };

                        humanNPC1.gameObject.AddComponent<BattleDialogueCtrl>().InitData(humanNPC1.posiTop.position, call, "", true);

                        unitIdx++;
                    }
                    if (unitIdx >= max)
                    {
                        break;
                    }
                }

                // 药田位置
                startPoint = SceneType.battle.battleMap.roomCenterPosi + new Vector2(-20, -6);

                UpdateAllCrop();
                uiBase.info.goGroupRoot.SetActive(false);
                InitFarmUI();
            };
            uiBase.AddCor(SceneType.battle.timer.Frame(battleFrame, 2));
        }

        private void UpdateAllCrop()
        {
            Cave.Log("灵田数量：" + data.data.Count);
            for (int i = 0; i < farmPoints.Length; i++)
            {
                if (i >= data.data.Count)
                {
                    data.data.Add(new DataFramItem());
                }
                if (i >= allFram.Count)
                {
                    allFram.Add(CreateOneCrop(i, farmPoints[i]));
                }
                else
                {
                    GameObject.Destroy(allFram[i]);
                    allFram[i] = CreateOneCrop(i, farmPoints[i]);
                }
            }
        }


        private GameObject CreateOneCrop(int idx, Vector2 point)
        {
            DataFramItem framData = data.data[idx];
            string resPath;
            string topTips;
            Action<GameObject> growFruit = null;


            string key = g.conf.gameKeyCode.GetKey(g.data.globle.key.battleDialogue);
            topTips = "按" + key + "键进行操作";
            ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
            // 未播种 显示土坑
            if (framData.itemID == 0 || item == null)
            {
                // 未播种 显示土坑
                resPath = "ModPrefab/tukeng";
            }
            else
            {
                if (framData.progress >= 1)
                {
                    // 已成熟 收获农作物
                    System.Random rand = new System.Random(framData.seed);
                    resPath = "ModPrefab/shu" + rand.Next(1, 5);
                    growFruit = (t) => GrowFruit(t, framData.itemID, framData.count, framData.seed);
                }
                else
                {
                    if (framData.progress == 0)
                    {
                        // 刚种下种子，填平土坑 
                        resPath = "ModPrefab/tianping";
                    }
                    else
                    {
                        // 未成熟 小树苗
                        resPath = "ModPrefab/shumiao";
                    }
                }
            }
            GameObject prefab = g.res.Load<GameObject>(resPath);
            GameObject tree = GameObject.Instantiate(prefab);
            tree.AddComponent<EffectAutoSortOrderCtrl>();
            tree.transform.position = point;

            // 添加交互脚本
            BattleDialogueCtrl dialogueCtrl = tree.gameObject.AddComponent<BattleDialogueCtrl>();
            Action onCall = () =>
            {
                OperateFram(framData);
            };
            dialogueCtrl.InitData(point, onCall, topTips, true, false);
            dialogueCtrl.dialogueDis = 3f;
            dialogueCtrl.goTip.transform.GetComponent<EffectLockScaleCtrl>().enabled = false; // 因为要修改文本大小，所以停用锁定大小的脚本
            dialogueCtrl.goTip.transform.Find("baoxiangbg").localScale = new Vector3(1.5f, 1, 1); // 修改交互文本背景图大小
            dialogueCtrl.goTip.transform.Find("baoxiangbg").localPosition = new Vector3(0.4f, 0, 1); // 修改交互文本背景图位置
            growFruit?.Invoke(tree);
            return tree;
        }
        // 让树上结出果实
        private void GrowFruit(GameObject tree, int propID, int maxCount, int seed)
        {
            System.Random rand = new System.Random(seed);
            int treeSortingOrder = tree.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder;
            List<SpriteRenderer> childs = new List<SpriteRenderer>(tree.transform.Find("Body/bone_1").GetComponentsInChildren<SpriteRenderer>());

            int fruitCount = 0;
            var propItem = g.conf.itemProps.GetItem(propID);
            if (propItem == null)
            {
                Console.WriteLine("道具id不存在 = " + propItem);
                return;
            }
            string iconName = propItem.icon;
            Sprite sprite = SpriteTool.GetSprite("CommonPropIcon", iconName);
            while (childs.Count > 0)
            {
                var child = childs[rand.Next(0, childs.Count)];
                childs.Remove(child);
                if (fruitCount < maxCount)
                {
                    child.gameObject.SetActive(true);
                    child.sprite = sprite;
                    fruitCount++;
                    child.sortingOrder = treeSortingOrder + fruitCount;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
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
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("MOD_d86I8k_ExitDungeonTips"), 2, exitDungeon, exitCancel);
            };
            // 设置离开传送法阵的位置
            SceneType.battle.battleMap.CreatePassLevelEffect(tranPoint, exitAction, null).isRepet = true;
            UIMask.GradientEffectToWhite();
            decorateMgr.Init(data.decorate);
        }

        private void OnBattleEnd(ETypeData e)
        {
            data.decorate = decorateMgr.GetData();
            DataFram.SaveData(data);
            g.events.On(EBattleType.BattleExit, new Action(()=>
            {
                GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
                new MainCave();
            }), 1, true);
        }

        private float GetHeight(DataFramItem framData)
        {
            ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
            if (item == null || framData.itemID == 0)
            {
                return 0.1f;
            }
            return (0.3f + Mathf.Lerp(0.0f, 0.7f, framData.progress));
        }

        bool openFram = false;
        // 操作农田
        private void OperateFram(DataFramItem framData)
        {
            if (openFram)
                return;
            GameObject goInfo = CreateUI.NewImage();

            openFram = true;
            GameObject go = CreateUI.NewImage();
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(9999, 9999);
            go.transform.SetParent(uiBase.transform, false);
            go.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            go.name = "OperateFram";

            GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg")); // 背景图
            bg.GetComponent<Image>().type = Image.Type.Tiled;
            bg.transform.SetParent(go.transform, false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 550);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -60);

            GameObject tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 关闭按钮底图
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -164);

            GameObject tmpText = CreateUI.NewText("关闭", tmpBtn.GetComponent<RectTransform>().sizeDelta); // 关闭按钮文字
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpText = CreateUI.NewText(framData.level + "级灵田", new Vector2(350, 100), 1); // 灵田信息
            tmpText.transform.SetParent(go.transform, false);
            tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 175);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;
            tmpText.GetComponent<Text>().raycastTarget = false;
            tmpText.GetComponent<Text>().fontSize = 30;

            Action closeAction = () =>
            {
                GameObject.Destroy(go);
                openFram = false;
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(closeAction);

            if (framData.level < 8)
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 升级按钮底图
                tmpBtn.transform.SetParent(go.transform, false);
                tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -100);

                tmpText = CreateUI.NewText("升级", tmpBtn.GetComponent<RectTransform>().sizeDelta); // 升级按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

                Action upAction = () =>
                {
                    int needMoney = 10000 * Mathf.CeilToInt(Mathf.Pow(framData.level, 2));
                    Action upLevelAction = () =>
                    {
                        int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                        if (money < needMoney)
                        {
                            UITipItem.AddTip("灵石不足！");
                            return;
                        }
                        g.world.playerUnit.data.CostPropItem(PropsIDType.Money, needMoney);
                        framData.level++;
                        closeAction();
                        OperateFram(framData);
                    };
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("升级提示", "确定花费" + needMoney + "灵石升级灵田？", 2, upLevelAction);
                };
                tmpBtn.AddComponent<Button>().onClick.AddListener(upAction);
            }

            string str;
            ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
            if (item == null || framData.itemID == 0)
            {
                Action tmpAction = () =>
                {
                    closeAction();
                    OperateCultivate(framData);
                };

                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojukukuang_1"));
                tmpBtn.transform.SetParent(go.transform, false);
                tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 67);
                tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);

                tmpText = CreateUI.NewText("种植", tmpBtn.GetComponent<RectTransform>().sizeDelta);
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }
            else
            {
                str = GameTool.LS(item.name)+"树";
                tmpText = CreateUI.NewText(str, new Vector2(250, 30));
                tmpText.transform.SetParent(go.transform, false);
                tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 5);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

                string str2;
                if (framData.progress < 1)
                {
                    float need = item.worth * 10 - framData.lingqi;
                    int month = Mathf.CeilToInt(need / DataFram.framLevelLingqi[framData.level]);
                    str2 = "约" + month + "个月后成熟";
                }
                else
                {
                    float need = item.worth * 2;
                    int month = Mathf.CeilToInt(need / DataFram.framLevelLingqi[framData.level]);
                    str2 = "结果周期约" + month + "个月";
                }

                tmpText = CreateUI.NewText(str2, new Vector2(250, 30));
                tmpText.transform.SetParent(go.transform, false);
                tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -30);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

                if (framData.count > 0)
                {
                    Action tmpAction = () => // 收获函数
                    {
                        closeAction();
                        GainItem(framData);
                    };

                    Action tmoAction2 = () =>
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("提示", "是否立即收割" + framData.count + "颗" + GameTool.LS(item.name) + "？", 2, tmpAction);
                    };

                    tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojukukuang_1"));
                    tmpBtn.transform.SetParent(go.transform, false);
                    tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 67);

                    DataProps.PropsData propsData = DataProps.PropsData.NewProps(framData.itemID, framData.count);
                    propsData.propsCount = framData.count;
                    UIIconItemBase propItem = UIIconTool.CreatePropsIcon(g.world.playerUnit, propsData, tmpBtn.transform, 1f);
                    propItem.btnRootIcon.gameObject.SetActive(true);
                    propItem.btnRootIcon.onClick.RemoveAllListeners();
                    propItem.btnRootIcon.onClick.AddListener(tmoAction2);
                }
            }
        }

        // 选择种植
        private void OperateCultivate(DataFramItem framData)
        {
            UIPropSelect ui = g.ui.OpenUI<UIPropSelect>(UIType.PropSelect);
            ui.dataMartial.NotFindAll();
            ui.dataProp.number = 1;
            ui.dataProp.propsFilter.className = new Il2CppSystem.Collections.Generic.List<int>();
            List<int> list = new List<int>() { 6291011, 6291012, 6291013, 6291014, 6291015, 6291016, 1051072, 1051073, 1051074, 1051075, 1051076, 1051082, 1051083, 1051084, 1051085, 1051086, 1051092, 1051093, 1051094, 1051095, 1051096, 1051102, 1051103, 1051104, 1051105, 1051106, 1051112, 1051113, 1051114, 1051115, 1051116, 1051122, 1051123, 1051124, 1051125, 1051126, 1051132, 1051133, 1051134, 1051135, 1051136, 1051142, 1051143, 1051144, 1051145, 1051146, 1051152, 1051153, 1051154, 1051155, 1051156, 1051162, 1051163, 1051164, 1051165, 1051166, 1051172, 1051173, 1051174, 1051175, 1051176, 1051182, 1051183, 1051184, 1051185, 1051186, 1051196, 1051206, 1051216, 1051226, 1051236, 1051246, 1051256, 1051266, 1051276, 1051286, 1051296, 1051306, 1051316, 1051326, 1051336, 1051346, 1051356, 1051366, 1051376, 1051386, 6161392, 6161393, 6161394, 6161395, 6161396 };
            Il2CppSystem.Collections.Generic.List<int> propID = new Il2CppSystem.Collections.Generic.List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                propID.Add(list[i]);
            }

            ui.dataProp.propsFilter.propID = propID;
            ui.UpdateFilter();

            Action okAction = () =>
            {
                Il2CppSystem.Collections.Generic.List<DataProps.PropsData> select = UIPropSelect.allSlectItems;
                for (int i = 0; i < select.Count; i++)
                {
                    framData.itemID = select[i].propsID;
                    framData.progress = 0;
                    framData.count = 0;
                    framData.lingqi = 0;
                    framData.seed = CommonTool.Random(int.MinValue, int.MaxValue);
                    UpdateAllCrop();
                }
            };

            ui.onOKCall = okAction;
        }


        private void GainItem(DataFramItem framData)
        {
            int propsID = framData.itemID;
            GameItemRewardData rewardData = new GameItemRewardData(1, new int[] { propsID }, framData.count);
            Il2CppSystem.Collections.Generic.List<GameItemRewardData> rewards = new Il2CppSystem.Collections.Generic.List<GameItemRewardData>();
            rewards.Add(rewardData);
            g.world.playerUnit.data.RewardItem(rewards);
            framData.count = 0;
            UpdateAllCrop();
        }
    }
}
