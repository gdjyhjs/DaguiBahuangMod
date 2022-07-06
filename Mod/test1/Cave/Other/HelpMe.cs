using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using Color = UnityEngine.Color;

namespace Cave
{
    public class HelpMe
    {
        public HelpMe(UILogin ui)
        {
            if (ui.transform.Find("Root/btnOpenHelpMe") != null)
            {
                GameObject.Destroy(ui.transform.Find("Root/btnOpenHelpMe").gameObject);
            }


            var btnOpenHelpMe = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tongyongbutton_2"));
            btnOpenHelpMe.transform.SetParent(ui.transform.Find("Root"), false);
            btnOpenHelpMe.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            btnOpenHelpMe.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            btnOpenHelpMe.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112, -112);
            btnOpenHelpMe.name = "btnOpenHelpMe";
            Action openHelpMe = () => { OpenHelpMe(ui); };
            btnOpenHelpMe.AddComponent<Button>().onClick.AddListener(openHelpMe);

            var go2 = CreateUI.NewText("洞天福地", btnOpenHelpMe.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(btnOpenHelpMe.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            go2.GetComponent<Text>().color = Color.black;
            go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 2);


            int version = PlayerPrefs.GetInt("www.yellowshange.con.cave.version", 0);

            Action checkVersion = () => // 检查版本
            {
                string url = "http://www.yellowshange.com/caveMod/version/";
                var result = Tools.GetHttp(url);
                int newVersion = 0;
                if (int.TryParse(result, out newVersion))
                {
                    if (newVersion > maxversion)
                    {
                        var btnOpenHelpMe2 = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tongyongbutton_2"));
                        btnOpenHelpMe2.transform.SetParent(ui.transform.Find("Root"), false);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                        btnOpenHelpMe2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-112, -172);
                        btnOpenHelpMe2.name = "btnOpenHelpMe";
                        Action openHelpMe2 = () => {
                            string conect = Tools.GetHttp("http://www.yellowshange.com/caveMod/updateDes/");
                            Action goUpdate = () =>
                            {
                                Application.OpenURL(Tools.GetHttp("http://www.yellowshange.com/caveMod/downloadUrl/"));
                                Application.Quit();
                            };
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), conect, 2, goUpdate);
                        };
                        btnOpenHelpMe2.AddComponent<Button>().onClick.AddListener(openHelpMe2);

                        var go3 = CreateUI.NewText("更新洞府", btnOpenHelpMe2.GetComponent<RectTransform>().sizeDelta);
                        go3.transform.SetParent(btnOpenHelpMe2.transform, false);
                        go3.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                        go3.GetComponent<Text>().color = Color.red;
                        go3.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 2);

                        openHelpMe2();
                    }
                }
            };
            g.timer.Frame(checkVersion, 1, false);


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


            var parent = CreateUI.NewImage();
            parent.transform.SetParent(ui.transform.Find("Root"), false);
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(9999, 9999);
            parent.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            parent.name = "HelpMe";


            var go = CreateUI.NewImage(SpriteTool.GetSprite("CommonMapGrid", "longmen"));
            go.transform.SetParent(parent.transform, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 792);



            var go2 = CreateUI.NewText("感谢使用洞天福地MOD！\n我的QQ群：50948165\n哔哩哔哩ID：八荒大鬼\n\n\n个人主页：www.yellowshange.com\n\n\n\n", new Vector2(340, 396));
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            go2.GetComponent<Text>().color = Color.black;
            go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -70);



            var tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -35);

            var tmpText = CreateUI.NewText("前往B站关注", tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            Action tmpAction = () =>
            {
                Application.OpenURL("https://space.bilibili.com/16445976");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);



            tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -134);

            tmpText = CreateUI.NewText("前往主页参观", tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                Application.OpenURL("https://www.yellowshange.com");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);




            tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -185);

            tmpText = CreateUI.NewText("前往3DM论坛", tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                Application.OpenURL("https://bbs.3dmgame.com/thread-6250101-1-1.html");
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);





            tmpBtn = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youxishezhi_1"));
            tmpBtn.transform.SetParent(go.transform, false);
            tmpBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -400);

            tmpText = CreateUI.NewText("关闭", tmpBtn.GetComponent<RectTransform>().sizeDelta);
            tmpText.transform.SetParent(tmpBtn.transform, false);
            tmpText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            tmpText.GetComponent<Text>().color = Color.black;

            tmpAction = () =>
            {
                GameObject.Destroy(parent.gameObject);
            };
            tmpBtn.AddComponent<Button>().onClick.AddListener(tmpAction);






            var rimgTrans = CreateUI.NewRawImage(new Vector2(500, 1000));
            rimgTrans.transform.SetParent(parent.transform, false);
            rimgTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(450, -2);
            rimgTrans.GetComponent<RawImage>().raycastTarget = false;

            SetDragonModel("Game/NPCDynamic/dt_genpilongnv", rimgTrans.GetComponent<RawImage>(),
                new Vector3(.6f, .6f, .6f), new Vector2(-.7f, -1.65f));





            var bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
            bg.transform.SetParent(parent.transform, false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 1080);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-650, -50);
            bg.GetComponent<Image>().type = Image.Type.Tiled;

            var title = CreateUI.NewText("", fontID:1);
            title.transform.SetParent(parent.transform, false);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 50);
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(-750, 420);
            title.GetComponent<Text>().color = Color.black;


            var scroll = CreateUI.NewScrollView(new Vector2(450, 840), spacing:new Vector2(0, 30));
            scroll.transform.SetParent(parent.transform, false);
            scroll.GetComponent<RectTransform>().anchoredPosition = new Vector2(-675, -30);
            var scrollRect = scroll.GetComponent<ScrollRect>();
            var connect = scrollRect.content;


            string[] pageNames = new string[] { "更新", "计划" };

            List<RectTransform> ui_allPage = new List<RectTransform>();
            // 页签按钮
            for (int i = 0; i < 2; i++)
            {
                Cave.Log(i+" begin "+ pageNames[i]);
                int idx = i;
                var tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojuxuanxiangbg"));
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

                var tmpGo2 = CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                tmpGo2.transform.SetParent(tmpParent, false);
                Text tmpText2 = tmpGo2.GetComponent<Text>();
                tmpText2.alignment = TextAnchor.MiddleCenter;
                tmpText2.fontSize = 16;
                tmpText2.text = pageNames[i];
                Cave.Log(i + " end " + pageNames[i]);
            }

            scrollPage = 0;
            UpdateScroll(connect, title.GetComponent<Text>());
        }

        public void UpdateScroll(Transform parent, Text title)
        {
            string[] pageTitles = new string[] { "<size=30>洞天福地更新公告</size>", "<size=30>洞天福地版本计划</size>" };
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

                var t = CreateUI.NewText(textTitle, new Vector2(400, 20));
                t.transform.SetParent(parent, false);
                t.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                t.GetComponent<Text>().color = Color.black;

                var c = CreateUI.NewText(textConnect, new Vector2(400, 20));
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

        public static string caveDes1 = "邀请说明：邀请NPC入住洞府，NPC将在下个月做出回复，回复之前将为其预留一间客房。下个月NPC若回复拒绝，此客房可重新邀请其他NPC；若NPC回复接受，NPC将入住并占用该客房。入住的NPC会经常在洞府逗留。";
        List<string[]> plan = new List<string[]>()
        {
            new string[]{"<size=22>【8.x依次】</size>", "可触发奇遇发现大能残魂，修复后发现是从其他世界破碎虚空而来，灵魂首创需要稀有材料修复灵魂，救活大能后可获得其炼丹传承，开启新炼丹系统。\n增加新炼丹系统。增加炼丹室建筑及功能。" },
            new string[]{"<size=22>【7.x 依次】</size>", "优化各个建筑的管理NPC，可分配好友帮忙打工。\n各个建筑每个月需要花费灵石维护等。\n增加好友打工功能，好友可帮助洞主赚取灵石。" },
            new string[]{"<size=22>【6.x 依次】</size>", "丰富阵法功能，聚灵阵等。\n大地图增加奇遇，可能遇到其他修仙者的洞府。" },
            new string[]{"<size=22>【5.x 依次】</size>", "增加主房功能，可与道侣在其中繁衍下一代。\n增加小孩养成功能，可塑造性格及先天气运等。" },
            new string[]{"<size=22>【4.x 依次】</size>", "增加生态拟真试炼塔，可刷怪试炼获取材料。\n增加情报楼，可搜集各种情报。\n增加花园建筑，可与好友及器灵在其中玩耍交互等。" },
            new string[]{"<size=22>【3.x 下一阶段】</size>", "增加地牢建筑，可关押击败的敌人。\n增加灵田建筑，可种植灵果。\n增加改名和性格功能（玩家和伙伴）" },
            new string[]{"<size=22>【2.x 已更新】</size>", "增加主要建筑灵阁功能，可免费修炼。\n增加客房功能，可邀请好友入住。\n增加更新内容显示、版本计划显示、更新提示功能。\n增加整容建筑，可更换发型、眼睛鼻子等。" },
            new string[]{"<size=22>【1.x 已更新】</size>", "增加洞府购买功能，以及洞府建设框架和主要建筑功能。" },
        };
        List<string[]> connect = new List<string[]>()
        {
            new string[]{"<size=22>【1.0抢鲜体验版】</size>2021年12月22日23时", "1、增加购买洞府功能，定居后可随时回家打开洞府。\n2、增加DIY洞府背景及建筑图片功能。\n3、增加建筑放置及部分主要建筑功能。" },
            new string[]{"<size=22>【1.1抢鲜体验版】</size>2021年12月23日13时", "1、增加更新公告功能及优化公告提示\n2、修复自己建造的建木与城镇的不互通（<color=red>请道友在上一个版本先将洞府建木物品取出</color>）。\n3、修复某些情况下定居/回家按钮消失不出现的问题。\n4、修复点进建筑然后右键取消直接回到大地图的问题。\n5、感谢3DM的<color=blue>dingchao52cc</color>和<color=blue>Karasunny</color>道友提供的BUG情报。" },
            new string[]{"<size=22>【2.0抢鲜体验版】</size>2021年12月24日23时", "1、修复自定义图片模糊的问题。\n2、修复未升到4级就可以切换4级工坊的问题以及类似问题。\n3、增加了灵阁的功能，可在灵阁免费修炼，灵阁四个等级对应四个州的灵阁修炼效果。\n4、增加更新提示，MOD可更新时开始界面右上角会有更新按钮，点击可立即前往论坛更新MOD。"+
                "5、增加洞府记事，记录洞府中发生的点点滴滴及美好回忆。\n6、增加客房功能，可邀请NPC入住。"+caveDes1},
            new string[]{"<size=22>【2.1抢鲜体验版】</size>2021年12月25日23时", "1、修复邀约的NPC会被传回洞府的问题。（已经发生的等邀约时间结束吧）\n2、建筑重新放置重置坐标，避免移除屏幕范围取不回来。\n3、感谢3DM的<color=blue>guigu007</color>道友提供的BUG情报。\n4、调整洞府记事的顺序。" },
            new string[]{"<size=22>【2.2抢鲜体验版】</size>2021年12月26日23时", "1、优化入住洞府的NPC过月会自动增加对府主的好感度。\n2、优化洞府建筑等级显示，若等级上限为1则不显示建筑等级。" },
            new string[]{"<size=22>【2.3抢鲜体验版】</size>2022年1月16日13时", "1、优化了mod框架，修复了一些已知BUG。\n2、优化了主界面按钮显示\n3、闲楼和厢房增加捏脸功能，闲楼可以给自己捏脸，厢房可以给位于洞府的伙伴捏脸。\n4、增加了版本预告。\n5、修改了建筑名称。\n"+
                "6、优化了字体的显示。\n7、优化了伙伴卡顿的问题。\n8、增加洞府改名功能。\n9、优化了邀请、请离伙伴会重置滚动位置的问题。\n10、增加建筑可以调整大小和自定义图片（支持png,jpg,gif)\n11、增加洞府菜单自动隐藏。\n12、增加支持自定义新增建筑，有兴趣的可以查看cave目录下的教程文档。\n13、洞府牌匾名字支持自定义格式，可在Cave目录下修改DataCustom.json文件。\n14、增加多个款式的牌匾。\n15、增加建筑支持挂载游戏内置特效。\n16、增加阵法功能，开启后在洞府过月可屏蔽其他人。"},
        };
        int maxversion = 10;
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
