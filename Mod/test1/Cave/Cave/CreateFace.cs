using Cave.Patch;
using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cave
{
    // 重新捏脸
    public class CreateFace
    {
        public CreateFace()
        {
        }

        public UICreatePlayer InitData(WorldUnitBase unit)
        {
            Cave.Log("打开捏脸 " + unit.data.unitData.propertyData.GetName());
            Patch_UICreatePlayer_DestroyUI.onCreateFace = true;
            var ui = g.ui.OpenUI<UICreatePlayer>(UIType.CreatePlayer);
            ui.transform.Find("Root/Meum").gameObject.SetActive(false);
            ui.transform.Find("Root/Group:Facade/LanguageGroup").gameObject.SetActive(false);
            ui.transform.Find("Root/Group:Facade/InTrait").gameObject.SetActive(false);
            ui.transform.Find("Root/Group:Facade/OutTrait").gameObject.SetActive(false);
            ui.InitData(100, GameLevelType.Common);

            Action delayAction = () =>
            {
                try
                {
                    int sex = (int)unit.data.unitData.propertyData.sex; ;
                    ui.playerData.dynUnitData.sex.baseValue = sex;
                    bool isWoman = unit.data.unitData.propertyData.sex == UnitSexType.Woman;
                    Cave.Log("sex = " + sex + "          isWoman = " + isWoman);
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
                    Cave.Log("初始化模型错误：" + e.Message + "\n" + e.StackTrace);
                }

                try
                {
                    ui.uiFacade.UpdateModelData();
                }
                catch (Exception e)
                {
                    Cave.Log("刷新模型错误：" + e.Message + "\n" + e.StackTrace);
                }
                try
                {
                    ui.uiFacade.UpdateFacadeUI();
                }
                catch (Exception e)
                {
                    Cave.Log("刷新界面错误：" + e.Message + "\n" + e.StackTrace);
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
                        Cave.Log("保存数据错误：" + e.Message + "\n" + e.StackTrace);
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
                        Cave.Log("刷新界面错误错误：" + e.Message + "\n" + e.StackTrace);
                    }
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "确定保存捏脸并退出吗？", 2, action);
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
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "确定直接退出吗？将不会保存捏脸结果！", 2, action);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);



            tmpBtn = CreateUI.NewText("梳妆台—" + unit.data.unitData.propertyData.GetName());
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
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "确定直接退出吗？将不会保存捏脸结果！", 2, action);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);
            return ui;
        }
    }
}
