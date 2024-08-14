
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Cave
{
    public class UIBarrierList
    {
        static GameObject itemPrefab;
        public GameObject bg;
        public Action<GameObject, int> clickCall;

        bool isEnter;
        bool isDrag;
        Vector3 clickOffset;
        public static Vector3 pos;

        GameObject bg2;
        GameObject goItem;
        GuiBaseUI.BigDataScroll bigDtaa;
        GuiBaseUI.ItemCell itemCell;
        ScrollRect mainScroll;
        List<int> loveBarrier = new List<int>();
        List<int> showBarrierItem = new List<int>();
        List<int> allBarrierItem;
        GameObject curTypeBtn;
        List<List<int>> typList = new List<List<int>>();
        List<string> classList = new List<string>()
        {
            "101-126,10001,10002,601-606,1401-1421,1801-1802,1901-1911,2001-2009,2101-2137,5101-5121",
            "401-426,901-917,1701-1717,2201-2231",
            "301-341,201-230,501-522,801-828,2301-2323,4101-4112",
            "701-719,4601-4624,1001-1022,1301-1310,1504-1507,1602,1603,3901-3942"
        };
        static int showListType = 0;

        public UIBarrierList(Transform parent, List<int> allBarrierItem)
        {
            this.allBarrierItem = allBarrierItem;
            string strLoveBarrier = g.data.obj.GetString("Cave", "LoveBarrier");
            try
            {
                if (string.IsNullOrEmpty(strLoveBarrier))
                {
                    loveBarrier = new List<int>();
                }
                else
                {
                    loveBarrier = JsonConvert.DeserializeObject<List<int>>(strLoveBarrier);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                loveBarrier = new List<int>();
            }

            if (pos == default)
            {
                pos = new Vector2(-580, 0);
            }
            try
            {
                // 区域背景
                bg = GuiBaseUI.CreateUI.NewImage();
                bg.transform.SetParent(parent, false);
                bg.GetComponent<RectTransform>().anchoredPosition = pos;
                bg.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 650);

                // 加个标题
                GameObject title = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "jingjiebg"));
                title.transform.SetParent(bg.transform, false);
                title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 310);
                title.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 30);

                var ui_event = title.gameObject.AddComponent<UIEventListener>();
                Action onEnter = () =>
                {
                    isEnter = true;
                };
                Action onExit = () =>
                {
                    isEnter = false;
                };
                ui_event.onMouseEnter.AddListener(onEnter);
                ui_event.onMouseExit.AddListener(onExit);
                TimerCoroutine cor = null;
                cor = SceneType.battle.timer.Frame(new Action(() =>
                {
                    try
                    {
                        if (bg == null)
                        {
                            SceneType.battle.timer.Stop(cor);
                            return;
                        }
                        if (isEnter && Input.GetMouseButtonDown(0))
                        {
                            isDrag = true;
                            Vector3 p = g.ui.uiCamera.ScreenToWorldPoint(Input.mousePosition);
                            clickOffset = p - bg.transform.position;
                        }
                        if (isDrag && Input.GetMouseButtonUp(0))
                        {
                            isDrag = false;
                        }
                        if (isDrag)
                        {
                            Vector3 p = g.ui.uiCamera.ScreenToWorldPoint(Input.mousePosition);
                            bg.transform.position = p - clickOffset;
                            pos = bg.GetComponent<RectTransform>().anchoredPosition;
                        }
                    }
                    catch (Exception e)
                    {
                        Cave.LogWarning("UIBarrierList:" + e.Message + "\n" + e.StackTrace);
                        Debug.LogError("UIBarrierList:" + e.Message + "\n" + e.StackTrace);
                    }
                }), 1, true);

                GameObject titleText = GuiBaseUI.CreateUI.NewText("购买装饰", new Vector2(220, 30));
                titleText.transform.SetParent(title.transform, false);
                Text tmpText = titleText.GetComponent<Text>();
                tmpText.alignment = TextAnchor.MiddleCenter;
                tmpText.color = Color.black;

                // 加个底显示灵石
                GameObject di = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "jingjiebg"));
                di.transform.SetParent(bg.transform, false);
                di.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -360);
                di.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 30);
                ShowMoney(di.transform);


                //// 窗口模式 showListType  Common/kuafujiankang,kuafujingli,kuafutili
                //var goListType = GuiBaseUI.CreateUI.NewButton(() =>
                //{
                //    showListType = (showListType + 1) % 2;
                //    UpdateShow(curTypeBtn);
                //}, SpriteTool.GetSprite("Common", "jingjiebg"));


                // 创建预制项
                goItem = CreateUnitItem();


                // 分类
                float width = 220f / (classList.Count + 2);
                float typPos = (-220f + width) / 2;

                Dictionary<GameObject, string> btnTyps = new Dictionary<GameObject, string>();

                GameObject goBtnAll = null, goBtnLove = null;
                goBtnAll = GuiBaseUI.CreateUI.NewButton(() =>
                {
                    showBarrierItem = allBarrierItem;
                    UpdateShow(goBtnAll);
                }, SpriteTool.GetSprite("Common", "jingjiebg"));
                goBtnAll.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                goBtnAll.transform.SetParent(bg.transform, false);
                goBtnAll.GetComponent<RectTransform>().anchoredPosition = new Vector2(typPos, 285);
                goBtnAll.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);
                goBtnAll.AddComponent<UISkyTipEffect>().InitData("全部装饰");
                btnTyps.Add(goBtnAll, "全部");
                typPos += width;

                goBtnLove = GuiBaseUI.CreateUI.NewButton(() =>
                {
                    showBarrierItem = loveBarrier;
                    UpdateShow(goBtnLove);
                }, SpriteTool.GetSprite("Common", "jingjiebg"));
                goBtnLove.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                goBtnLove.transform.SetParent(bg.transform, false);
                goBtnLove.GetComponent<RectTransform>().anchoredPosition = new Vector2(typPos, 285);
                goBtnLove.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);
                goBtnLove.AddComponent<UISkyTipEffect>().InitData("喜欢的装饰");
                btnTyps.Add(goBtnLove, "收藏");
                typPos += width;


                for (int i = 0; i < classList.Count; i++)
                {
                    var list = new List<int>();
                    typList.Add(list);
                    var cc = classList[i].Split(',');
                    foreach (var item in cc)
                    {
                        var ids = item.Split('-');
                        if (ids.Length < 2)
                        {
                            var id = int.Parse(ids[0]);
                            if (allBarrierItem.Contains(id))
                            {
                                list.Add(id);
                            }
                        }
                        else
                        {
                            for (int j = int.Parse(ids[0]); j <= int.Parse(ids[1]); j++)
                            {
                                var id = j;
                                if (allBarrierItem.Contains(id))
                                {
                                    list.Add(id);
                                }
                            }
                        }
                    }
                    GameObject goBtnType = null;
                    goBtnType = GuiBaseUI.CreateUI.NewButton(() =>
                    {
                        showBarrierItem = list;
                        UpdateShow(goBtnType);
                    }, SpriteTool.GetSprite("Common", "jingjiebg"));
                    goBtnType.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                    goBtnType.transform.SetParent(bg.transform, false);
                    goBtnType.GetComponent<RectTransform>().anchoredPosition = new Vector2(typPos, 285);
                    goBtnType.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);
                    goBtnType.AddComponent<UISkyTipEffect>().InitData("分类" + (i + 1));
                    btnTyps.Add(goBtnType, (i + 1)+ "类");
                    typPos += width;
                }

                foreach (var item in btnTyps)
                {
                    var btnTf = item.Key.GetComponent<RectTransform>();
                    GameObject typText = GuiBaseUI.CreateUI.NewText(item.Value, btnTf.sizeDelta);
                    typText.transform.SetParent(btnTf, false);
                    Text btnText = typText.GetComponent<Text>();
                    btnText.alignment = TextAnchor.MiddleCenter;
                    btnText.color = Color.black;
                    btnText.fontSize = 16;
                }

                showBarrierItem = allBarrierItem;
                UpdateShow(goBtnAll);
            }
            catch (Exception e)
            {
                if (bg == null)
                {
                    bg = new GameObject();
                }
                Cave.LogError(e.Message + "\n" + e.StackTrace);
            }
        }

        private void UpdateShow(GameObject clickBtn)
        {
            if (curTypeBtn != null)
            {
                curTypeBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            curTypeBtn = clickBtn;
            curTypeBtn.GetComponent<Image>().color = new Color(1, 0.5f, 0.5f, 1f);

            if (bg2)
            {
                GameObject.Destroy(bg2);
            }
            if (showListType == 0)
            {
            }
            else
            {
            }
            // 滚动背景
            bg2 = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("PlayerInfo", "daojukubg"));
            bg2.transform.SetParent(bg.transform, false);
            bg2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -35);
            bg2.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 620);
            bg2.GetComponent<Image>().type = Image.Type.Tiled;

            // 滚动框
            var tmpGo = GuiBaseUI.CreateUI.NewScrollView(new Vector2(200, 600), spacing: new Vector2(0, 8));
            tmpGo.transform.SetParent(bg2.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            tmpGo.name = "scr_createBuilds";
            mainScroll = tmpGo.GetComponent<ScrollRect>();

            if (showBarrierItem.Count > 0)
            {
                bigDtaa = new GuiBaseUI.BigDataScroll();
                itemCell = goItem.AddComponent<GuiBaseUI.ItemCell>();
                Action<GuiBaseUI.ItemCell, int> itemAction = (cellItem, i) =>
                {
                    int idx = i - 1;
                    if (idx >= 0 && idx < showBarrierItem.Count)
                    {
                        cellItem.gameObject.SetActive(true);
                        var id = showBarrierItem[idx];
                        UpdateUnitItem(cellItem.gameObject, id, idx);
                    }
                    else
                    {
                        cellItem.gameObject.SetActive(false);
                    }
                };
                bigDtaa.Init(mainScroll, itemCell, itemAction, goItem.GetComponent<RectTransform>().sizeDelta.y);
                bigDtaa.cellHeight = 122;
                bigDtaa.cellCount = showBarrierItem.Count;
            mainScroll.verticalNormalizedPosition = 0.9999f;
            }
        }

        public void SaveLove()
        {
            string strLoveBarrier = JsonConvert.SerializeObject(loveBarrier);
            g.data.obj.SetString("Cave", "LoveBarrier", strLoveBarrier);
        }

        private void ShowMoney(Transform parent)
        {

            GameObject go = GuiBaseUI.CreateUI.New();
            go.name = "vlg";
            go.transform.SetParent(parent.transform, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 20);
            var vlg = go.AddComponent<HorizontalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childScaleHeight = false;
            vlg.childScaleWidth = false;

            var tmpGo = GuiBaseUI.CreateUI.NewText("拥有灵石：", new Vector2(100, 20));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 18;
            tmpText.color = Color.black;

            tmpGo = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "lingshi2"));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.GetComponent<Image>().SetNativeSize();

            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(100, 20));
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text moneyText = tmpGo.GetComponent<Text>();
            moneyText.alignment = TextAnchor.MiddleCenter;
            moneyText.fontSize = 18;
            moneyText.color = Color.black;

            TimerCoroutine cor = null;
            cor = SceneType.battle.timer.Frame(new Action(() =>
            {
                if (moneyText == null)
                {
                    SceneType.battle.timer.Stop(cor);
                    return;
                }
                moneyText.text = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money).ToString();
            }), 1, true);
        }

        private GameObject CreateUnitItem()
        {
            if (itemPrefab != null)
                return itemPrefab;

            itemPrefab = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "qilingxuanzekuang1_1_1")); // 297 122
            itemPrefab.GetComponent<Image>().enabled = false;
            itemPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 140);
            itemPrefab.name = "item";

            // 主图
            GameObject tmpGo = GuiBaseUI.CreateUI.NewImage();
            tmpGo.name = "img";
            tmpGo.transform.SetParent(itemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
            // 灵石
            GameObject go = GuiBaseUI.CreateUI.New();
            go.name = "vlg";
            go.transform.SetParent(itemPrefab.transform, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -58);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 20);
            var vlg = go.AddComponent<HorizontalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childScaleHeight = false;
            vlg.childScaleWidth = false;

            tmpGo = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "lingshi2"));
            tmpGo.name = "icon";
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.GetComponent<Image>().SetNativeSize();

            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(100, 20));
            tmpGo.name = "textPrice";
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 18;
            tmpText.color = Color.black;

            // ID显示
            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(100, 20));
            tmpGo.name = "textID";
            tmpGo.transform.SetParent(itemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -78);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 20);
            tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 16;
            tmpText.color = Color.black;

            // 喜欢
            tmpGo = GuiBaseUI.CreateUI.NewImage();
            tmpGo.name = "imgLove";
            tmpGo.transform.SetParent(itemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 0);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
            var tmpImage = tmpGo.GetComponent<Image>();
            tmpImage.sprite = SpriteTool.GetSprite("Common", "haogan_2");
            tmpGo.AddComponent<UISkyTipEffect>().InitData("标记为喜欢");
            return itemPrefab;
        }

        private void UpdateUnitItem(GameObject go, int id, int idx)
        {
            try
            {
                Transform root = go.transform;
                string path = DecorateMgr.barrierPath + id;
                Image img = root.Find("img").GetComponent<Image>();
                GameObject obj = g.res.Load<GameObject>(path);
                try
                {
                    try
                    {
                        img.sprite = obj.transform.Find("Idle/Root").GetComponent<SpriteRenderer>().sprite;
                    }
                    catch (Exception)
                    {
                        img.sprite = obj.transform.GetComponentInChildren<SpriteRenderer>().sprite;
                    }
                    img.SetNativeSize();
                    if (img.rectTransform.sizeDelta.x > 200)
                    {
                        float scale = img.rectTransform.sizeDelta.x / 200;
                        img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x / scale, img.rectTransform.sizeDelta.y / scale);
                    }
                    if (img.rectTransform.sizeDelta.y > 100)
                    {
                        float scale = img.rectTransform.sizeDelta.y / 100;
                        img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x / scale, img.rectTransform.sizeDelta.y / scale);
                    }
                }
                catch (Exception)
                {
                    img.sprite = SpriteTool.GetSprite("Common", "lvsetiao");
                }
                int price;
                if (DecorateMgr.decoratePrice.ContainsKey(id))
                {
                    price = DecorateMgr.decoratePrice[id];
                }
                else
                {
                    try
                    {
                        price = Mathf.CeilToInt(img.sprite.texture.width * img.sprite.texture.width * 0.1f);
                        DecorateMgr.decoratePrice.Add(id, price);
                    }
                    catch (Exception)
                    {
                        price = 1000;
                    }
                }
                var btn = UnityAPIEx.GetComponentOrAdd<Button>(img.gameObject);

                Action changeBattle = () =>
                {
                    clickCall?.Invoke(obj, id);
                };
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(changeBattle);
                root.Find("vlg/textPrice").GetComponent<Text>().text = price.ToString();
                root.Find("textID").GetComponent<Text>().text = "ID:" + id;
                var imgLove = root.Find("imgLove").GetComponent<Image>();
                imgLove.sprite = SpriteTool.GetSprite("Common", loveBarrier.Contains(id) ? "haogan" : "haogan_2");
                var btnLove = UnityAPIEx.GetComponentOrAdd<Button>(imgLove.gameObject);
                btnLove.onClick.RemoveAllListeners();
                Action onClickLove = () =>
                {
                    if (loveBarrier.Contains(id))
                    {
                        loveBarrier.Remove(id);
                    }
                    else
                    {
                        loveBarrier.Add(id);
                    }
                    imgLove.sprite = SpriteTool.GetSprite("Common", loveBarrier.Contains(id) ? "haogan" : "haogan_2");
                };
                btnLove.onClick.AddListener(onClickLove);
            }
            catch (Exception e)
            {
                Cave.LogError("创建单位Item失败：\n" + e.Message + "\n" + e.StackTrace);
            }
        }

    }
}
