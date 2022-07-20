using System.Reflection;
using UnityEngine;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace FixData
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
        private static HarmonyLib.Harmony harmony;
        public static bool autoFixData;
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
                harmony = new HarmonyLib.Harmony("MOD_M60S96");
            }
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (PlayerPrefs.HasKey("GuiFixDataItem"))
            {
                autoFixData = PlayerPrefs.GetInt("GuiFixDataItem") == 1;
            }
            else
            {
                autoFixData = true;
                PlayerPrefs.SetInt("GuiFixDataItem", 1);
            }
        }
    }
}
