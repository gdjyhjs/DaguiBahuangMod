using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cave;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

// 处理关押战败敌人
namespace Cave
{
    [HarmonyPatch(typeof(UICustomDramaBattleNPCFailed2), "Init")]
    public class Patch_UICustomDramaBattleNPCFailed2_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UICustomDramaBattleNPCFailed2 __instance)
        {
            AddButton(__instance, __instance.onEndCall);
        }

        static string btnText = "押入地牢";
        public static void AddButton(UICustomDramaBase self, Il2CppSystem.Action onEndCall)
        {
            // 处理NPC战败 对话增加关押按钮
            self.dramaData.dialogueOptions[123010217] = btnText;
            Action click = () => {
                // 关押
                WorldUnitBase unit = self.dramaData.unitRight;
                if (unit != null)
                {
                    var data = DataDungeon.ReadData();
                    DataCave dataCave = DataCave.ReadData();
                    int err3 = data.units.Count < 100 ? 0 : 1, err1 = 1, err2 = 1;
                    Vector2Int point = dataCave.GetPoint();
                    if (err3 == 0)
                    {
                        err1 = unit.CreateAction(new UnitActionSetPoint(point));
                        if (err1 == 0)
                        {
                            UnitActionLuckAdd luckAdd = new UnitActionLuckAdd(BuildFunction.DungeonBuild.prisonerLuckId);
                            err2 = unit.CreateAction(luckAdd);
                        }
                    }
                    if (err2 == 0)
                    {
                        Cave.Log("关押成功");
                        data.units.Add(unit.data.unitData.unitID);
                        DataDungeon.SaveData(data);
                    }
                    else
                    {
                        Cave.Log("关押失败");
                    }
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
