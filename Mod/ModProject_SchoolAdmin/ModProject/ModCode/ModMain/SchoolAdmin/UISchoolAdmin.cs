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
    public class UISchoolAdmin
    {
        Toggle[] ui_mainTog;
        List<List<UIBehaviour>> ui_subTog;
        List<CanvasGroup> ui_canvas;
        List<RectTransform> uipageTog;
        int scrollPage;
        List<Toggle> ui_schools;
        Il2CppSystem.Collections.Generic.List<MapBuildSchool> schools;
        SchoolAdminData data;


        string[] pageNames = new string[] {
            GameTool.LS("schoolAdmin_condition")+"1",
            GameTool.LS("schoolAdmin_condition")+"2",
            GameTool.LS("schoolAdmin_condition")+"3",
            GameTool.LS("schoolAdmin_condition")+"4",
            GameTool.LS("schoolAdmin_condition")+"5",
            GameTool.LS("schoolAdmin_condition")+"6",
            GameTool.LS("schoolAdmin_condition")+"7",
            GameTool.LS("schoolAdmin_condition")+"8" };

        string[] funcNames = new string[] {
             GameTool.LS("common_xingbie")+GameTool.LS("maohao"),
             GameTool.LS("common_meili")+GameTool.LS("maohao"),
             GameTool.LS("playerInfo_xingquaihao")+GameTool.LS("maohao"),
             GameTool.LS("common_shenwang")+GameTool.LS("maohao"),
             GameTool.LS("npcInfo_xingge")+GameTool.LS("maohao"),
             GameTool.LS("common_jingjie")+GameTool.LS("maohao"),
             GameTool.LS("schoolAdmin_marriage")+GameTool.LS("maohao"),
             GameTool.LS("schoolAdmin_surname")+GameTool.LS("maohao"),
             GameTool.LS("mod_daoxin")+GameTool.LS("maohao")
        };

        string[] funcDess = new string[] {
            GameTool.LS("common_xingbie")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips1"),
            GameTool.LS("common_meili")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips2"),
            GameTool.LS("playerInfo_xingquaihao")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips3"),
            GameTool.LS("common_shenwang")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips4"),
            GameTool.LS("npcInfo_xingge")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips5"),
            GameTool.LS("common_jingjie")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips6"),
            GameTool.LS("schoolAdmin_marriage")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips7"),
            GameTool.LS("schoolAdmin_surname")+GameTool.LS("maohao")+GameTool.LS("schoolAdmin_tips8"),
            GameTool.LS("mod_daoxin")+GameTool.LS("maohao") + GameTool.LS("schoolAdmin_tips9")
        };

        public UISchoolAdmin(UISchoolPost ui)
        {
            var playerSchool = g.world.playerUnit.data.school;

            if (ui == null)
            {
                SchoolAdmin.Log("无法获取职位晋升UI");
                return;
            }
            if (ui.btnClose.transform.parent.Find("btnSchoolAdmin") != null)
            {
                SchoolAdmin.Log("已经创建入门条件UI");
                return;
            }


            InitOneKey(ui);

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

                        data = SchoolAdminData.ReadData();

                        UpdateUI();
                    }
                }
                else
                {
                    UITipItem.AddTip(GameTool.LS("schoolAdmin_tips13"));
                }
            };
            var btnSchoolAdmin = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "zhiweijinshenglingqu"));
            btnSchoolAdmin.name = "btnSchoolAdmin";
            btnSchoolAdmin.transform.SetParent(ui.btnClose.transform.parent, false);
            btnSchoolAdmin.AddComponent<Button>().onClick.AddListener(clickOpenUI);
            btnSchoolAdmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(-420, -228);

            GameObject textSchoolAdmin = CreateUI.NewText(GameTool.LS("npcInfo_zhaona") +" "+ GameTool.LS("schoolAdmin_condition"));
            textSchoolAdmin.transform.SetParent(btnSchoolAdmin.transform, false);
            textSchoolAdmin.GetComponent<RectTransform>().sizeDelta = btnSchoolAdmin.GetComponent<RectTransform>().sizeDelta;
            textSchoolAdmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 5);
            textSchoolAdmin.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;



            go = CreateUI.NewImage();
            go.transform.SetParent(ui.btnClose.transform.parent, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(99999, 99999);
            go.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            go.SetActive(false);

            GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
            bg.GetComponent<Image>().type = Image.Type.Tiled;
            bg.transform.SetParent(go.transform, false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(1450, 1100);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);

            // 关闭按钮
            Action clickCloseUI = () =>
            {
                if (go != null && go.activeSelf)
                {
                    ui.gameObject.AddComponent<UIFastClose>();
                    go.SetActive(false);
                    Action okAction = () =>
                    {
                        SchoolAdminData.SaveData(data);
                        data = SchoolAdminData.ReadData();
                    };
                    Action noAction = () =>
                    {
                        data = SchoolAdminData.ReadData();
                    };
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("schoolAdmin_zhaonatiaojian"), 2, okAction, noAction);

                }
            };
            var btnSchoolAdminClose = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tuichu"));
            btnSchoolAdminClose.transform.SetParent(bg.transform, false);
            btnSchoolAdminClose.AddComponent<Button>().onClick.AddListener(clickCloseUI);
            btnSchoolAdminClose.GetComponent<RectTransform>().anchoredPosition = new Vector2(596, 423);

            TimerCoroutine timer = null;
            Action framAction = () =>
            {
                if (go == null)
                {
                    g.timer.Stop(timer);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    clickCloseUI();
                }
            };
            timer = g.timer.Frame(framAction, 1, true);

            InitPage(bg);
            InitUI(bg);
            InitSchool(bg);
        }

        // 一键踢人
        private void InitOneKey(UISchoolPost ui)
        {

            Action action2 = () =>
            {
                var school = g.world.playerUnit.data.school;
                if (school != null && school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
                {
                    MapBuildSchool playerSchool = g.world.playerUnit.data.school;
                    var allSchool = playerSchool.GetAllSchools(true);
                    List<WorldUnitBase> units = new List<WorldUnitBase>();
                    foreach (var s in allSchool)
                    {
                        var allUnits = g.world.unit.GetUnit(s.buildData.GetPostUnit(SchoolPostType.None), false);
                        foreach (var u in allUnits)
                        {
                            if (u == g.world.playerUnit || s.buildData.GetPostType(u.data.unitData.unitID) == SchoolPostType.SchoolMain)
                                continue;
                            var result = new CheckJoinSchool(s, u).GetResult();
                            if (result != 0)
                            {
                                SchoolAdmin.Log(s.name + " " + u.data.unitData.propertyData.GetName() + " 不符宗门条件 即将离开-----------", true);
                                units.Add(u);
                            }
                            else
                            {
                                SchoolAdmin.Log(s.name + " " + u.data.unitData.propertyData.GetName() + " 通过宗门条件 可以留下+++++++++++", true);
                            }
                        }
                    }
                    Action action3 = () =>
                    {
                        foreach (var u in units)
                        {
                            u.CreateAction(new UnitActionSchoolExit());
                            SchoolAdmin.Log(u.data.unitData.propertyData.GetName() + " 离开了宗门", true);
                        }
                    };
                    if (units.Count > 1)
                    {
                        // 此举将踢出{0}位宗门成员！关系重大，请慎重抉择！是否继续踢出？"
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), string.Format(GameTool.LS("schoolAdmin_tirentishi"), units.Count), 2, action3);
                    }
                    else
                    {
                        // 宗门所有成员都符合条件！
                        UITipItem.AddTip(GameTool.LS("schoolAdmin_tirentishi2"));
                    }
                }
                else
                {
                    UITipItem.AddTip("宗主才能执行此操作！");
                }
            };


            Action action1 = () =>
            {
                var school = g.world.playerUnit.data.school;
                if (school != null && school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("schoolAdmin_tirentishi3"), 2, action2);

                }
                else
                {
                    UITipItem.AddTip("宗主才能执行此操作！");
                }
            };

            {
                var school = g.world.playerUnit.data.school;
                if (school != null && school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
                {
                    Texture2D tex = g.res.Load<Texture2D>("Texture/BG/zhuchuzongmen");
                    Sprite pic = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    GameObject btnSchoolAdmin = CreateUI.NewImage(pic);
                    btnSchoolAdmin.transform.SetParent(ui.btnClose.transform.parent, false);
                    btnSchoolAdmin.AddComponent<Button>().onClick.AddListener(action1);
                    btnSchoolAdmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(-515, -228);
                }
            }
        }

        private void InitUI(GameObject bg)
        {
            int idx = 0;
            int line = 0;
            int left = -560;
            int top = 450;
            int space = 120;
            int lineCount = 8;
            ui_mainTog = new Toggle[funcNames.Length];
            ui_subTog = new List<List<UIBehaviour>>();
            ui_canvas = new List<CanvasGroup>();

            {
                Action<int, int> CreateTitle = (i, l) =>
                {
                    GameObject textType = CreateUI.NewToggle(funcNames[i]);
                    textType.transform.SetParent(bg.transform, false);
                    textType.GetComponent<RectTransform>().anchoredPosition = new Vector2(-675, top - l * 50);
                    ui_mainTog[i] = textType.GetComponent<Toggle>();

                    var goTips = CreateUI.NewImage(SpriteTool.GetSprite("Common", "wenhao"));
                    goTips.transform.SetParent(bg.transform, false);
                    goTips.GetComponent<RectTransform>().anchoredPosition = new Vector2(-700, top - l * 50);
                    goTips.AddComponent<UISkyTipEffect>().InitData(funcDess[i]);
                };

                GameObject tmpGo;
                //SchoolAdmin.Log("// 性别");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    tmpGo = CreateUI.NewToggle(GameTool.LS("common_nan"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left, top - line * 50);
                    GameObject go1 = tmpGo;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("common_nv"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + space, top - line * 50);
                    GameObject go2 = tmpGo;

                    var data = new List<UIBehaviour>() { go1.GetComponent<Toggle>(), go2.GetComponent<Toggle>() };

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 魅力");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    Il2CppSystem.Collections.Generic.List<ConfRoleBeautyItem> list = g.conf.roleBeauty._allConfList;
                    List<UIBehaviour> data = new List<UIBehaviour>();
                    List<GameObject> gos = new List<GameObject>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        ConfRoleBeautyItem item = list[i];
                        tmpGo = CreateUI.NewToggle(GameTool.LS(item.text));
                        tmpGo.transform.SetParent(canvas.transform, false);
                        int x = left + i * space;
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, top - line * 50);
                        gos.Add(tmpGo);
                        data.Add(tmpGo.GetComponent<Toggle>());
                    }

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log(" // 兴趣");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    List<UIBehaviour> data = new List<UIBehaviour>();
                    List<GameObject> gos = new List<GameObject>();
                    Il2CppSystem.Collections.Generic.List<ConfRoleCreateHobbyItem> list = g.conf.roleCreateHobby._allConfList;
                    int x = left;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ConfRoleCreateHobbyItem item = list[i];
                        tmpGo = CreateUI.NewToggle(GameTool.LS(item.name));
                        tmpGo.transform.SetParent(canvas.transform, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, top - line * 50);
                        gos.Add(tmpGo);
                        data.Add(tmpGo.GetComponent<Toggle>());
                        x += space;
                        if (i % lineCount == (lineCount - 1))
                        {
                            x = left;
                            line++;
                        }
                    }

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 声望");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    List<UIBehaviour> data = new List<UIBehaviour>();
                    List<GameObject> gos = new List<GameObject>();
                    Il2CppSystem.Collections.Generic.List<ConfRoleReputationItem> list = g.conf.roleReputation._allConfList;
                    int x = left;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ConfRoleReputationItem item = list[i];
                        tmpGo = CreateUI.NewToggle(GameTool.LS(item.text));
                        tmpGo.transform.SetParent(canvas.transform, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, top - line * 50);
                        gos.Add(tmpGo);
                        data.Add(tmpGo.GetComponent<Toggle>());
                        x += space;
                        if (i % lineCount == (lineCount - 1))
                        {
                            x = left;
                            line++;
                        }
                    }

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 性格");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    List<UIBehaviour> data = new List<UIBehaviour>();
                    List<GameObject> gos = new List<GameObject>();
                    Il2CppSystem.Collections.Generic.List<ConfRoleCreateCharacterItem> list = g.conf.roleCreateCharacter._allConfList;
                    int x = left;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ConfRoleCreateCharacterItem item = list[i];
                        tmpGo = CreateUI.NewToggle(GameTool.LS(item.sc5asd_sd34));
                        tmpGo.transform.SetParent(canvas.transform, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, top - line * 50);
                        gos.Add(tmpGo);
                        data.Add(tmpGo.GetComponent<Toggle>());
                        x += space;
                        if (i % lineCount == (lineCount - 1))
                        {
                            x = left;
                            line++;
                        }
                    }

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 境界");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    List<UIBehaviour> data = new List<UIBehaviour>();
                    List<GameObject> gos = new List<GameObject>();
                    Il2CppSystem.Collections.Generic.List<ConfRoleGradeItem> list = g.conf.roleGrade._allConfList;
                    int x = left;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ConfRoleGradeItem item = list[i];
                        tmpGo = CreateUI.NewToggle(GameTool.LS(item.gradeName) + GameTool.LS(item.phaseName));
                        tmpGo.transform.SetParent(canvas.transform, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, top - line * 50);
                        gos.Add(tmpGo);
                        data.Add(tmpGo.GetComponent<Toggle>());
                        x += space;
                        if (i % lineCount == (lineCount - 1))
                        {
                            x = left;
                            line++;
                        }
                    }

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 婚姻");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    float offset = 0;
                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_UnlimitedMarriage"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go1 = tmpGo.GetComponent<Toggle>();
                    offset += space;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_Unmarried"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go2 = tmpGo.GetComponent<Toggle>();
                    offset += space * 0.6f;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_IsMarried"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go3 = tmpGo.GetComponent<Toggle>();
                    offset += space;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_Unlimitedchildren"));
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    tmpGo.transform.SetParent(canvas.transform, false);
                    Toggle go4 = tmpGo.GetComponent<Toggle>();
                    offset += space;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_NoChildren"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go5 = tmpGo.GetComponent<Toggle>();
                    offset += space * 0.75f;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_HaveChildren"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go6 = tmpGo.GetComponent<Toggle>();
                    offset += space;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_UnlimitedPartner"));
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    tmpGo.transform.SetParent(canvas.transform, false);
                    Toggle go7 = tmpGo.GetComponent<Toggle>();
                    offset += space;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_Unpartner"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go8 = tmpGo.GetComponent<Toggle>();
                    offset += space * 0.75f;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_HavePartner"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + offset, top - line * 50);
                    Toggle go9 = tmpGo.GetComponent<Toggle>();
                    offset += space * 0.75f;

                    GameObject group1 = CreateUI.New();
                    group1.transform.SetParent(canvas.transform, false);
                    ToggleGroup tg1 = group1.AddComponent<ToggleGroup>();
                    go1.group = tg1;
                    go2.group = tg1;
                    go3.group = tg1;

                    GameObject group2 = CreateUI.New();
                    group2.transform.SetParent(canvas.transform, false);
                    ToggleGroup tg2 = group2.AddComponent<ToggleGroup>();
                    go4.group = tg2;
                    go5.group = tg2;
                    go6.group = tg2;

                    GameObject group3 = CreateUI.New();
                    group3.transform.SetParent(canvas.transform, false);
                    ToggleGroup tg3 = group3.AddComponent<ToggleGroup>();
                    go7.group = tg3;
                    go8.group = tg3;
                    go9.group = tg3;


                    var data = new List<UIBehaviour>() { go1, go2, go3, go4, go5, go6, go7, go8, go9 };

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 姓氏");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    tmpGo = CreateUI.NewInputField(placeholder: GameTool.LS("schoolAdmin_inputsurname"));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + 120, top - line * 50 + 5);
                    GameObject go1 = tmpGo;

                    var data = new List<UIBehaviour>() { go1.GetComponent<InputField>() };

                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
                //SchoolAdmin.Log("// 道心");
                {
                    CreateTitle(idx, line);

                    GameObject obj = CreateUI.New();
                    obj.transform.SetParent(bg.transform, false);
                    var canvas = obj.AddComponent<CanvasGroup>();
                    ui_canvas.Add(canvas);

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_heaven_chosen"), new Vector2(150, 30));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left, top - line * 50);
                    GameObject go1 = tmpGo;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_paragon"), new Vector2(150, 30));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + space * 2, top - line * 50);
                    GameObject go2 = tmpGo;

                    tmpGo = CreateUI.NewToggle(GameTool.LS("schoolAdmin_ordinary"), new Vector2(150, 30));
                    tmpGo.transform.SetParent(canvas.transform, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(left + space * 4, top - line * 50);
                    GameObject go3 = tmpGo;

                    var data = new List<UIBehaviour>() { go1.GetComponent<Toggle>(), go2.GetComponent<Toggle>(), go3.GetComponent<Toggle>() };
                    ui_subTog.Add(data);

                    idx++;
                    line++;
                }
            }
        }

        private void InitPage(GameObject bg)
        {
            float height = 50;

            GameObject tmpGo = CreateUI.NewText(GameTool.LS("schoolAdmin_zhaonatiaojianshezhi"), new Vector2(300, 40));
            tmpGo.transform.SetParent(bg.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-575, height + 450);
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 30;
            tmpText.color = Color.black;
            tmpText.fontStyle = FontStyle.Bold;

            var goTips = CreateUI.NewImage(SpriteTool.GetSprite("Common", "wenhao_2"));
            goTips.transform.SetParent(bg.transform, false);
            goTips.GetComponent<RectTransform>().anchoredPosition = new Vector2(-700, height + 450);
            goTips.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("schoolAdmin_tips10"));

            // 页签按钮
            uipageTog = new List<RectTransform>();
            for (int i = 0; i < pageNames.Length; i++)
            {
                int idx = i;
                tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojuxuanxiangbg"));
                tmpGo.transform.SetParent(bg.transform, false);
                tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400 + (i * 85), idx == scrollPage ? 500 : 510);
                uipageTog.Add(tmpGo.GetComponent<RectTransform>());
                Action pageAction = () =>
                {
                    scrollPage = idx;
                    UpdateUI();
                };
                tmpGo.AddComponent<Button>().onClick.AddListener(pageAction);
                Transform tmpParent = tmpGo.transform;

                tmpGo = CreateUI.NewText(pageNames[i], tmpGo.GetComponent<RectTransform>().sizeDelta);
                tmpGo.transform.SetParent(tmpParent, false);
                tmpText = tmpGo.GetComponent<Text>();
                tmpText.alignment = TextAnchor.MiddleCenter;
                tmpText.fontSize = 16;
            }

            tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "wenhao_2"));
            tmpGo.transform.SetParent(bg.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(255, 510);
            tmpGo.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("schoolAdmin_tips11"));
        }

        private void InitSchool(GameObject bg)
        {

            GameObject tmpGo = CreateUI.NewText(GameTool.LS("schoolAdmin_AppliedToSectting"), new Vector2(220, 40));
            tmpGo.transform.SetParent(bg.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(510, 350);
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.color = Color.black;

            tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "wenhao_2"));
            tmpGo.transform.SetParent(bg.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(450, 350);
            tmpGo.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("schoolAdmin_tips12"));

            tmpGo = CreateUI.NewScrollView(new Vector2(220, 700), spacing: new Vector2(0, 10));
            var view = tmpGo.GetComponent<ScrollRect>();
            tmpGo.transform.SetParent(bg.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, -20);

            // 宗门列表
            ui_schools = new List<Toggle>();
            schools = g.world.playerUnit.data.school.GetAllSchools(true);
            for (int i = 0; i < schools.Count; i++)
            {
                MapBuildSchool s = schools[i];
                tmpGo = CreateUI.NewToggle(s.name, new Vector2(180, 30));
                tmpGo.transform.SetParent(view.content, false);
                ui_schools.Add(tmpGo.GetComponent<Toggle>());
            }
        }

        private void UpdateUI()
        {
            UpdateUIClear();
            UpdateUIValue();
            UpdateUIListener();
        }

        // 清空ui监听
        private void UpdateUIClear()
        {
            for (int i = 0; i < schools.Count; i++)
            {
                ui_schools[i].onValueChanged.RemoveAllListeners();
            }

            for (int i = 0; i < ui_mainTog.Length; i++)
            {
                ui_mainTog[i].onValueChanged.RemoveAllListeners();
            }

            for (int i = 0; i < ui_subTog.Count; i++)
            {
                for (int j = 0; j < ui_subTog[i].Count; j++)
                {
                    if (ui_subTog[i][j] is Toggle)
                    {
                        Toggle tog = ((Toggle)ui_subTog[i][j]);
                        tog.onValueChanged.RemoveAllListeners();
                    }
                    else if (ui_subTog[i][j] is InputField)
                    {
                        InputField ipt = ((InputField)ui_subTog[i][j]);
                        ipt.onValueChange.RemoveAllListeners();
                    }
                }
            }
        }
        // 给ui赋值
        private void UpdateUIValue()
        {
            for (int i = 0; i < uipageTog.Count; i++)
            {
                uipageTog[i].anchoredPosition = new Vector2(-400 + (i * 85), i == scrollPage ? 500 : 510);
            }

            for (int i = 0; i < 8; i++)
            {
                if (data.data.Count <= i)
                {
                    data.data.Add(new SchoolAdminData.PageData());
                }
            }

            SchoolAdminData.PageData d = data.data[scrollPage];

            for (int i = 0; i < schools.Count; i++)
            {
                var school = schools[i];
                bool isOn = d.IsCheckSchool(schools[i]);
                ui_schools[i].isOn = isOn;
            }

            for (int i = 0; i < ui_mainTog.Length; i++)
            {
                bool isOn = d.IsCheckMainTog(i);
                ui_mainTog[i].isOn = isOn;
                ui_canvas[i].alpha = isOn ? 1 : 0.5f;
                ui_canvas[i].interactable = isOn;
            }

            for (int i = 0; i < ui_subTog.Count; i++)
            {
                for (int j = 0; j < ui_subTog[i].Count; j++)
                {
                    var value = d.GetSubTog(i, j);
                    if (ui_subTog[i][j] is Toggle)
                    {
                        Toggle tog = ((Toggle)ui_subTog[i][j]);
                        tog.isOn = value == "1";
                    }
                    else if (ui_subTog[i][j] is InputField)
                    {
                        if (value == "0")
                        {
                            value = "";
                            d.ChangeSubTog(i, j, value);
                        }
                        InputField ipt = ((InputField)ui_subTog[i][j]);
                        ipt.text = value;
                    }
                }
            }
        }
        // 给ui添加监听
        private void UpdateUIListener()
        {
            for (int i = 0; i < uipageTog.Count; i++)
            {
                uipageTog[i].anchoredPosition = new Vector2(-400 + (i * 85), i == scrollPage ? 500 : 510);
            }

            for (int i = 0; i < 8; i++)
            {
                if (data.data.Count <= i)
                {
                    data.data.Add(new SchoolAdminData.PageData());
                }
            }

            SchoolAdminData.PageData d = data.data[scrollPage];

            for (int i = 0; i < schools.Count; i++)
            {
                var school = schools[i];
                Action<bool> onChangeValue = (isOn) =>
                {
                    data.data[scrollPage].ChangeSchool(school, isOn);
                };
                ui_schools[i].onValueChanged.AddListener(onChangeValue);
            }

            for (int i = 0; i < ui_mainTog.Length; i++)
            {
                int idx = i;
                Action<bool> onChangeValue = (isOn) =>
                {
                    data.data[scrollPage].ChangeMainTog(idx, isOn ? 1 : 0);
                    ui_canvas[idx].alpha = isOn ? 1 : 0.5f;
                    ui_canvas[idx].interactable = isOn;
                };
                ui_mainTog[i].onValueChanged.AddListener(onChangeValue);
            }

            for (int i = 0; i < ui_subTog.Count; i++)
            {
                for (int j = 0; j < ui_subTog[i].Count; j++)
                {
                    int ii = i, jj = j;
                    var value = d.GetSubTog(i, j);
                    if (ui_subTog[i][j] is Toggle)
                    {
                        Toggle tog = ((Toggle)ui_subTog[i][j]);
                        Action<bool> onChangeValue = (isOn) =>
                        {
                            data.data[scrollPage].ChangeSubTog(ii, jj, isOn ? "1" : "0");
                        };
                        tog.onValueChanged.AddListener(onChangeValue);
                    }
                    else if (ui_subTog[i][j] is InputField)
                    {
                        InputField ipt = ((InputField)ui_subTog[i][j]);
                        Action<string> onChangeValue = (connect) =>
                        {
                            data.data[scrollPage].ChangeSubTog(ii, jj, connect);
                        };
                        ipt.onValueChanged.AddListener(onChangeValue);
                    }
                }
            }
        }

    }
}
