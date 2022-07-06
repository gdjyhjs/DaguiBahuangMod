using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using Cave.BuildFunction;
using Cave.Config;

namespace Cave
{
    public class CaveBuildOpen
    {
        public UIType.UITypeBase uiType;
        public MainCave mainCave;
        public static CaveBuildData openItem;

        public CaveBuildOpen(MainCave mainCave, CaveBuildData caveItem, int level = 0)
        {
            openItem = caveItem;
            this.mainCave = mainCave;
            level = level == 0 ? caveItem.level : level;
            int area = level * 2;
            Il2CppSystem.Collections.Generic.List<MapBuildTown> allBuilds = g.world.build.GetBuilds<MapBuildTown>();
            MapBuildTown build = null;
            foreach (MapBuildTown item in allBuilds)
            {
                if (item.gridData.areaBaseID == area && item.buildTownData.isMainTown)
                {
                    build = item;
                    break;
                }
            }

            Il2CppSystem.Collections.Generic.List<MapBuildSchool> allSchools = g.world.build.GetBuilds<MapBuildSchool>();
            MapBuildSchool school = null;
            foreach (MapBuildSchool item in allSchools)
            {
                if (item.gridData.areaBaseID == area)
                {
                    school = item;
                    break;
                }
            }

            ConfBuildItem conf = ConfBuild.GetItem(caveItem.id);
            if (!string.IsNullOrWhiteSpace(conf.function))
            {
                try
                {
                    bool condition = UnitConditionTool.Condition(conf.condition, null);
                    string function = (condition ? conf.function : conf.failfuntion).Trim();
                    Cave.Log("条件=" + condition + "     function=" + function);
                    if (!DoFunction(function))
                    {
                        UITipItem.AddTip("功能未开放，敬请期待！");
                    }
                }
                catch (Exception e)
                {
                    Cave.Log(e.Message + "\n" + e.StackTrace, 0);
                    UITipItem.AddTip("功能未开放，敬请期待！");
                }
                return;
            }

            // 各种建筑功能
            if (caveItem.id == 2008) // 工坊
            {
                if (build != null)
                {
                    var ui = g.ui.OpenUI<UITownFactory>(UIType.TownFactory);
                    ui.InitData(build);
                    uiType = ui.uiType;

                    if (caveItem.level > 1)
                    {
                        AddArrow(caveItem, ui.gameObject, level);
                    }

                }
            }
            else if (caveItem.id == 2009) // 酒馆
            {
                if (build != null)
                {
                    var ui = g.ui.OpenUI<UITownPub>(UIType.TownPub);
                    ui.InitData(build);
                    uiType = ui.uiType;

                    if (caveItem.level > 1)
                    {
                        AddArrow(caveItem, ui.gameObject, level);
                    }

                }
            }
            else if (caveItem.id == 2011) // 建木
            {
                var allTowns = g.world.build.GetBuilds<MapBuildTown>();
                MapBuildTown town = null;
                int min = int.MaxValue;
                foreach (var item in allTowns)
                {
                    if (item.gridData.areaBaseID == 1)
                    {
                        if (item.points[0].x * item.points[0].y < min)
                        {
                            min = item.points[0].x * item.points[0].y;
                            town = item;
                        }
                    }
                }
                var ui = g.ui.OpenUI<UITownStorage>(UIType.TownStorage);
                ui.InitData(town);
                uiType = ui.uiType;

            }
            else if (caveItem.id == 1007) // 传送阵
            {
                var ui = g.ui.GetUI(UIType.GamePlanTip);
                if (ui != null)
                {
                    g.ui.CloseUI(UIType.GamePlanTip);
                }

                MapBuildTownTransfer.OpenTransfer(null);
                uiType = g.ui.GetUI<UIMinMap>(UIType.MinMap).uiType;

            }
            else if (caveItem.id == 1006) // 灵阁
            {
                UISchoolAura ui = g.ui.OpenUI<UISchoolAura>(UIType.SchoolAura);
                ui.InitData(school);
                Action uiUpdate = () =>
                {
                    if (ui == null)
                        return;

                    // 刷新状态
                    ui.actionSchoolAura.isDiscount = true;
                    ui.actionSchoolAura.money = 0;
                    ui.uISchoolSoulUp.noSchoolFate = true;
                    ui.UpdateAuraState();

                    // 隐藏部分UI
                    ui.uISchoolSoulUp.goGroupRoot.SetActive(false);
                    ui.uISchoolSoulUp.ptextDesc.text = "";
                    ui.textTip.text = "";
                    ui.textCostTitle.gameObject.SetActive(false);
                    ui.textNoFate.text = "等级：" + level;
                    ui.textIntimRate.gameObject.SetActive(false);

                    var com = ui.uISchoolSoulUp.goUpLev.gameObject.GetComponent<UIEventListener>();
                    if (com != null)
                    {
                        GameObject.Destroy(com);
                    }
                };
                uiUpdate();
                g.events.On(EGameType.CloseUIEnd, uiUpdate, -1, true);
                Action onCloseAura = () =>
                {
                    g.events.Off(EGameType.CloseUIEnd, uiUpdate);
                };
                ui.onCloseCall = onCloseAura;
                Action<float> onAuraChange = (f) => { uiUpdate(); };
                ui.sliDay.onValueChanged.AddListener(onAuraChange);
                ui.btnOK.onClick.AddListener(uiUpdate);
            }
            else if (caveItem.id == 1005) // 疗伤院
            {
                var ui = g.ui.OpenUI<UITownHotel>(UIType.TownHotel);
                ui.InitData(build);
                uiType = ui.uiType;
            }
            else
            {
                UITipItem.AddTip("功能暂未开放！");
            }
        }

        public void AddArrow(CaveBuildData caveItem, GameObject parent, int level)
        {
            var image = CreateUI.NewImage(SpriteTool.GetSprite("NPCInfoCommon", "daoxinmingzikuang"));
            image.transform.SetParent(parent.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 250);


            GameObject textGo = CreateUI.NewText(level + "级", image.GetComponent<RectTransform>().sizeDelta);
            textGo.transform.SetParent(parent.transform, false);
            textGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 250);
            var uiName = textGo.GetComponent<Text>();
            uiName.alignment = TextAnchor.MiddleCenter;
            uiName.color = Color.black;

             image = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youjian"));
            image.transform.SetParent(parent.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 250);
            Action btnYou = () =>
            {
                level++;
                if (level > caveItem.level)
                    level = 1;
                g.ui.CloseUI(uiType);
                new CaveBuildOpen(mainCave, caveItem, level);
            };
            image.AddComponent<Button>().onClick.AddListener(btnYou);

            image = CreateUI.NewImage(SpriteTool.GetSprite("Common", "zuojian"));
            image.transform.SetParent(parent.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-730, 250);
            Action btnZuo = () =>
            {
                level--;
                if (level < 1)
                    level = ConfBuild.GetItem(caveItem.id).maxLevel;
                g.ui.CloseUI(uiType);
                new CaveBuildOpen(mainCave, caveItem, level);
            };
            image.AddComponent<Button>().onClick.AddListener(btnZuo);
        }

        public static bool DoFunction(string function)
        {
            if (string.IsNullOrWhiteSpace(function))
                return false;
            DramaFunctionTool.OptionsFunction(function);
            return true;
        }
    }
}