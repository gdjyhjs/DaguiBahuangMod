using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace SchoolAdmin
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {

        private static int fixCount = 0;
        /// <summary>
        /// MOD初始化，进入游戏时会调用此函数
        /// </summary>
        public void Init()
        {
            try
            {
                if (fixCount == 0)
                {
                    //使用了Harmony补丁功能的，需要手动启用补丁。
                    //启动当前程序集的所有补丁
                    var item = new HarmonyLib.Harmony("MOD_eJCLnC");
                    item.PatchAll(Assembly.GetExecutingAssembly());
                    fixCount++;
                }

                fixCount++;
                new SchoolAdmin();
            }
            catch (Exception e)
            {
                GuiUITups();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void GuiUITups()
        {
            Action openTips = () =>
            {
                string modName = "<color=red>" + GameTool.LS("schoolAdmin_modName") + "</color>";
                string needName = "<color=red>" + GameTool.LS("schoolAdmin_needModName") + "</color>";
                string tips = string.Format(GameTool.LS("schoolAdmin_needModTips"), modName, needName);
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("<size=26>" + GameTool.LS("common_tishi") + "</size>", tips, 1);
            };
            g.events.On(EGameType.OpenUIEnd, openTips, 1);
        }
    }
}
