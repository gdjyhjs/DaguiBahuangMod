using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UIType;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UIBase), "DestroyUI")]
    public class Patch_UIBase_DestroyUI
    {
        [HarmonyPostfix]
        private static void Postfix(UIBase __instance)
        {
            UITypeBase uiType = __instance.uiType;
            if (uiType == UIType.GamePlanTip)
            {

            }
        }
    }
}
