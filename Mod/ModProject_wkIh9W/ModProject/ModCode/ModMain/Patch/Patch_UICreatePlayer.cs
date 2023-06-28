using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MOD_wkIh9W.Patch
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



    public class CreateFace
    {
        public CreateFace()
        {
        }

        public UICreatePlayer InitData(WorldUnitBase unit)
        {
            var ui = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
            if (ui != null)
                return ui;
            Patch_UICreatePlayer_DestroyUI.onCreateFace = true;
            ui = g.ui.OpenUI<UICreatePlayer>(UIType.CreatePlayer);
            ui.transform.Find("Root/Meum").localScale = Vector3.zero;
            //ui.transform.Find("Root/Group:Facade/LanguageGroup").gameObject.SetActive(false);
            //ui.transform.Find("Root/Group:Facade/InTrait").gameObject.SetActive(false);
            //ui.transform.Find("Root/Group:Facade/OutTrait").gameObject.SetActive(false);
            ui.facade.textLife.gameObject.SetActive(false);
            ui.facade.textCharm.gameObject.SetActive(false);
            ui.facade.textRace.gameObject.SetActive(false);
            ui.facade.textLevel.gameObject.SetActive(false);
            var inputName = ui.facade.goName.GetComponent<TMPro.TMP_InputField>();
            var tglWoman = ui.facade.tglWoman;
            var tglMan = ui.facade.tglMan;

            ui.InitData(100, GameLevelType.Common, g.data.world.npcCountId);

            Action delayAction = () =>
            {
                try
                {
                    try
                    {
                        ui.playerData.dynUnitData.inTrait.baseValue = unit.data.unitData.propertyData.inTrait;
                        ui.playerData.dynUnitData.outTrait1.baseValue = unit.data.unitData.propertyData.outTrait1;
                        ui.playerData.dynUnitData.outTrait2.baseValue = unit.data.unitData.propertyData.outTrait2;
                        int index1 = 0;
                        int index2 = 0;
                        foreach (var item in g.conf.roleCreateCharacter._allConfList)
                        {
                            if (item.type == 1)
                            {
                                if (item.id == unit.data.unitData.propertyData.inTrait)
                                {
                                    ui.facade.goInTraitRoot.transform.GetChild(index1).GetComponent<Toggle>().isOn = true;
                                }
                                index1++;
                            }
                            if (item.type == 2)
                            {
                                if (item.id == unit.data.unitData.propertyData.outTrait1 || item.id == unit.data.unitData.propertyData.outTrait2)
                                {
                                    ui.facade.goOutTrait.transform.GetChild(index2).GetComponent<Toggle>().isOn = true;
                                }
                                index2++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("初始化性格失败 " + e.ToString());
                    }
                    try
                    {
                        inputName.text = unit.data.unitData.propertyData.GetName();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("InitName "+e.ToString());
                    }

                    int sex = (int)unit.data.unitData.propertyData.sex;
                    ui.playerData.dynUnitData.sex.baseValue = sex;
                    bool isWoman = unit.data.unitData.propertyData.sex == UnitSexType.Woman;
                    //Console.WriteLine("sex = " + sex + "          isWoman = " + isWoman);
                    if (isWoman)
                    {
                        ui.facade.tglMan.isOn = false;
                        ui.facade.tglWoman.isOn = true;
                    }
                    else
                    {
                        ui.facade.tglWoman.isOn = false;
                        ui.facade.tglMan.isOn = true;
                    }
                    ui.playerData.unitData.propertyData.battleModelData = unit.data.unitData.propertyData.battleModelData.Clone();
                    var modelData = unit.data.unitData.propertyData.modelData.Clone();
                    ui.playerData.unitData.propertyData.modelData = modelData;

                    var facadeItems = ui.uiFacade.tglMan.isOn ? ui.uiFacade.manDressItems : ui.uiFacade.womanDressItems;
                    facadeItems[0].SetValueInID(modelData.hat);
                    facadeItems[1].SetValueInID(modelData.head);
                    facadeItems[2].SetValueInID(modelData.hair);
                    facadeItems[3].SetValueInID(modelData.hairFront);
                    facadeItems[4].SetValueInID(modelData.eyebrows);
                    facadeItems[5].SetValueInID(modelData.eyes);
                    facadeItems[6].SetValueInID(modelData.nose);
                    facadeItems[7].SetValueInID(modelData.mouth);
                    facadeItems[8].SetValueInID(modelData.body);
                    facadeItems[9].SetValueInID(modelData.back);
                    if (modelData.forehead != 0)
                    {
                        facadeItems[10].SetValueInID(modelData.forehead);
                    }
                    else if (modelData.faceLeft != 0)
                    {
                        facadeItems[10].SetValueInID(modelData.faceLeft);
                    }
                    else if (modelData.faceRight != 0)
                    {
                        facadeItems[10].SetValueInID(modelData.faceRight);
                    }
                    else if (modelData.faceFull != 0)
                    {
                        facadeItems[10].SetValueInID(modelData.faceFull);
                    }
                    else
                    {
                        facadeItems[10].SetValueInID(0);
                    }
                    facadeItems[4].offsetY = modelData.eyebrowsOffsetY;
                    facadeItems[5].offsetY = modelData.eyesOffsetY;
                    facadeItems[6].offsetY = modelData.noseOffsetY;
                    facadeItems[7].offsetY = modelData.mouthOffsetY;
                }
                catch (Exception e)
                {
                    Console.WriteLine("初始化模型错误：" + e.Message + "\n" + e.StackTrace);
                }

                try
                {
                    ui.uiFacade.UpdateModelData();
                }
                catch (Exception e)
                {
                    Console.WriteLine("刷新模型错误：" + e.Message + "\n" + e.StackTrace);
                }
                try
                {
                    ui.uiFacade.UpdateFacadeUI();
                }
                catch (Exception e)
                {
                    Console.WriteLine("刷新界面错误：" + e.Message + "\n" + e.StackTrace);
                }
            };
            ui.AddCor(g.timer.Frame(delayAction, 2));

            GameObject tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "kaishichuangguan"));
            tmpBtn.transform.SetParent(ui.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, -380);

            GameObject tmpText = CreateUI.NewText("完成", tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            try
            {
                ui.playerData.dynUnitData.inTrait.baseValue = unit.data.unitData.propertyData.inTrait;
                ui.playerData.dynUnitData.outTrait1.baseValue = unit.data.unitData.propertyData.outTrait1;
                ui.playerData.dynUnitData.outTrait2.baseValue = unit.data.unitData.propertyData.outTrait2;
            }
            catch (Exception e)
            {
                Console.WriteLine("初始化性格失败 " + e.ToString());
            }

            Action tmpAction = () =>
            {
                Action action = () =>
                {
                    try
                    {
                        BattleModelHumanData battleModelData = ui.playerData.unitData.propertyData.battleModelData;
                        var modelData = ui.playerData.unitData.propertyData.modelData.Clone();
                        unit.data.unitData.propertyData.modelData = modelData;
                        unit.data.dynUnitData.modelData = modelData;
                        unit.data.unitData.propertyData.battleModelData = battleModelData;
                        unit.data.dynUnitData.battleModelData = unit.data.unitData.propertyData.battleModelData.Clone();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("保存捏脸数据错误：" + e.Message + "\n" + e.StackTrace);
                    }
                    try
                    {
                        unit.data.unitData.propertyData.sex = tglMan.isOn ? UnitSexType.Man : UnitSexType.Woman;

                        string name = inputName.text;
                        if (!string.IsNullOrEmpty(name))
                        {
                            string name1 = name.Substring(0, 1);
                            string name2 = name.Substring(1, name.Length - 1);

                            unit.data.unitData.propertyData.name = new string[] { name1, name2 };
                        }

                        unit.data.unitData.propertyData.inTrait = ui.playerData.dynUnitData.inTrait.baseValue;
                        unit.data.unitData.propertyData.outTrait1 = ui.playerData.dynUnitData.outTrait1.baseValue;
                        unit.data.unitData.propertyData.outTrait2 = ui.playerData.dynUnitData.outTrait2.baseValue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("保存属性数据错误：" + e.Message + "\n" + e.StackTrace);
                    }
                    try
                    {
                        g.ui.CloseUI(UIType.CreatePlayer);
                        var ui_main = g.ui.GetUI<UIMapMain>(UIType.MapMain);
                        if (ui_main != null)
                        {
                            ui_main.uiPlayerInfo.OnPlayerEquipCloth();
                        }
                        SceneType.map.world.UpdatePlayerModel(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("刷新界面错误错误：" + e.Message + "\n" + e.StackTrace);
                    }
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_BaocunNielian"), 2, action);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);



            tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tuichu"));
            tmpBtn.transform.SetParent(ui.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(850, 350);

            tmpAction = () =>
            {
                Action action = () =>
                {
                    g.ui.CloseUI(UIType.CreatePlayer);
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "确定直接退出吗？不会保存修改数据！", 2, action);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);



            tmpBtn = CreateUI.NewText("捏脸修改：" + unit.data.unitData.propertyData.GetName());
            tmpBtn.transform.SetParent(ui.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 380);
            tmpBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 50);
            tmpBtn.GetComponent<Text>().color = Color.black;
            tmpBtn.GetComponent<Text>().fontSize = 30;
            tmpBtn.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            tmpAction = () =>
            {
                Action action = () =>
                {
                    g.ui.CloseUI(UIType.CreatePlayer);
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_TuichuNielian"), 2, action);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);
            return ui;
        }
    }

}
