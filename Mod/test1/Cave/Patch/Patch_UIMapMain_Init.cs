using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UIMapMain), "Init")]
    class Patch_UIMapMain_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UIMapMain __instance)
        {
            new MapMainCave(__instance);
        }
    }
}
