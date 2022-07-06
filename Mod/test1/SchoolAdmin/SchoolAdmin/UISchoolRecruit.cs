using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SchoolAdmin
{
    public class UISchoolRecruit
    {
        UISchoolMainLobby ui;
        MapBuildSchool school { get { return ui.school; } }
        bool ingonCondition;
        bool ingonNeed;
        int sortType = 0;

        public UISchoolRecruit(UISchoolMainLobby ui)
        {
            this.ui = ui;
            var playerSchool = g.world.playerUnit.data.school;
            if (playerSchool != null && playerSchool.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
            {
            }
            else
            {
                UITipItem.AddTip("宗主才能执行此操作！");
                return;
            }

            if (ui == null)
            {
                SchoolAdmin.Log("无法获取议事大厅UI");
                return;
            }
            if (ui.overview.goProperty.transform.Find("btnSchoolRecruit") != null)
            {
                SchoolAdmin.Log("已经创建管家按钮UI");
                return;
            }

            GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));

            GameObject go = null;
            Action clickOpenUI = () =>
            {
                var school = g.world.playerUnit.data.school;
                if (school != null && school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
                {
                    if (go != null && !go.activeSelf)
                    {
                        GameObject.Destroy(ui.GetComponent<UIFastClose>());
                        go.SetActive(true);

                        InitUI(bg);
                    }
                }
                else
                {
                    UITipItem.AddTip("宗主才能执行此操作！");
                }
            };

            GameObject btnSchoolAdmin = GameObject.Instantiate(ui.overview.btnResAllot.gameObject);
            btnSchoolAdmin.name = "btnSchoolRecruit";
            btnSchoolAdmin.transform.SetParent(ui.overview.goProperty.transform, false);
            btnSchoolAdmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(btnSchoolAdmin.GetComponent<RectTransform>().anchoredPosition.x, btnSchoolAdmin.GetComponent<RectTransform>().anchoredPosition.y - 40);
            btnSchoolAdmin.transform.GetComponentInChildren<Text>().text = "召唤管家";
            btnSchoolAdmin.GetComponent<Button>().onClick.AddListener(clickOpenUI);
            btnSchoolAdmin.SetActive(true);

            go = CreateUI.NewImage();
            go.transform.SetParent(ui.btnClose.transform.parent, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(99999, 99999);
            go.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            go.SetActive(false);

            bg.GetComponent<Image>().type = Image.Type.Tiled;
            bg.transform.SetParent(go.transform, false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(1450, 980);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);

            // 关闭按钮
            Action clickCloseUI = () =>
            {
                if (go != null && go.activeSelf)
                {
                    ui.gameObject.AddComponent<UIFastClose>();
                    go.SetActive(false);

                }
            };
            var btnSchoolAdminClose = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tuichu"));
            btnSchoolAdminClose.transform.SetParent(bg.transform, false);
            btnSchoolAdminClose.AddComponent<Button>().onClick.AddListener(clickCloseUI);
            btnSchoolAdminClose.GetComponent<RectTransform>().anchoredPosition = new Vector2(596, 423);
            btnSchoolAdminClose.name = "btnClose";

            TimerCoroutine timer = null;
            Action framAction = () =>
            {
                if (go == null)
                {
                    g.timer.Stop(timer);
                }
                if (Input.GetMouseButtonDown(1) && g.ui.GetUI(UIType.NPCInfo) == null && g.ui.GetUI(UIType.CheckPopup) == null)
                {
                    clickCloseUI();
                }
            };
            timer = g.timer.Frame(framAction, 1, true);

            { // 显示无需求的NPC
                GameObject go1, go2 = null, go3;
                Action action = () =>
                {
                    ingonNeed = !ingonNeed;
                    go2.SetActive(ingonNeed);
                    InitUI(bg);
                };
                go1 = CreateUI.NewButton(action, SpriteTool.GetSprite("Common", "kuang"));
                go2 = CreateUI.NewImage(SpriteTool.GetSprite("Common", "xianshigou"));
                go2.transform.SetParent(go1.transform, false);
                go3 = CreateUI.NewText("显示未知需求的NPC");
                go3.transform.SetParent(go1.transform, false);
                go3.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);
                go3.GetComponent<RectTransform>().anchoredPosition = new Vector2(15 + go3.GetComponent<RectTransform>().sizeDelta.x / 2, 0);
                go3.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                go3.GetComponent<Text>().color = Color.black;

                go1.transform.SetParent(bg.transform, false);
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-580, 450);
                go2.SetActive(ingonNeed);
            }
            { // 显示不合符入宗条件的NPC
                GameObject go1, go2 = null, go3;
                Action action = () =>
                {
                    ingonCondition = !ingonCondition;
                    go2.SetActive(ingonCondition);
                    InitUI(bg);
                };
                go1 = CreateUI.NewButton(action, SpriteTool.GetSprite("Common", "kuang"));
                go2 = CreateUI.NewImage(SpriteTool.GetSprite("Common", "xianshigou"));
                go2.transform.SetParent(go1.transform, false);
                go3 = CreateUI.NewText("显示不合符入宗条件的NPC");
                go3.transform.SetParent(go1.transform, false);
                go3.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);
                go3.GetComponent<RectTransform>().anchoredPosition = new Vector2(15 + go3.GetComponent<RectTransform>().sizeDelta.x / 2, 0);
                go3.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                go3.GetComponent<Text>().color = Color.black;

                go1.transform.SetParent(bg.transform, false);
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-280, 450);
                go2.SetActive(ingonCondition);
            }

            { // 排序 sortType {名字，性别，境界}
                string[] sortName = new string[] { "默认排序", "名字排序", "性别排序", "境界排序" };

                GameObject go1 = null, go3 = null, go4 = null, goBtn, goBtnText;
                Action action = null;

                Action btnTextAction = () =>
                {

                    if (sortType > 0)
                    {
                        go3.GetComponent<Text>().text = "↑" + sortName[sortType];
                    }
                    else if (sortType < 0)
                    {
                        go3.GetComponent<Text>().text = "↓" + sortName[Mathf.Abs(-sortType)];
                    }
                    else
                    {
                        go3.GetComponent<Text>().text = sortName[0];
                    }
                };


                action = () =>
                {
                    if (go4 == null)
                    {
                        go4 = CreateUI.NewImage();
                        go4.transform.SetParent(bg.transform, false);
                        go4.GetComponent<RectTransform>().sizeDelta = new Vector2(10000, 10000);
                        go4.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
                        go4.AddComponent<Button>().onClick.AddListener(action);

                        Action sortAction = null;

                        for (int i = 0; i < 4; i++)
                        {
                            int idx = i;
                            if (i == 0)
                            {
                                sortAction = () =>
                                {
                                    if (sortType != 0)
                                    {
                                        sortType = 0;
                                        InitUI(bg);
                                        btnTextAction();
                                    }
                                    GameObject.Destroy(go4);
                                    go4 = null;
                                };
                            }
                            else
                            {
                                sortAction = () =>
                                {
                                    if (sortType == idx)
                                    {
                                        sortType = -idx;
                                    }
                                    else
                                    {
                                        sortType = idx;
                                    }
                                    InitUI(bg);
                                    btnTextAction();
                                    GameObject.Destroy(go4);
                                    go4 = null;
                                };
                            }

                            goBtn = go1 = CreateUI.NewButton(sortAction);
                            goBtn.transform.SetParent(go4.transform, false);
                            goBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-580, 400 - 50 - i * 50);

                            goBtnText = CreateUI.NewText(sortName[i]);
                            goBtnText.transform.SetParent(goBtn.transform, false);
                            goBtnText.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                            goBtnText.GetComponent<Text>().color = Color.black;
                            goBtnText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        }
                    }
                    else
                    {
                        GameObject.Destroy(go4);
                        go4 = null;
                    }
                };
                go1 = CreateUI.NewButton(action);

                if (sortType > 0)
                {
                    go3 = CreateUI.NewText("↑" + sortName[sortType]);
                }
                else if (sortType < 0)
                {
                    go3 = CreateUI.NewText("↓" + sortName[Mathf.Abs(-sortType)]);
                }
                else
                {
                    go3 = CreateUI.NewText(sortName[0]);
                }

                go3.transform.SetParent(go1.transform, false);
                go3.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                go3.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                go3.GetComponent<Text>().color = Color.black;

                go1.transform.SetParent(bg.transform, false);
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-580, 400);
            }
        }


        private List<WorldUnitBase> allUnit = new List<WorldUnitBase>();

        // 初始化要显示的列表
        private void InitUI(GameObject bg)
        {
            allUnit.Clear();
            g.ui.OpenUI(UIType.Loading);

            Action threadAction = () =>
            {
                CallQueue cq = new CallQueue();

                allUnit.Clear();
                Il2CppSystem.Collections.Generic.List<WorldUnitBase> allUnitTemp = g.world.unit.GetUnits();
                foreach (WorldUnitBase item in allUnitTemp)
                {
                    WorldUnitBase unit = item;

                    Action unitAction = () =>
                    {
                        bool canAdd, checkCondition, checkNeed;

                        if (ingonCondition)
                        {
                            checkCondition = true;
                        }
                        else
                        {
                            if (new CheckJoinSchool(school, unit).GetResult() == 0 && !MapBuildSchool.IsEquipFactions(school, unit.data.school))
                            {
                                checkCondition = true;
                            }
                            else
                            {
                                checkCondition = false;
                            }
                        }

                        if (ingonNeed) {
                            checkNeed = true;
                        } else {
                            checkNeed = false;
                            if (unit.allTroubles.Count > 0)
                            {
                                foreach (UnitTroubleBase trouble in unit.allTroubles)
                                {
                                    if (trouble.troubleBaseItem.id == 10601 || trouble.troubleBaseItem.id == 10701 || trouble.troubleBaseItem.id == 10801)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        checkNeed = true;
                                        break;
                                    }
                                }
                            }
                        }

                        canAdd = checkCondition && checkNeed;
                        if (canAdd && unit != g.world.playerUnit)
                        {
                            allUnit.Add(unit);
                        }
                        cq.Next();
                    };
                    cq.Add(unitAction);
                }

                Action nextAction = () =>
                {
                    switch (sortType)
                    {
                        case 1: // 名字正序
                            allUnit.Sort((a, b) => string.Compare(a.data.unitData.propertyData.GetName(), b.data.unitData.propertyData.GetName()));
                            break;
                        case 2: // 性别正序
                            allUnit.Sort((a, b) => a.data.unitData.propertyData.sex == b.data.unitData.propertyData.sex ? 0 : (a.data.unitData.propertyData.sex < b.data.unitData.propertyData.sex) ? -1 : 1);
                            break;
                        case 3: // 境界正序
                            allUnit.Sort((a, b) => a.data.dynUnitData.GetGrade() == b.data.dynUnitData.GetGrade() ? 0 : (a.data.dynUnitData.GetGrade() < b.data.dynUnitData.GetGrade()) ? -1 : 1);
                            break;
                        case -1:
                            allUnit.Sort((a, b) => string.Compare(b.data.unitData.propertyData.GetName(), a.data.unitData.propertyData.GetName()));
                            break;
                        case -2:
                            allUnit.Sort((a, b) => a.data.unitData.propertyData.sex == b.data.unitData.propertyData.sex ? 0 : (a.data.unitData.propertyData.sex > b.data.unitData.propertyData.sex) ? -1 : 1);
                            break;
                        case -3:
                            allUnit.Sort((a, b) => a.data.dynUnitData.GetGrade() == b.data.dynUnitData.GetGrade() ? 0 : (a.data.dynUnitData.GetGrade() > b.data.dynUnitData.GetGrade()) ? -1 : 1);
                            break;
                        default:
                            break;
                    }

                    Action endAction = () =>
                    {
                        g.ui.CloseUI(UIType.Loading);
                        page = 0;
                        maxPage = Mathf.CeilToInt(allUnit.Count * 1f / pageCount);
                        UpdateNpcList(bg.transform);
                    };
                    GameTool.RunInMainThread(endAction);
                };
                cq.Add(nextAction);
                cq.Run();
            };

            g.timer.Thread(threadAction);
        }

        int pageCount = 15;
        int page = 0;
        int maxPage = 0;
        GameObject connect;

        private void UpdateNpcList(Transform bg)
        {

            if (connect != null)
            {
                GameObject.Destroy(connect);
            }

            connect = GuiBaseUI.CreateUI.New();
            connect.transform.SetParent(bg.transform, false);
            connect.transform.SetSiblingIndex(bg.transform.childCount - 2);

            float x = -580;
            float y = 360;

            for (int i = page * pageCount; i < page * pageCount + pageCount; i++)
            {
                if (i >= allUnit.Count)
                    break;
                WorldUnitBase unit = allUnit[i];

                UpdateNpcData(connect.transform, x, y, unit);
                y -= 46;
            }

            GameObject go = GuiBaseUI.CreateUI.NewText((page + 1) + "/" + (maxPage), new Vector2(100, 20));
            go.transform.SetParent(connect.transform, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -330);
            go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            go.GetComponent<Text>().color = Color.black;

            if (maxPage > 0)
            {
                Action action = () =>
                {
                    page--;
                    if (page < 0)
                    {
                        page = maxPage - 1;
                    }
                    UpdateNpcList(bg);
                };
                go = GuiBaseUI.CreateUI.NewButton(action);
                go.transform.SetParent(connect.transform, false);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(- 150, -330);

                GameObject go2 = GuiBaseUI.CreateUI.NewText("上一页", new Vector2(100, 20));
                go2.transform.SetParent(go.transform, false);
                go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                go2.GetComponent<Text>().color = Color.black;

                action = () =>
                {
                    page++;
                    if (page >= maxPage)
                    {
                        page = 0;
                    }
                    UpdateNpcList(bg);
                };
                go = GuiBaseUI.CreateUI.NewButton(action);
                go.transform.SetParent(connect.transform, false);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, -330);

                go2 = GuiBaseUI.CreateUI.NewText("下一页", new Vector2(100, 20));
                go2.transform.SetParent(go.transform, false);
                go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                go2.GetComponent<Text>().color = Color.black;
            }
        }

        private void UpdateNpcData(Transform connect, float x, float y, WorldUnitBase unit)
        {
            GameObject go = GuiBaseUI.CreateUI.NewText(unit.data.unitData.propertyData.GetName(), new Vector2(100, 20));
            go.transform.SetParent(connect, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            go.GetComponent<Text>().color = Color.black;
            x += 120;

            Action action = () =>
            {
                UINPCInfo ui = g.ui.OpenUI<UINPCInfo>(UIType.NPCInfo);
                ui.InitData(unit, false);
                ui.uiUnitInfo.ActiveMeum(false);
            };
            go = GuiBaseUI.CreateUI.NewButton(action);
            go.transform.SetParent(connect, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            GameObject go2 = GuiBaseUI.CreateUI.NewText("查看", new Vector2(100, 20));
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            go2.GetComponent<Text>().color = Color.black;

            x += 120;


            action = () =>
            {
                UIPropSelect ui = g.ui.OpenUI<UIPropSelect>(UIType.PropSelect);
                ui.isOKCost = false;
                ui.dataProp.propsKindMax = 10;
                ui.dataProp.number = 1;
                ui.RemoveSelectPropsData(PropsIDType.Money);
                ui.dataProp.propsFilter.containDieNotDrop = false;
                ui.UpdateFilter();
                Action onOKCall = () =>
                {
                    bool isOk = false;
                    foreach (DataProps.PropsData item in UIPropSelect.allSlectItems)
                    {
                        bool isNeed = false;
                        foreach (UnitTroubleBase trouble in unit.allTroubles)
                        {
                            if (trouble.IsLikeProps(item))
                            {
                                isNeed = true;
                            }
                        }
                        if (isNeed)
                        {
                            isOk = true;
                            var list = new Il2CppSystem.Collections.Generic.List<DataProps.PropsData>();
                            list.Add(item);
                            UnitActionRoleGive give = new UnitActionRoleGive(unit, list);
                            Action giveEndAction = () =>
                            {
                                if (unit.data.school != null)
                                {
                                    unit.data.school.buildData.ExitSchool(unit.data.school, unit.data.unitData.unitID);
                                }

                                //g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), $"已将{unit.data.unitData.propertyData.GetName()}邀请至{school.name}？", 1);
                                UICustomDramaDyn dramaDyn = new UICustomDramaDyn(21802);
                                dramaDyn.dramaData.unit = unit;
                                dramaDyn.dramaData.dialogueText[21802] = $"我正需要此物，您送的太及时了，今后我就是{school.name}的人了，还请宗主大人多多关照！";
                                dramaDyn.OpenUI();
                            };
                            give.onEndCall = giveEndAction;
                            int ok = g.world.playerUnit.CreateAction(give, true);
                            break;
                        }
                    }
                    if (!isOk)
                    {
                        UICustomDramaDyn dramaDyn = new UICustomDramaDyn(21802);
                        dramaDyn.dramaData.unit = unit;
                        dramaDyn.dramaData.dialogueText[21802] = $"就这也想要让我加入{school.name}？";
                        dramaDyn.OpenUI();
                    }
                };

                ui.onOKCall = onOKCall;
            };
            go = GuiBaseUI.CreateUI.NewButton(action);
            go.transform.SetParent(connect, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            go2 = GuiBaseUI.CreateUI.NewText("邀请", new Vector2(100, 20));
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            go2.GetComponent<Text>().color = Color.black;

            x += 120;


            action = () =>
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), $"打听到{unit.data.unitData.propertyData.GetName()}当前所在位置{unit.data.unitData.GetPoint().ToString()}。", 1);
            };
            go = GuiBaseUI.CreateUI.NewButton(action);
            go.transform.SetParent(connect, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            go2 = GuiBaseUI.CreateUI.NewText("追踪", new Vector2(100, 20));
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            go2.GetComponent<Text>().color = Color.black;


            string need = "未知";
            if (unit.allTroubles.Count > 0)
            {
                foreach (UnitTroubleBase trouble in unit.allTroubles)
                {
                    try
                    {
                        switch (trouble.troubleBaseItem.id)
                        {
                            case 10101:
                                need = trouble.GetTextValues()[1];
                                break;
                            case 10201:
                                need = GameTool.LS(g.conf.itemProps.GetItem(int.Parse(trouble.GetTextValues()[1].Split('|')[2])).name);
                                break;
                            case 10301:
                                need = trouble.GetTextValues()[0] + "法宝";
                                break;
                            case 10401:
                                need = trouble.GetTextValues()[0] + "坐骑";
                                break;
                            case 10501:
                                need = trouble.GetTextValues()[0] + "戒指";
                                break;
                            case 10601:
                            case 10701:
                            case 10801:
                                continue;
                            default:
                                need = string.Join(">", trouble.GetTextValues()) + "。" + trouble.troubleBaseItem.id;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        need = string.Join(">", trouble.GetTextValues()) + ">>" + trouble.troubleBaseItem.id + ">>" + e.Message;
                    }
                }
            }

            x += 470;

            go = GuiBaseUI.CreateUI.NewText("需求："+need, new Vector2(800, 20));
            go.transform.SetParent(connect, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            go.GetComponent<Text>().color = Color.black;
        }
    }
}
