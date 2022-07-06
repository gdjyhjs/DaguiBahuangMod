using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymnasium.Patch
{
    [HarmonyPatch(typeof(UIMartialTrain), "UpdateRealizeInfoUI")]
    class Patch_UIMartialTrain_UpdateRealizeInfoUI
    {
        [HarmonyPostfix]
        private static void Postfix(UIMartialTrain __instance)
        {
            if (Gumnasium.openGumnasium)
            {
                UIMartialTrain ui = __instance;
                ui.tglTitle1.gameObject.SetActive(false);
                ui.tglTitle2.gameObject.SetActive(false);
                ui.textRealize.text = GameTool.LS("Gymnasium_textRealize");
                ui.textTitle.text = GameTool.LS("Gymnasium_textTitle");
            }
        }
    }

    [HarmonyPatch(typeof(UIMartialTrain), "DestroyUI")]
    class Patch_UIMartialTrain_DestroyUI
    {
        [HarmonyPostfix]
        private static void Postfix(UIMartialTrain __instance)
        {
            if (Gumnasium.openGumnasium)
            {
                new Log("关闭升级技能");

                Gumnasium.openGumnasium = false;
            }
        }
    }
}
