using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GuiBaseUI
{
    public static class UIFastFunction
    {

        public static void FastInputText(string titleContent, Action<string, bool> call, bool togState, string togConnect)
        {
            Transform root = CreateUI.NewCanvas().transform;
            Transform mask = CreateUI.NewScrollView().transform;
            mask.SetParent(root, false);
            mask.GetComponent<RectTransform>().sizeDelta = new Vector2(10000, 10000);
            mask.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

            Transform bg = CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/huodebg")).transform;
            bg.SetParent(root, false);
            bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(24, -37);

            // 标题
            Transform title = CreateUI.NewText(titleContent, new Vector2(500, 90), 1).transform;
            title.SetParent(root, false);
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(2, 130);
            Text t_title = title.GetComponent<Text>();
            t_title.fontSize = 40;
            t_title.alignment = TextAnchor.MiddleCenter;
            t_title.color = Color.black;

            // 输入框
            Transform ipt = CreateUI.NewInputField().transform;
            ipt.SetParent(root, false);
            ipt.GetComponent<RectTransform>().anchoredPosition = new Vector2(2, 0);
            var iptField = ipt.GetComponent<InputField>();
            iptField.contentType = InputField.ContentType.Name;
            iptField.characterLimit = 9;

            // 开关
            Toggle toggle = null;
            if (!string.IsNullOrWhiteSpace(togConnect))
            {
                Transform tog = CreateUI.NewToggle(togConnect, new Vector2(300, 30)).transform;
                tog.SetParent(root, false);
                tog.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, -50);
                toggle = tog.GetComponent<Toggle>();
                toggle.isOn = togState;
            }


            Transform btn1 = CreateUI.NewButton(() =>
            {
                call(iptField.text, toggle == null ? togState : toggle.isOn);
                GameObject.Destroy(root.gameObject);
            }).transform;
            btn1.SetParent(root, false);
            btn1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-130, -90);

            Transform btn2 = CreateUI.NewButton(() =>
            {
                GameObject.Destroy(root.gameObject);
            }).transform;
            btn2.SetParent(root, false);
            btn2.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, -90);

            var t1 = CreateUI.NewText("确定").transform;
            t1.SetParent(btn1, false);
            var t2 = CreateUI.NewText("取消").transform;
            t2.SetParent(btn2, false);
            Text[] tt = new Text[] {t1.GetComponent<Text>(), t2.GetComponent<Text>() };
            foreach (var t in tt)
            {
                t.alignment = TextAnchor.MiddleCenter;
                t.color = Color.black;
            }
        }
    }
}
