using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
    public class Cave : MelonMod
    {
        public static CaveConfig config;
        public static bool isDebug = true;
        public bool isInit = false;
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            new PatchMgr();
            Log("洞天福地MOD 欢迎关注我bilibili：八荒大鬼  我的QQ群：50948165", 0);
            config = new CaveConfig();
            Log("洞府建筑配置文件" + ConfBuild.list.Count);


            var a = DataCustom.data;














        }

        public static void Log(string str, int show = 99)
        {
            return;
            if (isDebug || show < 0)
            {
                Debug.Log("[Cave]"+str);
                MelonLogger.Msg(str);
            }
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
            Log("过月结束 ");
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
    }
}
