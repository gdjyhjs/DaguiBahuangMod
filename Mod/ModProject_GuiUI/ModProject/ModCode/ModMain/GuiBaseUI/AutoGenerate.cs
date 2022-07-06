using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GuiBaseUI
{
    public static class AutoGenerate
    {
        public static GameObject Generate(Transform root, AutoData data) {

            if (data.debug == 1)
            {
                LogUI(root.gameObject, $"Mods/GuiUI/Log/{Path.GetFileNameWithoutExtension(data.uiType)}.log");
            }
            GameObject go = CreateUI.New();
            if (string.IsNullOrWhiteSpace(data.nodePath))
            {
                go.transform.SetParent(root, false);
            }
            else
            {
                go.transform.SetParent(root.Find(data.nodePath), false);
            }
            go.name = "Generate_"+ Path.GetFileNameWithoutExtension(data.path);
            GenerateUI(go, data.transforms);
            if (data.debug == 1)
            {
                LogUI(go, $"Mods/GuiUI/Log/{Path.GetFileNameWithoutExtension(data.path)}.log");
            }
            return go;
        }

        public static void GenerateUI(GameObject go, AutoRectTransform[] rectTransforms)
        {
            Dictionary<string, RectTransform> rtfs = new Dictionary<string, RectTransform>();
            foreach (AutoRectTransform item in rectTransforms)
            {
                try
                {
                    RectTransform tf = CreateUI.New().GetComponent<RectTransform>();
                    rtfs.Add(item.id, tf);
                }
                catch (Exception e)
                {
                    Print.LogError("RectTransform 创建失败 " + item.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
            }

            foreach (AutoRectTransform config in rectTransforms)
            {
                RectTransform tf = rtfs[config.id];
                if (rtfs.ContainsKey(config.parent))
                {
                    tf.SetParent(rtfs[config.parent], false);
                }
                else
                {
                    tf.SetParent(go.transform, false);
                }
                tf.name = config.id.ToString();
                tf.localScale = new Vector3(config.scale.x, config.scale.y, config.scale.z);
                tf.anchoredPosition = new Vector2(config.position.x, config.position.y);
                tf.sizeDelta = new Vector2(config.size.x, config.size.y);
                bool condition = Condition(config.active, rtfs);
                tf.gameObject.SetActive(condition);

                try
                {
                    if (config.image != null)
                    {
                        AutoImage item = config.image;
                        Image img = tf.gameObject.AddComponent<Image>();
                        img.color = new Color(item.color.r / 255f, item.color.g / 255f, item.color.b / 255f, item.color.a / 255f);
                        img.type = (Image.Type)item.type;
                        img.raycastTarget = item.raycast == 1;
                        if (string.IsNullOrEmpty(item.atlas))
                        {
                            if (File.Exists("Mods/GuiUI/Image/" + item.image))
                            {
                                ImageManager.LoadGuiUISprite(img, item.image);
                            }
                            else if (string.IsNullOrEmpty(item.atlas))
                            {
                                img.sprite = SpriteTool.GetSpriteBigTex(item.image);
                            }
                        }
                        else
                        {
                            img.sprite = SpriteTool.GetSprite(item.atlas, item.image);
                        }
                    }
                }
                catch (Exception e)
                {
                    Print.LogError("Image 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
                try
                {
                    if (config.button != null)
                    {
                        AutoButton item = config.button;
                        Button btn = tf.gameObject.AddComponent<Button>();
                        Action testAction = () =>
                        {
                            bool condi = Condition(item.condition, rtfs);
                            if (condi)
                            {
                                DoFunction(item.function, rtfs);
                            }
                            else
                            {
                                DoFunction(item.failfuntion, rtfs);
                            }
                        };
                        btn.onClick.AddListener(testAction);
                    }
                }
                catch (Exception e)
                {
                    Print.LogError("Button 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
                try
                {
                    if (config.text != null)
                    {
                        AutoText item = config.text;
                        Text text = tf.gameObject.AddComponent<Text>();
                        text.text = GameTool.LS(item.connect);
                        text.raycastTarget = item.raycast == 1;
                        SetAutoText(text, item.data);
                        if (item.auto == 1)
                        {
                            ContentSizeFitter fitter = tf.gameObject.AddComponent<ContentSizeFitter>();
                            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        }
                        else if (item.auto == 2)
                        {
                            ContentSizeFitter fitter = tf.gameObject.AddComponent<ContentSizeFitter>();
                            fitter.verticalFit =ContentSizeFitter.FitMode.PreferredSize;
                        }else if (item.auto == 2)
                        {
                            ContentSizeFitter fitter = tf.gameObject.AddComponent<ContentSizeFitter>();
                            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        }
                    }

                }
                catch (Exception e)
                {
                    Print.LogError("Text 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
                try
                {
                    if (config.module != null)
                    {
                        AutoModule item = config.module;
                        RawImage rimg = tf.gameObject.AddComponent<RawImage>();
                        rimg.raycastTarget = item.raycast == 1;
                        if (!string.IsNullOrWhiteSpace(item.path))
                        {
                            GameObject dragon = GameObject.Instantiate(g.res.Load<GameObject>(item.path));
                            ModelRenderTexture rt = new ModelRenderTexture(dragon, rimg, new Vector2(item.offset.x, item.offset.y));
                            rt.gameObject.transform.localScale = new Vector3(item.scale.x, item.scale.y, 1);
                            rimg.color = new Color(item.color.r / 255f, item.color.g / 255f, item.color.b / 255f, item.color.a / 255f);
                        }
                    }
                }
                catch (Exception e)
                {
                    Print.LogError("module 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
                try
                {
                    if (config.scroll != null)
                    {
                        AutoScroll item = config.scroll;
                        RectTransform parent = tf;
                        GameObject obj = CreateUI.NewScrollView(parent.sizeDelta, spacing: new Vector2(0, item.spacing));
                        obj.transform.SetParent(parent, false);
                        ScrollRect scrollRect = obj.GetComponent<ScrollRect>();
                        HorizontalOrVerticalLayoutGroup layoutGroup = scrollRect.content.transform.GetComponent<HorizontalOrVerticalLayoutGroup>();
                        layoutGroup.spacing = item.spacing;
                        if (!string.IsNullOrWhiteSpace(item.connect))
                        {
                            GameObject t = CreateUI.NewText(item.connect, parent.sizeDelta);
                            t.transform.SetParent(scrollRect.content, false);
                            SetAutoText(t.GetComponent<Text>(), item.textData);
                            ContentSizeFitter sizeFitter = t.AddComponent<ContentSizeFitter>();
                            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                            t.GetComponent<Text>().text = GameTool.LS(item.connect);
                        }
                        if (item.data != null)
                        {
                            GenerateUI(scrollRect.content.gameObject, item.data);
                        }
                    }

                }
                catch (Exception e)
                {
                    Print.LogError("scroll 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }
                try
                {
                    if (config.toggle != null)
                    {
                        AutoToggle item = config.toggle;
                        RectTransform parent = tf;
                        GameObject obj = CreateUI.NewToggle(GameTool.LS(item.connect), new Vector2(item.connectSize.x, item.connectSize.y));
                        obj.transform.SetParent(parent, false);
                        Text text = obj.GetComponentInChildren<Text>();
                        SetAutoText(text, item.textData);
                        Toggle tog = obj.GetComponent<Toggle>();
                        Action<bool> onValueChange = (isOn) => {
                            rtfs[item.target].gameObject.SetActive(isOn);
                        };
                        tog.onValueChanged.AddListener(onValueChange);
                        tog.isOn = item.value != 0;
                    }
                }
                catch (Exception e)
                {
                    Print.LogError("toggle 创建失败 " + config.id + "\n" + e.Message + "\n" + e.StackTrace + "\n\n\n");
                }

            }
        }

        public static void LogUI(GameObject go, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            List<string> str = new List<string>();
            str.Add("*" + go.transform.parent.name);
            PrintUI(go.transform, str, 0);
            File.WriteAllText("Mods/GuiUI/Log/ui.log", string.Join("\n", str));
        }

        public static void PrintUI(Transform parent, List<string> str, int pos) {
            string s = "\t";
            for (int i = 0; i < pos; i++) {
                s += "\t";
            }
            str.Add(s + parent.name);
            for (int i = 0; i < parent.childCount; i++) {
                PrintUI(parent.GetChild(i), str, pos + 1);
            }
        }

        public static bool Condition(string function, Dictionary<string, RectTransform> rtfs)
        {
            if (string.IsNullOrWhiteSpace(function))
                return false;
            try
            {
                List<string> list = new List<string>(function.Split('|'));
                List<string> dramaFuncs = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    string[] ss = list[i].Split('_');
                    var parm1 = ss[0];
                    if (parm1 == "isActive")
                    {
                        try
                        {
                            string parm2 = ss[1];
                            var obj = rtfs[parm2].gameObject;
                            return obj.activeSelf;
                        }
                        catch (Exception e)
                        {
                            Print.LogError("candi = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace + "\n\n\n");
                            return false;
                        }
                    }
                    else
                    {
                        dramaFuncs.Add(list[i]);
                    }
                }
                if (dramaFuncs.Count > 0)
                {
                    function = string.Join("|", dramaFuncs);
                    return UnitConditionTool.Condition(function);
                }
                return true;
            }
            catch (Exception e)
            {
                Print.LogError("function = " + function);
                Print.LogError(e.Message + "\n" + e.StackTrace + "\n\n\n");
                return false;
            }
        }

        public static bool DoFunction(string function, Dictionary<string, RectTransform> rtfs)
        {
            if (string.IsNullOrWhiteSpace(function))
                return false;
            try
            {
                List<string> list = new List<string>(function.Split('|'));
                List<string> dramaFuncs = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    string[] ss = list[i].Split('_');
                    var parm1 = ss[0];
                    if (parm1 == "setActive")
                    {
                        try
                        {
                            string parm2 = ss[1];
                            rtfs[parm2].gameObject.SetActive(ss[2] == "1");
                        }
                        catch (Exception e)
                        {
                            Print.LogError("fun = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace + "\n\n\n");
                            return false;
                        }
                    }else if (parm1 == "setText")
                    {
                        try {
                            string parm2 = ss[1];
                            rtfs[parm2].gameObject.GetComponent<Text>().text = GameTool.LS(ss[2]);
                        }
                        catch (Exception e)
                        {
                            Print.LogError("fun = " + list[i]);
                            Print.LogError(e.Message + "\n" + e.StackTrace + "\n\n\n");
                            return false;
                        }
                    }
                    else
                    {
                        dramaFuncs.Add(list[i]);
                    }
                }
                if (dramaFuncs.Count > 0)
                {
                    function = string.Join("|", dramaFuncs);
                    DramaFunctionTool.OptionsFunction(function);
                }
                return true;
            }
            catch (Exception e)
            {
                Print.LogError("function = " + function);
                Print.LogError(e.Message + "\n" + e.StackTrace + "\n\n\n");
                return false;
            }
        }

        public static void SetAutoText(Text text, AutoTextData data)
        {
            text.fontSize = data.fontSize;
            text.font = CreateUI.GetFont(data.type);
            text.color = new UnityEngine.Color(data.color.r / 255f, data.color.g / 255f, data.color.b / 255f, data.color.a / 255f);
            text.alignment = (TextAnchor)data.alignment;
            if (data.outline.color.a > 0)
            {
                UnityEngine.UI.Outline outline = text.gameObject.AddComponent<UnityEngine.UI.Outline>();
                outline.effectColor = new UnityEngine.Color(data.outline.color.r / 255f, data.outline.color.g / 255f, data.outline.color.b / 255f, data.outline.color.a / 255f);
                outline.effectDistance = new Vector2(data.outline.distance.x, data.outline.distance.y);
            }
        }
    }


    public class AutoData
    {
        public int debug;
        public int order;
        public string uiType;
        public string nodePath;
        public AutoRectTransform[] transforms;
        public string path;
    }

    public struct V2 {
        public float x;
        public float y;
    }

    public struct V3
    {
        public float x;
        public float y;
        public float z;
    }

    public class AutoRectTransform
    {
        public string id;
        public string active;
        public string parent;
        public V2 position;
        public V2 size;
        public V3 scale;
        public AutoImage image;
        public AutoButton button;
        public AutoText text;
        public AutoModule module;
        public AutoScroll scroll;
        public AutoToggle toggle;
    }

    public class AutoImage
    {
        public string atlas;
        public string image;
        public AutoColor color;
        public int type;
        public int raycast;
    }

    public class AutoButton
    {
        public string condition;
        public string function;
        public string failfuntion;
    }

    public class AutoText
    {
        public string connect;
        public int auto;
        public AutoTextData data;
        public int raycast;
    }

    public class AutoModule
    {
        public string path;
        public AutoColor color;
        public V2 offset;
        public V3 scale;
        public int raycast;
    }

    public class AutoTextData
    {
        public int fontSize;
        public int type;
        public AutoColor color;
        public AutoOutline outline;
        public int alignment;
    }

    public class AutoScroll
    {
        public string connect;
        public AutoTextData textData;
        public float spacing;
        public AutoRectTransform[] data;
    }

    public struct AutoColor
    {
        public int r;
        public int g;
        public int b;
        public int a;
    }
    public struct AutoOutline
    {
        public AutoColor color;
        public V2 distance;
    }

    public class AutoToggle {
        public string target;
        public string connect;
        public V2 connectSize;
        public AutoTextData textData;
        public int value;
    }


}
