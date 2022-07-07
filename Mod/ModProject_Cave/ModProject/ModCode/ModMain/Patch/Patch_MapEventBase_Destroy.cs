using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UIType;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(MapEventBase), "Destroy")]
    public class Patch_MapEventBase_Destroy
    {
        [HarmonyPostfix]
        private static void Postfix(MapEventBase __instance)
        {
            if (__instance.eventData.id == 6 && DataCave.ReadData().state == 2)
            {
                string str = "检测到洞府将被未知力量删除，请将以下内容复制给八荒大鬼查看是何种力量竟敢侵犯洞府安全！\n" + (new System.Diagnostics.StackTrace(true)).ToString();
                Cave.Log(str);
                Debug.Log(str);
                g.world.system.AddSystemInMap(new Action<Il2CppSystem.Action>((nextAction) =>
                {
                    g.ui.OpenUI<UITextInfoLong>(UIType.TextInfoLong).InitData("洞府危机", str, "", new Action(()=>
                    {
                        nextAction.Invoke();
                    }));
                }));
            }
        }
    }
}
