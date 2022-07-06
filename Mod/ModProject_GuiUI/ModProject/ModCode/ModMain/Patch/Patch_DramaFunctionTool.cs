using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GuiBaseUI.Patch
{
    [HarmonyPatch(typeof(DramaFunctionTool), "OptionsFunction")]
    class Patch_DramaFunctionTool_OptionsFunction
    {
        [HarmonyPrefix]
        static bool Prefix(ref string function)
        {
            if (string.IsNullOrWhiteSpace(function))
                return false;
            try
            {
                List<string> list = new List<string>(function.Split('|'));
                List<string> dramaFuncs = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    string[] ss = list[i].Split('_');
                    var parm1 = (ss.Length < 1 || string.IsNullOrWhiteSpace(ss[0])) ? null : ss[0];
                    if (parm1 == "class")
                    {
                        try
                        {
                            var parm2 = (ss.Length < 2 || string.IsNullOrWhiteSpace(ss[1])) ? "" : ss[1];
                            var parm3 = (ss.Length < 3 || string.IsNullOrWhiteSpace(ss[2])) ? "" : ss[2];
                            var parm4 = (ss.Length < 4 || string.IsNullOrWhiteSpace(ss[3])) ? "" : ss[3];
                            System.Runtime.Remoting.ObjectHandle objHandle = Activator.CreateInstance(parm2, parm3);
                            ClassBase obj = (ClassBase)objHandle.Unwrap();
                            if (obj != null)
                            {
                                try
                                {
                                    obj.Init(parm4);
                                }
                                catch (Exception e)
                                {
                                    Print.LogError(i + ":对象初始化失败 = " + list[i]);
                                    Print.LogError(e.Message + "\n" + e.StackTrace);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Print.LogError(i+":fun = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace);
                            return false;
                        }
                    }else if (parm1 == "openUrl")
                    {
                        try
                        {
                            var parm2 = ss[1];
                            Application.OpenURL(parm2);
                        }
                        catch (Exception e)
                        {
                            Print.LogError("fun = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace);
                            return false;
                        }
                    }
                    else
                    {
                        dramaFuncs.Add(list[i]);
                    }
                }
                if (dramaFuncs.Count > 0)
                {
                    function = string.Join("|", dramaFuncs);
                }
                return true;
            }
            catch (Exception e)
            {
                Print.LogError("function = " + function);
                Print.LogError(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
