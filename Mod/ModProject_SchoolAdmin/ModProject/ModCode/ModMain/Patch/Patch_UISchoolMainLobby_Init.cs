using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    [HarmonyPatch(typeof(UISchoolMainLobby))]
    [HarmonyPatch("Init")]
    class Patch_UISchoolMainLobby_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolMainLobby __instance)
        {
            new UISchoolRecruit(__instance);
        }
    }
}
