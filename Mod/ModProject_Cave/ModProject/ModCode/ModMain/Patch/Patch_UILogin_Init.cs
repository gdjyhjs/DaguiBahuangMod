using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UILogin), "Init")]
    class Patch_UISchoolPost_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UILogin __instance)
        {
            g.timer.Time(new Action(() =>
            {
                new HelpMe(__instance);
            }), 0.25f);
        }
    }
}
