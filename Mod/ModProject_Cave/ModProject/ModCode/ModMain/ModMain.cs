using System;
using System.Reflection;
using UnityEngine;
using Cave.Team;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace Cave
{
    /*
     
    地牢可以在NPC页面加一个选项，“绑架”然后打架打赢了就关地牢，地牢可以强行双修她老公可以随机来救，按性格来
    忠贞爱家必救，其他随机


     * */

    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
        private static PlayerTeam playerTeam;
        private static int fixCount = 0;
        private static LongChi longchi;

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
                    var item = new HarmonyLib.Harmony("MOD_8s4Ze4");
                    item.PatchAll(Assembly.GetExecutingAssembly());
                    fixCount++;
                }
            }
            catch (Exception e)
            {
                GuiUITups();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            try
            {
                new Cave();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " A\n" + e.StackTrace);
            }

            try
            {
                if (playerTeam == null)
                {
                    playerTeam = new PlayerTeam();
                    playerTeam.Init();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " B\n" + e.StackTrace);
            }
            try
            {
                if (longchi == null)
                {
                    longchi = new LongChi();
                    longchi.Init();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " C\n" + e.StackTrace);
            }
        }

        /// <summary>
        /// MOD销毁，回到主界面，会调用此函数并重新初始化MOD
        /// </summary>
        public void Destroy()
        {
            try
            {
                if (playerTeam != null)
                {
                    playerTeam.Destroy();
                    playerTeam = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            try
            {
                if (longchi != null)
                {
                    longchi.Destroy();
                    longchi = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        private void GuiUITups()
        {
            Action openTips = () =>
            {
                string modName = "<color=red><b>" + GameTool.LS("Cave_ModName") + "</b></color>";
                string needName = "<color=red><b>" + GameTool.LS("Cave_NeedModName") + "</b></color>";
                string tips = "<size=26>"+ string.Format(GameTool.LS("Cave_NeedModTips"), modName, needName) + "</size>";
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("<size=26>" + GameTool.LS("common_tishi")+"</size>", tips, 1);
            };
            g.events.On(EGameType.OpenUIEnd, openTips, 1);
        }

    }
}
