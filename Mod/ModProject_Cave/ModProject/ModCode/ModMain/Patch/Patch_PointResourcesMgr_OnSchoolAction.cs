﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(PointResourcesMgr), "OnSchoolAction")]
    public class Patch_PointResourcesMgr_OnSchoolAction
    {
        // 过月结束
        [HarmonyPostfix]
        private static void Postfix()
        {
            Cave.Log(g.world.playerUnit.data.unitData.propertyData.GetName() + "！是否有洞府=" + DataCave.ReadData().state);
            new CaveOnWorleRunEnd();
        }
    }
}