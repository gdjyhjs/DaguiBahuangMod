using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using GuiBaseUI;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_atLeW5
{
    [HarmonyPatch(typeof(AchievementMgr), "OnSaveData")]
    class Patch_AchievementMgr_OnSaveData
    {
        [HarmonyPostfix]
        public static void Postfix(UILogin __instance)
        {
            try
            {
                var path = g.cache.cachePath + "/" + g.cache.curCache.data.folder + "/" + ModMain.dataKey + ".cache";
                if (File.Exists(path))
                {
                    string toPath = path.Replace("CacheData/", "CacheDataTempguigubahuang/");
                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(toPath)))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(toPath));
                    }
                    System.IO.File.Copy(path, toPath, true);
                    Console.WriteLine(path + "\n" + toPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("存档管家数据保存错误 " + e.ToString());
            }
        }
    }
}
