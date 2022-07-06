using GuiBaseUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UIType;

namespace GuiUI.Patch
{
    [HarmonyPatch(typeof(UIBase), "Init")]
    public class Patch_UIBase_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UIBase __instance)
        {
            UIBase ui = __instance;
            UITypeBase uiType = ui.uiType;
            List<AutoData> autuDataList = ConfAutoUI.GetAutoUI(uiType);

            UnityEngine.Transform tf = ui.transform;
            foreach (AutoData item in autuDataList)
            {
                string path = item.path;
                AutoData data = item;
                TimerCoroutine timer = null;
                GameObject go = null;
                bool debug = false;
                Action action = () =>
                {
                    try
                    {
                        if (tf == null)
                        {
                            g.timer.Stop(timer);
                            return;
                        }
                        if (go != null)
                        {
                            GameObject.Destroy(go);
                        }
                        if (debug)
                        {
                            data = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoData>(File.ReadAllText(path));
                            data.path = path;
                        }
                        go = AutoGenerate.Generate(tf, data);
                    }
                    catch (Exception e)
                    {
                        Print.LogError("创建UI失败 "+e.Message + "\n" + e.StackTrace);
                    }
                };

                Action action2 = () => {
                    if (tf == null)
                    {
                        g.timer.Stop(timer);
                        return;
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        debug = true;
                        action();
                    }
                };
                Action delayAction = () =>
                {
                    if (tf == null)
                    {
                        g.timer.Stop(timer);
                        return;
                    }
                    if (data.debug == 1)
                    {
                        timer = g.timer.Frame(action2, 1, true);
                    }
                    action();
                };
                timer = g.timer.Frame(delayAction, 1);
            }
        }
    }
}
