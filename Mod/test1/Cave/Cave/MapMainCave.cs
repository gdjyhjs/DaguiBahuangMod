using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave
{
    public class MapMainCave
    {

        public MapMainCave(UIMapMain ui)
        {
            var root = ui.task.goGroupRoot.transform.Find("LanguageGroup");
            var btn = root.Find("btnCave");
            if (btn != null)
            {
                GameObjectTags.Destroy(btn.gameObject);
            }

            DataCave data = DataCave.ReadData();
            if (data.state == 1)
            {
                InitCreateHome(ui);
            }
            else if (data.state == 2)
            {
                data.InitCave();
                InitGoHome(ui);
            }
            Cave.Log("洞府状态：" + data.state);
        }


        public void InitCreateHome(UIMapMain ui)
        {
            Cave.Log("定居按钮");
            var root = ui.task.goGroupRoot.transform.Find("LanguageGroup");

            var bg = CreateUI.NewImage();
            bg.name = "btnCave";
            bg.transform.SetParent(root, false);
            RectTransform rtf_bg = bg.GetComponent<RectTransform>();
            rtf_bg.sizeDelta = new Vector2(180, 100);
            bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            var ui_event = bg.AddComponent<UIEventListener>();
            float value = 940;
            float speed = 500;
            bool enter = true;
            rtf_bg.anchoredPosition = new Vector2(value, 80);
            Action onEnter = () =>
            {
                enter = true;
            };
            Action onExit = () =>
            {
                enter = false;
            };
            Action onFram = () =>
            {
                if (enter && value != 940)
                {
                    value -= Time.deltaTime * speed;
                    if (value < 940)
                    {
                        value = 940;
                    }
                    rtf_bg.anchoredPosition = new Vector2(value, rtf_bg.anchoredPosition.y);
                }
                else if (!enter && value != 1040)
                {
                    value += Time.deltaTime * speed;
                    if (value > 1040)
                    {
                        value = 1040;
                    }
                    rtf_bg.anchoredPosition = new Vector2(value, rtf_bg.anchoredPosition.y);
                }
            };
            ui_event.onMouseEnter.AddListener(onEnter);
            ui_event.onMouseExit.AddListener(onExit);
            ui.AddCor(g.timer.Frame(onFram, 1, true));

            var go = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shenshoujiaohuanniu_2"));
            go.transform.SetParent(bg.transform, false);

            var go2 = CreateUI.NewText("定居", go.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<UnityEngine.UI.Text>().fontSize = 30;
            go2.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
            Action action = () =>
            {
                Vector2Int pos = g.world.playerUnit.data.unitData.GetPoint();
                MapBuildBase build = g.world.build.GetBuild(pos);
                if (build != null)
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "此处有其他建筑啦，无法定居！", 1);
                    return;
                }
                MapEventBase mapEvent = g.world.mapEvent.GetGridEvent(pos);
                if (mapEvent != null)
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "此处无法定居！", 1);
                    return;
                }
                // 点击定居
                Action action2 = () =>
                {
                    MapEventBase ent = g.world.mapEvent.AddGridEvent(pos, 6).t1;
                    if (ent != null)
                    {
                        DataCave data2 = DataCave.ReadData();
                        data2.state = 2;
                        data2.x = pos.x;
                        data2.y = pos.y;
                        data2.InitCave();
                        DataCave.SaveData(data2);

                        UIMapMain ui2 = g.ui.GetUI<UIMapMain>(UIType.MapMain);
                        if (ui2 != null)
                        {
                            new MapMainCave(ui2);
                        }
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "贵客，洞府为您造好了，我们先撤了！", 1);
                        DramaFunction.UpdateMapAllUI();
                        return;
                    }
                    else
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "此处无法定居！", 1);
                        return;
                    }
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "确定在此定居吗？", 2, action2);

            };
            go.AddComponent<Button>().onClick.AddListener(action);
        }

        public void InitGoHome(UIMapMain ui)
        {
            Cave.Log("回家按钮");
            var root = ui.task.goGroupRoot.transform.Find("LanguageGroup");

            var bg = CreateUI.NewImage();
            bg.name = "btnCave";
            bg.transform.SetParent(root, false);
            RectTransform rtf_bg = bg.GetComponent<RectTransform>();
            rtf_bg.sizeDelta = new Vector2(180, 100);
            bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            var ui_event = bg.AddComponent<UIEventListener>();
            float min = 940, max = 1045;
            float value = 940;
            float speed = 500;
            bool enter = true;
            rtf_bg.anchoredPosition = new Vector2(value, 80);
            Action onEnter = () =>
            {
                enter = true;
            };
            Action onExit = () =>
            {
                enter = false;
            };
            Action onFram = () =>
            {
                if (enter && value != min)
                {
                    value -= Time.deltaTime * speed;
                    if (value < min)
                    {
                        value = min;
                    }
                    rtf_bg.anchoredPosition = new Vector2(value, rtf_bg.anchoredPosition.y);
                }
                else if (!enter && value != max)
                {
                    value += Time.deltaTime * speed;
                    if (value > max)
                    {
                        value = max;
                    }
                    rtf_bg.anchoredPosition = new Vector2(value, rtf_bg.anchoredPosition.y);
                }
            };
            ui_event.onMouseEnter.AddListener(onEnter);
            ui_event.onMouseExit.AddListener(onExit);
            ui.AddCor(g.timer.Frame(onFram, 1, true));

            var go = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shenshoujiaohuanniu_2"));
            go.transform.SetParent(bg.transform, false);

            var go2 = CreateUI.NewText("回家", go.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<UnityEngine.UI.Text>().fontSize = 30;
            go2.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
            Action action = () =>
            {
                DataCave data = DataCave.ReadData();
                g.world.playerUnit.CreateAction(new UnitActionSetPoint(new Vector2Int(data.x, data.y)));
                DramaFunction.UpdateMapAllUI();
            };
            go.AddComponent<Button>().onClick.AddListener(action);
        }

    }
}