using System;
using System.Collections.Generic;
using UnityEngine;
using GuiBaseUI;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

namespace Comment
{
    public class UIComment
    {
        public static bool isAutoLogin = false;
        public static bool isOpen = true;
        public static Dictionary<string, CommentData> data = new Dictionary<string, CommentData>();

        int game = 0;
        public int targetType = 0;
        public int targetId = 0;
        public bool isInitUI = false;

        CommentData commentData;
        UIBase ui;
        public GameObject bg;
        GameObject bgLogin;
        GameObject bgRegister;
        GameObject bgUser;
        bool isGetData = false;
        Vector2 pos;
        GameObject openArrow;
        float arrowAlpha = 1; // 箭头透明度
        int sort = 0; // 0热度倒叙 1热度正序 2时间倒序 3时间正序
        int inputMask = 0;

        Sprite goodSpripte;
        Sprite badSpripte;
        Sprite chat_bg_01;
        Sprite btn_chat_bg_01;
        Sprite btn_chat_bg_05;
        Sprite btn_chat_content_01;
        Sprite btn_general_01_selete;
        Sprite chat__textfield_01;
        Sprite chat_label_01;
        Sprite img_social_bg_02;
        Sprite vip_number_bg_02;
        public UIComment()
        {
            goodSpripte = ImageManager.LoadSprite("Mods/GuiUI/Image/zan.png").t1;
            badSpripte = ImageManager.LoadSprite("Mods/GuiUI/Image/cai.png").t1;
            chat_bg_01 = ImageManager.LoadSprite("Mods/GuiUI/Image/chat_bg_01.png", new Vector4(8, 26, 17, 26)).t1;
            btn_chat_bg_01 = ImageManager.LoadSprite("Mods/GuiUI/Image/btn_chat_bg_01.png").t1;
            btn_chat_bg_05 = ImageManager.LoadSprite("Mods/GuiUI/Image/btn_chat_bg_05.png").t1;
            btn_chat_content_01 = ImageManager.LoadSprite("Mods/GuiUI/Image/btn_chat_content_01.png").t1;
            btn_general_01_selete = ImageManager.LoadSprite("Mods/GuiUI/Image/btn_general_01_selete.png").t1;
            chat__textfield_01 = ImageManager.LoadSprite("Mods/GuiUI/Image/chat__textfield_01.png", new Vector4(20, 20, 20, 20)).t1;
            chat_label_01 = ImageManager.LoadSprite("Mods/GuiUI/Image/chat_label_01.png", new Vector4(10, 10, 10, 10)).t1;
            img_social_bg_02 = ImageManager.LoadSprite("Mods/GuiUI/Image/img_social_bg_02.png", new Vector4(21, 7, 14, 21)).t1;
            vip_number_bg_02 = ImageManager.LoadSprite("Mods/GuiUI/Image/vip_number_bg_02.png").t1;
        }

        Transform CreateUIPoint(string smalp)
        {
            Transform root = ui.transform.Find("yellowshange.com.dingcheng");
            if (root == null)
            {
                GameObject go = GameObject.Instantiate(ui.transform.Find(smalp).gameObject);
                RectTransform rtf = go.GetComponent<RectTransform>();
                go.name = "yellowshange.com.dingcheng";
                root = rtf;
                DestroyChilds(rtf);
                rtf.SetParent(ui.transform, false);
                go.SetActive(true);
                rtf.anchorMin = new Vector2(0, 0);
                rtf.anchorMax = new Vector2(1, 1);
                rtf.anchoredPosition = new Vector2(0, 0);
                rtf.sizeDelta = new Vector2(0, 0);
                var canvs = root.GetComponent<Canvas>();
                canvs.sortingOrder += 5;
            }
            return root;
        }
        Transform CreateUIPoint()
        {
            Transform root = ui.transform.Find("yellowshange.com.dingcheng");
            if (root == null)
            {
                UnhollowerBaseLib.Il2CppArrayBase<Canvas> all = ui.GetComponentsInChildren<Canvas>();
                if (all.Count > 0)
                {
                    GameObject go = GameObject.Instantiate(all[0].gameObject);
                    RectTransform rtf = go.GetComponent<RectTransform>();
                    go.name = "yellowshange.com.dingcheng";
                    root = rtf;
                    DestroyChilds(rtf);
                    rtf.SetParent(ui.transform, false);
                    go.SetActive(true);
                    rtf.anchorMin = new Vector2(0, 0);
                    rtf.anchorMax = new Vector2(1, 1);
                    rtf.anchoredPosition = new Vector2(0, 0);
                    rtf.sizeDelta = new Vector2(0, 0);
                    var canvs = root.GetComponent<Canvas>();
                    canvs.sortingOrder += 5;

                    var imgs = go.GetComponents<Image>();
                    foreach (var item in imgs)
                    {
                        item.enabled = false;
                    }
                    var rimgs = go.GetComponents<RawImage>();
                    foreach (var item in rimgs)
                    {
                        item.enabled = false;
                    }
                }
            }
            else
            {
                return ui.transform;
            }
            return root;
        }

        private Transform GetUIPoint(UIBase ui)
        {
            try
            {
                List<string> rootItem = new List<string>() { "Login" };
                if (rootItem.Contains(ui.gameObject.name))
                {
                    return ui.transform.Find("Root");
                }
                return CreateUIPoint();
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
            return ui.transform;
        }

        public void Init(UIBase ui, int targetType, int targetId)
        {
            if (!isAutoLogin)
            {
                isAutoLogin = true;
                if (PlayerPrefs.HasKey("www.yellowshange.com/user") && PlayerPrefs.HasKey("www.yellowshange.com/pass"))
                {
                    string user = PlayerPrefs.GetString("www.yellowshange.com/user");
                    string pass = PlayerPrefs.GetString("www.yellowshange.com/pass");

                    string url = @"http://www.yellowshange.com/guiguMod/comment/Login?a=" + user + "&b=" + pass;
                    new HttpData().GetHttp(ui, url, (s) =>
                    {
                        if (s.StartsWith("{"))
                        {
                            try
                            {
                                CommentMain.playerPassword = pass;
                                try
                                {
                                    LoginOk(s, true);
                                }
                                catch (Exception)
                                {
                                }
                                return;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        PlayerPrefs.DeleteKey("www.yellowshange.com/user");
                        PlayerPrefs.DeleteKey("www.yellowshange.com/pass");
                    });
                }
            }
            this.ui = ui;
            this.targetType = targetType;
            this.targetId = targetId;
            arrowAlpha = isOpen ? 1 : 0;
            InitBg();
            if (isOpen)
            {
                GetData();
            }

            TimerCoroutine cor = null;
            Action action = () =>
            {
                if (bg == null)
                {
                    g.timer.Stop(cor);
                    return;
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    if (inputMask == 0)
                    {
                        MoveBg();
                    }
                }
                RectTransform rtf = bg.GetComponent<RectTransform>();
                rtf.anchoredPosition = Vector2.Lerp(rtf.anchoredPosition, pos, 0.1f);
            };
            cor = g.timer.Frame(action, 1, true);
        }

        private void InitBg()
        {
            try
            {
                bg = CreateUI.New();
                bg.transform.SetParent(GetUIPoint(ui), false);
                RectTransform rtf = bg.GetComponent<RectTransform>();
                rtf.anchorMin = new Vector2(0f, 0.5f);
                rtf.anchorMax = new Vector2(0f, 0.5f);
                rtf.anchoredPosition = new Vector2(165.9f, 0);
                rtf.sizeDelta = new Vector2(620, 920);

                // 打开箭头
                GameObject tmpGo1 = CreateUI.NewImage(btn_chat_bg_01);
                tmpGo1.transform.SetParent(bg.transform, false);
                tmpGo1.GetComponent<RectTransform>().anchoredPosition = new Vector2(336.3f, 71.799f);
                tmpGo1.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 135);

                GameObject tmpGo2 = CreateUI.NewImage(btn_chat_bg_01);
                tmpGo2.transform.SetParent(bg.transform, false);
                tmpGo2.GetComponent<RectTransform>().anchoredPosition = new Vector2(336.3f, -63.199f);
                tmpGo2.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 135);
                tmpGo2.transform.localScale = new Vector3(1, -1, 1);
                tmpGo2.transform.SetParent(tmpGo1.transform);

                openArrow = CreateUI.NewButton(() =>
                {
                    MoveBg();
                }, btn_chat_content_01);
                openArrow.transform.SetParent(bg.transform, false);
                openArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(330.5f, 1.7f);
                openArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 98);
                openArrow.transform.SetParent(tmpGo1.transform);

                Action onMouseEnter = () =>
                {
                    arrowAlpha = 1;
                };
                Action onMouseExit = () =>
                {
                    if (!isOpen)
                    {
                        arrowAlpha = 0;
                    }
                };
                UIEventListener eventListener = tmpGo1.AddComponent<UIEventListener>();
                eventListener.onMouseEnter.AddListener(onMouseEnter);
                eventListener.onMouseExit.AddListener(onMouseExit);
                CanvasGroup canvasGroup = tmpGo1.AddComponent<CanvasGroup>();
                TimerCoroutine cor = null;
                Action onFram = () =>
                {
                    if (canvasGroup == null)
                    {
                        g.timer.Stop(cor);
                        return;
                    }
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, arrowAlpha, 0.1f);
                };
                cor = g.timer.Frame(onFram, 1, true);

                GameObject bg2 = CreateUI.NewImage(chat_bg_01);
                bg2.transform.SetParent(bg.transform, false);
                bg2.GetComponent<Image>().type = Image.Type.Sliced;
                rtf = bg2.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(68f, 0);
                rtf.sizeDelta = new Vector2(484.12f, 920);

                MoveBg(false);
            }
            catch (Exception e)
            {
                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
            }
        }

        private void MoveBg(bool isClick = true)
        {
            if (isClick)
            {
                isOpen = !isOpen;
                pos = isOpen ? new Vector2(165.9f, 0) : new Vector2(-308f, 0);
                openArrow.transform.localScale = isOpen ? Vector3.one : new Vector3(-1, 1, 1);
                if (!isGetData && isOpen)
                {
                    GetData();
                }
                PlayerPrefs.SetInt("yellowshange.com.comment.isOpen", isOpen ? 1 : 0);
            }
            else
            {
                pos = isOpen ? new Vector2(165.9f, 0) : new Vector2(-308f, 0);
                openArrow.transform.localScale = isOpen ? Vector3.one : new Vector3(-1, 1, 1);
                bg.GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }

        public void GetData()
        {
            MelonLoader.MelonDebug.Msg($"获取类型 {targetType} {targetId}");
            isGetData = true;
            string key = $"{game}_{targetType}_{targetId}";
            string url = @"http://www.yellowshange.com/guiguMod/comment/GetCommentList/?a=0&b=" + targetType + "&c=" + targetId;
            new HttpData().GetHttp(ui, url, (s) =>
            {
                if (s == "error: http1")
                {
                    Action send = () =>
                    {
                        if (ui)
                        {
                            Init(ui, targetType, targetId);
                        }
                    };
                    var cor = g.timer.Time(send, 10);
                    ui.AddCor(cor);
                }
                if (s.StartsWith("["))
                {
                    //MelonLoader.MelonDebug.Msg("取得评论数据成功");
                    //CommentItem[] items = CommonTool.JsonToObject<CommentItem[]>(s);
                    List<CommentItem> items = JsonConvert.DeserializeObject<List<CommentItem>>(s);
                    //MelonLoader.MelonDebug.Msg("评论数量：" + items.Count);
                    commentData = new CommentData()
                    {
                        items = items,
                        updateTime = 0
                    };
                    data[key] = commentData;
                    SortItems();
                    InitUI();
                }
                else if (s == "ok")
                {
                    //MelonLoader.MelonDebug.Msg("不需要更新数据");
                    SortItems();
                    InitUI();
                }
                else
                {
                    MelonLoader.MelonDebug.Msg("获取数据错误 " + s);
                }
            });
        }

        void DestroyChilds(Transform tf)
        {
            int count = tf.childCount;
            Transform[] gos = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                try
                {
                    gos[i] = tf.GetChild(i);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg("err1:" + e.Message);
                }
            }
            for (int i = 0; i < count; i++)
            {
                try
                {
                    GameObject.DestroyImmediate(gos[i].gameObject);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg("err2:" + e.Message);
                }
            }
        }

        private void InitUI()
        {
            if (ui == null)
                return;
            isInitUI = true;
            string key = $"{game}_{targetType}_{targetId}";
            commentData = data[key];

            RectTransform rtf;
            if (bg)
            {
                GameObject.Destroy(bg);
            }
            InitBg();

            GameObject newGo = CreateUI.New();
            newGo.transform.SetParent(bg.transform, false);
            rtf = newGo.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0.5f, 1f);
            rtf.anchorMax = new Vector2(0.5f, 1f);
            rtf.anchoredPosition = new Vector2(70f, -35f);
            rtf.sizeDelta = new Vector2(445, 50);
            HorizontalLayoutGroup newGroup = newGo.AddComponent<HorizontalLayoutGroup>();
            newGroup.childAlignment = TextAnchor.LowerLeft;
            newGroup.childForceExpandWidth = true;
            newGroup.childForceExpandHeight = false;

            // 标题
            GameObject title = CreateUI.NewText(GetTypeTitle(targetType, targetId) + " ", new Vector2(488, 50));
            title.transform.SetParent(newGo.transform, false);
            Text titleText = title.GetComponent<Text>();
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = new Color(0.51f, 0.21f, 0.07f);
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleLeft;

            // 排序
            GameObject sortGo1 = CreateUI.NewText("热度" + (sort == 0 ? "↓" : "") + (sort == 1 ? "↑" : ""), new Vector2(488, 50));
            sortGo1.transform.SetParent(newGo.transform, false);
            Text sortText1 = sortGo1.GetComponent<Text>();
            sortText1.color = new Color(0.57f, 0.39f, 0.30f);
            sortText1.fontSize = 22;
            sortText1.alignment = TextAnchor.LowerCenter;
            sortText1.fontStyle = (sort == 0 || sort == 1) ? FontStyle.Bold : FontStyle.Normal;
            Action sortClick1 = () =>
            {
                sort = sort == 0 ? 1 : 0;
                SortItems();
                InitUI();
            };
            sortGo1.AddComponent<Button>().onClick.AddListener(sortClick1);

            GameObject sortGo2 = CreateUI.NewText("时间" + (sort == 2 ? "↓" : "") + (sort == 3 ? "↑" : ""), new Vector2(488, 50));
            sortGo2.transform.SetParent(newGo.transform, false);
            Text sortText2 = sortGo2.GetComponent<Text>();
            sortText2.color = new Color(0.57f, 0.39f, 0.30f);
            sortText2.fontSize = 22;
            sortText2.alignment = TextAnchor.LowerCenter;
            sortText2.fontStyle = (sort == 2 || sort == 3) ? FontStyle.Bold : FontStyle.Normal;
            Action sortClick2 = () =>
            {
                sort = sort == 2 ? 3 : 2;
                SortItems();
                InitUI();
            };
            sortGo2.AddComponent<Button>().onClick.AddListener(sortClick2);

            GameObject emptyGo = CreateUI.NewText("        ", new Vector2(488, 50));
            emptyGo.transform.SetParent(newGo.transform, false);





            GameObject scroll = CreateUI.NewScrollView();
            scroll.transform.SetParent(bg.transform, false);
            rtf = scroll.GetComponent<RectTransform>();
            rtf.anchoredPosition = new Vector2(71.5f, 10f);
            rtf.sizeDelta = new Vector2(445, 760);
            ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
            Image scrollBg = scroll.AddComponent<Image>();
            scrollBg.sprite = chat__textfield_01;
            scrollBg.type = Image.Type.Sliced;
            scrollRect.content.GetComponent<VerticalLayoutGroup>().padding.left = 5;

            GameObject input = CreateUI.NewInputFieldPro(null, chat__textfield_01, "自觉遵守互联网政策法规，严禁发布色情、暴力、反动言论", (s) => inputMask++, (s) => inputMask--);
            input.GetComponent<Image>().type = Image.Type.Sliced;
            input.transform.SetParent(bg.transform, false);
            rtf = input.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0.5f, 0f);
            rtf.anchorMax = new Vector2(0.5f, 0f);
            rtf.anchoredPosition = new Vector2(2.71f, 45f);
            rtf.sizeDelta = new Vector2(307.42f, 75f);
            TMP_InputField inputField = input.GetComponent<TMP_InputField>();

            GameObject sendBtn = CreateUI.NewButton(() =>
            {
                SendComment(inputField);
            }, btn_general_01_selete);
            sendBtn.transform.SetParent(bg.transform, false);
            rtf = sendBtn.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0.5f, 0f);
            rtf.anchorMax = new Vector2(0.5f, 0f);
            rtf.anchoredPosition = new Vector2(231.5f, 56f);
            rtf.sizeDelta = new Vector2(125f, 53f);

            GameObject btnText = CreateUI.NewText("发送", rtf.sizeDelta);
            btnText.transform.SetParent(sendBtn.transform, false);
            Text textBtn = btnText.GetComponent<Text>();
            textBtn.color = new Color(0.51f, 0.21f, 0.07f);
            textBtn.fontSize = 24;
            textBtn.alignment = TextAnchor.MiddleCenter;

            List<CommentItem> items = commentData.items; // 获取到评论
            if (items.Count == 0)
            {
                // 名字
                GameObject nameGo = CreateUI.NewText("还没有人评论，快来抢占沙发吧！");
                nameGo.transform.SetParent(scrollRect.content, false);
                ContentSizeFitter nameContent = nameGo.AddComponent<ContentSizeFitter>();
                nameContent.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                Text nameText = nameGo.GetComponent<Text>();
                nameText.color = new Color(0.196f, 196f, 196f, 0.5f);
                nameText.fontSize = 22;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 100)
                {
                    break;
                }
                CommentItem item = items[i];
                //MelonLoader.MelonDebug.Msg($"{i} >> 昵称：{item.e} 时间：{item.b} 好评：{item.c} 差评：{item.d} 内容：{item.a}");

                GameObject newBg = CreateUI.NewImage(img_social_bg_02);
                newBg.GetComponent<Image>().type = Image.Type.Sliced;
                newBg.transform.SetParent(scrollRect.content, false);
                newBg.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 50);
                newBg.transform.localScale = new Vector3(-1, 1, 1);
                VerticalLayoutGroup bgGroup = newBg.AddComponent<VerticalLayoutGroup>();
                bgGroup.childAlignment = TextAnchor.UpperRight;
                bgGroup.childControlWidth = true;
                bgGroup.childControlHeight = true;
                bgGroup.childForceExpandWidth = false;
                bgGroup.childForceExpandHeight = false;
                bgGroup.padding.left = 6;
                bgGroup.padding.right = 15;
                bgGroup.padding.top = 8;
                bgGroup.padding.bottom = 8;
                bgGroup.spacing = 10;
                ContentSizeFitter fitter = newBg.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                newGo = CreateUI.New();
                newGo.transform.SetParent(newBg.transform, false);
                newGo.transform.localScale = new Vector3(-1, 1, 1);
                newGroup = newGo.AddComponent<HorizontalLayoutGroup>();
                newGroup.childAlignment = TextAnchor.LowerLeft;
                newGroup.childControlWidth = true;
                newGroup.childControlHeight = true;
                newGroup.childForceExpandWidth = false;
                newGroup.childForceExpandHeight = false;

                GameObject nameBg = CreateUI.NewImage(chat_label_01);
                nameBg.GetComponent<Image>().type = Image.Type.Sliced;
                nameBg.transform.SetParent(newGo.transform, false);
                HorizontalLayoutGroup nameGroup = nameBg.AddComponent<HorizontalLayoutGroup>();
                nameGroup.childAlignment = TextAnchor.LowerLeft;
                nameGroup.childControlWidth = true;
                nameGroup.childControlHeight = true;
                nameGroup.childForceExpandWidth = false;
                nameGroup.childForceExpandHeight = false;
                nameGroup.padding.left = 4;
                nameGroup.padding.right = 4;
                nameGroup.padding.top = 2;
                nameGroup.padding.bottom = 2;

                // 名字
                GameObject nameGo = CreateUI.NewText(item.name);
                nameGo.transform.SetParent(nameBg.transform, false);
                Text nameText = nameGo.GetComponent<Text>();
                nameText.color = new Color(0.298f, 0.298f, 0.298f);
                nameText.fontSize = 24;

                DateTime GetDateTime(int timeStamp)
                {
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    long lTime = ((long)timeStamp * 10000000);
                    TimeSpan toNow = new TimeSpan(lTime);
                    DateTime targetDt = dtStart.Add(toNow);
                    return targetDt;
                }
                DateTime dateTime = GetDateTime(item.time);

                GameObject timeGo = CreateUI.NewText(dateTime.ToString(" yyyy-MM-dd HH:mm:ss "));
                timeGo.transform.SetParent(newGo.transform, false);
                Text timeText = timeGo.GetComponent<Text>();
                timeText.color = new Color(0.57f, 0.39f, 0.30f);
                timeText.fontSize = 20;

                // 内容
                GameObject contentGo = CreateUI.NewText(item.connect, new Vector2(450, 30));
                contentGo.transform.SetParent(newBg.transform, false);
                contentGo.transform.localScale = new Vector3(-1, 1, 1);
                ContentSizeFitter contentContent = contentGo.AddComponent<ContentSizeFitter>();
                contentContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                Text contentText = contentGo.GetComponent<Text>();
                contentText.color = new Color(0.51f, 0.21f, 0.07f);

                // 点赞
                newGo = CreateUI.New();
                newGo.transform.SetParent(newBg.transform, false);
                newGo.transform.localScale = new Vector3(-1, 1, 1);
                newGo.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 55);
                newGroup = newGo.AddComponent<HorizontalLayoutGroup>();
                newGroup.childAlignment = TextAnchor.UpperCenter;
                newGroup.childControlWidth = true;
                newGroup.childControlHeight = true;
                newGroup.childForceExpandWidth = false;
                newGroup.childForceExpandHeight = false;

                GameObject goodImg = CreateUI.NewImage();
                goodImg.transform.SetParent(newGo.transform, false);
                goodImg.GetComponent<Image>().sprite = goodSpripte;
                Action sendGood = () =>
                {
                    if (string.IsNullOrWhiteSpace(CommentMain.playerUser) || string.IsNullOrWhiteSpace(CommentMain.playerName))
                    {
                        Action okClick = () =>
                        {
                            // 打开登陆界面
                            InitLogin();
                        };
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "游客无法发表评论，需要先登录！", 2, okClick);

                        return;
                    }
                    string goodKey = $"a={game}&b={targetType}&c={targetId}&d=1&e={CommentMain.playerUser}&f={item.name}&g={item.time}";
                    if (PlayerPrefs.HasKey(goodKey))
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "已经赞过这条点评了，一人只能赞一次哦！", 1);
                        return;
                    }
                    PlayerPrefs.SetInt(goodKey, 1);
                    string url = @"http://www.yellowshange.com/guiguMod/comment/BadGood/?" + goodKey;
                    new HttpData().GetHttp(ui, url, (s) =>
                    {
                        if (s == "ok")
                        {
                            item.c++;
                            SortItems();
                            InitUI();
                            UITipItem.AddTip("点赞成功！");
                        }
                        else
                        {
                            PlayerPrefs.DeleteKey(goodKey);
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "点赞失败！" + s, 1);
                        }
                    });
                };
                goodImg.AddComponent<Button>().onClick.AddListener(sendGood);
                LayoutElement goodLayout = goodImg.AddComponent<LayoutElement>();
                goodLayout.preferredWidth = 22;
                goodLayout.preferredHeight = 22;


                GameObject goodGo = CreateUI.NewText(" " + item.good + "    ");
                goodGo.transform.SetParent(newGo.transform, false);
                Text goodText = goodGo.GetComponent<Text>();
                goodText.color = new Color(0.57f, 0.39f, 0.30f);
                goodText.fontSize = 22;

                GameObject badImg = CreateUI.NewImage();
                badImg.transform.SetParent(newGo.transform, false);
                badImg.GetComponent<Image>().sprite = badSpripte;
                Action sendBad = () =>
                {
                    if (string.IsNullOrWhiteSpace(CommentMain.playerUser) || string.IsNullOrWhiteSpace(CommentMain.playerName))
                    {
                        Action okClick = () =>
                        {
                            // 打开登陆界面
                            InitLogin();
                        };
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "游客无法发表评论，需要先登录！", 2, okClick);

                        return;
                    }
                    string badKey = $"a={game}&b={targetType}&c={targetId}&d=0&e={CommentMain.playerUser}&f={item.name}&g={item.time}";
                    if (PlayerPrefs.HasKey(badKey))
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "已经踩过这条点评了，一人只能踩一次哦！", 1);
                        return;
                    }
                    PlayerPrefs.SetInt(badKey, 1);
                    string url = @"http://www.yellowshange.com/guiguMod/comment/BadGood/?" + badKey;
                    new HttpData().GetHttp(ui, url, (s) =>
                    {
                        if (s == "ok")
                        {
                            item.d++;
                            SortItems();
                            InitUI();
                            UITipItem.AddTip("踩一踩成功！");
                        }
                        else
                        {
                            PlayerPrefs.DeleteKey(badKey);
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "踩一踩失败！" + s, 1);
                        }
                    });
                };
                badImg.AddComponent<Button>().onClick.AddListener(sendBad);
                LayoutElement badLayout = badImg.AddComponent<LayoutElement>();
                badLayout.preferredWidth = 22;
                badLayout.preferredHeight = 22;

                GameObject badGo = CreateUI.NewText(" " + item.bad);
                badGo.transform.SetParent(newGo.transform, false);
                Text badText = badGo.GetComponent<Text>();
                badText.color = new Color(0.57f, 0.39f, 0.30f);
                badText.fontSize = 22;

            }
            scrollRect.verticalNormalizedPosition = 1;

            bool isLogin = !string.IsNullOrWhiteSpace(CommentMain.playerName);
            //if (isLogin)
            //{
            //    MelonLoader.MelonDebug.Msg("登录用户：[" + CommentMain.playerName + "]");
            //}
            GameObject namePlayer = CreateUI.NewText(!isLogin ? "登录" : CommentMain.playerName, new Vector2(420, 30));
            namePlayer.transform.SetParent(bg.transform, false);
            Text playerName = namePlayer.GetComponent<Text>();
            playerName.fontSize = 24;
            playerName.color = !isLogin ? new Color(0.57f, 0.39f, 0.30f) : new Color(0.51f, 0.21f, 0.07f);
            playerName.alignment = TextAnchor.LowerRight;
            rtf = namePlayer.GetComponent<RectTransform>();
            rtf.pivot = new Vector2(1, 1);
            rtf.anchorMin = new Vector2(1, 1);
            rtf.anchorMax = new Vector2(1, 1);
            rtf.anchoredPosition = new Vector2(-10, -5);

            Button playerBtn = namePlayer.AddComponent<Button>();
            Action action = () =>
            {
                if (!isLogin)
                {
                    // 打开登陆界面
                    InitLogin();
                }
                else
                {
                    InitUser();
                }
            };
            playerBtn.onClick.AddListener(action);
        }

        bool onGetMyData = true;
        List<CommentItem> myData = new List<CommentItem>();
        public void InitUser()
        {
            {
                if (bgUser)
                {
                    GameObject.Destroy(bgUser);
                }

                // 背景
                GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
                bgUser = bg;
                bg.transform.SetParent(GetUIPoint(ui), false);
                bg.GetComponent<Image>().type = Image.Type.Sliced;
                RectTransform rtf = bg.GetComponent<RectTransform>();
                rtf.anchorMin = new Vector2(0f, 0.5f);
                rtf.anchorMax = new Vector2(0f, 0.5f);
                rtf.anchoredPosition = new Vector2(930f, 0);
                rtf.sizeDelta = new Vector2(580f, 1020);

                // 标题
                GameObject title = CreateUI.NewText("个人中心", new Vector2(488, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-26f, 460);
                Text titleText = title.GetComponent<Text>();
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.red;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleCenter;

                // 退出按钮
                GameObject exitBtn = CreateUI.NewButton(() =>
                {
                    GameObject.Destroy(bgUser);
                }, SpriteTool.GetSprite("Common", "tuichu"));
                exitBtn.transform.SetParent(bg.transform, false);
                rtf = exitBtn.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(201, 458);

                // 名字
                GameObject nameGo = CreateUI.NewText(CommentMain.playerName);
                nameGo.transform.SetParent(bg.transform, false);
                rtf = nameGo.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-60f, -315f);
                rtf.sizeDelta = new Vector2(400f, 80f);
                Text nameText = nameGo.GetComponent<Text>();
                nameText.fontStyle = FontStyle.Bold;
                nameText.color = Color.blue;
                nameText.fontSize = 45;
                nameText.alignment = TextAnchor.MiddleCenter;
            }
            if (!string.IsNullOrWhiteSpace(CommentMain.playerName))
            {
                GameObject bg = bgUser;
                RectTransform rtf;
                // 下线按钮
                GameObject outBtn = CreateUI.NewButton(() =>
                {
                    CommentMain.playerName = "";
                    CommentMain.playerUser = "";
                    CommentMain.playerPassword = "";
                    PlayerPrefs.DeleteKey("www.yellowshange.com/user");
                    PlayerPrefs.DeleteKey("www.yellowshange.com/pass");
                    InitUI();
                    GameObject.Destroy(bgUser);
                }, SpriteTool.GetSprite("Common", "duihuan"));
                outBtn.transform.SetParent(bg.transform, false);
                rtf = outBtn.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(185f, -315f);
                rtf.sizeDelta = new Vector2(90f, 58f);

                GameObject outText = CreateUI.NewText("下线", rtf.sizeDelta);
                outText.transform.SetParent(outBtn.transform, false);
                Text textBtn = outText.GetComponent<Text>();
                textBtn.fontSize = 38;
                textBtn.color = Color.black;
                textBtn.alignment = TextAnchor.MiddleCenter;



                GameObject scroll = CreateUI.NewScrollView(new Vector2(480, 690));
                scroll.transform.SetParent(bg.transform, false);
                rtf = scroll.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-23f, 85);
                ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
                Image scrollBg = scroll.AddComponent<Image>();
                scrollBg.sprite = SpriteTool.GetSprite("BattleInfoCommon", "zhaohuanwu_3");
                scrollBg.color = new Color(1, 1, 1, 0.5f);
                scrollRect.content.GetComponent<VerticalLayoutGroup>().padding.left = 5;

                if (myData.Count == 0)
                {
                    // 名字
                    GameObject tipGo = CreateUI.NewText("您还没有发表过评论哦！");
                    tipGo.transform.SetParent(scrollRect.content, false);
                    ContentSizeFitter nameContent = tipGo.AddComponent<ContentSizeFitter>();
                    nameContent.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    Text tipText = tipGo.GetComponent<Text>();
                    tipText.fontStyle = FontStyle.Bold;
                    tipText.color = Color.blue;
                    tipText.fontSize = 28;
                }

                for (int i = 0; i < myData.Count; i++)
                {
                    CommentItem item = myData[i];
                    //MelonLoader.MelonDebug.Msg($"{i} >> 昵称：{item.e} 时间：{item.b} 好评：{item.c} 差评：{item.d} 内容：{item.a}");

                    var newGo = CreateUI.New();
                    newGo.transform.SetParent(scrollRect.content, false);
                    newGo.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 35);
                    var newGroup = newGo.AddComponent<HorizontalLayoutGroup>();
                    newGroup.childAlignment = TextAnchor.LowerLeft;
                    newGroup.childControlWidth = true;
                    newGroup.childForceExpandWidth = false;
                    newGroup.childForceExpandHeight = false;

                    // 标题
                    GameObject nameGo = CreateUI.NewText(GetTypeTitle(item.type, item.target)); ;
                    nameGo.transform.SetParent(newGo.transform, false);
                    Text nameText = nameGo.GetComponent<Text>();
                    nameText.fontStyle = FontStyle.Bold;
                    nameText.color = Color.blue;
                    nameText.fontSize = 28;

                    DateTime GetDateTime(int timeStamp)
                    {
                        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        long lTime = ((long)timeStamp * 10000000);
                        TimeSpan toNow = new TimeSpan(lTime);
                        DateTime targetDt = dtStart.Add(toNow);
                        return targetDt;
                    }
                    DateTime dateTime = GetDateTime(item.time);

                    GameObject timeGo = CreateUI.NewText(dateTime.ToString(" yyyy-MM-dd HH:mm:ss "));
                    timeGo.transform.SetParent(newGo.transform, false);
                    Text timeText = timeGo.GetComponent<Text>();
                    timeText.color = Color.red;
                    timeText.fontSize = 22;

                    // 内容
                    GameObject contentGo = CreateUI.NewText(item.connect, new Vector2(450, 30));
                    contentGo.transform.SetParent(scrollRect.content, false);
                    ContentSizeFitter contentContent = contentGo.AddComponent<ContentSizeFitter>();
                    contentContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    Text contentText = contentGo.GetComponent<Text>();
                    contentText.color = Color.black;

                    // 点赞
                    newGo = CreateUI.New();
                    newGo.transform.SetParent(scrollRect.content, false);
                    newGo.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 55);
                    newGroup = newGo.AddComponent<HorizontalLayoutGroup>();
                    newGroup.childAlignment = TextAnchor.UpperCenter;
                    newGroup.childControlWidth = true;
                    newGroup.childForceExpandWidth = false;
                    newGroup.childForceExpandHeight = false;

                    GameObject goodImg = CreateUI.NewImage();
                    goodImg.transform.SetParent(newGo.transform, false);
                    goodImg.GetComponent<RectTransform>().sizeDelta = new Vector2(17, 22);
                    goodImg.GetComponent<Image>().sprite = goodSpripte;
                    Action sendGood = () =>
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "这是您本条评论获得的点赞数！", 1);
                    };
                    goodImg.AddComponent<Button>().onClick.AddListener(sendGood);


                    GameObject goodGo = CreateUI.NewText(" " + item.good + "    ");
                    goodGo.transform.SetParent(newGo.transform, false);
                    Text goodText = goodGo.GetComponent<Text>();
                    goodText.color = Color.green;
                    goodText.fontSize = 22;

                    GameObject badImg = CreateUI.NewImage();
                    badImg.transform.SetParent(newGo.transform, false);
                    badImg.GetComponent<RectTransform>().sizeDelta = new Vector2(17, 22);
                    badImg.GetComponent<Image>().sprite = badSpripte;
                    Action sendBad = () =>
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "这是您本条评论被踩的数量！", 1);
                    };
                    badImg.AddComponent<Button>().onClick.AddListener(sendBad);

                    GameObject badGo = CreateUI.NewText(" " + item.bad);
                    badGo.transform.SetParent(newGo.transform, false);
                    Text badText = badGo.GetComponent<Text>();
                    badText.color = Color.red;
                    badText.fontSize = 22;

                }
                scrollRect.verticalNormalizedPosition = 1;


                if (onGetMyData)
                {
                    // 获取自己的评论数据
                    string url = @"http://www.yellowshange.com/guiguMod/comment/getUserData.php?a=0&b=" + CommentMain.playerName;
                    new HttpData().GetHttp(ui, url, (s) =>
                    {
                        onGetMyData = false;

                        if (s.StartsWith("["))
                        {
                            try
                            {
                                myData = JsonConvert.DeserializeObject<List<CommentItem>>(s);
                            }
                            catch (Exception e)
                            {
                                MelonLoader.MelonDebug.Msg("获取到的数据 " + s);
                                MelonLoader.MelonDebug.Msg(e.Message + "\n" + e.StackTrace);
                                myData = new List<CommentItem>();
                            }
                            InitUser();
                        }
                        else
                        {
                            MelonLoader.MelonDebug.Msg("获取数据错误 " + s);
                        }
                    });
                }
            }
        }

        private void SortItems()
        {
            if (commentData == null || commentData.items == null)
                return;
            List<CommentItem> items = commentData.items;
            items.Sort((a, b) =>
            {
                // 0热度倒叙 1热度正序 2时间倒序 3时间正序
                switch (sort)
                {
                    case 3:
                        return (a.time) > (b.time) ? 1 : -1;
                    case 2:
                        return (a.time) < (b.time) ? 1 : -1;
                    case 1:
                        return (a.good - a.bad * 1.5f) > (b.good - b.bad * 1.5f) ? 1 : -1;
                    default:
                        return (a.good - a.bad * 1.5f) < (b.good - b.bad * 1.5f) ? 1 : -1;
                }
            });
        }

        public bool SendComment(TMP_InputField inputField)
        {
            string str = inputField.text;
            inputField.text = "";

            if (string.IsNullOrWhiteSpace(CommentMain.playerUser) || string.IsNullOrWhiteSpace(CommentMain.playerName))
            {
                Action okClick = () =>
                {
                    // 打开登陆界面
                    InitLogin();
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "游客无法发表评论，需要先登录！", 2, okClick);

                return false;
            }
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            if (g.conf.textBlock.IsBlock(str, false) != 0)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "内容不合法", 1);
                return false;
            }

            if (str.Length > 256)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "内容过长", 1);
                return false;
            }

            string url = @"http://www.yellowshange.com/guiguMod/comment/AddComment?a=" + game + "&b=" + targetType + "&c=" + targetId + "&d=" + str + "&e=" + CommentMain.playerUser;
            new HttpData().GetHttp(ui, url, (s) =>
            {
                if (s == "ok")
                {
                    // DateTime时间格式转换为Unix时间戳格式
                    int DateTimeToStamp(DateTime time)
                    {
                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                        return (int)(time - startTime).TotalSeconds;
                    }
                    CommentItem item = new CommentItem()
                    {
                        a = str,
                        b = DateTimeToStamp(DateTime.Now),
                        c = 0,
                        d = 0,
                        e = CommentMain.playerName,
                    };
                    commentData.items.Insert(0, item);
                    UITipItem.AddTip("发送成功！");
                    InitUI();
                    onGetMyData = true;
                }
                else if (s.StartsWith("error"))
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "发送失败：" + s, 1);
                }
                else
                {
                    CommentMain.playerUser = "";
                    CommentMain.playerName = "";
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "发送失败，已退出登录：" + s, 1);
                }
            });
            return true;
        }
        public void InitLogin()
        {
            if (!bgLogin)
            {
                GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
                bgLogin = bg;
                bg.transform.SetParent(GetUIPoint(ui), false);
                bg.GetComponent<Image>().type = Image.Type.Sliced;
                RectTransform rtf = bg.GetComponent<RectTransform>();
                rtf.anchorMin = new Vector2(0f, 0.5f);
                rtf.anchorMax = new Vector2(0f, 0.5f);
                rtf.anchoredPosition = new Vector2(930f, 0);
                rtf.sizeDelta = new Vector2(580f, 1020);

                // 标题
                GameObject title = CreateUI.NewText("登录账号", new Vector2(488, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-26f, 365f);
                Text titleText = title.GetComponent<Text>();
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.red;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleCenter;

                // 账号文字
                title = CreateUI.NewText("账号", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 230f);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 密码文字
                title = CreateUI.NewText("密码", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 60f);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;


                // 账号
                GameObject input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "输入用户名", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 230f);
                TMP_InputField inputField1 = input.GetComponent<TMP_InputField>();
                inputField1.contentType = TMP_InputField.ContentType.Alphanumeric;

                // 密码
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "输入密码", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 60f);
                TMP_InputField inputField2 = input.GetComponent<TMP_InputField>();
                inputField2.contentType = TMP_InputField.ContentType.Password;

                //// 提示文本
                //title = CreateUI.NewText("", new Vector2(440, 270));
                //title.transform.SetParent(bg.transform, false);
                //rtf = title.GetComponent<RectTransform>();
                //rtf.anchoredPosition = new Vector2(-26f, -195f);
                //Text tipText = title.GetComponent<Text>();
                //tipText.color = Color.red;
                //tipText.fontSize = 45;
                //tipText.alignment = TextAnchor.UpperLeft;

                // 登录按钮
                GameObject goMemory = CreateUI.NewToggle("记住账号，下次自动登录", new Vector2(300, 60));
                goMemory.transform.SetParent(bg.transform, false);
                rtf = goMemory.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-26, -160);
                Toggle toggleMemory = goMemory.GetComponent<Toggle>();

                GameObject loginBtn = CreateUI.NewButton(() =>
                {
                    string user = inputField1.text;
                    string pass = inputField2.text;
                    bool isMemory = toggleMemory == null ? false : toggleMemory.isOn;
                    if (user.Length < 3 || user.Length > 20 || pass.Length < 3 || pass.Length > 20)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "账号或密码长度错误！", 1);
                    }
                    else
                    {
                        string url = @"http://www.yellowshange.com/guiguMod/comment/Login?a=" + user + "&b=" + pass;
                        new HttpData().GetHttp(ui, url, (s) =>
                        {
                            if (s.StartsWith("{"))
                            {
                                try
                                {
                                    CommentMain.playerPassword = pass;
                                    LoginOk(s, isMemory);
                                    return;
                                }
                                catch (Exception)
                                {
                                    //MelonLoader.MelonDebug.Msg(s);
                                    throw;
                                }
                            }
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "登录失败" + s, 1);
                        });

                        GameObject.Destroy(bgLogin);
                    }
                }, SpriteTool.GetSprite("Common", "tongyongbutton_2"));
                loginBtn.transform.SetParent(bg.transform, false);
                rtf = loginBtn.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(133, 60f);
                rtf.anchoredPosition = new Vector2(-26, -100);
                // 文字
                title = CreateUI.NewText("登录", new Vector2(120, 50));
                title.transform.SetParent(loginBtn.transform, false);
                titleText = title.GetComponent<Text>();
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.black;

                // 注册按钮
                GameObject registerBtn = CreateUI.NewButton(() =>
                {
                    InitRegister();
                    GameObject.Destroy(bgLogin);
                }, SpriteTool.GetSprite("Common", "tongyongbutton_2"));
                registerBtn.transform.SetParent(bg.transform, false);
                rtf = registerBtn.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(133, 60f);
                rtf.anchoredPosition = new Vector2(-26, -300);
                // 文字
                title = CreateUI.NewText("注册账号", new Vector2(120, 50));
                title.transform.SetParent(registerBtn.transform, false);
                titleText = title.GetComponent<Text>();
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.black;

                // 退出按钮
                GameObject exitBtn = CreateUI.NewButton(() =>
                {
                    GameObject.Destroy(bgLogin);
                }, SpriteTool.GetSprite("Common", "tuichu"));
                exitBtn.transform.SetParent(bg.transform, false);
                rtf = exitBtn.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(201, 458);
            }
        }

        private void LoginOk(string s, bool isMonory = false)
        {
            LoginData data = JsonConvert.DeserializeObject<LoginData>(s);
            CommentMain.playerName = data.name;
            CommentMain.playerUser = data.user;
            if (isMonory)
            {
                PlayerPrefs.SetString("www.yellowshange.com/user", CommentMain.playerUser);
                PlayerPrefs.SetString("www.yellowshange.com/pass", CommentMain.playerPassword);
            }
            else
            {
                PlayerPrefs.DeleteKey("www.yellowshange.com/user");
                PlayerPrefs.DeleteKey("www.yellowshange.com/pass");
            }
            Action action = () =>
            {

            };
            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "欢迎回来：" + data.name, 1, action);
            if (isInitUI)
            {
                InitUI();
            }
        }

        public void InitRegister()
        {
            if (!bgRegister)
            {
                GameObject bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg"));
                bgRegister = bg;
                bg.transform.SetParent(GetUIPoint(ui), false);
                bg.GetComponent<Image>().type = Image.Type.Sliced;
                RectTransform rtf = bg.GetComponent<RectTransform>();
                rtf.anchorMin = new Vector2(0f, 0.5f);
                rtf.anchorMax = new Vector2(0f, 0.5f);
                rtf.anchoredPosition = new Vector2(930f, 0);
                rtf.sizeDelta = new Vector2(580f, 1020);

                // 标题

                GameObject title = CreateUI.NewText("注册账号", new Vector2(488, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-26f, 400);
                Text titleText = title.GetComponent<Text>();
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.red;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleCenter;

                // 昵称文字
                title = CreateUI.NewText("昵称<color=red>*</color>", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 278);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 账号文字
                title = CreateUI.NewText("账号<color=red>*</color>", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 198);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 密码文字
                title = CreateUI.NewText("密码<color=red>*</color>", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 118);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 确认密码文字
                title = CreateUI.NewText("密码<color=red>*</color>", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, 38);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 手机号文字
                title = CreateUI.NewText("问题", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, -42);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;

                // 身份证文字
                title = CreateUI.NewText("答案", new Vector2(120, 50));
                title.transform.SetParent(bg.transform, false);
                rtf = title.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-200f, -122);
                titleText = title.GetComponent<Text>();
                titleText.color = Color.blue;
                titleText.fontSize = 45;
                titleText.alignment = TextAnchor.MiddleLeft;


                // 昵称
                GameObject input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "输入昵称（评论时显示）", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 278);
                TMP_InputField inputField1 = input.GetComponent<TMP_InputField>();

                // 账号
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "输入账号（登录使用）", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 198);
                TMP_InputField inputField2 = input.GetComponent<TMP_InputField>();
                inputField2.contentType = TMP_InputField.ContentType.Alphanumeric;

                // 密码
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "输入密码（登录使用）", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 118);
                TMP_InputField inputField3 = input.GetComponent<TMP_InputField>();
                inputField3.contentType = TMP_InputField.ContentType.Password;

                // 确认密码
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "再次输入密码确认", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, 38);
                TMP_InputField inputField4 = input.GetComponent<TMP_InputField>();
                inputField4.contentType = TMP_InputField.ContentType.Password;

                // 手机号
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "找回密码的问题<color=red> 选填</color>", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, -42);
                TMP_InputField inputField5 = input.GetComponent<TMP_InputField>();
                inputField5.contentType = TMP_InputField.ContentType.Name;


                // 身份证
                input = CreateUI.NewInputFieldPro(null, SpriteTool.GetSprite("Common", "daojukukuang_2"), "找回密码的答案<color=red> 选填</color>", (s) => inputMask++, (s) => inputMask--);
                input.GetComponent<Image>().type = Image.Type.Sliced;
                input.transform.SetParent(bg.transform, false);
                rtf = input.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(300f, 60f);
                rtf.anchoredPosition = new Vector2(30f, -122);
                TMP_InputField inputField6 = input.GetComponent<TMP_InputField>();
                inputField6.contentType = TMP_InputField.ContentType.Name;

                //// 提示文本
                //title = CreateUI.NewText("", new Vector2(440, 270));
                //title.transform.SetParent(bg.transform, false);
                //rtf = title.GetComponent<RectTransform>();
                //rtf.anchoredPosition = new Vector2(-26f, -195f);
                //Text tipText = title.GetComponent<Text>();
                //tipText.color = Color.red;
                //tipText.fontSize = 45;
                //tipText.alignment = TextAnchor.UpperLeft;

                // 注册按钮
                GameObject goMemory = CreateUI.NewToggle("记住账号，下次自动登录", new Vector2(300, 60));
                goMemory.transform.SetParent(bg.transform, false);
                rtf = goMemory.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(-26, -360);
                Toggle toggleMemory = goMemory.GetComponent<Toggle>();

                GameObject loginBtn = CreateUI.NewButton(() =>
                {
                    string name = inputField1.text;
                    string user = inputField2.text;
                    string pass = inputField3.text;
                    string pass2 = inputField4.text;
                    string phone = inputField5.text;
                    string card = inputField6.text;
                    bool isMemory = toggleMemory == null ? false : toggleMemory.isOn;

                    if (user.Length < 3 || user.Length > 20 || pass.Length < 3 || pass.Length > 20)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "账号或密码长度错误！", 1);
                        return;
                    }
                    if (pass != pass2)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "两次输入密码不一致！", 1);
                        return;
                    }
                    if (g.conf.textBlock.IsBlock(name, false) != 0)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "输入的昵称不合法", 1);
                        return;
                    }
                    if (card.Length > 0 && g.conf.textBlock.IsBlock(card, false) != 0)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "输入的问题不合法", 1);
                        return;
                    }
                    if (phone.Length > 0 && g.conf.textBlock.IsBlock(phone, false) != 0)
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "输入的答案不合法", 1);
                        return;
                    }

                    string url = @"http://www.yellowshange.com/guiguMod/comment/Register?a=" + user + "&b=" + pass + "&c=" + name + "&d=" + phone + "&e=" + card;
                    new HttpData().GetHttp(ui, url, (s) =>
                    {
                        if (s.StartsWith("{"))
                        {
                            try
                            {
                                CommentMain.playerPassword = pass;
                                LoginOk(s, isMemory);
                                return;
                            }
                            catch (Exception)
                            {
                                //MelonLoader.MelonDebug.Msg(s);
                                throw;
                            }

                        }
                        else if (s == "error:repetition")
                        {
                            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "该账号已被注册" + s, 1);
                            return;
                        }
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "注册失败" + s, 1);
                    });

                    GameObject.Destroy(bgRegister);
                }, SpriteTool.GetSprite("Common", "tongyongbutton_2"));
                loginBtn.transform.SetParent(bg.transform, false);
                rtf = loginBtn.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(133, 60f);
                rtf.anchoredPosition = new Vector2(-26, -300);
                // 文字
                title = CreateUI.NewText("注册", new Vector2(120, 50));
                title.transform.SetParent(loginBtn.transform, false);
                titleText = title.GetComponent<Text>();
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.black;


                // 退出按钮
                GameObject exitBtn = CreateUI.NewButton(() =>
                {
                    GameObject.Destroy(bgRegister);
                }, SpriteTool.GetSprite("Common", "tuichu"));
                exitBtn.transform.SetParent(bg.transform, false);
                rtf = exitBtn.GetComponent<RectTransform>();
                rtf.anchoredPosition = new Vector2(201, 458);
            }

        }

        public string GetTypeTitle(int type, int target)
        {
            int targetType = type;
            int targetId = target;
            if (targetType == 0) // 壁纸
            {
                string[] list = new string[] { "吹笛人和龙", "美女蛇", "冥山", "御剑飞行", "昊天眼", };
                if (targetId < list.Length)
                {
                    return list[targetId];
                }
            }
            else if (targetType == 1) // 地图格子
            {
                var x = targetId / 10000;
                var y = targetId % 10000;
                return $"坐标 {x},{y}";
            }
            else if (targetType == 2) // 道具
            {
                try
                {
                    return GameTool.LS(g.conf.itemProps.GetItem(targetId).name);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 3) // 器灵
            {
                try
                {
                    return GameTool.LS(g.conf.artifactSprite.GetItem(targetId).name);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 4) // 功法
            {
                try
                {
                    int id = targetId;
                    ConfBattleSkillAttackItem skillAttackItem = g.conf.battleSkillAttack.GetItem(id);
                    if (skillAttackItem != null)
                    {
                        return GameTool.LS(skillAttackItem.name);
                    }
                    ConfBattleAbilityBaseItem abilityBaseItem = g.conf.battleAbilityBase.GetItem(id);
                    if (abilityBaseItem != null)
                    {
                        return GameTool.LS(abilityBaseItem.name);
                    }
                    ConfBattleStepBaseItem stepBaseItem = g.conf.battleStepBase.GetItem(id);
                    if (stepBaseItem != null)
                    {
                        return GameTool.LS(stepBaseItem.name);
                    }
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 5) // 二级建筑
            {
                try
                {
                    int id = targetId;
                    string[] list = new string[] {
                        "？？？", "建木", "升仙台", "拍卖行", "通天榜", "琅琊阁", // 0-5
                        "客栈", "酒馆", "坊市", "工坊", "仙衣阁", // 6-10
                        "悬赏榜", "大地图", "御兽峰", "灵阁", "任务大厅", // 11-15
                        "议事大厅", "聚宝仙楼", "疗伤院", "藏经阁", "青鳞池", // 16-20
                        "道号", "每月事件", "双鱼佩", "任务信件", "获得奖励", // 21-25
                        "准备战斗", "战斗结束", "正道宗门", "魔道宗门", "张三" }; // 26-30
                    if (id < list.Length)
                    {
                        return list[id];
                    }
                    if (id >= 100)
                    {
                        return GameTool.LS("areaName" + (id - 100));
                    }
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 6) // 剧情
            {
                try
                {
                    var item = targetId.ToString();
                    return item;
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 7) // 野外建筑 通灵秘境等
            {
                try
                {
                    int id = targetId;
                    ConfWorldBuilding10005Item item = g.conf.worldBuilding10005.GetItem(id);
                    return GameTool.LS(item.name) + (id % 10);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 8) // 战斗结束BOSS界面
            {
                try
                {
                    int id = targetId;
                    ConfBattleUnitAttrItem item = g.conf.battleUnitAttr.GetItem(id);
                    return GameTool.LS(item.name);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            else if (targetType == 9) // 大地图建筑位置
            {
                try
                {
                    int id = targetId;
                    ConfBattleUnitAttrItem item = g.conf.battleUnitAttr.GetItem(id);
                    return GameTool.LS(item.name);
                }
                catch (Exception e)
                {
                    MelonLoader.MelonDebug.Msg(e.Message);
                }
            }
            MelonLoader.MelonDebug.Msg("未分类|" + targetType + "|" + targetId);
            return "未分类";
        }
    }
}
