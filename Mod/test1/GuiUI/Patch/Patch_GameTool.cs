using GuiBaseUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UIType;

namespace GuiUI.Patch
{
    [HarmonyPatch(typeof(GameTool), "LS", new Type[] { typeof(string), typeof(int)})]
    public class Patch_GameTool_LS2
    {
        [HarmonyPostfix]
        private static void Postfix(ref string __result, string keys, int bgType)
        {
            if (string.IsNullOrEmpty(__result) && keys.StartsWith("__"))
            {
                __result = keys.Substring(2, keys.Length - 2);
            }
        }
    }
}
