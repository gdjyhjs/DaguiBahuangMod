using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cave.Patch
{
    [HarmonyPatch(typeof(UICreatePlayer), "DestroyUI")]
    class Patch_UICreatePlayer_DestroyUI
    {
        public static TimerCoroutine corMoveCall;
        public static bool onCreateFace;

        [HarmonyPrefix]
        private static bool Prefix(UICreatePlayer __instance)
        {
            if (onCreateFace)
            {
                if (corMoveCall != null)
                {
                    corMoveCall.Stop();
                }
                if (__instance.onCloseCall != null)
                {
                    __instance.onCloseCall.Invoke();
                }
                for (int i = 0; i < __instance.allCor.Count; i++)
                {
                    g.timer.Stop(__instance.allCor[i]);
                }
                __instance.allCor = new Il2CppSystem.Collections.Generic.List<TimerCoroutine>();
                for (int i = 0; i < __instance.allCq.Count; i++)
                {
                    __instance.allCq[i].Destroy();
                }
                onCreateFace = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UICreatePlayerFacade), "OnAttackClick")]
    class Patch_UICreatePlayerFacade_OnAttackClick
    {
        [HarmonyPrefix]
        private static bool Prefix(UICreatePlayerFacade __instance)
        {
            if (Patch_UICreatePlayer_DestroyUI.onCreateFace)
            {
                if (Patch_UICreatePlayer_DestroyUI.corMoveCall != null)
                {
                    Patch_UICreatePlayer_DestroyUI.corMoveCall.Stop();
                }
                __instance.playerCtrl.anim.SetBool(UnitAnimState.Run, false);
                __instance.playerCtrl.anim.Play(UnitAnimState.Attack);
                GameObject prefab = g.res.Load<GameObject>("Effect/UI/WanjiaGongji");
                GameObject effectGo = GameObject.Instantiate<GameObject>(prefab, __instance.rimgPlayer2.transform);
                if (effectGo != null)
                {
                    effectGo.transform.localPosition = new Vector3(40f, 5f);
                    effectGo.transform.localEulerAngles = new Vector3(0, 0, -90f);
                    GameEffectTool.SetSortOrder(effectGo, __instance.createPlayer.sortingOrder + 1);
                }
                Action action = () =>
                {
                    if (effectGo != null)
                    {
                        GameObject.Destroy(effectGo);
                    }
                };
                Patch_UICreatePlayer_DestroyUI.corMoveCall = __instance.createPlayer.DelayTime(action, 2f);
                Cave.Log("GG");
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UICreatePlayerFacade), "OnMoveClick")]
    class Patch_UICreatePlayerFacade_OnMoveClick
    {
        [HarmonyPrefix]
        private static bool Prefix(UICreatePlayerFacade __instance)
        {
            if (Patch_UICreatePlayer_DestroyUI.onCreateFace)
            {
                __instance.playerCtrl.anim.SetBool(UnitAnimState.Run, true);

                if (Patch_UICreatePlayer_DestroyUI.corMoveCall != null)
                {
                    Patch_UICreatePlayer_DestroyUI.corMoveCall.Stop();
                }
                Action action = () =>
                {
                    __instance.playerCtrl.anim.SetBool(UnitAnimState.Run, false);
                };
                Patch_UICreatePlayer_DestroyUI.corMoveCall = __instance.createPlayer.DelayTime(action, 1f);
                return false;
            }
            return true;
        }
    }

}
