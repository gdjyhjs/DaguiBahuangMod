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
            new HelpMe(__instance);
        }
    }
}
