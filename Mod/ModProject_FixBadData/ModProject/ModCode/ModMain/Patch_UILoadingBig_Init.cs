using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FixData
{
    [HarmonyPatch(typeof(UILoadingBig), "Init")]
    class Patch_UILoadingBig_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UILoadingBig __instance)
        {
            try
            {
                ModMain.InitFixTip();
                var ui = __instance;
                var tip1 = GameObject.Instantiate(ui.textTip, ui.textTip.transform.parent).GetComponent<Text>();
                var tip2 = GameObject.Instantiate(ui.textTip, ui.textTip.transform.parent).GetComponent<Text>();
                var tip0 = GameObject.Instantiate(ui.textTip, ui.textTip.transform.parent).GetComponent<Text>();

                tip0.rectTransform.anchoredPosition = new Vector2(0, -340);
                tip0.rectTransform.sizeDelta = new Vector2(1920, 20);
                tip0.color = new Color(0, 0, 0, 1);
                tip0.transform.localScale = Vector3.one;
                tip0.text = "";


                tip1.rectTransform.anchoredPosition = new Vector2(0, -370);
                tip1.rectTransform.sizeDelta = new Vector2(1920, 20);
                tip2.rectTransform.anchoredPosition = new Vector2(0, -400);
                tip2.rectTransform.sizeDelta = new Vector2(1920, 20);
                tip1.transform.localScale = Vector3.one;
                tip2.transform.localScale = Vector3.one;
                tip1.color = new Color(0, 0, 0, 1);
                tip2.color = new Color(0, 0, 0, 1);
                tip1.text = ModMain.fixTip1;
                tip2.text = ModMain.fixTip2;
                __instance.AddCor(g.timer.Frame((Action)(() =>
                {
                    if (tip1)
                        tip1.text = ModMain.fixTip1;
                    if (tip2)
                        tip2.text = ModMain.fixTip2;
                    if (!(string.IsNullOrEmpty(ModMain.fixTip1) && string.IsNullOrEmpty(ModMain.fixTip2)))
                    {
                        tip0.text = "大鬼修复存档MOD检测中...";
                    }
                }), 5, true));
            }
            catch (Exception e)
            {
                Console.WriteLine("创建进度ui异常 " + e.ToString());
            }
        }
    }
}
