using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
using Color = UnityEngine.Color;

namespace Cave
{
    public class HelpMe
    {
        public HelpMe(UILogin ui)
        {
            if (ui != null)
                InitUI(ui);
        }

        private void InitUI(UILogin ui)
        {
            if (ui.transform.Find("Root/btnOpenHelpMe") != null)
            {
                GameObject.Destroy(ui.transform.Find("Root/btnOpenHelpMe").gameObject);
            }

            var btnOpenHelpMe = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "tongyongbutton_2"));
            btnOpenHelpMe.transform.SetParent(ui.transform.Find("Root"), false);
            btnOpenHelpMe.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            btnOpenHelpMe.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            btnOpenHelpMe.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112, -112);
            btnOpenHelpMe.name = "btnOpenHelpMe";
            Action openHelpMe = () => { OpenHelpMe(ui); };
            btnOpenHelpMe.AddComponent<Button>().onClick.AddListener(openHelpMe);

            var go2 = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_ModName"), btnOpenHelpMe.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(btnOpenHelpMe.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            go2.GetComponent<Text>().color = Color.black;
            go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 2);


            int version = PlayerPrefs.GetInt("www.yellowshange.con.cave.version", 0);
            /*
            Action checkVersion = () => // 检查版本
            {
                string url = "http://www.yellowshange.com/caveMod/version/";
                var result = GuiBaseUI.Tools.GetHttp(url);
                int newVersion = 0;
                if (int.TryParse(result, out newVersion))
                {
                    if (newVersion > maxversion)
                    {
                        var btnOpenHelpMe2 = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "tongyongbutton_2"));
                        btnOpenHelpMe2.transform.SetParent(ui.transform.Find("Root"), false);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112, -172);
                        btnOpenHelpMe2.name = "btnOpenHelpMe";
                        Action openHelpMe2 = () => {
                            string conect = GuiBaseUI.Tools.GetHttp("http://www.yellowshange.com/caveMod/updateDes/");
                            Action goUpdate = () =>
                            {
                                Application.OpenURL(GuiBaseUI.Tools.GetHttp("http://www.yellowshange.com/caveMod/downloadUrl/"));
                                Application.Quit();
                            };
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), conect, 2, goUpdate);
                        };
                        btnOpenHelpMe2.AddComponent<Button>().onClick.AddListener(openHelpMe2);

                        var go3 = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_HelpText1"), btnOpenHelpMe2.GetComponent<RectTransform>().sizeDelta);
                        go3.transform.SetParent(btnOpenHelpMe2.transform, false);
                        go3.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        go3.GetComponent<Text>().color = Color.red;
                        go3.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 2);

                        openHelpMe2();
                    }
                }
            };
            g.timer.Frame(checkVersion, 1, false);
            */

            if (version >= maxversion)
            {
                return;
            }
            PlayerPrefs.SetInt("www.yellowshange.con.cave.version", maxversion);
            OpenHelpMe(ui);
        }

        int scrollPage = 0;
        private void OpenHelpMe(UILogin ui)
        {
            if (ui.transform.Find("Root/HelpMe") != null)
            {
                GameObject.Destroy(ui.transform.Find("Root/HelpMe").gameObject);
            }


            var parent = GuiBaseUI.CreateUI.NewImage();
            parent.transform.SetParent(ui.transform.Find("Root"), false);
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(9999, 9999);
            parent.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            parent.name = "HelpMe";


            var go = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("CommonMapGrid", "longmen"));
            go.transform.SetParent(parent.transform, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 792);



            var go2 = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_HelpText2"), new Vector2(340, 396));
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            go2.GetComponent<Text>().color = Color.black;
            go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -70);



            var tmpBtn = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -35);

            var tmpText = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_HelpText3"), tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            Action tmpAction = () =>
            {
                Application.OpenURL("https://space.bilibili.com/16445976");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);



            tmpBtn = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -134);

            tmpText = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_HelpText4"), tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                Application.OpenURL("https://www.yellowshange.com");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);




            tmpBtn = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -185);

            tmpText = GuiBaseUI.CreateUI.NewText(GameTool.LS("Cave_HelpText5"), tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                Application.OpenURL("https://bbs.3dmgame.com/thread-6250101-1-1.html");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);





            tmpBtn = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -400);

            tmpText = GuiBaseUI.CreateUI.NewText(GameTool.LS("setting_guanbi"), tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                GameObject.Destroy(parent.gameObject);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);






            var rimgTrans = GuiBaseUI.CreateUI.NewRawImage(new Vector2(500, 1000));
            rimgTrans.transform.SetParent(parent.transform, false);
            rimgTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(450, -2);
            rimgTrans.GetComponent<RawImage>().raycastTarget = false;

            SetDragonModel("Game/NPCDynamic/dt_genpilongnv", rimgTrans.GetComponent<RawImage>(),
                new Vector3(.6f, .6f, .6f), new Vector2(-.7f, -1.65f));





            var bg = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
            bg.transform.SetParent(parent.transform, false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 1080);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-650, -50);
            bg.GetComponent<Image>().type = Image.Type.Tiled;

            var title = GuiBaseUI.CreateUI.NewText("", fontID: 1);
            title.transform.SetParent(parent.transform, false);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 50);
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(-750, 420);
            title.GetComponent<Text>().color = Color.black;


            var scroll = GuiBaseUI.CreateUI.NewScrollView(new Vector2(450, 840), spacing: new Vector2(0, 30));
            scroll.transform.SetParent(parent.transform, false);
            scroll.GetComponent<RectTransform>().anchoredPosition = new Vector2(-675, -30);
            var scrollRect = scroll.GetComponent<ScrollRect>();
            var connect = scrollRect.content;


            string[] pageNames = new string[] { GameTool.LS("Cave_HelpText6"), GameTool.LS("Cave_HelpText7") };

            List<RectTransform> ui_allPage = new List<RectTransform>();
            // 页签按钮
            for (int i = 0; i < 2; i++)
            {
                int idx = i;
                var tmpGo = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojuxuanxiangbg"));
                tmpGo.transform.SetParent(parent.transform, false);
                tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-750 + (idx * 100), idx == scrollPage ? 470 : 485);
                ui_allPage.Add(tmpGo.GetComponent<RectTransform>());
                Action pageAction = () =>
                {
                    scrollPage = idx;
                    for (int j = 0; j < ui_allPage.Count; j++)
                    {
                        ui_allPage[j].anchoredPosition = new Vector2(-750 + (j * 100), j == scrollPage ? 470 : 485);
                    }
                    UpdateScroll(connect, title.GetComponent<Text>());
                };
                tmpGo.AddComponent<Button>().onClick.AddListener(pageAction);
                Transform tmpParent = tmpGo.transform;

                var tmpGo2 = GuiBaseUI.CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                tmpGo2.transform.SetParent(tmpParent, false);
                Text tmpText2 = tmpGo2.GetComponent<Text>();
                tmpText2.alignment = TextAnchor.MiddleCenter;
                tmpText2.fontSize = 16;
                tmpText2.text = pageNames[i];
            }

            scrollPage = 0;
            UpdateScroll(connect, title.GetComponent<Text>());
        }

        public void UpdateScroll(Transform parent, Text title)
        {
            string[] pageTitles = new string[] { "<size=30>" + GameTool.LS("Cave_HelpText8") + "</size>", "<size=30>" + GameTool.LS("Cave_HelpText9") + "</size>" };
            title.text = pageTitles[scrollPage];
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(0);
                child.SetParent(null);
                GameObject.Destroy(child.gameObject);
            }

            List<string[]> data = scrollPage == 0 ? connect : plan;
            for (int i = data.Count - 1; i >= 0; i--)
            {
                var textTitle = data[i][0];
                var textConnect = data[i][1];

                var t = GuiBaseUI.CreateUI.NewText(textTitle, new Vector2(400, 20));
                t.transform.SetParent(parent, false);
                t.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                t.GetComponent<Text>().color = Color.black;

                var c = GuiBaseUI.CreateUI.NewText(textConnect, new Vector2(400, 20));
                c.transform.SetParent(parent, false);
                c.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                c.GetComponent<Text>().color = Color.black;
            }
        }

        private void SetDragonModel(string path, RawImage rimg, Vector3 scale, Vector2 offset)
        {
            rimg.gameObject.SetActive(true);
            GameObject dragon = GameObject.Instantiate(g.res.Load<GameObject>(path));
            ModelRenderTexture rt = new ModelRenderTexture(dragon, rimg, offset);
            rt.gameObject.transform.localScale = scale;
        }

        public static string caveDes1 { get { return GameTool.LS("Cave_HelpText10"); } }
        List<string[]> plan = new List<string[]>() // 版本计划
        {
            new string[]{"<size=22>【8.x"+ GameTool.LS("Cave_HelpText11") + "】</size>", GameTool.LS("Cave_HelpYudao1") },
            new string[]{ "<size=22>【7.x " + GameTool.LS("Cave_HelpText11") + "】</size>", GameTool.LS("Cave_HelpYudao2") },
            new string[]{ "<size=22>【6.x " + GameTool.LS("Cave_HelpText11") + "】</size>", GameTool.LS("Cave_HelpYudao3") },
            new string[]{ "<size=22>【5.x " + GameTool.LS("Cave_HelpText11") + "】</size>", GameTool.LS("Cave_HelpYudao4") },
            new string[]{ "<size=22>【4.x " + GameTool.LS("Cave_HelpText11") + "】</size>", GameTool.LS("Cave_HelpYudao5") },
            new string[]{ "<size=22>【3.x " + GameTool.LS("Cave_HelpText12") + "】</size>", GameTool.LS("Cave_HelpYudao6") },
            new string[]{ "<size=22>【2.x " + GameTool.LS("Cave_HelpText13") + "】</size>", GameTool.LS("Cave_HelpYudao7") },
            new string[]{ "<size=22>【1.x " + GameTool.LS("Cave_HelpText13") + "】</size>", GameTool.LS("Cave_HelpYudao8") },
        };
        List<string[]> connect = new List<string[]>() // 更新公告
        {            new string[]{ "<size=22>【1.0" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2021,12,22,23), GameTool.LS("Cave_HelpGengxin1")},
            new string[]{ "<size=22>【1.1" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2021,12,23,13), GameTool.LS("Cave_HelpGengxin2")},
            new string[]{"<size=22>【2.0" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2021,12,24,23), GameTool.LS("Cave_HelpGengxin3")+caveDes1},
            new string[]{ "<size=22>【2.1" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2021,12,25,23), GameTool.LS("Cave_HelpGengxin4")},
            new string[]{ "<size=22>【2.2" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2021,12,26,23), GameTool.LS("Cave_HelpGengxin5")},
            new string[]{"<size=22>【2.3" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2022,1,16,13), GameTool.LS("Cave_HelpGengxin6")},
            new string[]{ "<size=22>【2.4" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2022,5,30,16), GameTool.LS("Cave_HelpGengxin7")},
            new string[]{ "<size=22>【2.5" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2022,6,4,2), GameTool.LS("Cave_HelpGengxin8")},
            new string[]{ "<size=22>【2.6" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2022,7,6,1), "1、修复了洞府的一些BUG。\n2、优化了日月泉功能，增加可切换性别，点击词条名字可更换词条。\n3、修复了龙人没有跟随进入战斗的问题。\n4、优化了同道功能，同道不再一同进入战斗，可在战斗准备界面自行选择是否出战。\n修复和伙伴一起进入残破锁灵阵/封魔阵无效的问题"},
            new string[]{ "<color=red>" + "<size=22>【3.0" + GameTool.LS("Cave_HelpText14") + "】</size>"+string.Format(GameTool.LS("Cave_HelpText15"),2022,7,11,2) + "</Color>", "<color=red>更新3.0版本，洞府建筑新增灵田和地牢。灵田可种植各种灵果和药材，地牢可以关押击败的敌人，并且可在地牢中对其进行操作。\n增加装修功能，灵田和地牢均可进行自由装修。</Color>"},
        };
        int maxversion = 17;





        /*
         <size=22>【2.3抢鲜体验版】</size>2021年12月26日23时", "1、优化了mod框架，修复了一些已知BUG。\n2、优化了主界面按钮显示\n3、主房和客房增加捏脸功能。\n4、增加了版本预告。\n5、修改了建筑名称。
         

        主界面按钮靠边吸附。 回家按钮隐藏
        框架修改。
        两个楼的BUG。
        搬家重新读档BUG。

        醉仙楼


        优化了mod框架，修复了一些已知BUG
        主界面按钮自动靠边
        主房和客房增加捏脸功能
        修改了建筑名称
        增加了版本预告

        伙伴卡顿优化
        有时候回家按钮莫名其妙消失
         */
    }
}
