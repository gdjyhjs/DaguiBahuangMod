using GuiBaseUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gymnasium.Patch
{
    [HarmonyPatch(typeof(UIMartialRealize), "UpdateUI")]
    class Patch_UIMartialRealize_UpdateUI
    {
        [HarmonyPostfix]
        private static void Postfix(UIMartialRealize __instance)
        {
            if (Gumnasium.openGumnasium)
            {
                UpdateUI(__instance);
            }
        }

        private static void UpdateUI(UIMartialRealize ui)
        {
            // 判断技能等级
            DataProps.MartialData martialData = ui.martialData;
            DataProps.PropsData propsData = martialData.data;
            DataProps.PropsSkillData data = propsData.To<DataProps.PropsSkillData>();
            int level = data.martialInfo.level; // 技能的颜色
            int grade = data.martialInfo.grade; // 技能的境界
            bool isUpGrade = level == 6; // 是否可以提升境界

            ui.textTip.transform.parent.Find("Item").gameObject.SetActive(false);
            ui.textTip.text = GameTool.LS(isUpGrade ? "Gymnasium_gradeTips" : "Gymnasium_levelTips");
            ui.textOK.text = GameTool.LS("Gymnasium_textRealize");
            ui.textTitle.text = GameTool.LS("Gymnasium_textTitle");

            ui.btnOK.onClick.RemoveAllListeners();
            // 重写按钮功能
            Action action = () =>
            {
                if (isUpGrade)
                {
                    UpGrade(ui);
                }
                else
                {
                    UpLevel(ui);
                }
            };
            ui.btnOK.onClick.AddListener(action);
        }

        // 提升品质
        private static void UpLevel(UIMartialRealize ui)
        {
            DataProps.MartialData martialData = ui.martialData;
            DataProps.PropsData propsData = martialData.data;
            DataProps.PropsSkillData data = propsData.To<DataProps.PropsSkillData>();
            if (data.martialInfo.level >= 6)
            {
                UITipItem.AddTip(GameTool.LS("Gymnasium_gradeUpMax"));
                return;
            }

            Action<List<DataProps.PropsData>> upLevel = (list) =>
            {
                bool ok = true;
                // 升级技能品质，消耗3本同等境界以上更高品质的秘籍。
                for (int i = 0; i < list.Count; i++)
                {
                    DataProps.PropsData props = list[i];
                    var martial = props.To<DataProps.PropsSkillData>().martialInfo;
                    if (martial.level <= data.martialInfo.level || martial.grade < data.martialInfo.grade)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    new Tips(GameTool.LS("common_tishi"), string.Format(GameTool.LS("Gymnasium_levelUpOk" + (data.martialInfo.level + 1)),
                        GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 1)), 2, ()=>
                        {
                            data.martialInfo.level = data.martialInfo.level + 1;
                            ui.UpdateUI();
                        }, null);
                }
                else
                {
                    new Tips(GameTool.LS("common_tishi"), GameTool.LS("common_tishi"), 2, null, null);
                }
            };

            InputItem(ui, upLevel);
        }

        // 提升境界
        private static void UpGrade(UIMartialRealize ui)
        {
            DataProps.MartialData martialData = ui.martialData;
            DataProps.PropsData propsData = martialData.data;
            DataProps.PropsSkillData data = propsData.To<DataProps.PropsSkillData>();
            if (data.martialInfo.grade >= 10)
            {
                UITipItem.AddTip(GameTool.LS("Gymnasium_gradeUpMax"));
                return;
            }




            Action<List<DataProps.PropsData>> upGrade = (list) =>
            {
                bool ok = true;
                // 升级技能品质，消耗3本同等境界以上更高品质的秘籍。
                for (int i = 0; i < list.Count; i++)
                {
                    DataProps.PropsData props = list[i];
                    var martial = props.To<DataProps.PropsSkillData>().martialInfo;
                    if (martial.level < 6 || martial.grade <= data.martialInfo.grade)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    data.martialInfo.grade = data.martialInfo.grade + 1;

                    new Log("获取词条数量 技能ID="+ martialData.baseID);
                    int prefixCount = g.conf.battleSkillLevel.GetPrefixCount(data.martialInfo.level, data.martialInfo.grade, g.conf.battleSkillLevel.GetGradeMin(martialData.martialType, martialData.baseID));
                    new Log("获取词条数量=" + prefixCount);
                    Il2CppSystem.Collections.Generic.List<SkillPrefixData> prefixDatasTrain = g.conf.battleSkillPrefixValue.GetPrefix(martialData);
                    new Log("当前词条数量=" + prefixDatasTrain.Count);
                    var aaa = g.conf.battleSkillPrefixValue.RandomPrefixs(prefixCount, martialData.baseID, data.martialInfo.grade, data.martialInfo.level, null);

                    CallQueue cq = new CallQueue();

                    g.ui.OpenUI(UIType.Mask);
                    GameEffectTool.CreateGo("Effect/UI/canwuwancheng", ui.transform, ui.sortingOrder + 1, 6f);
                    Action action = () =>
                    {
                        ui.UpdateUI();
                        g.ui.CloseUI(UIType.Mask);
                        UITipItem.AddTip(GameTool.LS("canwu_chenggong"));
                    };
                    ui.DelayTime(action, 2.2f);

                    for (int i = prefixDatasTrain.Count; i < prefixCount; i++)
                    {
                        Transform tf = ui.goPrefixRoot.transform.GetChild(i);
                        new Log($"第{i}个" + tf);
                        GameObject go = null;
                        if (tf == null)
                        {
                            go = GameObject.Instantiate(ui.goPrefixItem, ui.goPrefixRoot.transform);
                            go.SetActive(true);
                        } else
                        {
                            go = tf.gameObject;
                        }
                        int idx = i;
                        int newLevel = 6;

                        Action action1 = () =>
                        {
                            new Log($"第{i}条词条");
                            go.transform.Find("Loading").gameObject.SetActive(true);
                            GameEffectTool.CreateGo("Effect/UI/canwuzhong", go.transform, ui.sortingOrder + 1, 3f);
                            Action delayAction1 = () =>
                            {
                                if (newLevel >= 4)
                                {
                                    GameEffectTool.CreateGo("Effect/UI/canwucitiao" + newLevel, go.transform, ui.sortingOrder + 1, 3f);
                                }
                                //string prefixText = UIMartialInfoTool.GetDescRichText(prefixData, prefixData.prefixValueItemDesc, new BattleSkillValueData(martialData), "", "e", 1, true);
                                //prefixText = GameTool.SetTextReplaceColorKey(prefixText, GameTool.LevelToColorKey(newLevel), 1);
                                //go.transform.Find("TextRightPrefix").GetComponent<TMPro.TextMeshProUGUI>().text = prefixText;
                                go.transform.Find("TextRightPrefix").gameObject.AddComponent<UITextTypewriterEffect>().PlayEffectTime(1f, null, false);

                                Action delayAction2 = () =>
                                {
                                    //int prefixState = UnitActionMartialRealize.GetPerfect(martialData, prefixData);
                                    //go.transform.Find("RightState").gameObject.SetActive(prefixState != 0);
                                    //go.transform.Find("RightState").GetComponent<Image>().SetSpriteAutoSize(SpriteTool.GetSprite("MartialInfoCommon", "biaoshi_" + (newLevel - 3)));
                                    //if (prefixState == 1) go.transform.Find("RightState/Text").GetComponent<Text>().text = GameTool.LS("canwu_wanmei");
                                    //else if (prefixState == 2) go.transform.Find("RightState/Text").GetComponent<Text>().text = GameTool.LS("canwu_jiejinwanmei");

                                    //if (prefixState != 0)
                                    //{
                                    //    GameEffectTool.CreateGo("Effect/UI/ChuangjueQiyun" + (newLevel - 3), go.transform.Find("RightState"), sortingOrder + 1, 3f);
                                    //}

                                    //int oldLevel = 0;
                                    //int restoreCost = g.conf.battleSkillFlashCost.GetItem(martialData.martialInfo.grade, oldLevel).flashCost;
                                    //go.transform.Find("Toggle/Text").GetComponent<Text>().text = GameTool.LS("canwu_huisu");
                                    //go.transform.Find("Toggle/TextTitle").GetComponent<Text>().text = GameTool.LS("wuxue_xiaohao");
                                    //go.transform.Find("Toggle/TextValue").GetComponent<Text>().text = restoreCost.ToString();
                                    //go.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener((v) =>
                                    //{
                                    //    if (v)
                                    //    {
                                    //        if (restoreCost + GetUseRestorePoint() > maxRestorePoint)
                                    //        {
                                    //            UITipItem.AddTip(GameTool.LS("canwu_huisudianbuzu"));
                                    //            go.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
                                    //            return;
                                    //        }
                                    //    }
                                    //    go.transform.Find("LeftLine").DOScaleX(v ? 0f : 1f, 0.1f);
                                    //    go.transform.Find("RightLine").DOScaleX(v ? 1f : 0f, 0.1f);

                                    //    go.transform.Find("Toggle/Text").GetComponent<Text>().text = v ? GameTool.LS("canwu_huisuquxiao") : GameTool.LS("canwu_huisu");
                                    //    realizeData.ingore = v;
                                    //    UpdateRestoreValueUI();
                                    //});

                                    cq.Next();
                                };
                                ui.DelayTime(delayAction2, 0.5f);
                            };

                            //go.transform.Find("Loading/Text").gameObject.AddComponent<UITextTypewriterEffect>().PlayEffectTime(1f, GameTool.LS("canwu_canwuzhong") + "......", () => {
                            //    go.transform.Find("Loading").gameObject.SetActive(false);
                            //    ui.AddCor(g.timer.Time(delayAction1, 0.1f));
                            //}, false);
                        };

                    }
                    Action action2 = cq.Next;
                    Action action3 = () => { ui.DelayTime(action2, 0.3f); };
                    cq.Add(action3);
                    Action action4 = () =>
                    {
                        ui.btnOK.gameObject.SetActive(true);
                        g.ui.CloseUI(UIType.Mask);
                    };
                    cq.Add(action4);
                    cq.Run();

                    new Tips(GameTool.LS("common_tishi"), string.Format(GameTool.LS("Gymnasium_gradeUpOk"),
                        GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 1),
                        g.conf.roleGrade.GetGradeName(martialData.data.To<DataProps.MartialData>().martialInfo.grade)), 2, ()=>
                        {
                            ui.UpdateUI();
                        }, null);
                }
                else
                {
                    new Tips(GameTool.LS("common_tishi"), GameTool.LS("common_tishi"), 2, null, null);
                }
            };

            InputItem(ui, upGrade);
        }

        // 放入物品
        private static void InputItem(UIMartialRealize mui, Action<List<DataProps.PropsData>> action)
        {
            DataProps.MartialData martialData = mui.martialData;
            DataProps.PropsData propsData = martialData.data;
            DataProps.PropsSkillData data = propsData.To<DataProps.PropsSkillData>();
            int level = data.martialInfo.level; // 技能的颜色
            bool isUpGrade = level == 6; // 是否可以提升境界

            UIPropSelect ui = g.ui.OpenUI<UIPropSelect>(UIType.PropSelect);
            ui.textInfo.text = GameTool.LS(isUpGrade ? "Gymnasium_gradeUpPutItemTitle" : "Gymnasium_levelUpPutItemTitle");
            ui.dataMartial.NotFindAll();
            ui.dataProp.number = 3;
            ui.dataProp.propsFilter.type = new Il2CppSystem.Collections.Generic.List<int>();
            ui.dataProp.propsFilter.type.Add(5);
            ui.dataProp.propsFilter.className = new Il2CppSystem.Collections.Generic.List<int>();
            ui.dataProp.propsFilter.className.Add(5);
            ui.UpdateFilter();

            Action okAction = () =>
            {
                //var aa = ui.allSlectDataProps.allShowProps;
                Il2CppSystem.Collections.Generic.List<DataProps.PropsData> select = UIPropSelect.allSlectItems;
                List<DataProps.PropsData> list = new List<DataProps.PropsData>();
                foreach (var item in select)
                {
                    Cave.Cave.Log("选择道具：" + item.propsID);
                    list.Add(item);
                }
                action(list);
            };
            ui.onOKCall = okAction;
        }
    }
}
