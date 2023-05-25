using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace MOD_wkIh9W
{
    [HarmonyPatch(typeof(DataGloble.GameSetting), "UpdateSetting")]
    class Patch_DataGloble_UpdateSetting
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                if (PlayerPrefs.GetInt("GuiFixWindowsItem", 0) == 1)
                {
                    int screenWidth = g.data.globle.gameSetting.screenWidth;
                    int screenHeight = g.data.globle.gameSetting.screenHeight;

                    if ((float)screenWidth / screenHeight > 3f)
                    {
                        screenWidth = Mathf.RoundToInt(screenHeight * 3f);
                    }
                    if ((float)screenHeight / screenWidth > 1f)
                    {
                        screenHeight = Mathf.RoundToInt(screenWidth * 1f);
                    }

                    Console.WriteLine("使用最大化窗口 ");
                    Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.MaximizedWindow);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }


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
            var oldItem = root.Find("GuiFixWindowsItem");
            if (oldItem != null)
            {
                GameObject.Destroy(oldItem.gameObject);
            }
            var item = GameObject.Instantiate(root.Find("Item (7)").gameObject);
            item.name = "GuiFixWindowsItem";
            item.transform.SetParent(root, false);
            item.transform.SetSiblingIndex(root.Find("Item (1)").GetSiblingIndex() + 1);
            var textItem = item.transform.GetChild(0).GetComponent<Text>();
            textItem.text = "全屏时改用无边框窗口";
            var toggleItem = item.transform.GetChild(1).GetComponent<Toggle>();
            toggleItem.onValueChanged.RemoveAllListeners();
            toggleItem.isOn = PlayerPrefs.GetInt("GuiFixWindowsItem", 0) == 1;
            toggleItem.onValueChanged.AddListener(new Action<bool>((isOn) =>
            {
                PlayerPrefs.SetInt("GuiFixWindowsItem", isOn ? 1 : 0);
            }));
        }
    }

}
