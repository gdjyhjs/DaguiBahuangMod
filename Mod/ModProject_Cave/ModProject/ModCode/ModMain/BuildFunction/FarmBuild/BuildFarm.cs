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
        UIBattleInfo uiBase;
        DataFram data;

        private Vector2 playerBronPoint = new Vector2(2.6f, 9.2f); // 玩家出生点
        private Vector2 tranPoint = new Vector2(1.0f, 9.2f); // 传送阵点
        private DecorateMgr decorateMgr = new DecorateMgr();

        int operate = 0; // 1:布置摆件 2:移除摆件
        RectTransform listObj;
        float defVerticalNormalizedPosition = 1;
        List<int> allBarrierID;
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

        List<GameObject> allFram = new List<GameObject>(); // 所有的灵田单位
        List<UnitCtrlBase> allUnits = new List<UnitCtrlBase>(); // 所有的关押单位
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


            InitDungeon();
        }

        private void InitFarmUI()
        {
            if (data.open == 0)
            {
                Cave.OpenDrama("第一次来灵田吧！我来给你简单介绍一下灵田的使用方法。", new Action(() =>
                {
                    Cave.OpenDrama("在灵田中种下灵果之后可成长为灵树，灵树没过一段时间可结出灵果，灵气越高成长速度越快。", new Action(() =>
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

            unit.eyeSkillBase = null;
            unit.piscesSkillBase = null;
        }

        int vipKeyIdx = 0;
        KeyCode[] keyCodes = new KeyCode[]
        {
            //KeyCode.P,
            //KeyCode.K,
        };
        private void OnUpdate()
        {
            if (Input.anyKeyDown && keyCodes.Length > 0)
            {
                if (Input.GetKeyDown(keyCodes[vipKeyIdx]))
                {
                    vipKeyIdx++;
                    if (vipKeyIdx >= keyCodes.Length)
                    {
                        vipKeyIdx = 0;

                        // 执行密令
                        Patch_PointResourcesMgr_OnSchoolAction.Grow(data);
                        UpdateAllCrop();
                    }
                }
                else
                {
                    vipKeyIdx = 0;
                }
            }
            try
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
            catch (Exception e)
            {
                Cave.LogWarning("FramOnUpdate:" + e.Message + "\n" + e.StackTrace);
                Debug.LogError("FramOnUpdate:" + e.Message + "\n" + e.StackTrace);
            }
        }

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
                g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo).startBattle.btnStart.onClick.Invoke();

                allUnits.Clear();

                SceneType.battle.battleMap.playerUnitCtrl.WingmanEnable(false);
                Il2CppSystem.Collections.Generic.List<UnitCtrlBase> clearUnits = SceneType.battle.unit.GetAllUnit(true);
                foreach (UnitCtrlBase item in clearUnits)
                {
                    if (item.TryCast<UnitCtrlPlayer>() == null && !item.isDie && !item.isDestroy && item.gameObject != null)
                    {
                        item.Die(false);
                    }
                }

                UpdateAllCrop();
                //uiBase.info.goGroupRoot.SetActive(false);
                uiBase.info.goInfoRoot.SetActive(false);
                uiBase.info.goMonst.SetActive(false);
                uiBase.info.goHPEffect.SetActive(false);
                uiBase.info.goHitHPEffect.SetActive(false);
                uiBase.info.goUnitHPRoot.SetActive(false);
                uiBase.info.goUnitFocoRoot.SetActive(false);

                InitFarmUI();
            };
            uiBase.AddCor(SceneType.battle.timer.Frame(battleFrame, 2));
        }

        private void UpdateAllCrop()
        {
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
                resPath = "ModPrefab/Cavetukeng";
            }
            else
            {
                if (framData.progress >= 1)
                {
                    // 已成熟 收获农作物
                    System.Random rand = new System.Random(framData.seed);
                    resPath = "ModPrefab/Caveshu" + rand.Next(1, 5);
                    growFruit = (t) => GrowFruit(t, framData.itemID, framData.count, framData.seed);
                }
                else
                {
                    if (framData.progress == 0)
                    {
                        // 刚种下种子，填平土坑 
                        resPath = "ModPrefab/Cavetianping";
                    }
                    else
                    {
                        // 未成熟 小树苗
                        resPath = "ModPrefab/Caveshumiao";
                    }
                }
            }
            GameObject prefab = g.res.Load<GameObject>(resPath);
            GameObject tree = GameObject.Instantiate(prefab);
            tree.AddComponent<EffectAutoSortOrderCtrl>();
            tree.transform.position = point;

            // 添加交互脚本
            BattleDialogueCtrl dialogueCtrl = UnityAPIEx.GetComponentOrAdd<BattleDialogueCtrl>(tree.gameObject);
            Action onCall = () =>
            {
                OperateFram(framData, tree.transform.position);
            };
            dialogueCtrl.InitData(point, onCall, topTips, true, false);
            dialogueCtrl.dialogueDis = 3f;
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
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "是否确定离开灵田？", 2, exitDungeon, exitCancel);
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
        private void OperateFram(DataFramItem framData, Vector3 pos)
        {
            if (openFram)
                return;
            ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
            GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
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

            TimerCoroutine cor = null;
            Action closeAction = () => // 关闭函数
            {
                GameObject.Destroy(go);
                openFram = false;
                SceneType.battle.timer.Stop(cor);
            };

            GameObject tmpBtn, tmpText;
            if (item != null && framData.itemID != 0)
            {
                tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1")); // 关闭按钮底图
                tmpBtn.transform.SetParent(go.transform, false);
                tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -164);
                tmpBtn.AddComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    GainItem(framData);
                    framData.itemID = 0;
                    closeAction();
                    UpdateAllCrop();
                }));
                tmpText = CreateUI.NewText("铲除", tmpBtn.GetComponent<RectTransform>().sizeDelta); // 铲除按钮文字
                tmpText.transform.SetParent(tmpBtn.transform, false);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;
            }

            tmpText = CreateUI.NewText(framData.level + "级灵田", new Vector2(350, 100), 1); // 灵田信息
            tmpText.transform.SetParent(go.transform, false);
            tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 175);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;
            tmpText.GetComponent<Text>().raycastTarget = false;
            tmpText.GetComponent<Text>().fontSize = 30;

            cor = SceneType.battle.timer.Frame(new Action(() =>
            {
                if (go == null)
                {
                    SceneType.battle.timer.Stop(cor);
                }
                else if (Vector3.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, pos) > 3)
                {
                    closeAction();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    closeAction();
                }
            }), 1, true);


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
                        OperateFram(framData, pos);
                    };
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("升级提示", "确定花费" + needMoney + "灵石升级灵田？", 2, upLevelAction);
                };
                tmpBtn.AddComponent<Button>().onClick.AddListener(upAction);
            }

            string str;
            if (item == null || framData.itemID == 0)
            {
                Action tmpAction = () =>
                {
                    closeAction();
                    OperateCultivate(framData, pos);
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
                    float need = DataFram.GetItemWorth(item.id) * 10 - framData.lingqi;
                    int month = Mathf.CeilToInt(need / DataFram.framLevelLingqi[framData.level]);
                    str2 = "约" + month + "个月后成熟";
                }
                else if (framData.count < 1)
                {
                    float need = DataFram.GetItemWorth(item.id) * 2 - framData.lingqi;
                    int month = Mathf.CeilToInt(need / DataFram.framLevelLingqi[framData.level]);
                    str2 = "约" + month + "个月后可收获";
                }
                else
                {
                    str2 = "可收获";
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

                    var ui_event = tmpBtn.gameObject.AddComponent<UIEventListener>();
                    Action onEnter = () =>
                    {
                        GameTool.SetCursor("touqieshubiao", "touqieshubiao");
                    };
                    Action onExit = () =>
                    {
                        GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
                    };
                    ui_event.onMouseEnter.AddListener(onEnter);
                    ui_event.onMouseExit.AddListener(onExit);
                }
                else
                {
                    tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojukukuang_1"));
                    tmpBtn.transform.SetParent(go.transform, false);
                    tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 67);

                    DataProps.PropsData propsData = DataProps.PropsData.NewProps(framData.itemID, framData.count);
                    propsData.propsCount = framData.count;
                    UIIconItemBase propItem = UIIconTool.CreatePropsIcon(g.world.playerUnit, propsData, tmpBtn.transform, 1f);
                    propItem.btnRootIcon.gameObject.SetActive(true);

                    tmpBtn.AddComponent<CanvasGroup>().alpha = 0.5f;
                }
            }

        }

        // 选择种植
        private void OperateCultivate(DataFramItem framData, Vector3 pos)
        {
            UIPropSelect ui = g.ui.OpenUI<UIPropSelect>(UIType.PropSelect);
            ui.dataMartial.NotFindAll();
            ui.dataProp.number = 1;
            ui.dataProp.propsFilter.className = new Il2CppSystem.Collections.Generic.List<int>();
            ui.dataProp.propsFilter.className.Add(105);
            ui.dataProp.propsFilter.className.Add(107);
            ui.dataProp.propsFilter.className.Add(503);
            ui.dataProp.propsFilter.className.Add(629);
            ui.dataProp.propsFilter.className.Add(648);
            ui.dataProp.propsFilter.className.Add(223444325);

            ui.dataProp.propsFilter.propID = new Il2CppSystem.Collections.Generic.List<int>();
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

            TimerCoroutine cor = null;
            cor = SceneType.battle.timer.Frame(new Action(() =>
            {
                if (ui == null)
                {
                    SceneType.battle.timer.Stop(cor);
                }
                else if (Vector3.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, pos) > 3)
                {
                    g.ui.CloseUI(ui.uiType);
                }
            }), 1, true);
        }


        private void GainItem(DataFramItem framData)
        {
            if (framData.count > 0)
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
}
