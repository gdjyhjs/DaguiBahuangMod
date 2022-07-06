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
                new Log("DoFunction = " + function);
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
                                obj.Init(parm4);
                            }
                        }
                        catch (Exception e)
                        {
                            new Log(i+":fun = " + list[i]);
                            new Log(e.Message + "\n" + e.StackTrace);
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
                            new Log("fun = " + list[i]);
                            new Log(e.Message + "\n" + e.StackTrace);
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
                new Log("function = " + function);
                new Log(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
