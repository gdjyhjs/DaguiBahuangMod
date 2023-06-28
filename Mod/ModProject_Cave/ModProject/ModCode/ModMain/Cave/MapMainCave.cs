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
            var root = ui.task.tglShow.transform;
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
            Cave.Log("定居按钮 ");
            var root = ui.task.tglShow.transform;
            var bg = CreateUI.NewImage();
            bg.name = "btnCave";
            bg.transform.SetParent(root, false);
            RectTransform rtf_bg = bg.GetComponent<RectTransform>();
            rtf_bg.sizeDelta = new Vector2(200, 100);
            bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            var ui_event = bg.AddComponent<UIEventListener>();
            rtf_bg.anchoredPosition = new Vector2(4, -342); 
            Action onEnter = () =>
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_bg);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(-30, rtf_bg.anchoredPosition.y), 0.2f);
                DG.Tweening.TweenSettingsExtensions.OnComplete(tween, new Action(() =>
                {
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(4, rtf_bg.anchoredPosition.y), 0.05f);
                }));
            };
            Action onExit = () =>
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_bg);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(144, rtf_bg.anchoredPosition.y), 0.2f);
            };
            ui_event.onMouseEnter.AddListener(onEnter);
            ui_event.onMouseExit.AddListener(onExit);

            var go = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shenshoujiaohuanniu_2"));
            go.transform.SetParent(bg.transform, false);

            var go2 = CreateUI.NewText(GameTool.LS("Cave_Dingju"), go.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<UnityEngine.UI.Text>().fontSize = 30;
            go2.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
            Action action = () =>
            {
                Vector2Int pos = g.world.playerUnit.data.unitData.GetPoint();
                MapBuildBase build = g.world.build.GetBuild(pos);
                if (build != null)
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_DingjuShibai1"), 1);
                    return;
                }
                MapEventBase mapEvent = g.world.mapEvent.GetGridEvent(pos);
                if (mapEvent != null)
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_DingjuShibai2"), 1);
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
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_OkCavePosi"), 1);
                        DramaFunction.UpdateMapAllUI();
                        return;
                    }
                    else
                    {
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_DingjuShibai2"), 1);
                        return;
                    }
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_QueDingCavePosi"), 2, action2);

            };
            go.AddComponent<Button>().onClick.AddListener(action);
        }

        public void InitGoHome(UIMapMain ui)
        {
            //Cave.Log("回家按钮");
            var root = ui.task.tglShow.transform;
            var bg = CreateUI.NewImage();
            bg.name = "btnCave";
            bg.transform.SetParent(root, false);
            RectTransform rtf_bg = bg.GetComponent<RectTransform>();
            rtf_bg.sizeDelta = new Vector2(200, 100);
            bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            var ui_event = bg.AddComponent<UIEventListener>();
            rtf_bg.anchoredPosition = new Vector2(4, -342);
            Action onEnter = () =>
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_bg);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(-30, rtf_bg.anchoredPosition.y), 0.2f);
                DG.Tweening.TweenSettingsExtensions.OnComplete(tween, new Action(() =>
                {
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(4, rtf_bg.anchoredPosition.y), 0.05f);
                }));
            };
            Action onExit = () =>
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_bg);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(144, rtf_bg.anchoredPosition.y), 0.2f);
            };
            ui_event.onMouseEnter.AddListener(onEnter);
            ui_event.onMouseExit.AddListener(onExit);

            var go = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shenshoujiaohuanniu_2"));
            go.transform.SetParent(bg.transform, false);

            var go2 = CreateUI.NewText(GameTool.LS("Cave_GoHome"), go.GetComponent<RectTransform>().sizeDelta);
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