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

namespace CaveFram
{
    // 灵田
    public class BuildFarm: ClassBase
    {
        public const int prisonerLuckId = 123010217; // 囚犯气运ID
        UIBase uiBase;
        Transform ui;
        int level;
        DataFram data;
        List<WorldUnitBase> units = new List<WorldUnitBase>(); // 被关押的单位
        // 初始化灵田
        public override void Init(string param)
        {
            Debug.Log("初始化灵田 " + param);
            MainCave.Close();
            DataCave dataCave = DataCave.ReadData();
            this.units.Clear();
            var units = g.world.unit.GetUnitExact(dataCave.GetPoint(), 1, true, false);
            foreach (var unit in units)
            {
                MelonLoader.MelonLogger.Msg(unit.data.unitData.propertyData.GetName() + ":" + (unit == g.world.playerUnit || unit.GetLuck(prisonerLuckId) == null));
                if (unit == g.world.playerUnit || unit.GetLuck(prisonerLuckId) == null)
                {
                    continue;
                }
                this.units.Add(unit);
            }
            level = dataCave.GetBuildLevel(3001);
            UIGameVote uiGameVote = g.ui.OpenUI<UIGameVote>(UIType.GameVote);
            uiBase = uiGameVote;
            ui = uiBase.transform;
            for (int i = 0; i < ui.childCount; i++)
            {
                if (ui.GetChild(i).name != "G:btnClose")
                {
                    ui.GetChild(i).gameObject.SetActive(false);
                }
            }
            GameObject.Destroy(ui.GetComponent<UIFastClose>());
            uiGameVote.btnClose.onClick.RemoveAllListeners();

            UIMask.disableFastKey++;
            Action closeAction = () =>
            {
                g.ui.CloseUI(UIType.GameVote);
                DataFram.SaveData(data);
                UIMask.disableFastKey--;
            };
            uiGameVote.btnClose.onClick.AddListener(closeAction);

            data = DataFram.ReadData();
            CreateBattle();
        }

        // 创建战斗场景
        private void CreateBattle()
        {
            g.ui.GetUI(UIType.MapMain).gameObject.SetActive(false);
            g.ui.OpenUI(UIType.Mask);
            //打开战斗场景
            TimerCoroutine cor = null;

            Action frameAction = () => {
                if (this == null)
                {
                    g.timer.Stop(cor);
                    return;
                }
                if (SceneType.battle == null)
                {
                    g.timer.Stop(cor);
                    Action onBattleStart = () => {
                        if (this != null)
                        {
                            OnCreateBattle();

                            Action delayAction = () =>
                            {
                                if (this != null)
                                {
                                    g.ui.CloseUI(UIType.Mask);
                                    SceneType.battle.battleMap.playerUnitCtrl.conf = new ConfMgrVariant();
                                }
                            };
                            g.timer.Frame(delayAction, 3);
                        }
                    };
                    g.events.On(EBattleType.BattleStart, onBattleStart, 1);

                    DataMap.MonstData monstData = new DataMap.MonstData();
                    monstData.id = 10;
                    monstData.SetLevel(1);
                    g.world.battle.IntoBattleInit(new Vector2Int(-1, -1), monstData);
                    g.scene.LoadSceneAsync(SceneType.Battle, null, null, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    g.ui.CloseUI(UIType.LoadingBig);
                }
            };

            cor = g.timer.Frame(frameAction, 1, true);

            Action<Il2CppSystem.Action> onCloseHandlerCall = (call) => {
                SceneType.battle.Destroy();

                g.ui.CloseUI(UIType.BattleInfo);

                SceneType.map.root.transform.position = Vector3.zero;
                g.ui.OpenUI(UIType.Mask);
                TimerCoroutine corClose = null;
                Action closeAction = () => {
                    if (this == null)
                    {
                        g.timer.Stop(corClose);
                        return;
                    }
                    if (SceneType.battle != null)
                    {
                        g.timer.Stop(corClose);
                        Action onUnloadScene = () => {
                            g.ui.CloseUI(UIType.Mask);

                            SceneType.map.cameraCtrl.SetPosition(SceneType.map.world.playerRoot.transform.position);
                            SceneType.map.world.UpdatePathEffectTip();
                            SceneType.map.cameraCtrl.isTarget = true;
                            g.ui.GetUI(UIType.MapMain).gameObject.SetActive(true);
                            call.Invoke();
                        };
                        g.scene.UnloadScene(SceneType.Battle, onUnloadScene);
                    }
                };
                corClose = g.timer.Frame(closeAction, 1, true);
            };

            uiBase.onCloseHandlerCall = onCloseHandlerCall;
        }


        List<UnitCtrlBase> allFram = new List<UnitCtrlBase>(); // 所以的灵田单位
        List<UnitCtrlBase> allUnits = new List<UnitCtrlBase>(); // 所以的关押单位
        private void OnCreateBattle()
        {
            g.ui.CloseUI(UIType.Mask);

            Action battleFrame = () =>
            {
                g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo).startBattle.btnStart.onClick.Invoke();

                allFram.Clear();
                allUnits.Clear();

                int unitIdx = 0, max = 100;
                Vector2 startPoint = SceneType.battle.battleMap.roomCenterPosi + new Vector2(-30, -10);
                // 关押的单位
                MelonLoader.MelonLogger.Msg("关押的NPC数量：" + units.Count);
                foreach (WorldUnitBase unit in units)
                {
                    MelonLoader.MelonLogger.Msg("关押的NPC "+ unit.data.unitData.propertyData.GetName());
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
                //int line = 4;
                int low = 5;
                startPoint = SceneType.battle.battleMap.roomCenterPosi + new Vector2(-20, -6);

                for (int i = 0; i < level; i++)
                {
                    DataFramItem framData = data.data[i];
                    int idx = i;
                    Action onCollect = () =>
                    {
                        //Cave.Log(idx + " 互动3");
                    };

                    int monstId = 8027;
                    Vector2 resPoint = startPoint + new Vector2(i / low * 8, i % low * 4);
                    UnitCtrlBase fram = SceneType.battle.unit.CreateNPC(
                     resPoint,
                        monstId, onCollect, UnitType.NPC, true);

                    BattleDialogueCtrl dialogueCtrl = fram.gameObject.GetComponent<BattleDialogueCtrl>();
                    dialogueCtrl.isHideDownKeyEffect = true;
                    dialogueCtrl.downKeyDuration = 1;

                    Action onDownContinueKeyCall = () =>
                    {
                        //Cave.Log(idx + " 互动1");
                        OperateFram(framData, fram);
                    };
                    dialogueCtrl.onDownContinueKeyCall = onDownContinueKeyCall;

                    //Action onEndContinueKeyCall = () =>
                    //{
                    //    Cave.Log(idx + " 互动2");
                    //};
                    //dialogueCtrl.onEndContinueKeyCall = onEndContinueKeyCall;

                    allFram.Add(fram);
                    //fram.transform.Find("Root/Body").localScale = new Vector3(GetHeight(framData), GetHeight(framData), GetHeight(framData));
                }

                // 玩家位置
                UnitCtrlPlayer player = SceneType.battle.battleMap.playerUnitCtrl;
                player.move.SetPosition(startPoint + new Vector2(-2, 0));


                Action framTest = () =>
                {
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        Action<UnitCtrlMonst> call = (monst) =>
                        {
                            monst.posiTop.localPosition = new Vector2(0, 0.5f);
                            monst.triggerBox.enabled = true;
                            monst.moveBox.enabled = true;
                        };
                        SceneType.battle.unit.CreateUnitMonst(123000010, SceneType.battle.battleMap.roomCenterPosi + new Vector2(CommonTool.Random(-10, 10), CommonTool.Random(-7, 7)), UnitType.Monst, 8, call, null);
                    }
                };
                SceneType.battle.timer.Frame(framTest, 1, true);
            };
            uiBase.AddCor(SceneType.battle.timer.Frame(battleFrame, 2));
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
        private void OperateFram(DataFramItem framData, UnitCtrlBase fram)
        {
            if (openFram)
                return;
            GameObject goInfo = CreateUI.NewImage();

            Cave.Cave.Log("操作农田");
            openFram = true;
            GameObject go = CreateUI.NewImage();
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(9999, 9999);
            go.transform.SetParent(ui.transform, false);
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
                new Log("关闭操作农田");
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
                    new Log("升级操作农田");
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
                        OperateFram(framData, fram);
                        //fram.transform.Find("Root/Body").localScale = new Vector3(GetHeight(framData), GetHeight(framData), GetHeight(framData));
                    };
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("升级提示", "确定花费" + needMoney + "灵石升级灵田？", 2, upLevelAction);
                };
                tmpBtn.AddComponent<Button>().onClick.AddListener(upAction);
            } else

            new Log("当前道具："+ framData.itemID+"  种植进度："+ framData.progress);

            string str;
            ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
            if (item == null || framData.itemID == 0)
            {
                new Log("未种植灵种");
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
                str = GameTool.LS(item.name);
                new Log(((int)(framData.progress * 100)) + "成长中 " + GameTool.LS(item.name));
                int aa = (int)(framData.progress * 100);
                int bb = (int)(framData.progress * 1000) - aa * 10;
                str = GameTool.LS(item.name) + " " + aa + "." + bb + "%";

                tmpText = CreateUI.NewText(str, new Vector2(250, 30));
                tmpText.transform.SetParent(go.transform, false);
                tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, 5);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

                tmpText = CreateUI.NewText("约" + Mathf.CeilToInt((1 - framData.progress) / (1f / BuildFarm.GetNeedTime(framData.level, item.level))) + "个月后结果", new Vector2(250, 30));
                tmpText.transform.SetParent(go.transform, false);
                tmpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -30);
                tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tmpText.GetComponent<Text>().color = Color.black;

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
        /*
        同等级每年产出1-2颗
        每个月增加的进度有20%随机偏移
        */

        public static int GetNeedTime(int framLevel, int itemLevel)
        {
            if (framLevel == itemLevel)
            {
                return 12;
            }
            if (framLevel == itemLevel + 1)
            {
                return 6;
            }
            if (framLevel == itemLevel + 2)
            {
                return 3;
            }
            if (framLevel == itemLevel + 3)
            {
                return 2;
            }
            if (framLevel > itemLevel + 3)
            {
                return 1;
            }
            if (framLevel + 1 == itemLevel)
            {
                return 24;
            }
            if (framLevel + 2 == itemLevel)
            {
                return 48;
            }
            if (framLevel + 3 == itemLevel)
            {
                return 96;
            }
            if (framLevel + 3 < itemLevel)
            {
                return 128;
            }
            return 12;
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
                    new Log("选择道具：" + select[i].propsID);
                    framData.itemID = select[i].propsID;
                    framData.progress = 0;
                    framData.count = 1;
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

            framData.itemID = 0;
            framData.progress = 0;
            framData.count = 0;
        }
    }
}
