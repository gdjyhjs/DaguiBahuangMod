using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(WorldMgr), "Destroy")]
    class Patch_WorldMgr_Destroy
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            g.events.Off(EGameType.WorldRunEnd);
            Cave.Log(g.world.playerUnit.data.unitData.propertyData.GetName() + "离开八荒世界！是否有洞府=" + DataCave.ReadData().state);
        }
    }
}
