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
        public UIBarrierList(Transform parent, List<int> allBarrierItem)
        {
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
                }), 1, true);

                GameObject titleText = GuiBaseUI.CreateUI.NewText("装饰列表", new Vector2(220, 30));
                titleText.transform.SetParent(title.transform, false);
                Text tmpText = titleText.GetComponent<Text>();
                tmpText.alignment = TextAnchor.MiddleCenter;
                tmpText.color = Color.black;

                // 加个底显示灵石
                GameObject di = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "jingjiebg"));
                di.transform.SetParent(bg.transform, false);
                di.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -340);
                di.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 30);
                ShowMoney(di.transform);

                // 滚动背景
                GameObject bg2 = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("PlayerInfo", "daojukubg"));
                bg2.transform.SetParent(bg.transform, false);
                bg2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -15);
                bg2.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 620);
                bg2.GetComponent<Image>().type = Image.Type.Tiled;

                // 滚动框
                var tmpGo = GuiBaseUI.CreateUI.NewScrollView(new Vector2(200, 600), spacing: new Vector2(0, 8));
                tmpGo.transform.SetParent(bg2.transform, false);
                tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                tmpGo.name = "scr_createBuilds";
                var tmpScroll = tmpGo.GetComponent<ScrollRect>();

                Action<GuiBaseUI.ItemCell, int> itemAction = (cellItem, i) =>
                {
                    int idx = i - 1;
                    if (idx >= 0 && idx < allBarrierItem.Count)
                    {
                        cellItem.gameObject.SetActive(true);
                        var id = allBarrierItem[idx];
                        UpdateUnitItem(cellItem.gameObject, id, idx);
                    }
                    else
                    {
                        cellItem.gameObject.SetActive(false);
                    }
                };

                var goItem = CreateUnitItem();
                var bigDtaa = new GuiBaseUI.BigDataScroll();
                var itemCell = goItem.AddComponent<GuiBaseUI.ItemCell>();
                bigDtaa.Init(tmpScroll, itemCell, itemAction, goItem.GetComponent<RectTransform>().sizeDelta.y);
                bigDtaa.cellHeight = 122;
                bigDtaa.cellCount = allBarrierItem.Count;
                tmpScroll.verticalNormalizedPosition = 0.9999f;

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
            itemPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 122);
            itemPrefab.name = "item";

            // 主图
            GameObject tmpGo = GuiBaseUI.CreateUI.NewImage();
            tmpGo.name = "img";
            tmpGo.transform.SetParent(itemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);

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
            tmpGo.name = "textName";
            tmpGo.transform.SetParent(go.transform, false);
            tmpGo.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 18;
            tmpText.color = Color.black;

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

                var textName = root.Find("vlg/textName").GetComponent<Text>();
                textName.text = price.ToString();
            }
            catch (Exception e)
            {
                Cave.LogError("创建单位Item失败：\n" + e.Message + "\n" + e.StackTrace);
            }
        }

    }
}
