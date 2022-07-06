using GuiBaseUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var parent = __instance.transform;
                if (parent.Find("openChangeFace") != null)
                {
                    GameObject.Destroy(parent.Find("openChangeFace").gameObject);
                }


                if (__instance.unit.data.unitData.GetPoint() != DataCave.ReadData().GetPoint())
                {

                }
                else
                {
                    var go = CreateUI.NewImage(SpriteTool.GetSprite("CreatePlayerCommon", "waimao"));
                    go.name = "openChangeFace";
                    go.transform.SetParent(parent, false);
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-35, -49);
                    var btn = go.AddComponent<Button>();
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
