using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;

namespace Cave
{
    public static class UITool
    {
        public static void SetUILeft(UIBase ui, RectTransform rect, float height, string text)
        {
            if (ui != null)
            {
                rect.SetParent(ui.transform, false);
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(0, 0.5f);
                rect.pivot = new Vector2(0, 0.5f);
                rect.anchoredPosition = new Vector2(10, height);

                var go = CreateUI.NewText(text);
                var rtf = go.GetComponent<RectTransform>();
                rtf.SetParent(rect, false);
                rtf.sizeDelta = rect.sizeDelta;

                var txt = go.GetComponent<Text>();
                txt.color = new Color(0, 0, 0, 1);
                txt.alignment = TextAnchor.MiddleCenter;

                txt.fontSize = 16;
            }
        }
    }
}
