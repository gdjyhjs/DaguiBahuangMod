using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Comment.Patch
{
    [HarmonyPatch(typeof(UIMapMain), "Init")]
    class Patch_UIMapMain
    {
        [HarmonyPostfix]
        private static void Postfix(UIMapMain __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                int type;
                int target = GetTargetID(uiComment,out type);
                Action action = () =>
                {
                    if (target != GetTargetID(uiComment, out type))
                    {
                        target = GetTargetID(uiComment, out type);
                        uiComment.targetType = type;
                        uiComment.targetId = target;
                        uiComment.GetData();
                    }
                };
                TimerCoroutine cor = g.timer.Frame(action, 1, true);
                __instance.AddCor(cor);
                uiComment.Init(__instance, type, target);

            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIComment uiComment,out int targetType)
        {
            targetType = 1;
            try
            {
                var unit = g.world.playerUnit;
                var point = unit.data.unitData.GetPoint();

                List<Vector2Int> points = new List<Vector2Int>() { point };
                MapBuildSchool schoolBuild = g.world.build.GetBuild<MapBuildSchool>(point);

                if (schoolBuild != null) // 有宗门
                {
                    if (schoolBuild.schoolData.stand == 1)
                    {
                        targetType = 5;
                        return 28; // 正道宗门
                    }
                    else
                    {
                        targetType = 5;
                        return 29; // 魔道宗门
                    }
                }

                MapBuildTown townBuild = g.world.build.GetBuild<MapBuildTown>(point);
                if (townBuild != null)
                {
                    DataGrid.GridData gridData = g.data.grid.GetGridData(point);
                    int areaId = gridData.areaBaseID;
                    targetType = 5;
                    return 100 + areaId; // 大洲频道
                }


                return point.x * 10000 + point.y;
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
            return 0;
        }
    }
}
