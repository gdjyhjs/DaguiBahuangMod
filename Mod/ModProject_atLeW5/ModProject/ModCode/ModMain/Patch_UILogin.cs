using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using GuiBaseUI;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_atLeW5
{
    [HarmonyPatch(typeof(UILogin), "UpdateUI")]
    class Patch_UILogin_UpdateUI
    {
        [HarmonyPrefix]
        private static bool Prefix(UILogin __instance)
        {
            ModMain.ReadData();
            string test = "A";
            try
            {
                var title = __instance.textIntoGame.text;
                test = "B";
                __instance.totalCacheCount = StaticData.fixCacheCount;
                test = "C";

                var cacheRoot = __instance.goCacheRoot.GetComponent<RectTransform>();
                var cache = __instance.goCache.GetComponent<RectTransform>();
                test = "D";

                cache.gameObject.SetActive(false);
                var scroll = CreateUI.NewScrollView(contentType: ContentType.Grid).GetComponent<ScrollRect>();
                scroll.transform.SetParent(cacheRoot, false);
                test = "E";
                scroll.rectTransform.anchoredPosition = new Vector2(15, -24);
                scroll.rectTransform.sizeDelta = new Vector2(1505, 620);
                test = "F";

                __instance.allObjs["goCache"] = scroll.content.gameObject;

                if (cacheRoot.Find("goCachePro"))
                {
                    GameObject.DestroyImmediate(cacheRoot.Find("goCachePro").gameObject);
                }
                if (cache)
                {
                    GameObject.DestroyImmediate(cache);
                }
                test = "G";

                scroll.name = "goCachePro";

                var go1 = CreateUI.NewToggle("默认");
                var go2 = CreateUI.NewToggle("列表");
                var go3 = CreateUI.NewToggle("极简");
                test = "H";
                go1.transform.SetParent(scroll.transform, false);
                go2.transform.SetParent(scroll.transform, false);
                go3.transform.SetParent(scroll.transform, false);
                test = "I";
                go1.GetComponent<RectTransform>().anchoredPosition = new Vector2(335, -330);
                go2.GetComponent<RectTransform>().anchoredPosition = new Vector2(490, -330);
                go3.GetComponent<RectTransform>().anchoredPosition = new Vector2(650, -330);
                test = "J";
                var group = scroll.gameObject.AddComponent<ToggleGroup>();
                go1.GetComponent<Toggle>().group = group;
                go2.GetComponent<Toggle>().group = group;
                go3.GetComponent<Toggle>().group = group;
                test = "K";
                if (StaticData.sortType == 1)
                {
                    go1.GetComponent<Toggle>().isOn = true;
                }
                else if (StaticData.sortType == 2)
                {
                    go2.GetComponent<Toggle>().isOn = true;
                }
                else if (StaticData.sortType == 3)
                {
                    go3.GetComponent<Toggle>().isOn = true;
                }
                test = "L";
                go1.GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)((v) =>
                {
                    StaticData.sortType = 1;
                }));
                go2.GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)((v) =>
                {
                    StaticData.sortType = 2;
                }));
                go3.GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)((v) =>
                {
                    StaticData.sortType = 3;
                }));
                test = "M";
                {
                    var btnFixCount = CreateUI.NewButton(() =>
                    {
                        var ui = g.ui.OpenUI<UIModTableInputStringLong>(UIType.ModTableInputStringLong);
                        ui.InitData("修改存档数量", StaticData.fixCacheCount.ToString(), (Action<string>)((str) =>
                        {
                            if (int.TryParse(str, out int count) || count < 1 || count > 999)
                            {
                                StaticData.fixCacheCount = count;
                            }
                            else
                            {
                                UITipItem.AddTip("属入的数字不正确(请属入1-999的数字)");
                            }
                        }), 3);
                    });
                    test = "N";
                    btnFixCount.transform.SetParent(scroll.transform, false);
                    btnFixCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(-650, -330);
                    btnFixCount.GetComponent<RectTransform>().sizeDelta = new Vector2(145, 40);
                    test = "O";

                    var textFixCount = CreateUI.NewText("修改存档数量", new Vector2(150, 40));
                    textFixCount.transform.SetParent(btnFixCount.transform, false);
                    textFixCount.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    textFixCount.GetComponent<Text>().color = new Color(0, 0, 0);
                }

                test = "M1";
                {
                    var btnUpdateData = CreateUI.NewButton(() =>
                    {
                        var ui = g.ui.OpenUI<UIModTableInputStringLong>(UIType.ModTableInputStringLong);
                        ui.InitData("修改存档数量", StaticData.fixCacheCount.ToString(), (Action<string>)((str) =>
                        {
                            if (int.TryParse(str, out int count) || count < 1 || count > 999)
                            {
                                StaticData.fixCacheCount = count;
                            }
                            else
                            {
                                UITipItem.AddTip("属入的数字不正确(请属入1-999的数字)");
                            }
                        }), 3);
                    });
                    test = "N2";
                    btnUpdateData.transform.SetParent(scroll.transform, false);
                    btnUpdateData.GetComponent<RectTransform>().anchoredPosition = new Vector2(-450, -330);
                    btnUpdateData.GetComponent<RectTransform>().sizeDelta = new Vector2(145, 40);
                    test = "O3";

                    var textUpdateData = CreateUI.NewText("刷新存档", new Vector2(150, 40));
                    textUpdateData.transform.SetParent(btnUpdateData.transform, false);
                    textUpdateData.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    textUpdateData.GetComponent<Text>().color = new Color(0, 0, 0);
                    test = "O5";
                }

                var giTip = CreateUI.NewImage(SpriteTool.GetSprite("Common", "wenhao"));
                giTip.transform.SetParent(scroll.transform, false);
                giTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-730, -330);
                UnityAPIEx.GetComponentOrAdd<UISkyTipEffect>(giTip).InitData("鼠标悬浮在备注上方可查看完整备注\n鼠标点击存档编号可打开该存档所在的文件夹");

                Console.WriteLine("修改登录选角Ok");
            }
            catch (Exception e)
            {
                Console.WriteLine(test+"修改登录选角Fail " +e.ToString());
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(UILogin __instance)
        {
            try
            {
                for (int i = 0; i < __instance.allCacheItem.Count; i++)
                {
                    int index = i;
                    var item = __instance.allCacheItem[index];
                    var cache = __instance.allCacheItem[index].cacheData;
                    FixItem(item, cache, index + 1);
                }
                Console.WriteLine("修改登录角色项OK ");
                FixUI(__instance);
            }
            catch (Exception e)
            {
                Console.WriteLine("修改登录角色项Fail " + e.ToString());
            }
        }

        private static void FixUI(UILogin __instance)
        {
            Console.WriteLine("FixUI");
            var grid = __instance.goCache.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                Console.WriteLine("grid null");
                return;
            }
            if (StaticData.sortType == 1)
            {
                grid.cellSize = new Vector2(500, 200);
            }
            else if (StaticData.sortType == 2)
            {
                grid.cellSize = new Vector2(1500, 50);
            }
            else if (StaticData.sortType == 3)
            {
                grid.cellSize = new Vector2(500, 50);
            }
        }


        private static void FixItem(UILoginCacheItem __instance, CacheMgr.CacheData cacheData, int num)
        {
            if (__instance == null || __instance.btnDel == null)
            {
                return;
            }
            string test = "A";
            try
            {

                if (__instance.goGroupRoot.transform.Find("cacheIndex1"))
                {
                    GameObject.DestroyImmediate(__instance.goGroupRoot.transform.Find("cacheIndex1").gameObject);
                }
                test = "B";
                __instance.btnDel.transform.localScale = Vector3.one;
                test = "C";
                var bg = CreateUI.NewImage(SpriteTool.GetSprite("BattleInfoCommon", "kuaijiejianbg"));
                bg.name = "cacheIndex1";
                bg.transform.SetParent(__instance.goGroupRoot.transform, false);
                bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, 30);
                test = "D";
                if (StaticData.sortType == 1)
                {
                    bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200, -60);
                    __instance.btnDel.GetComponent<RectTransform>().anchoredPosition = new Vector2(190, 56);
                    __instance.btnDel.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                    __instance.btnRoot1.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 144);
                    __instance.btnModInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-192, 57);
                    __instance.goRoot1.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-138, 4);
                    __instance.goRoot1.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(-138, 4);
                    __instance.goRoot1.transform.GetChild(1).localScale = Vector3.one * 1f;
                    __instance.goRoot1.transform.GetChild(2).localScale = Vector3.one * 1f;
                    __instance.goAnimaWeapons.GetComponent<RectTransform>().anchoredPosition = new Vector2(-175, -33);
                    __instance.goRoot1.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(170, 50); // 难度
                    __instance.goRoot1.transform.GetChild(4).localScale = Vector3.one * 1f;
                    __instance.goRoot1.transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(454, 160);

                    __instance.textName.GetComponent<RectTransform>().anchoredPosition = new Vector2(26, 46);
                    __instance.textName.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                    __instance.textLv.GetComponent<RectTransform>().anchoredPosition = new Vector2(26, 2);
                    __instance.textLv.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                    __instance.textDate.GetComponent<RectTransform>().anchoredPosition = new Vector2(158, 10);
                    __instance.textDate.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
                    __instance.textTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(68, -52);
                    __instance.textTip.GetComponent<RectTransform>().sizeDelta = new Vector2(282, 60);
                    __instance.textTip.alignment = TextAnchor.UpperLeft;

                    __instance.btnRoot2.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 144);
                    __instance.btnRoot2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(454, 160);

                    if (SceneLogin.languageType == LanguageType.English)
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(3).gameObject.SetActive(true);
                    }
                    else
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(2).gameObject.SetActive(true);
                    }
                }
                else if (StaticData.sortType == 2)
                {
                    bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-700, 0);
                    __instance.btnDel.GetComponent<RectTransform>().anchoredPosition = new Vector2(700, 0);
                    __instance.btnDel.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                    __instance.btnRoot1.GetComponent<RectTransform>().sizeDelta = new Vector2(1450, 50);
                    __instance.btnModInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-513, 50);
                    __instance.goRoot1.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-410, 3);
                    __instance.goRoot1.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(-410, 3);
                    __instance.goRoot1.transform.GetChild(1).localScale = Vector3.one * 0.38f;
                    __instance.goRoot1.transform.GetChild(2).localScale = Vector3.one * 0.38f;
                    __instance.goAnimaWeapons.GetComponent<RectTransform>().anchoredPosition = new Vector2(-463, 50);
                    __instance.goRoot1.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(-560, 3);
                    __instance.goRoot1.transform.GetChild(4).localScale = Vector3.one * 1f;
                    __instance.goRoot1.transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(1480, 62);

                    __instance.textName.GetComponent<RectTransform>().anchoredPosition = new Vector2(-288, 0);
                    __instance.textName.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                    __instance.textLv.GetComponent<RectTransform>().anchoredPosition = new Vector2(-121, 0);
                    __instance.textLv.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                    __instance.textDate.GetComponent<RectTransform>().anchoredPosition = new Vector2(68, 0);
                    __instance.textDate.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                    __instance.textTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(410, 0);
                    __instance.textTip.GetComponent<RectTransform>().sizeDelta = new Vector2(520, 30);
                    __instance.textTip.alignment = TextAnchor.MiddleLeft;

                    __instance.btnRoot2.GetComponent<RectTransform>().sizeDelta = new Vector2(1450, 50);
                    __instance.btnRoot2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1480, 62);

                    if (SceneLogin.languageType == LanguageType.English)
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(3).gameObject.SetActive(false);
                    }
                    else
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(2).gameObject.SetActive(false);
                    }

                }
                else if (StaticData.sortType == 3)
                {
                    bg.GetComponent<RectTransform>().anchoredPosition = new Vector2(-235 + 24, 12);
                    __instance.btnDel.GetComponent<RectTransform>().anchoredPosition = new Vector2(198, 2);
                    __instance.btnDel.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                    __instance.btnRoot1.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 50);
                    __instance.btnModInfo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-214 + 24, 12);
                    __instance.goRoot1.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-144 + 24, 2);
                    __instance.goRoot1.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(-144 + 24, 2);
                    __instance.goRoot1.transform.GetChild(1).localScale = Vector3.one * 0.38f;
                    __instance.goRoot1.transform.GetChild(2).localScale = Vector3.one * 0.38f;
                    __instance.goAnimaWeapons.GetComponent<RectTransform>().anchoredPosition = new Vector2(-183 + 24, 0);
                    __instance.goRoot1.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(-200 + 24, -10); // 难度
                    __instance.goRoot1.transform.GetChild(4).localScale = Vector3.one * .6f;
                    __instance.goRoot1.transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(485, 62);

                    __instance.textName.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 10);
                    __instance.textName.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
                    __instance.textLv.GetComponent<RectTransform>().anchoredPosition = new Vector2(40, 10);
                    __instance.textLv.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
                    __instance.textDate.GetComponent<RectTransform>().anchoredPosition = new Vector2(130, 10);
                    __instance.textDate.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
                    __instance.textTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(40, -10);
                    __instance.textTip.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 20);
                    __instance.textTip.alignment = TextAnchor.MiddleLeft;

                    __instance.btnRoot2.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 50);
                    __instance.btnRoot2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(485, 62);

                    if (SceneLogin.languageType == LanguageType.English)
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(3).gameObject.SetActive(false);
                    }
                    else
                    {
                        __instance.goRoot1.transform.GetChild(6).GetChild(2).gameObject.SetActive(false);
                    }

                }
                test = "E";
                var text = CreateUI.NewText(num.ToString());
                text.transform.SetParent(bg.transform, false);
                text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                text.GetComponent<Text>().raycastTarget = false;
                __instance.textTip.gameObject.SetActive(true);
                __instance.textTip.raycastTarget = true;
                test = "";

                if (cacheData != null)
                {
                    test = "F";
                    string tip = "";
                    //var path = g.cache.GetPath(cacheData.data.folder, ModMain.dataKey);
                    var path = g.cache.cachePath + "/" + cacheData.data.folder + "/" + ModMain.dataKey + ".cache"; ;
                    test = "G";
                    bg.AddComponent<Button>().onClick.AddListener((Action)(() =>
                    {
                        Application.OpenURL("file://" + Path.GetDirectoryName(path));
                    }));
                    test = "H";
                    UnityAPIEx.GetComponentOrAdd<UISkyTipEffect>(bg).InitData("点击可打开存档目录");
                    test = "I";
                    try
                    {
                        if (File.Exists(path))
                        {
                            tip = File.ReadAllText(path);
                            UpdateText(__instance.textTip, tip);
                        }
                        else
                        {
                            UpdateText(__instance.textTip, null);
                        }
                        test = "J";
                    }
                    catch (Exception)
                    {
                        test = "K";
                        UpdateText(__instance.textTip, null);
                        test = "L";
                    }
                    var btn = UnityAPIEx.GetComponentOrAdd<Button>(__instance.textTip.gameObject);
                    test = "M";
                    btn.onClick.AddListener((Action)(() =>
                    {
                        var ui = g.ui.OpenUI<UIModTableInputStringLong>(UIType.ModTableInputStringLong);
                        ui.InitData("添加存档备注", tip, (Action<string>)((str) =>
                        {
                            tip = str;
                            UpdateText(__instance.textTip, str);
                            File.WriteAllText(path, str);
                        }), 1000);
                    }));
                    test = "N";
                }
                else
                {
                    __instance.textTip.text = "";
                    test = "O";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(test + "修改项Fail " + e.ToString());
            }
        }

        private static void UpdateText(Text text, string str)
        {
            if (str == null)
            {
                text.text = "点击添加存档备注";
                text.color = new Color(0.4f, 0.4f, 0.4f);
                text.fontStyle = FontStyle.Italic;
            }
            else
            {
                text.text = str;
                text.color = new Color(0, 0, 0);
                text.fontStyle = FontStyle.Normal;
                UnityAPIEx.GetComponentOrAdd<UISkyTipEffect>(text.gameObject).InitData(str);
            }
        }
    }


    //[HarmonyPatch(typeof(DataBase<DataWorld.World>), "Save")]
    //class Patch_DataBase_Save
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(DataBase<DataWorld.World> __instance)
    //    {
    //        Console.WriteLine("保存 "+__instance);
    //        try
    //        {
    //            //string path = g.cache.GetPath(__instance.GetName());
    //            //Console.WriteLine("Save " + __instance + " " + (__instance) + " " + path);


    //            //if (__instance is DataWorld)
    //            //{
    //            //    var path = g.cache.GetPath(cacheData.data.folder, ModMain.dataKey);

    //            //}
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("FX" + e.ToString());
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(DataBase), "Save")]
    //class SavePatch
    //{
    //    static void Postfix(DataBase __instance)
    //    {
    //        // 这里添加你要执行的代码
    //        Console.WriteLine("保存2 " + __instance);
    //    }
    //}

    //[HarmonyReversePatch]
    //[HarmonyPatch(typeof(DataBase), "Save")]
    //class SaveReversePatch
    //{
    //    static void BaseSave(DataBase __instance)
    //    {
    //        Console.WriteLine("保存1 "+__instance);
    //        __instance.Save();
    //    }
    //}
}
