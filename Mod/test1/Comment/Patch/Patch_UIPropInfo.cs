using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Comment.Patch
{
    [HarmonyPatch(typeof(UIPropInfo), "InitData", new Type[] { typeof(WorldUnitBase), typeof(Vector3), typeof(bool) })]
    class Patch_UIPropInfo
    {
        [HarmonyPostfix]
        private static void Postfix(UIPropInfo __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                int target = GetTargetID(__instance);
                uiComment.Init(__instance, 2, target);

            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIPropInfo __instance)
        {
            return __instance.propData != null ? __instance.propData.propsID : 0;
        }
    }

    [HarmonyPatch(typeof(UIArtifactInfo), "InitData")]
    class Patch_UIArtifactInfo
    {
        [HarmonyPostfix]
        private static void Postfix(UIArtifactInfo __instance)
        {
            try
            {

                UIComment uiComment = new UIComment();
                int target = GetTargetID(__instance);
                uiComment.Init(__instance, 2, target);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIArtifactInfo __instance)
        {
            return __instance.shapeProp != null ? __instance.shapeProp.propsID : 0;
        }
    }

    [HarmonyPatch(typeof(UIMartialInfo), "InitData")]
    class Patch_UIMartialInfo
    {
        [HarmonyPostfix]
        private static void Postfix(UIMartialInfo __instance)
        {
            try
            {

                UIComment uiComment = new UIComment();
                int target = GetTargetID(__instance);
                uiComment.Init(__instance, 2, target);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIMartialInfo __instance)
        {
            return __instance.martialData != null ? __instance.martialData.baseID : 0;
        }
    }







    [HarmonyPatch(typeof(UIMartialPropInfo), "InitData")]
    class Patch_UIMartialPropInfo
    {
        [HarmonyPostfix]
        private static void Postfix(UIMartialPropInfo __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                int target = GetTargetID(__instance);
                uiComment.Init(__instance, 2, target);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIMartialPropInfo __instance)
        {
            return __instance.martialData != null ? __instance.martialData.baseID : 0;
        }
    }








    [HarmonyPatch(typeof(UIArtifact), "InitData")]
    class Patch_UIArtifact
    {
        [HarmonyPostfix]
        private static void Postfix(UIArtifact __instance)
        {
            try
            {
                UIComment uiComment = new UIComment();
                int type;
                int target = GetTargetID(__instance, out type);
                bool isInit = false;

                Action action = () =>
                {
                    if (target != GetTargetID(__instance, out int type2) && type2 != type)
                    {
                        target = GetTargetID(__instance, out type);
                        if (target != 0)
                        {
                            if (isInit)
                            {
                                uiComment.targetType = type2;
                                uiComment.targetId = target;
                                uiComment.GetData();
                            }
                            else
                            {
                                isInit = true;
                                uiComment.Init(__instance, type, target);
                            }
                            if (uiComment.bg)
                            {
                                uiComment.bg.transform.SetParent(__instance.transform.Find(type == 2 ? "Group:Shape" : "Group:Sprite"));
                            }
                        }
                    }
                };
                TimerCoroutine cor = g.timer.Frame(action, 1, true);
                __instance.AddCor(cor);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private static int GetTargetID(UIArtifact __instance, out int type)
        {
            if (__instance.transform.Find("Group:Shape").gameObject.activeSelf)
            {
                // 法宝
                type = 2;
                if (!string.IsNullOrEmpty(__instance.uiShape.selSoleId))
                {
                    DataProps.PropsData shapeProp = g.world.playerUnit.data.unitData.propData.GetProps(__instance.uiShape.selSoleId);
                    return shapeProp.propsID;
                }
            }
            else if (__instance.transform.Find("Group:Sprite").gameObject.activeSelf)
            {
                // 器灵
                type = 3;
                DataUnit.ArtifactSpriteData.Sprite sprite = g.world.playerUnit.data.unitData.artifactSpriteData.GetSpriteInSoleID(__instance.uiSprite.selSpriteSoleId);
                if (sprite != null && __instance.uiSprite.selSpriteSoleId != 0)
                {
                    return sprite.spriteID;
                }
            }else
            {
                type = 0;
                return 0;
            }
            return 0;
        }
    }


}
