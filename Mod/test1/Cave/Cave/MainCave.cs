﻿using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cave.Config;
using Cave.Patch;

namespace Cave
{
    public class MainCave
    {
        public HeadAtlasTool headAtlas;
        public static DataCave data;
        public static MainCave cave;
        public static CaveBuildData createItem; // 正在创建的建筑
        public static GameObject createItemObj; // 正在创建的建筑
        UIGamePlanTip ui_plan;
        GameObject root;
        public int scrollPage = 1; // 滚动页签 0商店 1仓库 2伙伴
        public int operateBuildIdx = -1; // 当前正在操作的建筑
        List<WorldUnitBase> drawUnits = new List<WorldUnitBase>();
        CanvasGroup closeCanvasGroup;
        CanvasGroup panelCanvasGroup;
        Vector3 offsetPos; // 右侧的子项tips偏移
        Vector3 offsetPos2; // 建筑右侧按钮tips偏移

        public MainCave()
        {
            Cave.config.Init();
            DataCustom.Init();
            ConfBuild.Init();

            offsetPos = new Vector3(-5, 0, 0);
            offsetPos2 = new Vector3(-5, 0, 0);
            cave = this;
            data = DataCave.ReadData();
            if (data.open == 0)
            {
                Cave.OpenDrama("欢迎回家！您的主界面已经加入了回家按钮，只要点击回家按钮就能瞬间回家！", Init);
            }
            else
            {
                Init();
            }
        }

        public void Init()
        {
            data.open++;
            InitUI();
        }

        public void SaveData()
        {
            DataCave.SaveData(data);
        }

        public void InitUI()
        {
            if (root != null)
            {
                GameObject.Destroy(root);
            }
            ui_plan = g.ui.OpenUI<UIGamePlanTip>(UIType.GamePlanTip);
            for (int i = 0; i < ui_plan.transform.childCount; i++)
            {
                ui_plan.transform.GetChild(i).gameObject.SetActive(false);
            }
            Action onClose = () =>
            {
                MainCave.data.InitCave();
                DataCave.SaveData(MainCave.data);
                Cave.Log("保存洞府数据");
            };
            ui_plan.gameObject.AddComponent<UIFastClose>().onClose = onClose;
            ui_plan.btnOK.onClick.AddListener(onClose);

            root = CreateUI.NewImage();
            root.transform.SetParent(ui_plan.transform, false);
            var rtf = root.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0, 0);
            rtf.anchorMax = new Vector2(1, 1);
            rtf.sizeDelta = new Vector2(0, 0);
            rtf.pivot = new Vector2(0.5f, 0.5f);

            CreateBg();
            CreateBuilds();
            UpdateScroll();
            CreateTopButton();
        }

        public void CreateBg()
        {
            var bg = root.transform.Find("bg");
            if (bg != null)
            {
                GameObject.Destroy(bg.gameObject);
            }
            GameObject go = CreateUI.NewImage();
            go.transform.SetParent(root.transform, false);
            go.transform.SetAsFirstSibling();
            var rtf = go.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0, 0);
            rtf.anchorMax = new Vector2(1, 1);
            rtf.sizeDelta = new Vector2(0, 0);
            rtf.pivot = new Vector2(0.5f, 0.5f);
            go.name = "bg";
            go.GetComponent<Image>().color = Color.black;

            int idx = data.bg;
            if (idx > 3)
            {
                int id = idx - 4;
                int stand = id % 2;
                int level = id / 4;
                CreateBg2(stand, level);
            }
            else
            {
                CreateBg1(idx);
            }
            bg = root.transform.Find("bg");

            List<BgConfig> config = Cave.config.bgConfig;
            foreach (BgConfig item in config)
            {
                GameObject img = CreateUI.NewImage(CaveTool.LoadSprite("Mods/Cave/" + item.name, item.width, item.height));
                img.transform.SetParent(bg, false);
                RectTransform tf = img.GetComponent<RectTransform>();
                switch (item.anchor.Trim())
                {
                    case "L":
                        tf.anchorMin = new Vector2(0f, 0.5f);
                        tf.anchorMax = new Vector2(0f, 0.5f);
                        tf.pivot = new Vector2(0f, 0.5f);
                        break;
                    case "R":
                        tf.anchorMin = new Vector2(1f, 0.5f);
                        tf.anchorMax = new Vector2(15f, 0.5f);
                        tf.pivot = new Vector2(1f, 0.5f);
                        break;
                    case "T":
                        tf.anchorMin = new Vector2(0.5f, 1f);
                        tf.anchorMax = new Vector2(0.5f, 1f);
                        tf.pivot = new Vector2(0.5f, 1f);
                        break;
                    case "B":
                        tf.anchorMin = new Vector2(0.5f, 0f);
                        tf.anchorMax = new Vector2(0.5f, 0f);
                        tf.pivot = new Vector2(0.5f, 0f);
                        break;
                    case "LT":
                        tf.anchorMin = new Vector2(0f, 1f);
                        tf.anchorMax = new Vector2(0f, 1f);
                        tf.pivot = new Vector2(0f, 1f);
                        break;
                    case "LB":
                        tf.anchorMin = new Vector2(0f, 0f);
                        tf.anchorMax = new Vector2(0f, 0f);
                        tf.pivot = new Vector2(0f, 0f);
                        break;
                    case "RT":
                        tf.anchorMin = new Vector2(1f, 1f);
                        tf.anchorMax = new Vector2(1f, 1f);
                        tf.pivot = new Vector2(1f, 1f);
                        break;
                    case "RB":
                        tf.anchorMin = new Vector2(1f, 0f);
                        tf.anchorMax = new Vector2(1f, 0f);
                        tf.pivot = new Vector2(1f, 0f);
                        break;
                    default:
                        tf.anchorMin = new Vector2(0.5f, 0.5f);
                        tf.anchorMax = new Vector2(0.5f, 0.5f);
                        tf.pivot = new Vector2(0.5f, 0.5f);
                        break;
                }
                tf.sizeDelta = new Vector2(item.width, item.height);
                tf.anchoredPosition = new Vector2(item.x, item.y);
            }

            SaveData();
        }

        public void CreateBg1(int level)
        {
            var bg = root.transform.Find("bg");
            string[] path = new string[] { "chengzhenbg", "chengzhen3bg", "chengzhen2bg" };
            var image = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/" + path[level - 1]));
            image.transform.SetParent(bg, false);
            image.transform.SetAsFirstSibling();
            var rtf = image.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0, 0);
            rtf.anchorMax = new Vector2(1, 1);
            rtf.sizeDelta = new Vector2(0, 0);
            rtf.pivot = new Vector2(0.5f, 0.5f);
        }

        public void CreateBg2(int stand, int level)
        {
            var bg = root.transform.Find("bg");
            string path = "SchoolMainPage/" + (stand == 1 ? "zhengdao" : "modao") + "_" + (level + 1) + "/";
            var image = CreateUI.NewImage(SpriteTool.GetSpriteBigTex(path + "houjing"));
            image.transform.SetParent(bg, false);
            image.transform.SetAsFirstSibling();
            var rtf = image.GetComponent<RectTransform>();
            rtf.anchorMin = new Vector2(0, 0);
            rtf.anchorMax = new Vector2(1, 1);
            rtf.sizeDelta = new Vector2(0, 0);
            rtf.pivot = new Vector2(0.5f, 0.5f);

            var qianjing = CreateUI.NewImage(SpriteTool.GetSpriteBigTex(path + "qianjing"));
            qianjing.transform.SetParent(image.transform, false);
        }

        public void CreateTopButton()
        {
            Transform tfGo = root.transform.Find("top");
            if (tfGo != null)
            {
                GameObject.Destroy(tfGo.gameObject);
            }

            // 顶部按钮
            GameObject go = new GameObject("top");
            go.transform.SetParent(root.transform, false);
            go.transform.SetAsLastSibling();

            // 退出按钮
            GameObject tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tuichu"));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(865, 450);
            Action btnTuichu = () =>
            {
                g.ui.CloseUI(UIType.GamePlanTip);
            };
            tmpGo.AddComponent<Button>().onClick.AddListener(btnTuichu);
            tmpGo.name = "img_close";
            closeCanvasGroup = tmpGo.AddComponent<CanvasGroup>();

            SaveData();
        }

        public void UpdateScroll()
        {
            Transform tfGo = root.transform.Find("right");
            if (tfGo != null)
            {
                GameObject.Destroy(tfGo.gameObject);
            }

            // 右侧滚动
            GameObject right = new GameObject("right");
            right.transform.SetParent(root.transform, false);
            right.transform.SetSiblingIndex(2);
            panelCanvasGroup = right.AddComponent<CanvasGroup>();

            GameObject go = CreateUI.NewImage();
            go.transform.SetParent(right.transform, false);
            RectTransform goRtf = go.GetComponent<RectTransform>();
            goRtf.anchoredPosition = new Vector2(875, 0);
            goRtf.sizeDelta = new Vector2(450, 2000);
            go.GetComponent<Image>().color = new Color(1, 1, 1, 0f);


            float value = 3, speed = 5;
            bool enter = true;
            Tools.AddScale(go, 1, () =>
            {
                enter = true;
            }, () =>
            {
                enter = false;
            });
            TimerCoroutine hideCor = null;
            Action updatePanel = () =>
            {
                panelCanvasGroup.alpha = Mathf.Min(1,  value - 1);
                closeCanvasGroup.alpha = Mathf.Min(1, value - 1);
                goRtf.anchoredPosition = new Vector2(875 + (1 - Mathf.Max(0, value - 2)) * 250, 0);
            };
            Action hideAction = () =>
            {
                if (go == null)
                {
                    g.timer.Stop(hideCor);
                    return;
                }
                if (operateBuildIdx >= 0)
                {
                    if (right.activeSelf)
                    {
                        right.SetActive(false);
                    }
                    return;
                }
                if (!right.activeSelf)
                {
                    right.SetActive(true);
                }


                if (!enter && value != 0f)
                {
                    value -= Time.deltaTime * speed;
                    if (value < 0f)
                    {
                        value = 0f;
                    }
                    updatePanel();
                }
                else if (enter && value != 3)
                {
                    value += Time.deltaTime * speed;
                    if (value > 3)
                    {
                        value = 3;
                    }
                    updatePanel();
                }

            };
            hideCor = g.timer.Frame(hideAction, 1, true);


            GameObject tmpGo = null;
            string[] pageNames = new string[] { "网购", "资产", "伙伴", "记事" };
            // 页签按钮
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("Common", "daojuxuanxiangbg"));
                tmpGo.transform.SetParent(go.transform, false);
                tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-185 + (i * 61), idx == scrollPage ? 320 : 340);
                Action pageAction = () =>
                {
                    if (scrollPage == idx)
                    {
                        scrollPage = -1;
                    }
                    else
                    {
                        scrollPage = idx;
                    }
                    UpdateScroll();
                };
                tmpGo.AddComponent<Button>().onClick.AddListener(pageAction);
                Transform tmpParent = tmpGo.transform;


                tmpGo = CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                tmpGo.transform.SetParent(tmpParent, false);
                Text tmpText = tmpGo.GetComponent<Text>();
                tmpText.alignment = TextAnchor.MiddleCenter;
                tmpText.fontSize = 16;
                tmpText.text = pageNames[i];
            }

            if (scrollPage == -1)
                return;

            // 滚动背景
            tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("PlayerInfo", "daojukubg"));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 0);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 620);
            tmpGo.GetComponent<Image>().type = Image.Type.Tiled;

            // 滚动框
            tmpGo = CreateUI.NewScrollView(new Vector2(200, 600), spacing: new Vector2(0, 8));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 0);
            tmpGo.name = "scr_createBuilds";
            var tmpScroll = tmpGo.GetComponent<ScrollRect>();


            if (scrollPage == 0)
            {
                // 网购列表
                var list = ConfBuild.list;
                foreach (ConfBuildItem item in list)
                {
                    ConfBuildItem confBuild = item;
                    var caveItem = data.GetBuild(confBuild.id);
                    int count = caveItem == null ? 0 : data.GetBuildCount(confBuild.id);
                    if (count < item.maxCount)
                    {
                        UISkyTipEffect tipsObj = null;
                        tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("NPCInfoCommon", "daoxinmingzikuang"));
                        tmpGo.transform.SetParent(tmpScroll.content, false);
                        Action clickAction = () =>
                        {
                            if (count < 1)
                            {
                                Tips($"确认花费{confBuild.price}灵石购买{confBuild.GetName()}吗？", 2, () =>
                                {
                                    int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                                    if (money < confBuild.price)
                                    {
                                        Tips("灵石不足");
                                    }
                                    else
                                    {
                                        g.world.playerUnit.data.CostPropItem(PropsIDType.Money, confBuild.price);
                                        data.allBuilds.Add(new CaveBuildData(confBuild.id));
                                        scrollPage = 1;
                                        data.AddLog($"花费<color=#{CaveStateData.blud}>{confBuild.price}</color>灵石从仙居阁购买了<color=#{CaveStateData.blud}>1级{confBuild.GetName()}</color>。");
                                        UpdateScroll();
                                    }
                                });
                            }
                            else
                            {
                                int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                                if (money < confBuild.price)
                                {
                                    Tips("灵石不足");
                                }
                                else
                                {
                                    g.world.playerUnit.data.CostPropItem(PropsIDType.Money, confBuild.price);
                                    data.allBuilds.Add(new CaveBuildData(confBuild.id));
                                    data.AddLog($"花费<color=#{CaveStateData.blud}>{confBuild.price}</color>灵石从仙居阁购买了<color=#{CaveStateData.blud}>1级{confBuild.GetName()}</color>。");
                                    tipsObj.InitData($"<size=26><b>{item.GetName()}</b></size>\n价格：{item.price}灵石\n<color=#FF0000>点击购买</color>\n" + (string.IsNullOrWhiteSpace(item.des) ? "" : GameTool.LS(item.des)) +
                                        $"\n当前持有数量：{data.GetBuildCount(confBuild.id)}/{item.maxCount}");
                                    if (data.GetBuildCount(confBuild.id) >= item.maxCount)
                                    {
                                        scrollPage = 1;
                                        UpdateScroll();
                                    }

                                }
                            }
                        };
                        tmpGo.AddComponent<Button>().onClick.AddListener(clickAction);
                        Transform tmpParent = tmpGo.transform;

                        tmpGo = CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                        tmpGo.transform.SetParent(tmpParent, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        Text tmpText = tmpGo.GetComponent<Text>();
                        tmpText.alignment = TextAnchor.MiddleCenter;
                        tmpText.color = Color.black;
                        tmpText.text = GameTool.LS(confBuild.name);

                        tmpGo = CreateUI.NewImage(item.GetSprite());
                        tmpGo.transform.SetParent(tmpParent, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);
                        tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(38, 38);

                        tipsObj = tmpParent.gameObject.AddComponent<UISkyTipEffect>();
                        tipsObj.InitData($"<size=26><b>{item.GetName()}</b></size>\n价格：{item.price}灵石\n<color=#FF0000>点击购买</color>\n" + (string.IsNullOrWhiteSpace(item.des) ?"": GameTool.LS(item.des))+
                            $"\n当前持有数量：{count}/{item.maxCount}", offsetPos);
                    }
                }
            }
            else if (scrollPage == 1)
            {
                // 洞府资产
                for (int i = 0; i < data.allBuilds.Count; i++)
                {
                    CaveBuildData item = data.allBuilds[i];
                    int idx = i;
                    CaveBuildData caveItem = item;
                    if (ConfBuild.GetItem(caveItem.id) == null)
                        continue;
                    tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("NPCInfoCommon", "daoxinmingzikuang"));
                    tmpGo.transform.SetParent(tmpScroll.content, false);
                    Transform tmpParent = tmpGo.transform;
                    Action clickAction = () =>
                    {
                        caveItem.x = 0;
                        caveItem.y = 0;
                        caveItem.scale = 1;
                        caveItem.angle = 0;
                        caveItem.put = !caveItem.put;
                        if (caveItem.put)
                        {
                            operateBuildIdx = idx;
                        }
                        else
                        {
                            operateBuildIdx = -1;
                        }
                        CreateBuilds();
                        UpdateScroll();
                    };
                    tmpGo.AddComponent<Button>().onClick.AddListener(clickAction);

                    tmpGo = CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                    tmpGo.transform.SetParent(tmpParent, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    Text tmpText = tmpGo.GetComponent<Text>();
                    tmpText.alignment = TextAnchor.MiddleCenter;
                    tmpText.color = Color.black;
                    tmpText.text = caveItem.GetName();

                    tmpGo = CreateUI.NewImage(caveItem.GetSprite());
                    tmpGo.transform.SetParent(tmpParent, false);
                    tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);
                    tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(38, 38);

                    string state = caveItem.put ? "使用中" : "未使用";
                    string operate = caveItem.put ? "点击收回仓库\n（按下鼠标滚轮可重置坐标）" : "点击放置";
                    tmpParent.gameObject.AddComponent<UISkyTipEffect>().InitData(
                        $"<size=26><b>{caveItem.GetName()}</b></size>\n等级：{caveItem.level}\n升级需要{caveItem.GetUpLevelNeedMoney()}灵石\n当前状态：{state}\n<color=#FF0000>{operate}</color>\n" + (string.IsNullOrWhiteSpace(ConfBuild.GetItem(caveItem.id).des) ? "" : GameTool.LS(ConfBuild.GetItem(caveItem.id).des)), offsetPos);

                    if (caveItem.put)
                    {
                        tmpGo = CreateUI.NewText("", new Vector2(15, 60));
                        tmpGo.transform.SetParent(tmpParent, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(85, 0);
                        tmpText = tmpGo.GetComponent<Text>();
                        tmpText.alignment = TextAnchor.MiddleCenter;
                        tmpText.color = Color.red;
                        tmpText.text = "使用中";
                        tmpText.lineSpacing = 0.6f;
                        tmpText.fontSize = 12;
                    }
                    if (caveItem.level < ConfBuild.GetItem(caveItem.id).maxLevel)
                    {
                        tmpGo = CreateUI.NewImage(SpriteTool.GetSprite("TownCommon", "jiantou"));
                        tmpGo.transform.SetParent(tmpParent, false);
                        tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);
                        tmpParent = tmpGo.transform;
                        Action levelUp = () =>
                        {
                            var conf = ConfBuild.GetItem(caveItem.id);
                            int need = caveItem.GetUpLevelNeedMoney();
                            int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                            if (money < need)
                            {
                                Tips("灵石不足");
                            }
                            else
                            {
                                g.world.playerUnit.data.CostPropItem(PropsIDType.Money, need);
                                caveItem.level += 1;
                                data.AddLog($"花费<color=#{CaveStateData.blud}>{need}</color>灵石将<color=#{CaveStateData.blud}>{caveItem.GetName()}</color>升级至<color=#{CaveStateData.blud}>{caveItem.level}</color>级。");
                                CreateBuilds();
                                UpdateScroll();
                            }
                        };
                        tmpGo.AddComponent<Button>().onClick.AddListener(levelUp);


                        tmpGo = CreateUI.NewText("", new Vector2(15, 60));
                        tmpGo.transform.SetParent(tmpParent, false);
                        tmpText = tmpGo.GetComponent<Text>();
                        tmpText.alignment = TextAnchor.MiddleCenter;
                        tmpText.color = Color.black;
                        tmpText.text = "升级";
                        tmpText.lineSpacing = 0.6f;
                        tmpText.fontSize = 12;
                    }
                }
            }
            else if (scrollPage == 2)
            {
                // 洞府伙伴
                //Il2CppSystem.Collections.Generic.List<DataUnit.ArtifactSpriteData.Sprite> spriteList = g.world.playerUnit.data.unitData.artifactSpriteData.sprites;
                Il2CppSystem.Collections.Generic.List<WorldUnitBase> allKindred = g.world.unit.GetUnit(g.world.playerUnit.data.unitData.relationData.GetAllRelation());
                Il2CppSystem.Collections.Generic.List<WorldUnitBase> allFriends = g.world.playerUnit.data.unitData.relationData.GetIntimUnitData().friendUnits;

                if (headAtlas == null)
                {
                    headAtlas = new HeadAtlasTool(g.ui.GetUI(UIType.MapMain));
                    Action onCloseMapMain = () => { headAtlas = null; };
                }

                List<WorldUnitBase> list = new List<WorldUnitBase>();
                foreach (WorldUnitBase item in allKindred)
                {
                    list.Add(item);
                }
                foreach (WorldUnitBase item in allFriends)
                {
                    if (allKindred.Contains(item))
                        continue;
                    list.Add(item);
                }

                Action<GameObject, int> itemAction = (g, i) =>
                {
                    if (i >= 0 && i < list.Count)
                    {
                        UpdateUnitItem(g, list[i]);
                    }
                    else
                    {
                        Cave.Log("超索引 " + i + "/" + list.Count);
                    }
                };

                tmpScroll.verticalNormalizedPosition = 1;
                var goItem = CreateUnitItem();
                var bigDtaa = new BigDataScroll();
                bigDtaa.Init(tmpScroll, goItem, itemAction, goItem.GetComponent<RectTransform>().sizeDelta.y);
                bigDtaa.cellCount = list.Count;
            }
            else if (scrollPage == 3)
            {
                // 洞府记事
                foreach (string log in data.caveLog)
                {
                    tmpGo = CreateUI.NewText("", tmpGo.GetComponent<RectTransform>().sizeDelta);
                    tmpGo.transform.SetParent(tmpScroll.content, false);
                    tmpGo.transform.SetAsFirstSibling();
                    tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 0);
                    Text tmpText = tmpGo.GetComponent<Text>();
                    tmpText.alignment = TextAnchor.MiddleLeft;
                    tmpText.color = Color.black;
                    tmpText.text = log;
                    tmpGo.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
            }

            SaveData();
        }

        static GameObject unitItemPrefab;
        /* item ui结构
            imgHead : Image,UISkyTipEffect
            btnOperate : Button
                textState : Text
            textName : Text
            textRelation : Text
        */
        private GameObject CreateUnitItem()
        {
            if (unitItemPrefab != null)
                return unitItemPrefab;

            unitItemPrefab = CreateUI.NewImage(SpriteTool.GetSprite("Common", "qilingxuanzekuang1_1_1")); // 297 122
            unitItemPrefab.GetComponent<Image>().enabled = false;
            unitItemPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 122);
            unitItemPrefab.name = "item";

            // npc单位
            GameObject tmpGo = CreateUI.NewRawImage();
            tmpGo.name = "imgHead";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(15, 0);
            tmpGo.AddComponent<UISkyTipEffect>();

            tmpGo = CreateUI.NewButton(null, SpriteTool.GetSprite("SchoolCommon", "jubaoanniu_3"));
            tmpGo.name = "btnOperate";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 20);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 56);
            Transform tmpParent = tmpGo.transform;

            tmpGo = CreateUI.NewText("", new Vector2(18, 46));
            tmpGo.name = "textState";
            tmpGo.transform.SetParent(tmpParent, false);
            Text tmpText2 = tmpGo.GetComponent<Text>();
            tmpText2.alignment = TextAnchor.MiddleCenter;
            tmpText2.lineSpacing = 0.65f;
            tmpText2.fontSize = 16;

            tmpGo = CreateUI.NewText("", new Vector2(25, 100));
            tmpGo.name = "textName";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-90, 0);
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 22;
            tmpText.color = Color.black;
            tmpText.lineSpacing = 0.75f;


            tmpGo = CreateUI.NewText("", new Vector2(25, 100));
            tmpGo.name = "textRelation";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -15);
            tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 15;
            tmpText.color = Color.black;
            tmpText.lineSpacing = 0.7f;
            tmpText.fontStyle = FontStyle.Italic;

            return unitItemPrefab;
        }

        private void UpdateUnitItem(GameObject go, WorldUnitBase unit)
        {
            try
            {
                Transform root = go.transform;
                // npc单位
                string name = unit.data.unitData.propertyData.GetName();
                string relation = g.conf.npcPartFitting.GetPartName(unit.data.unitData.unitID, g.world.playerUnit);
                int state = data.GetNpcIntoState(unit.data.unitData.unitID);
                string stateStr;
                if (state == 2)
                {
                    if (data.GetPoint() == unit.data.unitData.GetPoint())
                    {
                        stateStr = "已入住（洞府中）";
                    }
                    else
                    {
                        stateStr = "已入住（外出中）";
                    }
                }
                else if (state == 1)
                {
                    stateStr = "邀请中";
                }
                else
                {
                    stateStr = "未入住";
                }
                Cave.Log("获取ui：" + state, 999);
                var imgHead = root.Find("imgHead").GetComponent<RawImage>();
                Cave.Log("imgHead：" + imgHead, 999);
                var btnOperate = root.Find("btnOperate").GetComponent<Button>();
                Cave.Log("btnOperate：" + btnOperate, 999);
                var textState = root.Find("btnOperate/textState").GetComponent<Text>();
                Cave.Log("textState：" + textState, 999);
                var textName = root.Find("textName").GetComponent<Text>();
                Cave.Log("textName：" + textName, 999);
                var textRelation = root.Find("textRelation").GetComponent<Text>();
                Cave.Log("textRelation：" + textRelation, 999);
                imgHead.GetComponent<UISkyTipEffect>().InitData($"<size=22>{name}</size>({relation})\n状态：{stateStr}", offsetPos);
                Cave.Log("设置滑入信息：" + $"<size=22>{name}</size>({relation})\n状态：{stateStr}", 999);
                PortraitModel.CreateTexture(unit, imgHead, new Vector2(0, -21.5f), 3f, true);
                Cave.Log("设置画像：", 999);

                if (state != 1)
                {
                    Action updateText = () =>
                    {
                        if (data.GetNpcIntoState(unit.data.unitData.unitID) != 1)
                        {
                            btnOperate.GetComponent<Image>().enabled = true;
                            textState.text = data.GetNpcIntoState(unit.data.unitData.unitID) == 0 ? "邀请" : "请离";
                            textState.color = data.GetNpcIntoState(unit.data.unitData.unitID) == 0 ? Color.black : Color.red;
                        }
                        else
                        {
                            btnOperate.GetComponent<Image>().enabled = false;
                            textState.text = "邀请中";
                            textState.color = Color.red;
                        }
                    };
                    updateText();

                    Action levelUp = () =>
                    {
                        data.into++;
                        Action sendAction = () =>
                        {
                            if (data.GetNpcIntoState(unit.data.unitData.unitID) == 0)
                            {
                                Cave.Log("入住数：" + data.npcDatas.Count + "     客房数：" + data.GetBuildLevel(4004));
                                if (data.npcDatas.Count < data.GetBuildLevel(4004))
                                {
                                    UITipItem.AddTip($"已经向<color=#{CaveStateData.blud}>{name}</color>发送邀请！");
                                    data.SetNpcIntoState(unit.data.unitData.unitID, 1);
                                    data.AddLog($"<color=#{CaveStateData.blud}>{g.world.playerUnit.data.unitData.propertyData.GetName()}</color>向<color=#{CaveStateData.blud}>{name}</color>发送飞剑传书，邀请其入住<color=#{CaveStateData.blud}>{data.name}</color>。");
                                    Cave.Log("入住数：" + data.npcDatas.Count + "     客房数：" + data.GetBuildLevel(4004));
                                    updateText();
                                }
                                else
                                {
                                    Tips($"酒馆入住人数已满，请升级酒馆！", 1);
                                }
                            }
                            else if (data.GetNpcIntoState(unit.data.unitData.unitID) == 2)
                            {
                                Tips($"确认将<color=#{CaveStateData.blud}>{name}</color>请离出洞府吗？", 2, () =>
                                {
                                    UITipItem.AddTip($"已经将<color=#{CaveStateData.blud}>{name}</color>请离出洞府！");
                                    data.SetNpcIntoState(unit.data.unitData.unitID, 0);
                                    data.AddLog($"<color=#{CaveStateData.blud}>{g.world.playerUnit.data.unitData.propertyData.GetName()}</color>将<color=red</color>请离了<color=#{CaveStateData.blud}>{data.name}</color>。");
                                    unit.data.unitData.relationData.AddHate(g.world.playerUnit.data.unitData.unitID, CommonTool.Random(40, 150));
                                    Cave.Log("入住数：" + data.npcDatas.Count + "     客房数：" + data.GetBuildLevel(2009));
                                    updateText();
                                });
                            }
                            else
                            {
                                //intoGo.SetActive(false);
                            }
                        };
                        if (data.into == 0)
                        {
                            Tips(HelpMe.caveDes1 + "<color=red>(该说明只出现一次，后续需要阅读可到更新公告【2.0抢鲜体验版】查看)</color>", 2, () =>
                            {
                                sendAction();
                            });
                        }
                        else
                        {
                            sendAction();
                        }
                        SaveData();
                    };
                    btnOperate.onClick.RemoveAllListeners();
                    btnOperate.onClick.AddListener(levelUp);
                }
                else
                {
                    btnOperate.GetComponent<Image>().enabled = false;
                    textState.text = "邀请中";
                    textState.color = Color.red;
                }
                textName.text = name;
                textRelation.text = relation;
            }
            catch (Exception e)
            {
                Cave.Log("创建单位Item失败：\n" + e.Message + "\n" + e.StackTrace);
            }
        }


        public void CreateBuilds()
        {
            var bg = root.transform.Find("buildRoot");
            if (bg != null)
            {
                GameObject.Destroy(bg.gameObject);
            }

            GameObject go = CreateUI.NewImage();
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            go.name = "buildRoot";
            go.transform.SetParent(root.transform, false);
            go.transform.SetSiblingIndex(1);
            go.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            Transform parent = go.transform;

            List<RectTransform> allBuilds = new List<RectTransform>();
            RectTransform operateBuildRtf = null;
            Action sortBuilds = () =>
            {
                allBuilds.Sort((a, b) =>
                {
                    if (a == operateBuildRtf)
                        return 1;
                    if (b == operateBuildRtf)
                        return -1;
                    if (a.anchoredPosition.y == b.anchoredPosition.y)
                        return 0;
                    return a.anchoredPosition.y < b.anchoredPosition.y ? 1 : -1;
                });
                int idx = 0;
                for (int i = 0; i < allBuilds.Count; i++)
                {
                    allBuilds[i].SetSiblingIndex(idx);
                    idx++;
                }
            };

            for (int i = 0; i < data.allBuilds.Count; i++)
            {
                CaveBuildData item = data.allBuilds[i];
                int idx = i;
                CaveBuildData caveItem = item;
                var conf = ConfBuild.GetItem(caveItem.id);
                if (conf == null)
                    continue;
                if (caveItem.level > 0 && caveItem.put)
                {
                    BuildConfig config = caveItem.GetConfig();
                    GameObject buildImg = CreateUI.NewImage();
                    createItem = caveItem;
                    createItemObj = buildImg;
                    buildImg.transform.SetParent(parent, false);
                    if (conf.effect != null)
                    {
                        foreach (var effectPath in conf.effect)
                        {
                            var effectRoot = CreateUI.New();
                            effectRoot.transform.SetParent(buildImg.transform, false);
                            effectRoot.GetComponent<RectTransform>().sizeDelta = buildImg.GetComponent<RectTransform>().sizeDelta;
                            var effectGo = Cave.CreateGo(effectPath, effectRoot.transform, effectRoot.GetComponentInParent<Canvas>().sortingOrder + 1);
                            GameEffectTool.SetEffectOutsideMask(effectGo);
                        }
                    }
                    if (config != null)
                    {
                        buildImg.GetComponent<RectTransform>().sizeDelta = new Vector2(config.width, config.height);
                    }
                    RectTransform buildRtf = buildImg.GetComponent<RectTransform>();
                    buildRtf.pivot = new Vector2(0.5f, 0f);
                    buildRtf.anchoredPosition = new Vector2(caveItem.x, caveItem.y);
                    buildRtf.localEulerAngles = new Vector3(0, 0, caveItem.angle);
                    buildRtf.localScale = Vector3.one * caveItem.scale;
                    TimerCoroutine cor = UpdateBuildImage(buildRtf.gameObject, caveItem);
                    allBuilds.Add(buildRtf);

                    if (!conf.hideName) {
                        // 名字
                        Vector2 namePos = new Vector2(-buildImg.GetComponent<RectTransform>().sizeDelta.x / 2 + 10, 10.2f);
                        var ui_namebg = CreateUI.NewImage(SpriteTool.GetSprite("Common", "jianzhumingzibg"));
                        ui_namebg.transform.SetParent(buildImg.transform, false);
                        ui_namebg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = namePos;
                        ui_namebg.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(30, 101);

                        var ui_name = CreateUI.NewText(caveItem.GetName() + (conf.maxLevel <= 1 ? "" : "\n<size=14>" + caveItem.level + "级</size>"), fontID: 1);
                        ui_name.transform.SetParent(ui_namebg.transform, false);
                        ui_name.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                        ui_name.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
                        ui_name.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
                        ui_name.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new Vector2(0, -3);
                        ui_name.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(30, 101);
                        ui_name.GetComponent<UnityEngine.UI.Text>().lineSpacing = 0.65f;
                        ui_name.GetComponent<UnityEngine.UI.Text>().fontSize = 25;
                        ui_name.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
                        ContentSizeFitter sizeFitter = ui_name.AddComponent<ContentSizeFitter>();
                        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                        UITargetSizeDelta sizeDelta = ui_namebg.AddComponent<UITargetSizeDelta>();
                        sizeDelta.rect = ui_name.GetComponent<RectTransform>();
                        sizeDelta.offsetSize = new Vector2Int(0, 40);
                    }

                    if (operateBuildIdx == idx)
                    {
                        operateBuildRtf = buildRtf;
                        var ui_operate = CreateUI.NewImage(SpriteTool.GetSprite("TownCommon", "huosheng_2"));
                        ui_operate.transform.SetParent(buildImg.transform, false);
                        ui_operate.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(0, 0);
                        ui_operate.GetComponent<UnityEngine.RectTransform>().sizeDelta = buildImg.GetComponent<RectTransform>().sizeDelta;
                        Image img_operate = ui_operate.GetComponent<Image>();
                        ui_operate.name = "ui_operate";

                        int clickType = 0;
                        Vector2 lastPosition = default;
                        Action<UnityEngine.EventSystems.PointerEventData> onBeginDrag = (data) =>
                        {
                            lastPosition = data.position;
                            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                            {
                                clickType = 2;
                            }
                            else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt))
                            {
                                clickType = 1;
                            }
                            else
                            {
                                clickType = 0;
                            }
                        };

                        var ui_data = CreateUI.NewText(string.Format("位置{0:F},{1:F}\n旋转{2:F}\n缩放{3:F}", caveItem.x, caveItem.y, caveItem.angle, caveItem.scale));
                        ui_data.transform.SetParent(buildImg.transform, false);
                        ui_data.GetComponent<Text>().color = Color.red;
                        ui_data.GetComponent<Text>().fontSize = 16;
                        ui_data.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-62 - buildImg.GetComponent<RectTransform>().sizeDelta.x, 40);
                        ui_data.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(124, 80);
                        var text_data = ui_data.GetComponent<Text>();

                        Action<UnityEngine.EventSystems.PointerEventData> onDrag = (data) =>
                        {
                            Vector2 position = data.position;
                            float offset = (position.y - lastPosition.y) + (position.x - lastPosition.x);
                            if (clickType == 0) // 移动
                            {
                                position.x -= Screen.width / 2;
                                position.y -= Screen.height / 2;
                                buildRtf.anchoredPosition = position;
                                caveItem.x = position.x;
                                caveItem.y = position.y;
                                sortBuilds();
                            }
                            else if(clickType == 1) // 旋转
                            {
                                float angle = (caveItem.angle + offset * 0.1f) % 360;
                                caveItem.angle = angle;
                                buildRtf.localEulerAngles = new Vector3(0, 0, caveItem.angle);
                            }
                            else if (clickType == 2) // 缩放
                            {
                                float scale = caveItem.scale + offset * 0.002f;
                                caveItem.scale = scale;
                                buildRtf.localScale = Vector3.one * caveItem.scale;
                            }
                            lastPosition = position;
                            text_data.text = string.Format("位置{0:F},{1}\n旋转{2:F}\n缩放{3:F}", caveItem.x, caveItem.y, caveItem.angle, caveItem.scale);
                        };
                        ui_operate.AddComponent<UIEventListenerDrag>().onDragCall = onDrag;
                        ui_operate.AddComponent<UIEventListenerDrag>().onBeginDragCall2 = onBeginDrag;

                        TimerCoroutine timer = null;
                        Action doFram = () =>
                        {
                            if (ui_operate == null)
                            {
                                g.timer.Stop(timer);
                                return;
                            }
                            float v = Time.time - (int)Time.time;
                            if (v < 0.5f)
                            {
                                v = v / 0.5f;
                            }
                            else
                            {
                                v = (1 - v) / 0.5f;
                            }
                            img_operate.color = new Color(1, 1, 1, v);
                        };
                        timer = g.timer.Frame(doFram, 1, true);
                        ui_plan.AddCor(timer);

                        var ui_ok = CreateUI.NewImage(SpriteTool.GetSprite("Common", "cost"));
                        ui_ok.transform.SetParent(buildImg.transform, false);
                        ui_ok.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(buildImg.GetComponent<RectTransform>().sizeDelta.x / 2 + 10, buildImg.GetComponent<RectTransform>().sizeDelta.y / 2);
                        ui_ok.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(20, 20);
                        Action okAction = () =>
                        {
                            operateBuildIdx = -1;
                            CreateBuilds();
                        };
                        ui_ok.AddComponent<Button>().onClick.AddListener(okAction);
                        ui_ok.gameObject.AddComponent<UISkyTipEffect>().InitData("点击确定位置", offsetPos2);

                        var ui_okIcon = CreateUI.NewImage(SpriteTool.GetSprite("Common", "gou"));
                        ui_okIcon.transform.SetParent(ui_ok.transform, false);

                        var ui_no = CreateUI.NewImage(SpriteTool.GetSprite("Common", "cost"));
                        ui_no.transform.SetParent(buildImg.transform, false);
                        ui_no.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(buildImg.GetComponent<RectTransform>().sizeDelta.x / 2 + 10, buildImg.GetComponent<RectTransform>().sizeDelta.y / 2 - 45);
                        ui_no.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(20, 20);
                        Action noAction = () =>
                        {
                            caveItem.put = false;
                            CreateBuilds();
                            UpdateScroll();
                        };
                        ui_no.AddComponent<Button>().onClick.AddListener(noAction);
                        ui_no.gameObject.AddComponent<UISkyTipEffect>().InitData("点击放回仓库位置", offsetPos2);

                        var ui_noIcon = CreateUI.NewImage(SpriteTool.GetSprite("Common", "cha3"));
                        ui_noIcon.transform.SetParent(ui_no.transform, false);
                        ui_noIcon.GetComponent<Image>().color = Color.red;



                        var ui_icon = CreateUI.NewImage(SpriteTool.GetSprite("Common", "cost"));
                        ui_icon.transform.SetParent(buildImg.transform, false);
                        ui_icon.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(buildImg.GetComponent<RectTransform>().sizeDelta.x / 2 + 10, buildImg.GetComponent<RectTransform>().sizeDelta.y / 2 - 90);
                        ui_icon.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(20, 20);
                        Action iconAction = () =>
                        {
                            CallQueue cq = new CallQueue();
                            string path = "";
                            Action cq1 = () =>
                            {
                                path = OpenFileDialog.OpenFile("图片文件(*.jpg,*.png,*.bmp)|*.jpg;*.png;*.bmp");
                                cq.Next();
                            };
                            Action cq2 = () =>
                            {
                                ImageManager.BuildGif(path, caveItem.id);
                                cq.Next();
                            };
                            Action cq3 = () =>
                            {
                                g.timer.Stop(cor);
                                cor =  UpdateBuildImage(buildRtf.gameObject, caveItem);
                            };

                            cq.Add(cq1);
                            cq.Add(cq2);
                            cq.Add(cq3);
                            cq.Next();
                        };
                        ui_icon.AddComponent<Button>().onClick.AddListener(iconAction);
                        ui_icon.gameObject.AddComponent<UISkyTipEffect>().InitData("点击更换建筑图片", offsetPos2);

                        var ui_noIconIcon = CreateUI.NewImage(SpriteTool.GetSprite("CommonPropIcon", "zhupian"));
                        ui_noIconIcon.transform.SetParent(ui_icon.transform, false);
                        ui_noIconIcon.GetComponent<Image>().color = Color.red;
                        ui_noIconIcon.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(20, 20);


                        var ui_des = CreateUI.NewText("鼠标拖动调整位置，按住ctrl+鼠标拖动调整大小，按住alt+鼠标拖动调整角度");
                        ui_des.transform.SetParent(buildImg.transform, false);
                        ui_des.GetComponent<Text>().color = Color.red;
                        ui_des.GetComponent<Text>().fontSize = 16;
                        ui_des.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        ui_des.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(0, -10 - buildImg.GetComponent<RectTransform>().sizeDelta.y / 2);
                        ui_des.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(800, 20);
                        ui_des.GetComponent<Text>().raycastTarget = false;

                    }
                    else
                    {
                        GameObject clickRoot = CreateUI.NewImage();
                        clickRoot.transform.SetParent(buildImg.transform, false);
                        clickRoot.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        clickRoot.GetComponent<RectTransform>().sizeDelta = buildRtf.sizeDelta;

                        Tools.AddScale(buildImg, baseScale: caveItem.scale, enter: () =>
                         {
                             CaveBuildOpen.DoFunction(conf.enterfuntion);
                         }, exit: () =>
                         {
                             CaveBuildOpen.DoFunction(conf.exitfuntion);
                         }, triggerObj: clickRoot);
                        Action clickBuildAction = () =>
                        {
                            SaveData();
                            new CaveBuildOpen(this, caveItem);
                        };
                        clickRoot.AddComponent<Button>().onClick.AddListener(clickBuildAction);
                    }

                    DataCustom.CaveNameCustom nameCustom = DataCustom.GetCaveNameCustom(caveItem.id);
                    if (nameCustom != null) // 洞府名字
                    {
                        string connect = string.IsNullOrWhiteSpace(caveItem.param) ? data.name : caveItem.param;
                        int fontSize = nameCustom.custom.size;
                        bool shu = buildRtf.sizeDelta.y > buildRtf.sizeDelta.x; //是否竖向显示
                        if (nameCustom.custom.direction != 0) {
                            shu = nameCustom.custom.direction == 2;
                        }
                        fontSize = fontSize * 5;
                        Vector2 size = new Vector2(shu ? fontSize * 1.5f : 10000, shu ? 10000 : fontSize * 1.5f);
                        GameObject nameObj = CreateUI.NewText(connect, size, nameCustom.custom.type);
                        nameObj.transform.SetParent(buildImg.transform, false);
                        nameObj.transform.localScale = Vector3.one / 5;

                        Text tmpText = nameObj.GetComponent<Text>();
                        tmpText.alignment = TextAnchor.MiddleCenter;
                        tmpText.raycastTarget = false;
                        tmpText.lineSpacing = 0.8f;

                        // 自定义数据
                        tmpText.fontSize = fontSize;
                        tmpText.color = new Color(nameCustom.custom.color.r/255f, nameCustom.custom.color.g / 255f, nameCustom.custom.color.b / 255f, nameCustom.custom.color.a / 255f);
                        nameObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(nameCustom.custom.position.x, nameCustom.custom.position.y);
                        if (nameCustom.custom.outline.color.a > 0) {
                            Outline outLine = nameObj.AddComponent<Outline>();
                            outLine.effectColor = new Color(nameCustom.custom.outline.color.r / 255f, nameCustom.custom.outline.color.g / 255f, nameCustom.custom.outline.color.b / 255f, nameCustom.custom.outline.color.a / 255f);
                            outLine.effectDistance = new Vector2(nameCustom.custom.outline.distance.x, nameCustom.custom.outline.distance.y);
                        }
                    }
                    // 调用创建建筑结束代码

                    if (operateBuildIdx == idx)
                    {

                    }
                    else
                    {

                        CaveBuildOpen.DoFunction(conf.createfuntion);
                    }

                }
            }

            sortBuilds();

            SaveData();
        }

        private TimerCoroutine UpdateBuildImage(GameObject go, CaveBuildData item)
        {
            var image = go.GetComponent<Image>();
            if (image == null)
            {
                image = go.AddComponent<Image>();
            }
            TimerCoroutine cor = ImageManager.LoadGif(image, "Mods/Cave/" + item.id);
            if (image.sprite == null)
            {
                image.sprite = item.GetSprite();
                image.SetNativeSize();
            }
            return cor;
        }

        public void Tips(string connect, int count = 1, Action call = null)
        {
            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), connect, count, call);
        }

        public static void Close()
        {
            var ui = g.ui.GetUI(UIType.GamePlanTip);
            if (ui != null)
            {
                g.ui.CloseUI(UIType.GamePlanTip);
            }
        }
    }
}