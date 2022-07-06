using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuiBaseUI.Patch
{
    [HarmonyPatch(typeof(UnitConditionTool), "Condition")]
    class Patch_UnitConditionTool_Condition
    {
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result, ref string condition, UnitConditionData data)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return false;
            try
            {
                //new Log("DoCondition = " + condition);
                if (condition == "0")
                {
                    __result = true;
                    return false;
                }
                List<string> list = new List<string>(condition.Split('|'));
                List<string> conditions = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    string[] ss = list[i].Split('_');
                    var parm1 = (ss.Length < 1 || string.IsNullOrWhiteSpace(ss[0])) ? null : ss[0];
                    if (parm1 == "class")
                    {
                        try
                        {
                            var parm2 = (ss.Length < 2 || string.IsNullOrWhiteSpace(ss[1])) ? null : ss[1];
                            var parm3 = (ss.Length < 3 || string.IsNullOrWhiteSpace(ss[2])) ? null : ss[2];
                            var parm4 = (ss.Length < 4 || string.IsNullOrWhiteSpace(ss[3])) ? null : ss[3];
                            System.Runtime.Remoting.ObjectHandle objHandle = Activator.CreateInstance(parm2, parm3);
                            ConditionBase obj = (ConditionBase)objHandle.Unwrap();
                            if (obj != null)
                            {
                                __result = obj.Init(parm4);
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            Print.LogError("Condi = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace);
                            return false;
                        }
                    }
                    else
                    {
                        conditions.Add(list[i]);
                    }
                }
                if (conditions.Count > 0)
                {
                    condition = string.Join("|", conditions);
                }
                return true;
            }
            catch (Exception e)
            {
                Print.LogError("Condition = " + condition);
                Print.LogError(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
