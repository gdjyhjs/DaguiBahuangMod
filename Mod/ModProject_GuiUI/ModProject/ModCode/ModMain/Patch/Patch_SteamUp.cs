using HarmonyLib;
using Steamworks;
using System;
using System.Reflection;
using GuiBaseUI;
using UnhollowerRuntimeLib;

namespace GuiUI.Patch
{
    // steam测试
    [HarmonyPatch(typeof(CallResult), "OnRunCallResult")]
    class Patch_CallResult_OnRunCallResult
    {
        [HarmonyPrefix]
        private static bool Prefix(CallResult __instance)
        {
            GuiBaseUI.Print.Log("回调AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA ");
            return false;
        }
    }
}
