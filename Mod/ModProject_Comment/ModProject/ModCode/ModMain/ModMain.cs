using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace Comment
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
            //try
            //{
            //    if (fixCount == 0)
            //    {
            //        //使用了Harmony补丁功能的，需要手动启用补丁。
            //        //启动当前程序集的所有补丁
            //        var item = new HarmonyLib.Harmony("MOD_vJhg4G");
            //        item.PatchAll(Assembly.GetExecutingAssembly());
            //    }

            //    new CommentMain();
            //}
            //catch (Exception e)
            //{
            //    GuiUITups();
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine(e.StackTrace);
            //}
        }

        private void GuiUITups()
        {
            string modName = "<color=red><b>宗门管家</b></color>";
            string needName = "<color=red><b>UI框架</b></color>";
            string tips = $"<size=26>{modName}模组需要安装{needName}模组才能使用，检测到您尚未正确安装{needName}，可能会影响模组功能的使用。您可以到模组商店搜索<b>大鬼</b>订阅{needName}，订阅后需要到本地模组将{needName}移至{modName}模组上方。</size>";
            Console.WriteLine(tips);
            Action openTips = () =>
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("<size=26>" + GameTool.LS("common_tishi") + "</size>", tips, 1);
            };
            g.events.On(EGameType.OpenUIEnd, openTips, 1);
        }
    }
}
