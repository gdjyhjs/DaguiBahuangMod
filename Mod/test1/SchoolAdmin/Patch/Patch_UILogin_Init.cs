using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    [HarmonyPatch(typeof(UILogin))]
    [HarmonyPatch("Init")]
    class Patch_UILogin_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UILogin __instance)
        {
            if (!SchoolAdmin.isTrue)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), SchoolAdmin.trueTips, 1, null);
                return;
            }
        }
    }
}
