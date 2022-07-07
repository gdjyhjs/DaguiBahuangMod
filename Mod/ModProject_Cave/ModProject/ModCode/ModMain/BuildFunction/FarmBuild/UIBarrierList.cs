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
        public UIBarrierList(Transform parent, Vector2 pos, List<int> allBarrierItem)
        {
            try
            {
                // 滚动背景
                bg = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("PlayerInfo", "daojukubg"));
                bg.transform.SetParent(parent.transform, false);
                bg.GetComponent<RectTransform>().anchoredPosition = pos;
                bg.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 620);
                bg.GetComponent<Image>().type = Image.Type.Tiled;

                // 滚动框
                var tmpGo = GuiBaseUI.CreateUI.NewScrollView(new Vector2(200, 600), spacing: new Vector2(0, 8));
                tmpGo.transform.SetParent(bg.transform, false);
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
            tmpGo.AddComponent<UISkyTipEffect>();

            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(220, 20));
            tmpGo.name = "textName";
            tmpGo.transform.SetParent(itemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -58);
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
                catch (Exception e)
                {
                    img.sprite = SpriteTool.GetSprite("Common", "lvsetiao");
                    Cave.LogWarning(id + " loadError");
                }
                var btn = UnityAPIEx.GetComponentOrAdd<Button>(img.gameObject);

                Action changeBattle = () =>
                {
                    clickCall?.Invoke(obj, id);
                };
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(changeBattle);

                var textName = root.Find("textName").GetComponent<Text>();
                textName.text = id.ToString();
            }
            catch (Exception e)
            {
                Cave.LogError("创建单位Item失败：\n" + e.Message + "\n" + e.StackTrace);
            }
        }

    }
}
