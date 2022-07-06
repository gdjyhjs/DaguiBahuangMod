using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(PointResourcesMgr), "Init")]
    class Patch_PointResourcesMgr_Init
    {
        [HarmonyPostfix]
        private static void Postfix(PointResourcesMgr __instance)
        {
            onWorldRunStart = OnWorldRunStart;
            onWorldRunEnd = OnWorldRunEnd;
            g.world.run.On(WorldRunOrder.Start, onWorldRunStart);
            g.world.run.On(WorldRunOrder.End, onWorldRunEnd);
        }

        public static Action onWorldRunStart;
        public static Action onWorldRunEnd;
        public static WorldUnitEffectBase hideEffect;
        public static void OnWorldRunStart()
        {
            Cave.Log("开始过月 "+ IsOnZhenfa());
            if (IsOnZhenfa())
            {
                hideEffect = WorldUnitEffectTool.CreateEffect(1012, g.world.playerUnit, new BattleSkillValueData(g.world.playerUnit));
            }
        }
        public static void OnWorldRunEnd()
        {
            Cave.Log("结束过月 " + IsOnZhenfa());
            if (hideEffect != null)
            {
                hideEffect.Destroy();
                g.world.playerUnit.allEffectsCustom.Remove(hideEffect);
                hideEffect = null;
            }
        }

        public static bool IsOnZhenfa()
        {
            DataCave data = DataCave.ReadData();
            Vector2Int pos = new UnityEngine.Vector2Int(data.x, data.y);
            bool open = BuildFunction.FaZhenData.IsOpen(data);
            if (open && g.world.playerUnit.data.unitData.GetPoint() == pos)
            {
                return true;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PointResourcesMgr), "Destroy")]
    class Patch_PointResourcesMgr_Destroy
    {
        [HarmonyPostfix]
        private static void Postfix(PointResourcesMgr __instance)
        {
            g.world.run.Off(WorldRunOrder.System, Patch_PointResourcesMgr_Init.onWorldRunStart);
            g.world.run.Off(WorldRunOrder.End, Patch_PointResourcesMgr_Init.onWorldRunEnd);
        }
    }
}
