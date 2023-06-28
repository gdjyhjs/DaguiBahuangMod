using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MOD_8sVtCY
{
    [HarmonyPatch(typeof(UIDLCSelectRole), "Init")]
    class Patch_UIDLCSelectRole_Init
    {
        [HarmonyPrefix]
        private static void Prefix(UIDLCSelectRole __instance)
        {
            try
            {
                List<int> allSkills = new List<int>();
                foreach (ConfDLCPrefixStoreItem item in g.dlc.dlcConf.dLCPrefixStore._allConfList)
                {
                    if (item.weight != 0 && g.dlc.dlcConf.dLCSkillPrefix.GetItem(item.id) != null && g.dlc.dlcConf.dLCSkillPrefix.GetItem(item.id).group == 0)
                    {
                        allSkills.Add(int.Parse(g.dlc.dlcConf.dLCSkillPrefix.GetItem(item.id).skillId));
                    }
                }

                foreach (var item in g.dlc.dlcConf.dLCRoleBase._allConfList)
                {
                    item.skills = allSkills.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[玄镜觉醒]修改技能错误 " + e.Message + "\n" + e.StackTrace);
            }
        }

        [HarmonyPostfix]
        private static void Postfix(UIDLCSelectRole __instance)
        {
            try
            {

                __instance.goSkillItemRoot2.GetComponent<GridLayoutGroup>().padding.right = 120;
                __instance.goSkillItemRoot2.GetComponent<GridLayoutGroup>().spacing = new Vector2(30, 40);
                __instance.goSkillItemRoot2.transform.parent.GetComponent<RectMask2D>().softness = new Vector2Int(30, 75);
            }
            catch (Exception e)
            {
                Console.WriteLine("[玄镜觉醒]修改ui错误 " + e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
