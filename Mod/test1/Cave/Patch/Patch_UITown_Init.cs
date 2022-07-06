using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UITown), "Init")]
    class Patch_UITown_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UITown __instance)
        {
            new TownCave(__instance);
        }
    }
}
