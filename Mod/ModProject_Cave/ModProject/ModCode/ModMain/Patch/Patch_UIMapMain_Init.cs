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
        public static MapMainCave mapMainCave;
        [HarmonyPostfix]
        private static void Postfix(UIMapMain __instance)
        {
            mapMainCave = new MapMainCave(__instance);
        }
    }

    [HarmonyPatch(typeof(UIMapMain), "Update")]
    class Patch_UIMapMain_Update
    {
        [HarmonyPostfix]
        private static void Postfix(UIMapMain __instance)
        {
            var mapMainCave = Patch_UIMapMain_Init.mapMainCave;
            if (mapMainCave != null)
            {
                mapMainCave.Update();
            }
        }
    }
}
