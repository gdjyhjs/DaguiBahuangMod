using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace FixData
{
    [HarmonyPatch(typeof(UIGameSetting), "Init")]
    class Patch_UIGameSetting_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UIGameSetting __instance)
        {
            try
            {
                AddFixToggle(__instance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        private static void AddFixToggle(UIGameSetting ui)
        {
            var root = ui.goSystem.transform.Find("Scroll View/Root");
            var oldItem = root.Find("GuiFixDataItem");
            if (oldItem != null)
            {
                GameObject.Destroy(oldItem.gameObject);
            }
            var item= GameObject.Instantiate( root.Find("Item (7)").gameObject);
            item.name = "GuiFixDataItem";
            item.transform.SetParent(root, false);
            item.transform.SetSiblingIndex(1);
            var textItem = item.transform.GetChild(0).GetComponent<Text>();
            textItem.text = "大鬼自动修复存档";
            var toggleItem = item.transform.GetChild(1).GetComponent<Toggle>();
            toggleItem.onValueChanged.RemoveAllListeners();
            toggleItem.isOn = ModMain.autoFixData;
            toggleItem.onValueChanged.AddListener(new Action<bool>((isOn)=>
            {
                ModMain.autoFixData = isOn;
                if (ModMain.autoFixData)
                {
                    UITipItem.AddTip("开启自动修复存档！");
                }
                else
                {
                    UITipItem.AddTip("关闭自动修复存档！");
                }
            }));
            GameObject btnObj = GameObject.Instantiate(ui.btnSystemOK.gameObject);
            btnObj.transform.SetParent(item.transform, false);
            btnObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, 0);
            btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 28);
            btnObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(45, 28);
            btnObj.transform.GetChild(1).GetComponent<Text>().text = "赞助"; 
            var btn = btnObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(new Action(()=>
            {
                Application.OpenURL("http://www.yellowshange.com/about.php");
            }));
        }
    }

}
