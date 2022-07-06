using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Comment.Patch
{
    // 各种建筑  包括其他定制UI界面

    [HarmonyPatch(typeof(UITownStorage), "InitData")]
    class Patch_UITownStorage
    {
        [HarmonyPostfix]
        private static void Postfix(UITownStorage __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 1);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownFight), "InitData")]
    class Patch_UITownFight
    {
        [HarmonyPostfix]
        private static void Postfix(UITownFight __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 2);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownAuction), "InitData")]
    class Patch_UITownAuction
    {
        [HarmonyPostfix]
        private static void Postfix(UITownAuction __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 3);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownTaoistRank), "InitData")]
    class Patch_UITownTaoistRank
    {
        [HarmonyPostfix]
        private static void Postfix(UITownTaoistRank __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 4);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownMarketBook), "InitData")]
    class Patch_UITownMarketBook
    {
        [HarmonyPostfix]
        private static void Postfix(UITownMarketBook __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 5);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownHotel), "InitData")]
    class Patch_UITownHotel
    {
        [HarmonyPostfix]
        private static void Postfix(UITownHotel __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 6);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownPub), "InitData")]
    class Patch_UITownPub
    {
        [HarmonyPostfix]
        private static void Postfix(UITownPub __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 7);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownMarket), "InitData")]
    class Patch_UITownMarket
    {
        [HarmonyPostfix]
        private static void Postfix(UITownMarket __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 8);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownFactory), "InitData")]
    class Patch_UITownFactory
    {
        [HarmonyPostfix]
        private static void Postfix(UITownFactory __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 9);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UITownMarketCloth), "InitData")]
    class Patch_UITownMarketCloth
    {
        [HarmonyPostfix]
        private static void Postfix(UITownMarketCloth __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 10);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UITownBounty), "InitData")]
    class Patch_UITownBounty
    {
        [HarmonyPostfix]
        private static void Postfix(UITownBounty __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 11);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    //[HarmonyPatch(typeof(UIMinMap), "InitData", new Type[] {typeof(List<WorldUnitBase>) })]
    //class Patch_UIMinMap
    //{
    //    [HarmonyPostfix]
    //    private static void Postfix(UIMinMap __instance)
    //    {
    //        try
    //        {
    //            UIComment uiComment = new UIComment();
    //            uiComment.Init(__instance, 5, 12);
    //        }
    //        catch (Exception e)
    //        {
    //            MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(UIMinMap), "InitData", new Type[] { })]
    //class Patch_UIMinMap2
    //{
    //    [HarmonyPostfix]
    //    private static void Postfix(UIMinMap __instance)
    //    {
    //        try
    //        {
    //            UIComment uiComment = new UIComment();
    //            uiComment.Init(__instance, 5, 12);
    //        }
    //        catch (Exception e)
    //        {
    //            MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
    //        }
    //    }
    //}


    /////////////////////////////// 宗门建筑



    [HarmonyPatch(typeof(UISchoolGodBeast), "InitData")]
    class Patch_UISchoolGodBeast
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolGodBeast __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 13);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolAura), "InitData")]
    class Patch_UISchoolAura
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolAura __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 14);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolTaskLobby), "InitData")]
    class Patch_UISchoolTaskLobby
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolTaskLobby __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 15);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolMainLobby), "InitData")]
    class Patch_UISchoolMainLobby
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolMainLobby __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 16);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolStore), "InitData")]
    class Patch_UISchoolStore
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolStore __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 17);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolHospital), "InitData")]
    class Patch_UISchoolHospital
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolHospital __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 18);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UISchoolLibrary), "InitData")]
    class Patch_UISchoolLibrary
    {
        [HarmonyPostfix]
        private static void Postfix(UISchoolLibrary __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 19);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UIDragonDoorUpgrade), "Init")]
    class Patch_UIDragonDoorUpgrade
    {
        [HarmonyPostfix]
        private static void Postfix(UIDragonDoorUpgrade __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 20);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(UIBuild10005), "InitData")]
    class Patch_UIBuild10005 // 野外建筑
    {
        [HarmonyPostfix]
        private static void Postfix(UIBuild10005 __instance, ConfWorldBuilding10005Item building10005Item, MapBuild10005 build10005)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 7, building10005Item.id);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }



    [HarmonyPatch(typeof(UITaoistSelect), "Init")]
    class Patch_UITaoistSelect
    {
        [HarmonyPostfix]
        private static void Postfix(UITaoistSelect __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 21);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }




    [HarmonyPatch(typeof(UIMonthLog), "Init")]
    class Patch_UIMonthLog
    {
        [HarmonyPostfix]
        private static void Postfix(UIMonthLog __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 22);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UIResurgeInfo), "Init")]
    class Patch_UIResurgeInfo
    {
        [HarmonyPostfix]
        private static void Postfix(UIResurgeInfo __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 23);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UIPlayerTask), "Init")]
    class Patch_UIPlayerTask
    {
        [HarmonyPostfix]
        private static void Postfix(UIPlayerTask __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 24);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UIGetReward), "Init")]
    class Patch_UIGetReward
    {
        [HarmonyPostfix]
        private static void Postfix(UIGetReward __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 25);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UIMapBattlePre), "Init")]
    class Patch_UIMapBattlePre
    {
        [HarmonyPostfix]
        private static void Postfix(UIMapBattlePre __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 26);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }


    [HarmonyPatch(typeof(UIBattleEnd), "Init")]
    class Patch_UIBattleEnd
    {
        [HarmonyPostfix]
        private static void Postfix(UIBattleEnd __instance)
        {
            try
            {

                if (SceneType.battle.battleMap.bossUnitAttrItem != null)
                {
                    int id = SceneType.battle.battleMap.bossUnitAttrItem.baseID;
                    UIComment uiComment = new UIComment();
                    uiComment.Init(__instance, 8, id);
                }
                else
                {
                    UIComment uiComment = new UIComment();
                    uiComment.Init(__instance, 5, 27);
                }

            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }

    // 28 正道宗门
    // 29 魔道宗门
    // 100+ 保留+areaID城镇
    // 30 张三

    [HarmonyPatch(typeof(UIIntoGameDrama), "Init")]
    class Patch_UIIntoGameDrama
    {
        [HarmonyPostfix]
        private static void Postfix(UIIntoGameDrama __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                uiComment.Init(__instance, 5, 30);

            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
