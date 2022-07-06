using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UINPCUnitInfoItem), "InitData")]
    class Patch_UINPCUnitInfoItem_InitData
    {
        public static bool addOpenChangeFace;

        [HarmonyPostfix]
        private static void Postfix(UINPCUnitInfoItem __instance)
        {
            if (addOpenChangeFace)
            {
                UpdateItem(__instance);
            }
        }

        private static void UpdateItem(UINPCUnitInfoItem __instance)
        {
            var parent = __instance.transform;
            if (parent.Find("openChangeFace") != null)
            {
                GameObject.Destroy(parent.Find("openChangeFace").gameObject);
            }
            if (parent.Find("openPlayerTeam") != null)
            {
                GameObject.Destroy(parent.Find("openChangeFace").gameObject);
            }

            {
                // 整容按钮
                var go = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("CreatePlayerCommon", "waimao"));
                go.name = "openChangeFace";
                go.transform.SetParent(parent, false);
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(45, -74);
                var btn = go.AddComponent<Button>();
                if (__instance.unit.data.unitData.GetPoint() != DataCave.ReadData().GetPoint())
                {
                    go.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    Action onClick = () =>
                    {
                        UITipItem.AddTip(__instance.unit.data.unitData.propertyData.GetName() + GameTool.LS("Cave_NpcWaiChu"));
                    };
                    btn.onClick.AddListener(onClick);
                }
                else
                {
                    Action onClick = () =>
                    {
                        var ui = g.ui.GetUI<UINPCSearch>(UIType.NPCSearch);
                        if (ui != null)
                        {
                            g.ui.CloseUI(UIType.NPCSearch);
                        }
                        Action closeDelayAction = () =>
                        {
                            new CreateFace().InitData(__instance.unit);
                        };
                        g.timer.Frame(closeDelayAction, 3, false);
                    };
                    btn.onClick.AddListener(onClick);
                }
            }


            {
                // 同道按钮
                var go = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("CreatePlayerCommon", "xinggebg"));
                go.name = "openPlayerTeam";
                go.transform.SetParent(parent, false);
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(73, 31);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(2, -74);
                var btn = go.AddComponent<Button>();
                string btnStr;
                if (Team.DataTeam.teamUnits.Contains(__instance.unit.data.unitData.unitID))
                {
                    btnStr = GameTool.LS("Cave_NpcLiDui");
                    go.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    Action onClick = () =>
                    {
                        if (Team.DataTeam.teamUnits.Contains(__instance.unit.data.unitData.unitID))
                        {
                            Team.DataTeam.teamUnits.Remove(__instance.unit.data.unitData.unitID);
                            Team.DataTeam.battleUnits.Remove(__instance.unit.data.unitData.unitID);
                        }
                        UpdateItem(__instance);
                    };
                    btn.onClick.AddListener(onClick);
                }
                else
                {
                    btnStr = GameTool.LS("Cave_NpcZuDui");
                    if (__instance.unit.data.unitData.GetPoint() != DataCave.ReadData().GetPoint())
                    {
                        go.AddComponent<CanvasGroup>().alpha = 0.5f;
                        Action onClick = () =>
                        {
                            UITipItem.AddTip(__instance.unit.data.unitData.propertyData.GetName() + GameTool.LS("Cave_NpcWaiChu2"));
                        };
                        btn.onClick.AddListener(onClick);
                    }
                    else
                    {
                        Action onClick = () =>
                        {
                            if (!Team.DataTeam.teamUnits.Contains(__instance.unit.data.unitData.unitID))
                            {
                                Team.DataTeam.teamUnits.Add(__instance.unit.data.unitData.unitID);
                            }
                            UpdateItem(__instance);
                        };
                        btn.onClick.AddListener(onClick);
                    }
                }

                var go2 = GuiBaseUI.CreateUI.NewText(btnStr, new Vector2(40, 20));
                go2.transform.SetParent(go.transform, false);
                go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                go2.GetComponent<Text>().fontSize = 18;
                go2.GetComponent<Text>().color = Color.black;
            }
        }
    }

    [HarmonyPatch(typeof(UINPCSearch), "DestroyUI")]
    class Patch_UINPCSearch_DestroyUI
    {
        [HarmonyPostfix]
        private static void Postfix(UINPCSearch __instance)
        {
            Patch_UINPCUnitInfoItem_InitData.addOpenChangeFace = false;
        }
    }


}
