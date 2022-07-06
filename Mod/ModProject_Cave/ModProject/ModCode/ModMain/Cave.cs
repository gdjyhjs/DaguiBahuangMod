using UnityEngine;
using System;
using Cave.Patch;
using Cave.Config;

namespace Cave
{
    // 洞府MOD
    /*
     

    自创道号。


    屏蔽社交  需要布置阵法。打开了阵法熟人无法找到自己，敌人境界过低无法找到自己。


    后花园器灵

    洞府挂载优化，不挂在主界面上
     * */
    public class Cave
    {
        public static CaveConfig config;
        public static bool isDebug = true;
        public bool isInit = false;
        public Cave()
        {
            new PatchMgr();
            config = new CaveConfig();
            DataCustom.Custom custom = DataCustom.data;
            Log("洞府配置加载完毕" + custom);
        }

        public static void Log(string str, int show = 99)
        {
            GuiBaseUI.Print.Log(str, "[大鬼洞府]");
        }

        public static void LogError(string str, int show = 99)
        {
            GuiBaseUI.Print.LogError(str + "\n" + (new System.Diagnostics.StackTrace(true)).ToString(), "[大鬼洞府]");
        }

        public static void LogWarning(string str, int show = 99)
        {
            GuiBaseUI.Print.LogWarring(str, "[大鬼洞府]");
        }

        // 神秘人说话
        public static void OpenDrama(string str = "", Action call = null)
        {
            g.conf.dramaDialogue.GetItem(1009037).nextDialogue = "0";
            UICustomDramaDyn dramaDyn = new UICustomDramaDyn(1009037);
            dramaDyn.dramaData.dialogueText[1009037] = str;
            dramaDyn.dramaData.onDramaEndCall = call;
            dramaDyn.OpenUI();
        }

        // 过月结束
        public void OnWorldRunEnd()
        {
            new CaveOnWorleRunEnd();
        }


        public static GameObject CreateGo(string path, Transform par = null, int depth = -1)
        {
            GameObject goPrefab = g.res.Load<GameObject>(path);
            if (goPrefab == null)
            {
                return null;
            }
            GameObject go = GameObject.Instantiate(goPrefab, par, false);
            go.name = goPrefab.name;
            GameTool.UpdateTextLanguage(go);
            go.transform.localPosition = Vector3.zero;
            if (depth != -1)
            {
                GameEffectTool.SetSortOrder(go, depth);
            }
            return go;
        }

        public static string GetModDir()
        {
            string dir = g.mod.GetModPathRoot("8s4Ze4") + "/ModAssets/Cave";
            return dir;
        }
    }
}
