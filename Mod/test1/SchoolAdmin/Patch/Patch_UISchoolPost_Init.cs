using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    [HarmonyPatch(typeof(UISchoolPost))]
    [HarmonyPatch("Init")]
    class Patch_UISchoolPost_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolPost __instance)
        {
            if (!SchoolAdmin.isTrue)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), SchoolAdmin.trueTips, 1, null);
                return;
            }

            new UISchoolAdmin(__instance);
        }
    }
}
