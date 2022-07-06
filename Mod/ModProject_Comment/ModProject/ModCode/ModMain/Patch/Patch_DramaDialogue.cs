using HarmonyLib;
using System;

namespace Comment.Patch
{

    [HarmonyPatch(typeof(UIDramaBase), "InitData")]
    class Patch_UIDramaBase
    {
        [HarmonyPostfix]
        private static void Postfix(UIDramaBase __instance, int id, DramaData data)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 6, id);
            }
            catch (Exception e)
            {
                CommentMain.Log(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
