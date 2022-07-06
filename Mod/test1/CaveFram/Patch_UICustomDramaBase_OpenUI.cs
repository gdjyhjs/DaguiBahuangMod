using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cave;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CaveFram
{
    [HarmonyPatch(typeof(UICustomDramaBattleNPCFailed2), "Init")]
    public class Patch_UICustomDramaBattleNPCFailed2_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UICustomDramaBattleNPCFailed2 __instance)
        {
                AddButton(__instance, __instance.onEndCall);
        }

        static string btnText = "我要将你关押";
        public static void AddButton(UICustomDramaBase self, Il2CppSystem.Action onEndCall)
        {
            // 处理NPC战败 对话增加关押按钮
            self.dramaData.dialogueOptions[123010217] = btnText;
            Action click = () => {
                // 关押
                WorldUnitBase unit = self.dramaData.unitRight;
                if (unit != null)
                {
                    DataCave dataCave = DataCave.ReadData();
                    Vector2Int point = dataCave.GetPoint();
                    unit.CreateAction(new UnitActionSetPoint(point));
                    UnitActionLuckAdd luckAdd = new UnitActionLuckAdd(BuildFarm.prisonerLuckId);
                    unit.CreateAction(luckAdd);
                }
                onEndCall?.Invoke();
            };
            self.OnOptionBackCall(123010217, click);
        }
    }

    [HarmonyPatch(typeof(UICustomDramaBattleNPCFailed3), "Init")]
    public class Patch_UICustomDramaBattleNPCFailed3_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UICustomDramaBattleNPCFailed3 __instance)
        {
                Patch_UICustomDramaBattleNPCFailed2_Init.AddButton(__instance, __instance.onEndCall);
        }
    }
}
