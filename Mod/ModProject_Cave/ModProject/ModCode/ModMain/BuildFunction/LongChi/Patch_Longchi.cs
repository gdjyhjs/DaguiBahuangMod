using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UIType;
using GuiBaseUI;

namespace Cave
{
    [HarmonyPatch(typeof(UIDragonDoorUpgrade), "Init")]
    public class Patch_UIDragonDoorUpgrade_Init
    {
        public static bool openCaveLongChi = false; // 是否正在打开洞府月亮泉龙池
        public static bool onCaveLongChi = false; // 当前打开的是否洞府月亮泉龙池
        public static int dragonSex = 2; // 龙性别
        static UIDragonDoorUpgrade ui;
        [HarmonyPostfix]
        private static void Postfix(UIDragonDoorUpgrade __instance)
        {
            if (!openCaveLongChi)
                return;
            ui = __instance;
            try
            {
                InitBuild();
            }
            catch (Exception e)
            {
                Cave.Log("化龙池错误：\n" + e.Message + "\n" + e.StackTrace);
            }

            // 加个关闭按钮
            var go1 = CreateUI.NewButton(() =>
            {
                openCaveLongChi = false;
                onCaveLongChi = false;
                g.ui.CloseUI(UIType.DragonDoorUpgrade);
            }, SpriteTool.GetSprite("SchoolCommon", "Liantiniu_1"));
            go1.transform.SetParent(ui.transform.Find("Root"), false);
            go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-686, -430);

            var go2 = CreateUI.NewText("退出", go1.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(go1.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            go2.GetComponent<Text>().color = Color.black;
            openCaveLongChi = false;
        }

        private static void InitBuild()
        {
            onCaveLongChi = true;
            var list = g.world.build.GetBuilds<MapBuildSchool>();
            foreach (var item in list)
            {
                if (item.gridData.areaBaseID == 8)
                {
                    ui.school = item;
                    break;
                }
            }

            g.data.obj.SetString("UIDragonDoorUpgrade", "OpenDragonDoor", "1");
            dragonSex = g.data.obj.GetInt("DramaFunctionTool_SetWorldData", "www.yellowshange.com.caveDragonSex");

            AddEffect();
            InitText();
            InitBtn();
            UpdateTips();
            ui.UpdateUI();
            var titleGo = ui.transform.GetChild(4);
            titleGo.gameObject.SetActive(false);
            var title = GuiBaseUI.CreateUI.NewText(GameTool.LS("schoolBuildingName1112"), new Vector2(320, 70), 1);
            title.transform.SetParent(titleGo.transform.parent, false);
            title.GetComponent<RectTransform>().anchoredPosition = titleGo.GetComponent<RectTransform>().anchoredPosition;
            title.GetComponent<Text>().color = Color.black;
            title.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            title.GetComponent<Text>().fontSize = 46;
        }

        public static void LongChiDataInit()
        {
            if (!g.data.obj.ContainsKey("www.yellowshange.com", "caveDragonBattle"))
            {
                bool show = g.world.playerUnit.GetLuck(g.conf.schoolDragonDoorInfo.dragonLuck5) != null && g.data.buildSchool.dragonDoor.GetSex(g.world.playerUnit.data.unitData.unitID) != UnitSexType.None;
                g.data.obj.SetString("www.yellowshange.com", "caveDragonBattle", show ? 1 : 0);
            }
            if (!g.data.obj.ContainsKey("DramaFunctionTool_SetWorldData", "www.yellowshange.com.caveDragonSex"))
            {
                if (g.world.playerUnit.GetLuck(g.conf.schoolDragonDoorInfo.dragonLuck5) != null)
                {
                    g.data.obj.SetString("DramaFunctionTool_SetWorldData", "www.yellowshange.com.caveDragonSex", (int)g.data.buildSchool.dragonDoor.GetSex(g.world.playerUnit.data.unitData.unitID));
                }
                else
                {
                    g.data.obj.SetString("DramaFunctionTool_SetWorldData", "www.yellowshange.com.caveDragonSex", 2);
                }
            }
        }

        public static void UpdateTips()
        {
            GameObject[] gos = new GameObject[] { ui.goLevelPoint1, ui.goLevelPoint2, ui.goLevelPoint3, ui.goLevelPoint4, ui.goLevelPoint5 };
            for (int idx = 0; idx < 5; idx++)
            {
                int lv = idx + 1;
                string str = "";
                var buffs = g.data.buildSchool.dragonDoor.GetPlayerBuffs();
                if (buffs.ContainsKey(lv)) // 有这个等级的效果
                {
                    List<string> des = new List<string>(buffs[lv].Count + 1) { string.Format(GameTool.LS("dragonDoor_effectTitle"), lv) };
                    for (int i = 0; i < buffs[lv].Count; i++)
                    {
                        int buffID = buffs[lv][i];
                        ConfBattleEffectItem effectItem =
                            g.conf.battleEffect.GetItem(g.conf.schoolDragonDoorBuff.GetEffect(buffID));
                        string effectDes = UIMartialInfoTool.GetDescRichText(GameTool.LS(effectItem.desc),
                            new BattleSkillValueData(g.world.playerUnit), 2);
                        des.Add(effectDes);
                    }

                    if (des.Count > 1)
                    {
                        str = string.Join(" \n", des);
                    }
                }
                gos[idx].AddComponent<UISkyTipEffect>().InitData(str);
            }
        }

        private static void InitText()
        {
            ui.gameObject.AddComponent<UIFastClose>();
            ui.textDes.text = GameTool.LS("dragonDoor_des");
            ui.textIntimacy.text = GameTool.LS("dragonDoor_intimacy");
            ui.textUpgrade.text = GameTool.LS("dragonDoor_upgrade");
            ui.textUpgradeCostTips.text = GameTool.LS("dragonDoor_upgradeCostTips");
            ui.textDesTitle.text = GameTool.LS("dragonDoor_desTitle");
            ui.textUpgradeEffectTips.text = GameTool.LS("dragonDoor_upgradeEffectTips");
            ui.textTransfiguration.text = GameTool.LS("dragonDoor_transfiguration");
            ui.textTransOk.text = GameTool.LS("dragonDoor_transOk");
            GameEffectTool.CreateGo("Effect/UI/genpilong_huaxing", ui.btnTransfiguration.transform, ui.sortingOrder + 1);
            ui.textDialog.text = GameTool.LS("npcInfo_duihua");
            ui.textDialogDragon.text = GameTool.LS("npcInfo_duihua");
            ui.InitTransInputListener();
            ui.imgLevelMaxTips.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("dragonDoor_levelMaxTips"));
            ui.imgLevelTips.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("dragonDoor_levelTips"));
            ui.btnDialog.gameObject.AddComponent<UIButtonGray>();
            ui.btnDialogDragon.gameObject.AddComponent<UIButtonGray>();
        }

        private static void InitBtn()
        {
            ui.btnUpgrade.onClick.AddListener(new Action(() =>
            {
                int areaID = 10;
                ConfSchoolDragonDoorItem conf = g.data.buildSchool.dragonDoor.GetConf(areaID);
                var cost = Mathf.Max(1000, conf.upgradeCostMoney + conf.upgradeCostMedicina * 10 + conf.upgradeCostMine * 10);
                int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                if (money < cost)
                {
                    UITipItem.AddTip(GameTool.LS("dragonDoor_upgradeFailCost"));
                    return;
                }
                g.data.buildSchool.dragonDoor.poolLevel++;
                conf = g.data.buildSchool.dragonDoor.GetConf(areaID);
                CallQueue cq = new CallQueue();
                int length = Mathf.Max(conf.dragonBuff.Length, conf.playerBuff.Length);
                for (int i = 0; i < length; i++)
                {
                    List<int> buffs = new List<int>();
                    if (conf.dragonBuff.Length > 0) buffs.AddRange(conf.dragonBuff[i]);
                    if (conf.playerBuff.Length > 0) buffs.AddRange(conf.playerBuff[i]);
                    buffs.RemoveAll(v => v == 0 || g.data.buildSchool.dragonDoor.ContainsBuff(g.world.playerUnit.data.unitData.unitID, v));
                    cq.Add(new Action(() =>
                    {
                        Il2CppSystem.Collections.Generic.List<DataStruct<string, string>> des = new Il2CppSystem.Collections.Generic.List<DataStruct<string, string>>(buffs.Count);
                        for (int j = 0; j < buffs.Count; j++)
                        {
                            int buffID = buffs[j];
                            ConfBattleEffectItem effectItem = g.conf.battleEffect.GetItem(g.conf.schoolDragonDoorBuff.GetEffect(buffID));
                            DataStruct<string, string> effectDes = new DataStruct<string, string>(GameTool.LS(effectItem.name), UIMartialInfoTool.GetDescRichText(GameTool.LS(effectItem.desc), new BattleSkillValueData(g.world.playerUnit), 2));
                            des.Add(effectDes);
                        }

                        g.ui.OpenUI<UIDragonDoorSelectEffect>(UIType.DragonDoorSelectEffect).InitData(des, new Action<int>((idx) =>
                        {
                            g.data.buildSchool.dragonDoor.AddBuff(g.world.playerUnit.data.unitData.unitID, g.data.buildSchool.dragonDoor.GetLevel(areaID), buffs[idx]);
                            cq.Next();
                        }));
                    }));
                }

                cq.Add(new Action(() =>
                {
                    ui.UpdateUI();
                    ui.PlayUpgradeEffect();
                }));
                cq.Run();
            }));

            // 出战按钮
            {
                GameObject changeBattleGo = GameObject.Instantiate(ui.btnDialog.gameObject);
                changeBattleGo.transform.SetParent(ui.btnDialog.transform.parent, false);
                changeBattleGo.GetComponentInChildren<Text>().text = g.data.obj.GetInt("www.yellowshange.com", "caveDragonBattle") == 1 ? GameTool.LS("Cave_LongBattle") : GameTool.LS("Cave_LongIdle");
                Button changeBattleBtn = changeBattleGo.GetComponent<Button>();
                changeBattleBtn.onClick.RemoveAllListeners();
                changeBattleBtn.onClick.AddListener(new Action(() =>
                {
                    var value = g.data.obj.GetInt("www.yellowshange.com", "caveDragonBattle") == 1 ? 0 : 1;
                    g.data.obj.SetString("www.yellowshange.com", "caveDragonBattle", value);
                    string str = value == 1 ? GameTool.LS("Cave_LongBattleTalk") : GameTool.LS("Cave_LongIdleTalk");
                    ui.ShowDialog(str, new Vector2(751f, 270f));
                    changeBattleGo.GetComponentInChildren<Text>().text = value == 1 ? GameTool.LS("Cave_LongBattle") : GameTool.LS("Cave_LongIdle");
                }));
                changeBattleGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(ui.btnDialog.GetComponent<RectTransform>().anchoredPosition.x, ui.btnDialog.GetComponent<RectTransform>().anchoredPosition.y + 50);
                changeBattleGo.SetActive(true);
            }

            // 性别按钮
            {
                GameObject changeBattleGo = GameObject.Instantiate(ui.btnDialog.gameObject);
                changeBattleGo.transform.SetParent(ui.btnDialog.transform.parent, false);
                changeBattleGo.GetComponentInChildren<Text>().text = "性别";
                Button changeBattleBtn = changeBattleGo.GetComponent<Button>();
                changeBattleBtn.onClick.RemoveAllListeners();
                changeBattleBtn.onClick.AddListener(new Action(() =>
                {
                    dragonSex = dragonSex == 1 ? 2 : 1;
                    g.data.obj.SetString("DramaFunctionTool_SetWorldData", "www.yellowshange.com.caveDragonSex", dragonSex);
                    Patch_UIMgr_CloseUI.DelEffect();
                    AddEffect();
                    ui.UpdateUI();
                }));
                changeBattleGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(ui.btnDialog.GetComponent<RectTransform>().anchoredPosition.x, ui.btnDialog.GetComponent<RectTransform>().anchoredPosition.y + 100);
                changeBattleGo.SetActive(true);
            }

            ui.btnDialog.onClick.AddListener(new Action(() =>
            {
                g.data.buildSchool.dragonDoor.talkCount = 999;
            }));
        }

        public static void AddEffect()
        {
            int level = g.data.buildSchool.dragonDoor.GetLevel(10);
            WorldUnitBase unit = g.world.playerUnit;

            var unitLucks = g.data.world.playerLog.upGrade;
            if (!unitLucks.ContainsKey(700188))
            {
                var data = new DataWorld.World.PlayerLogData.GradeData();
                data.luck = 700188;
                data.quality = 1;
                unitLucks.Add(700188, data);
                g.data.buildSchool.dragonDoor.SetSex(g.world.playerUnit.data.unitData.unitID, (UnitSexType)dragonSex);
            }

            WorldUnitLuckBase luck = unit.GetLuck(700188);
            if (luck == null)
            {
                int err = unit.CreateAction(new UnitActionLuckAdd(700188));
                Console.WriteLine("添加气运");
            }
        }
    }


    [HarmonyPatch(typeof(UIDragonDoorUpgrade), "UpdateUI")]
    public class Patch_UIDragonDoorUpgrade_UpdateUI
    {
        static UIDragonDoorUpgrade ui;
        [HarmonyPostfix]
        private static void Postfix(UIDragonDoorUpgrade __instance)
        {
            ui = __instance;
            int areaID = 10;
            int level = g.data.buildSchool.dragonDoor.GetLevel(areaID);
            int maxLevel = g.data.buildSchool.dragonDoor.GetMaxLevel(areaID);
            try
            {
                ConfSchoolDragonDoorItem conf = g.data.buildSchool.dragonDoor.GetConf(areaID);
                ui.textLevel.text = string.Format(GameTool.LS("dragonDoor_level"), level);
                ui.textMaxLevel.text = string.Format(GameTool.LS("dragonDoor_maxLevel"), maxLevel);
                ui.ptextUpgradeCost2.gameObject.SetActive(false);
                ui.ptextUpgradeCost3.gameObject.SetActive(false);
                ui.ptextMoney2.gameObject.SetActive(false);
                ui.ptextMoney3.gameObject.SetActive(false);
                if (level >= maxLevel)
                {
                    ui.ptextUpgradeCost1.text = string.Format(GameTool.LS("dragonDoor_money1"), "-");
                    ui.textUpgradeCostTips.gameObject.SetActive(false);
                }
                else
                {
                    var cost = Mathf.Max(1000, conf.upgradeCostMoney + conf.upgradeCostMedicina * 10 + conf.upgradeCostMine * 10);
                    ui.ptextUpgradeCost1.text = string.Format(GameTool.LS("dragonDoor_money1"), cost);
                    ui.textUpgradeCostTips.gameObject.SetActive(true);
                }
                int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                ui.ptextMoney1.text = string.Format(GameTool.LS("dragonDoor_money1"), money); // 灵石
                ui.ptextMoney1.transform.localPosition = ui.ptextMoney2.transform.localPosition;
                ui.ptextUpgradeCost1.transform.localPosition = ui.ptextUpgradeCost2.transform.localPosition;
                // 如果满级隐藏升级按钮
                ui.btnUpgrade.gameObject.SetActive(g.data.buildSchool.dragonDoor.poolLevel < g.data.buildSchool.dragonDoor.GetMaxLevel(10));
            }
            catch (Exception e)
            {
                Cave.Log(e.Message + "\n" + e.StackTrace);
            }

            try
            {
                // 增加修改按钮
                var childCount = ui.goRoot.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    int lv = i + 1;
                    var go = ui.goRoot.transform.GetChild(i).Find("Name").gameObject;
                    go.GetComponent<Text>().raycastTarget = true;
                    var btn = UnityAPIEx.GetComponentOrAdd<Button>(go);
                    btn.onClick.AddListener(new Action(() =>
                    {
                        ChangeEffect(lv);
                    }));
                }
            }
            catch (Exception e)
            {
                Cave.Log(e.Message + "\n" + e.StackTrace);
            }
        }

        //变更一个效果
        private static void ChangeEffect(int lv)
        {
            try
            {
                var effects = g.data.buildSchool.dragonDoor.unitsBuffs[g.world.playerUnit.data.unitData.unitID];
                if (effects.ContainsKey(lv))
                {
                    effects[lv].Clear();
                }

                ConfSchoolDragonDoorItem conf = g.conf.schoolDragonDoor.GetItem(lv);
                CallQueue cq = new CallQueue();
                int length = Mathf.Max(conf.dragonBuff.Length, conf.playerBuff.Length);
                for (int i = 0; i < length; i++)
                {
                    Il2CppSystem.Collections.Generic.List<int> buffs = new Il2CppSystem.Collections.Generic.List<int>();
                    if (conf.dragonBuff.Length > 0)
                    {
                        foreach (var v in conf.dragonBuff[i])
                        {
                            if (v == 0 || g.data.buildSchool.dragonDoor.ContainsBuff(g.world.playerUnit.data.unitData.unitID, v))
                            {
                            }
                            else
                            {
                                buffs.Add(v);
                            }
                        }
                    }
                    if (conf.playerBuff.Length > 0)
                    {
                        foreach (var v in conf.playerBuff[i])
                        {
                            if (v == 0 || g.data.buildSchool.dragonDoor.ContainsBuff(g.world.playerUnit.data.unitData.unitID, v))
                            {
                            }
                            else
                            {
                                buffs.Add(v);
                            }
                        }
                    }
                    cq.Add(new Action(() =>
                    {
                        Il2CppSystem.Collections.Generic.List<DataStruct<string, string>> des = new Il2CppSystem.Collections.Generic.List<DataStruct<string, string>>(buffs.Count);
                        for (int j = 0; j < buffs.Count; j++)
                        {
                            int buffID = buffs[j];
                            ConfBattleEffectItem effectItem =
                                g.conf.battleEffect.GetItem(g.conf.schoolDragonDoorBuff.GetEffect(buffID));
                            DataStruct<string, string> effectDes = new DataStruct<string, string>(
                                GameTool.LS(effectItem.name),
                                UIMartialInfoTool.GetDescRichText(GameTool.LS(effectItem.desc),
                                    new BattleSkillValueData(g.world.playerUnit), 2));
                            des.Add(effectDes);
                        }

                        g.ui.OpenUI<UIDragonDoorSelectEffect>(UIType.DragonDoorSelectEffect).InitData(des, new Action<int>(idx =>
                        {
                            g.data.buildSchool.dragonDoor.AddBuff(g.world.playerUnit.data.unitData.unitID, lv, buffs[idx]);
                            cq.Next();
                        }));
                    }));
                }

                cq.Add(new Action(() =>
                {
                    try
                    {
                        ui.UpdateUI();
                    }
                    catch (Exception e)
                    {
                        Cave.Log(e.Message + "\n" + e.StackTrace);
                    }
                }));
                cq.Run();
            }
            catch (Exception e)
            {
                Cave.Log(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UIMgr), "CloseUI", new Type[] { typeof(UIBase), typeof(bool) })]
    public class Patch_UIMgr_CloseUI
    {
        [HarmonyPrefix]
        public static bool Prefix(UIBase ui)
        {
            UITypeBase uiType = ui.uiType;
            Console.WriteLine("关闭UI ：" + uiType.uiName + "  " + (uiType.uiName == UIType.DragonDoorUpgrade.uiName));
            if (uiType.uiName == UIType.DragonDoorUpgrade.uiName)
            {
                if (Patch_UIDragonDoorUpgrade_Init.openCaveLongChi)
                {
                    return false;
                }
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(UIBase ui)
        {
            UITypeBase uiType = ui.uiType;
            if (uiType.uiName == UIType.DragonDoorUpgrade.uiName)
            {
                if (Patch_UIDragonDoorUpgrade_Init.onCaveLongChi)
                {
                    // 离开青鳞池如果设置不出战则取消气运
                    bool isBattle = g.data.obj.GetInt("www.yellowshange.com", "caveDragonBattle") == 1;
                    Console.WriteLine("是否出战 " + isBattle);
                    if (!isBattle)
                    {
                        DelEffect();
                    }
                }
            }

            UIMapMain mapMain = g.ui.GetUI<UIMapMain>(UIType.MapMain);
            if (mapMain != null)
            {
                mapMain.uiPlayerInfo.ChangeDragon();
            }
        }

        public static void DelEffect()
        {
            int level = g.data.buildSchool.dragonDoor.GetLevel(10);
            WorldUnitBase unit = g.world.playerUnit;
            WorldUnitLuckBase luck = unit.GetLuck(700188);
            if (luck != null)
            {
                int err = unit.CreateAction(new UnitActionLuckDel(luck));
                var unitLucks = g.data.world.playerLog.upGrade;
                if (unitLucks.ContainsKey(700188))
                {
                    Console.WriteLine("删除气运");
                    unitLucks.Remove(700188);
                }
            }
        }
    }


    [HarmonyPatch(typeof(DataBuildSchoolDragonDoor), "IsActivateBuff")]
    public class Patch_DataBuildSchoolDragonDoor_IsActivateBuff
    {
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }



    [HarmonyPatch(typeof(UIDragonDoorUpgrade), "OnTransImageClick")]
    public class Patch_UIDragonDoorUpgrade_OnTransImageClick
    {
        static float lastTime = 0;
        [HarmonyPrefix]
        private static bool Prefix(UIDragonDoorUpgrade __instance)
        {
            if (Time.time - lastTime < 1.5f)
            {
                return false;
            }

            string[] ss = new string[]
            {
                "Cave_LongTalk1",
                "Cave_LongTalk2",
                "Cave_LongTalk3",
            };
            __instance.ShowDialog(GameTool.LS(ss[CommonTool.Random(0, ss.Length)]), new Vector2(751f, 270f));

            lastTime = Time.time;
            return true;
        }
    }
}
