using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace MOD_atLeW5
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
		private static HarmonyLib.Harmony harmony;
        public static string dataKey = "daguiOeZqgPIXJ65l6K5XR==";

        /// <summary>
        /// MOD初始化，进入游戏时会调用此函数
        /// </summary>
        public void Init()
        {
			//使用了Harmony补丁功能的，需要手动启用补丁。
			//启动当前程序集的所有补丁
			if (harmony != null)
			{
				harmony.UnpatchSelf();
				harmony = null;
			}
			if (harmony == null)
			{
				harmony = new HarmonyLib.Harmony("MOD_atLeW5");
			}
			harmony.PatchAll(Assembly.GetExecutingAssembly());

        }

        public static void SaveData()
        {
            string str = JsonConvert.SerializeObject(StaticData.data);
            g.data.globle.obj.SetString("www.yellowshenge.com", dataKey, str);
            g.data.dataGloble.Save();

        }

        public static void ReadData()
        {
            try
            {
                if (g.data.globle.obj.ContainsKey("www.yellowshenge.com", dataKey))
                {
                    var str = g.data.globle.obj.GetString("www.yellowshenge.com", dataKey);
                    StaticData.data = JsonConvert.DeserializeObject<StaticData.Data>(str);
                }
                else
                {
                    StaticData.data = new StaticData.Data();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("读取错误 " + e.ToString());
                StaticData.data = new StaticData.Data();
            }
        }
    }
}
