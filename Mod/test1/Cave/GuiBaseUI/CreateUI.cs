using MelonLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GuiBaseUI
{
    public enum BarType
    {
        None,
        Vertical,
    }
    public enum ContentType
    {
        Node,
        Grid,
        VerticalLayout,
    }
    public static class CreateUI
    {
        public static Font defFont;
        public static Font font
        {
            get
            {
                if (defFont == null)
                {
                    List<Text> texts = Tools.FindObjectsOfType<Text>();
                    texts.RemoveAll((v) => v.font == null);
                    if (texts.Count > 0)
                    {
                        defFont = texts[0].font;
                    }
                    else
                    {
                        MelonLogger.Msg("没有字体");
                    }
                }
                return defFont;
            }
        }
        static int sort = 10000;
        public static GameObject NewCanvas(int sortLayer = int.MinValue)
        {
            //创建画布及设置参数
            GameObject go2 = new GameObject("Canvas");
            go2.AddComponent<RectTransform>();
            go2.AddComponent<Canvas>();
            go2.AddComponent<CanvasScaler>();
            go2.AddComponent<GraphicRaycaster>();
            go2.layer = int.MaxValue;
            Canvas canvas = go2.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortLayer == int.MinValue ? ++sort : sortLayer;
            CanvasScaler canvasScaler = go2.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            var allScaler = GameObject.FindObjectsOfType<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

            return go2;
        }

        public static GameObject NewScrollView(Vector2 size = default(Vector2), BarType barType = BarType.Vertical, ContentType contentType = ContentType.VerticalLayout, Vector2 spacing = default(Vector2), Vector2 gridSize = default(Vector2))
        {
            //创建ScrollRect
            GameObject go1 = new GameObject("ScrollView");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<ScrollRect>();
            go1.layer = int.MaxValue;
            RectTransform tf1 = go1.GetComponent<RectTransform>();
            tf1.anchoredPosition = new Vector2(0, 0);
            tf1.sizeDelta = size == default(Vector2) ? new Vector2(100, 100) : size;
            ScrollRect scrollRect = go1.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 10;

            //创建Viewport
            GameObject go2 = new GameObject("Viewport");
            go2.AddComponent<RectTransform>();
            go2.AddComponent<RectMask2D>();
            go2.AddComponent<Image>();
            go2.layer = int.MaxValue;
            RectTransform tf2 = go2.GetComponent<RectTransform>();
            tf2.SetParent(tf1, false);
            tf2.anchorMin = new Vector2(0, 0);
            tf2.anchorMax = new Vector2(1, 1);
            tf2.anchoredPosition = new Vector2(0, 0);
            tf2.sizeDelta = new Vector2(-17, -17);
            tf2.pivot = new Vector2(0, 1);
            Image image2 = go2.GetComponent<Image>();
            image2.color = new Color(0, 0, 0, 0);

            //创建Content
            GameObject go3 = new GameObject("Content");
            go3.AddComponent<RectTransform>();
            go3.AddComponent<ContentSizeFitter>();
            go2.layer = int.MaxValue;
            RectTransform tf3 = go3.GetComponent<RectTransform>();
            Image img3 = go3.AddComponent<Image>();
            img3.color = new Color(0, 0, 0, 0.01f);
            img3.raycastTarget = true;
            tf3.SetParent(tf2, false);
            tf3.anchorMin = new Vector2(0, 1);
            tf3.anchorMax = new Vector2(1, 1);
            tf3.anchoredPosition = new Vector2(0, 0);
            tf3.sizeDelta = new Vector2(0, 0);
            tf3.pivot = new Vector2(0.5f, 1);
            ContentSizeFitter contentSizeFitter = tf3.GetComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            switch (contentType)
            {
                case ContentType.Grid:
                    GridLayoutGroup gridLayoutGroup = go3.AddComponent<GridLayoutGroup>();
                    gridLayoutGroup.cellSize = gridSize == default(Vector2) ? new Vector2(100, 100) : gridSize;
                    gridLayoutGroup.spacing = spacing;
                    break;
                case ContentType.VerticalLayout:
                    VerticalLayoutGroup verticalLayoutGroup = go3.AddComponent<VerticalLayoutGroup>();
                    verticalLayoutGroup.spacing = spacing.y;
                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childControlWidth = false;
                    verticalLayoutGroup.childForceExpandHeight = false;
                    verticalLayoutGroup.childForceExpandWidth = false;
                    verticalLayoutGroup.childAlignment = default(TextAnchor);
                    break;
                default:
                    break;
            }

            //关联控件
            scrollRect.viewport = tf2;
            scrollRect.content = tf3;
            switch (barType)
            {
                case BarType.Vertical:
                    GameObject go4 = new GameObject("ScrollbarVertical");
                    go4.AddComponent<RectTransform>();
                    go4.AddComponent<Image>();
                    go4.AddComponent<Scrollbar>();
                    go4.layer = int.MaxValue;
                    RectTransform tf4 = go4.GetComponent<RectTransform>();
                    tf4.SetParent(tf1, false);
                    tf4.anchorMin = new Vector2(1, 0);
                    tf4.anchorMax = new Vector2(1, 1);
                    tf4.anchoredPosition = new Vector2(0, 0);
                    tf4.sizeDelta = new Vector2(8, 0);
                    tf4.pivot = new Vector2(1, 1);
                    Image image4 = go4.GetComponent<Image>();
                    image4.sprite = SpriteTool.GetSprite("Common", "ladongtiaodi");
                    Scrollbar scrollbar = go4.GetComponent<Scrollbar>();
                    scrollbar.direction = Scrollbar.Direction.BottomToTop;

                    GameObject go5 = new GameObject("SlidingArea");
                    go5.AddComponent<RectTransform>();
                    go5.layer = int.MaxValue;
                    RectTransform tf5 = go5.GetComponent<RectTransform>();
                    tf5.SetParent(tf4, false);
                    tf5.anchorMin = new Vector2(0, 0);
                    tf5.anchorMax = new Vector2(1, 1);
                    tf5.anchoredPosition = new Vector2(0, 0);
                    tf5.sizeDelta = new Vector2(0, 0);
                    tf5.pivot = new Vector2(0.5f, 0.5f);

                    GameObject go6 = new GameObject("Handle");
                    go6.AddComponent<RectTransform>();
                    go6.AddComponent<Image>();
                    go6.layer = int.MaxValue;
                    RectTransform tf6 = go6.GetComponent<RectTransform>();
                    tf6.SetParent(tf5, false);
                    tf6.anchorMin = new Vector2(0, 0.5f);
                    tf6.anchorMax = new Vector2(1, 1);
                    tf6.anchoredPosition = new Vector2(0, 0);
                    tf6.sizeDelta = new Vector2(-1, -1);
                    tf6.pivot = new Vector2(0.5f, 0.5f);
                    Image image6 = go6.GetComponent<Image>();
                    image6.sprite = SpriteTool.GetSprite("Common", "ladongtiao");
                    image6.type = Image.Type.Tiled;

                    scrollbar.targetGraphic = image6;
                    scrollbar.handleRect = tf6;
                    scrollRect.verticalScrollbar = scrollbar;
                    scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                    scrollRect.verticalScrollbarSpacing = -3;
                    break;
                default:
                    break;
            }
            return go1;
        }

        // 创建图片
        public static GameObject NewImage(Sprite sprite = null)
        {
            GameObject go1 = new GameObject("Image");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<Image>();
            go1.layer = int.MaxValue;
            RectTransform tf1 = go1.GetComponent<RectTransform>();
            Image image = go1.GetComponent<Image>();
            if(sprite == null)
            {
                tf1.sizeDelta = new Vector2(100, 100);
            }
            else
            {
                image.sprite = sprite;
                image.SetNativeSize();
            }

            return go1;
        }

        // 创建图片
        public static GameObject NewRawImage(Vector2 size = default(Vector2))
        {
            GameObject go1 = new GameObject("Image");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<RawImage>();
            go1.layer = int.MaxValue;
            RectTransform tf1 = go1.GetComponent<RectTransform>();
            Image image = go1.GetComponent<Image>();
            if (size == default(Vector2))
            {
                tf1.sizeDelta = new Vector2(100, 100);
            }
            else
            {
                tf1.sizeDelta = size;
            }

            return go1;
        }

        // 创建文本
        public static GameObject NewText(string s = null,Vector2 size=default(Vector2))
        {
            GameObject go1 = new GameObject("Text");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<Text>();
            go1.layer = int.MaxValue;
            RectTransform tf1 = go1.GetComponent<RectTransform>();
            Text text = go1.GetComponent<Text>();
            text.text = s;
            text.fontSize = 20;
            text.font = font;
            
            if (size == default(Vector2))
            {
                tf1.sizeDelta = new Vector2(100, 100);
            }
            else
            {
                tf1.sizeDelta = size;
            }
            

            return go1;
        }

        public static GameObject New()
        {
            GameObject go1 = new GameObject("");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<Text>();


            return go1;
        }

        // 创建下拉框
        public static GameObject NewDropdown(string[] options)
        {
            GameObject go1 = new GameObject("Dropdown");
            go1.AddComponent<RectTransform>();
            go1.AddComponent<Dropdown>();
            go1.AddComponent<Image>();
            go1.layer = int.MaxValue;
            RectTransform tf1 = go1.GetComponent<RectTransform>();
            tf1.sizeDelta = new Vector2(200, 30);
            Image image = go1.GetComponent<Image>();
            image.color = new Color(1, 0, 0, 1);
            Dropdown dropdown = go1.GetComponent<Dropdown>();
            dropdown.targetGraphic = image;

            GameObject go2 = new GameObject("Label");
            go2.AddComponent<RectTransform>();
            go2.AddComponent<Text>();
            go2.layer = int.MaxValue;
            RectTransform tf2 = go2.GetComponent<RectTransform>();
            tf2.SetParent(tf1, false);
            tf2.anchorMin = new Vector2(0, 0);
            tf2.anchorMax = new Vector2(1, 1);
            tf2.anchoredPosition = new Vector2(-7.5f, 0.5f);
            tf2.sizeDelta = new Vector2(-35, -13);
            tf2.pivot = new Vector2(0.5f, 0.5f);
            Text text = go2.GetComponent<Text>();
            text.fontSize = 14;
            text.font = font;
            text.color = new Color(0, 0, 0);
            text.alignment = TextAnchor.MiddleLeft;


            GameObject go3 = new GameObject("Arrow");
            go3.AddComponent<RectTransform>();
            go3.AddComponent<Image>();
            go3.layer = int.MaxValue;
            RectTransform tf3 = go3.GetComponent<RectTransform>();
            tf3.SetParent(tf1, false);
            tf3.anchorMin = new Vector2(1f, 0.5f);
            tf3.anchorMax = new Vector2(1f, 0.5f);
            tf3.anchoredPosition = new Vector2(-15f, 0f);
            tf3.sizeDelta = new Vector2(20, 20);
            tf3.pivot = new Vector2(0.5f, 0.5f);
            Image img3 = go3.GetComponent<Image>();
            img3.color = new Color(1, 1, 0, 1);

            GameObject go4 = NewScrollView(default(Vector2), BarType.Vertical, ContentType.Node);
            go4.SetActive(false);
            go4.name = "Template";
            RectTransform tf4 = go4.GetComponent<RectTransform>();
            tf4.SetParent(tf1, false);
            tf4.anchorMin = new Vector2(0, 0);
            tf4.anchorMax = new Vector2(1, 0);
            tf4.anchoredPosition = new Vector2(0, 2);
            tf4.sizeDelta = new Vector2(0, 150);
            tf4.pivot = new Vector2(0.5f, 1);
            ScrollRect scrollRect = go4.GetComponent<ScrollRect>();
            scrollRect.content.sizeDelta = new Vector2(0, 28);
            scrollRect.viewport.sizeDelta = new Vector2(-18, 0);
            Image img4 = go4.AddComponent<Image>();

            GameObject go5 = new GameObject("Item");
            go5.AddComponent<RectTransform>();
            go5.AddComponent<Toggle>();
            go5.layer = int.MaxValue;
            RectTransform tf5 = go5.GetComponent<RectTransform>();
            tf5.SetParent(scrollRect.content, false);
            tf5.anchorMin = new Vector2(0, 0.5f);
            tf5.anchorMax = new Vector2(1, 0.5f);
            tf5.anchoredPosition = new Vector2(0, 0);
            tf5.sizeDelta = new Vector2(0, 20);
            tf5.pivot = new Vector2(0.5f, 0.5f);
            Toggle toggle = go5.GetComponent<Toggle>();



            GameObject go6 = new GameObject("Item Background");
            go6.AddComponent<RectTransform>();
            go6.AddComponent<Image>();
            go6.layer = int.MaxValue;
            RectTransform tf6 = go6.GetComponent<RectTransform>();
            tf6.SetParent(tf5, false);
            tf6.anchorMin = new Vector2(0, 0);
            tf6.anchorMax = new Vector2(1, 1);
            tf6.anchoredPosition = new Vector2(0, 0);
            tf6.sizeDelta = new Vector2(0, 0);
            tf6.pivot = new Vector2(0.5f, 0.5f);
            Image img6 = go6.GetComponent<Image>();
            img6.color = new Color(1, 0, 0, 1);


            GameObject go7 = new GameObject("Item Checkmark");
            go7.AddComponent<RectTransform>();
            go7.AddComponent<Image>();
            go7.layer = int.MaxValue;
            RectTransform tf7 = go7.GetComponent<RectTransform>();
            tf7.SetParent(tf5, false);
            tf7.anchorMin = new Vector2(0, 0.5f);
            tf7.anchorMax = new Vector2(0, 0.5f);
            tf7.anchoredPosition = new Vector2(10, 0);
            tf7.sizeDelta = new Vector2(20, 20);
            tf7.pivot = new Vector2(0.5f, 0.5f);
            Image img7 = go7.GetComponent<Image>();
            img7.color = new Color(1, 1, 0, 1);


            GameObject go8 = new GameObject("Item Label");
            go8.AddComponent<RectTransform>();
            go8.AddComponent<Text>();
            go8.layer = int.MaxValue;
            RectTransform tf8 = go8.GetComponent<RectTransform>();
            tf8.SetParent(tf5, false);
            tf8.anchorMin = new Vector2(0, 0);
            tf8.anchorMax = new Vector2(1, 1);
            tf8.anchoredPosition = new Vector2(0, 0);
            tf8.sizeDelta = new Vector2(0, 0);
            tf8.pivot = new Vector2(0.5f, 0.5f);
            Text t8 = go8.GetComponent<Text>();
            t8.fontSize = 14;
            t8.color = new Color(0, 0, 0);

            toggle.targetGraphic = img6;
            toggle.graphic = img7;

            dropdown.targetGraphic = image;
            dropdown.template = tf4;
            dropdown.captionText = text;
            dropdown.itemText = t8;
            dropdown.options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
            for (int i = 0; i < options.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(options[i]));
            }
            if (options.Length > 0)
            {
                text.text = options[0];
                t8.text = options[0];
            }
            else
            {
                text.text = string.Empty;
                t8.text = string.Empty;
            }


            return go1;
        }
    }
}