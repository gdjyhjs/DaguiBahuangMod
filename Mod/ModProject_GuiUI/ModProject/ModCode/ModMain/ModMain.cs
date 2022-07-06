using System.Reflection;
using GuiBaseUI;
using System;
using Steamworks;
using UnhollowerRuntimeLib;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace GuiUI
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
        private static string name = "【大鬼】";
        private static int fixCount = 0;
        /// <summary>
        /// MOD初始化，进入游戏时会调用此函数
        /// </summary>
        public void Init()
        {
            if (fixCount == 0)
            {
                ClassInjector.RegisterTypeInIl2Cpp<ItemCell>();
                //使用了Harmony补丁功能的，需要手动启用补丁。
                //启动当前程序集的所有补丁
                var item = new HarmonyLib.Harmony("MOD_GV2cem");
                item.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
                fixCount++;
            }

            new GuiUI();
        }
    }
}
