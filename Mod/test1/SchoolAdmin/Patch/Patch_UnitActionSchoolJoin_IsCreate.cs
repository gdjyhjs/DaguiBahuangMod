using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    [HarmonyPatch(typeof(UnitActionSchoolJoin))]
    [HarmonyPatch("IsCreate")]
    internal class Patch_UnitActionSchoolJoin_IsCreate
	{
		[HarmonyPostfix]
		private static void Postfix(ref int __result, UnitActionSchoolJoin __instance)
        {
            if (!SchoolAdmin.isTrue)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), SchoolAdmin.trueTips, 1, null);
                return;
            }

            SchoolAdmin.Log($"{__instance.unit.data.unitData.propertyData.GetName()} {__instance.school.name} 检查加入宗门 __result={__result}  isTest={__instance.isTest}  isAddLog={__instance.isAddLog}");
            if (__result == 0)
            {
                MapBuildSchool playerSchool = g.world.playerUnit.data.school;

                var post = playerSchool != null ? playerSchool.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID).ToString() : "散人";
                SchoolAdmin.Log($"玩家宗门 {playerSchool} 职位={post}");
                if (playerSchool != null && __instance.school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain) // 玩家是宗主
                {
                    if (__instance.isAddLog == true)
                    {
                        __result = new CheckJoinSchool(__instance.school, __instance.unit).GetResult();
                        if (__result == 0)
                        {
                            SchoolAdmin.Log($"{__instance.unit.data.unitData.propertyData.GetName()} 通过了入宗条件 加入了宗门 {__instance.school.name}", true);
                        }
                    }
                }
            }
            SchoolAdmin.Log($"{__instance.unit.data.unitData.propertyData.GetName()} {__instance.school.name} 是否可以加入宗门 __result={__result}");
        }
	}
}
