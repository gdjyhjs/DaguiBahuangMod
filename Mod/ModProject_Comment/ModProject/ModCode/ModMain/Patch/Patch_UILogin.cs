using HarmonyLib;
using System;

namespace Comment.Patch
{
    [HarmonyPatch(typeof(UILogin), "Init")]
    class Patch_UILogin
    {
        [HarmonyPostfix]
        private static void Postfix(UILogin __instance)
        {
            //CommentMain.Log("登陆界面启动");

            try
            {
                Action action2 = () =>
                {
                    UIComment uiComment = new UIComment();
                    string paper = __instance.curPaper;
                    Action action = () =>
                    {
                        if (__instance.curPaper != paper)
                        {
                            paper = __instance.curPaper;
                            uiComment.targetId = GetTargetID(paper);
                            uiComment.GetData();
                        }
                    };
                    TimerCoroutine cor = g.timer.Frame(action, 1, true);
                    __instance.AddCor(cor);
                    uiComment.Init(__instance, 0, GetTargetID(paper));
                };
                TimerCoroutine cor2 = g.timer.Frame(action2, 1);
                __instance.AddCor(cor2);

            }
            catch (Exception e)
            {
                CommentMain.Log(e.Message + "\n" + e.StackTrace);
            }   
        }

        private static int GetTargetID(string paper)
        {
            string[] paperNames = { "beijinglong", "beijinglong1", "beijinglong2", "beijinglong3", "beijinglong4", };
            for (int i = 0; i < paperNames.Length; i++)
            {
                if (paperNames[i] == paper)
                    return i;
            }
            return 0;
        }
    }
}
