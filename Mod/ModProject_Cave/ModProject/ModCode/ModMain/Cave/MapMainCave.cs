using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using DG.Tweening;

namespace Cave
{
    public class MapMainCave
    {
        BtnData btnData = new BtnData();
        RectTransform rtf_bg;
        RectTransform rtf_btn;
        bool isInit = true;
        bool isEnter = false;
        string dataKey = "www.yellowshange.com.cava.btnData";

        public MapMainCave(UIMapMain ui)
        {
            var root = ui.task.tglShow.transform;
            var btn = root.Find("btnCave");
            if (btn != null)
            {
                GameObjectTags.Destroy(btn.gameObject);
            }

            DataCave data = DataCave.ReadData();
            Cave.Log("洞府状态：" + data.state);
            if (data.state == 1)
            {
                InitCreateHome(ui);
            }
            else if (data.state == 2)
            {
                data.InitCave();
                InitGoHome(ui);
            }
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
            if (PlayerPrefs.HasKey(dataKey))
            {
                btnData = JsonConvert.DeserializeObject<BtnData>(PlayerPrefs.GetString(dataKey));
            }

            Cave.Log("回家按钮");
            var root = ui.task.goGroupRoot.transform;
            var bg = CreateUI.NewImage();
            bg.name = "btnCave";
            bg.transform.SetParent(root, false);
            rtf_bg = bg.GetComponent<RectTransform>();
            rtf_bg.sizeDelta = new Vector2(120, 120);
            bg.GetComponent<Image>().color = new Color(0, 0, 0, 0);


            var go = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shenshoujiaohuanniu_2"));
            go.transform.SetParent(bg.transform, false);
            rtf_btn = go.GetComponent<RectTransform>();

            var go2 = CreateUI.NewText(GameTool.LS("Cave_GoHome"), go.GetComponent<RectTransform>().sizeDelta);
            go2.transform.SetParent(go.transform, false);
            go2.GetComponent<UnityEngine.UI.Text>().fontSize = 30;
            go2.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;

            GameObject down, right, left, up;
            {
                var tmpGo = CreateUI.New();
                tmpGo.transform.SetParent(root, false);
                var rectTransform = tmpGo.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(1, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                right = tmpGo;
            }
            {
                var tmpGo = CreateUI.New();
                tmpGo.transform.SetParent(root, false);
                var rectTransform = tmpGo.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                down = tmpGo;
            }
            {
                var tmpGo = CreateUI.New();
                tmpGo.transform.SetParent(root, false);
                var rectTransform = tmpGo.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                left = tmpGo;
            }
            {
                var tmpGo = CreateUI.New();
                tmpGo.transform.SetParent(root, false);
                var rectTransform = tmpGo.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                up = tmpGo;
            }
            {
                var ui_event = bg.AddComponent<UIEventListener>();
                Action onEnter = () =>
                {
                    isEnter = true;
                    UpdateBtnMove();
                };
                Action onExit = () =>
                {
                    isEnter = false;
                    UpdateBtnMove();
                };
                ui_event.onMouseEnter.AddListener(onEnter);
                ui_event.onMouseExit.AddListener(onExit);


                var ui_drag = bg.AddComponent<UIEventListenerDrag>();

                Action onBeginDrag = () =>
                {
                    rtf_bg.anchorMin = new Vector2(0.5f, 0.5f);
                    rtf_bg.anchorMax = new Vector2(0.5f, 0.5f);
                    rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                };
                Action<PointerEventData> onDrag = (eventData) =>
                {
                    var size = g.ui.canvas.GetComponent<RectTransform>().sizeDelta;
                    var pos = new Vector2(size.x * (eventData.position.x / Screen.width - 0.5f), size.y * (eventData.position.y / Screen.height - 0.5f));
                    rtf_bg.anchoredPosition = pos;
                };
                Action onEndDrag = () =>
                {
                    Console.WriteLine($"拖拽结束 {right} {down} {left} {up}");
                    var pos = rtf_bg.position;
                    rtf_bg.anchorMin = new Vector2(0, 0);
                    rtf_bg.anchorMax = new Vector2(0, 0);
                    rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                    rtf_bg.position = pos;
                    btnData.dir = 0;
                    Console.WriteLine("拖拽结束a");
                    var leftPos = left.transform.position.x;
                    var rightPos = right.transform.position.x;
                    var upPos = up.transform.position.y;
                    var downPos = down.transform.position.y;
                    Console.WriteLine($"{rightPos} {downPos} {leftPos} {upPos} {rtf_bg.position.x} {rtf_bg.position.y}");
                    float value = Mathf.Abs(right.transform.position.x - rtf_bg.position.x);
                    btnData.pos = (rtf_bg.position.y - downPos) / (upPos - downPos);
                    if (Mathf.Abs(down.transform.position.y - rtf_bg.position.y) < value)
                    {
                        Console.WriteLine("拖拽结束b");
                        value = Mathf.Abs(down.transform.position.y - rtf_bg.position.y);
                        btnData.dir = 1;
                        btnData.pos = (rtf_bg.position.x - leftPos) / (rightPos - leftPos);
                    }
                    if (Mathf.Abs(left.transform.position.x - rtf_bg.position.x) < value)
                    {
                        Console.WriteLine("拖拽结束c");
                        value = Mathf.Abs(left.transform.position.x - rtf_bg.position.x);
                        btnData.dir = 2;
                        btnData.pos = (rtf_bg.position.y - downPos) / (upPos - downPos);
                    }
                    if (Mathf.Abs(up.transform.position.y - rtf_bg.position.y) < value)
                    {
                        Console.WriteLine("拖拽结束d");
                        value = Mathf.Abs(up.transform.position.y - rtf_bg.position.y);
                        btnData.dir = 3;
                        btnData.pos = (rtf_bg.position.x - leftPos) / (rightPos - leftPos);
                    }
                    UpdateBtnPos();
                };
                ui_drag.onBeginDragCall += onBeginDrag;
                ui_drag.onDragCall += onDrag;
                ui_drag.onEndDragCall += onEndDrag;




                Action action = () =>
                {
                    DataCave caveData = DataCave.ReadData();
                    g.world.playerUnit.CreateAction(new UnitActionSetPoint(new Vector2Int(caveData.x, caveData.y)));
                    DramaFunction.UpdateMapAllUI();
                };
                go.AddComponent<Button>().onClick.AddListener(action);

                //go.AddComponent<UISkyTipEffect>().InitData("拖动可改变按钮所在的位置\n按下G可快速点击按钮\nCtrl+G重置按钮位置\nAlt+G弹出按钮");
            }
            Cave.Log("回家按钮ok " + bg.activeInHierarchy);
            UpdateBtnPos();
            isInit = false;
        }

        private void UpdateBtnMove()
        {
            Vector2 enterPos = default, exitPos = default, movePos = default;
            float enterValue = 60, exitValue = 45, moveValue = 100;
            if (btnData.dir == 0)
            {
                enterPos = new Vector2(-enterValue, 0);
                exitPos = new Vector2(exitValue, 0);
                movePos = new Vector2(-moveValue, 0);
            }
            else if (btnData.dir == 1)
            {
                enterPos = new Vector2(0, enterValue);
                exitPos = new Vector2(0, -exitValue);
                movePos = new Vector2(0, moveValue);
            }
            else if (btnData.dir == 2)
            {
                enterPos = new Vector2(enterValue, 0);
                exitPos = new Vector2(-exitValue, 0);
                movePos = new Vector2(moveValue, 0);
            }
            else if (btnData.dir == 3)
            {
                enterPos = new Vector2(0, -enterValue);
                exitPos = new Vector2(0, exitValue);
                movePos = new Vector2(0, -moveValue);
            }

            if (isEnter)
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_btn);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_btn, movePos, 0.2f);
                DG.Tweening.TweenSettingsExtensions.OnComplete(tween, new Action(() =>
                {
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_btn, enterPos, 0.05f);
                }));
            }
            else
            {
                DG.Tweening.ShortcutExtensions.DOKill(rtf_btn);
                var tween = DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_btn, exitPos, 0.2f);
            }
        }

        private void UpdateBtnPos()
        {
            Console.WriteLine("更新位置");
            DG.Tweening.ShortcutExtensions.DOKill(rtf_bg);
            Console.WriteLine("dir=" + btnData.dir);
            Console.WriteLine("pos=" + btnData.pos);
            var size = g.ui.canvas.GetComponent<RectTransform>().sizeDelta;
            var pos = rtf_bg.position;
            if (btnData.dir == 3)
            {
                rtf_bg.anchorMin = new Vector2(0.5f, 1);
                rtf_bg.anchorMax = new Vector2(0.5f, 1);
                rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                rtf_bg.position = pos;
                if (isInit)
                    rtf_bg.anchoredPosition = new Vector2(0, 0);
                else
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(size.x * (btnData.pos - 0.5f), 0), 0.2f);
            }
            if (btnData.dir == 2)
            {
                rtf_bg.anchorMin = new Vector2(0, 0.5f);
                rtf_bg.anchorMax = new Vector2(0, 0.5f);
                rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                rtf_bg.position = pos;
                if (isInit)
                    rtf_bg.anchoredPosition = new Vector2(0, 0);
                else
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(0, size.y * (btnData.pos - 0.5f)), 0.2f);
            }
            if (btnData.dir == 1)
            {
                rtf_bg.anchorMin = new Vector2(0.5f, 0);
                rtf_bg.anchorMax = new Vector2(0.5f, 0);
                rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                rtf_bg.position = pos;
                if (isInit)
                    rtf_bg.anchoredPosition = new Vector2(0, 0);
                else
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(size.x * (btnData.pos - 0.5f), 0), 0.2f);
            }
            if (btnData.dir == 0)
            {
                rtf_bg.anchorMin = new Vector2(1, 0.5f);
                rtf_bg.anchorMax = new Vector2(1, 0.5f);
                rtf_bg.pivot = new Vector2(0.5f, 0.5f);
                rtf_bg.position = pos;
                if (isInit)
                    rtf_bg.anchoredPosition = new Vector2(0, 0);
                else
                    DG.Tweening.ShortcutExtensions46.DOAnchorPos(rtf_bg, new Vector2(0, size.y * (btnData.pos - 0.5f)), 0.2f);
            }
            UpdateBtnMove();
            PlayerPrefs.SetString(dataKey, JsonConvert.SerializeObject(btnData));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    // 重置
                    btnData = new BtnData();
                    UpdateBtnPos();
                }
                else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    // 进入
                    isEnter = !isEnter;
                    UpdateBtnMove();
                }
                else
                {
                    // 点击
                    rtf_btn.GetComponent<Button>().onClick?.Invoke();
                }
            }
        }

        class BtnData
        {
            public int dir = 0; // 右 下 左 上 0 1 2 3
            public float pos = 0.5f; // 位置 0 - 1
        }
    }
}