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
    public class TownCave
    {
        UITown uiTown;

        int idx, minIdx = 1, maxIdx = 3, price = 0;
        GameObject canvas;
        GameObject image;
        Text uiName;
        Text uiPrice;

        GameObject ui_bg;

        public TownCave(UITown ui)
        {
            this.uiTown = ui;
            Init();

            //// 寻找合适位置的代码
            ///

            //bool onInputX = true;
            //int inputX, inputY;
            //List<string> inputValue = new List<string>();
            //ui.AddCor(g.timer.Frame((Action)(() =>
            //{
            //    try
            //    {
            //        if (Input.GetKeyDown(KeyCode.X))
            //        {
            //            Console.WriteLine("X " + inputX + "," + inputY);
            //            inputValue.Clear();
            //            onInputX = true;
            //        }
            //        if (Input.GetKeyDown(KeyCode.Y))
            //        {
            //            Console.WriteLine("Y " + inputX + "," + inputY);
            //            inputValue.Clear();
            //            onInputX = false;
            //        }
            //        if (Input.GetKeyDown(KeyCode.J))
            //        {
            //            string str = string.Join("", inputValue);
            //            if (onInputX)
            //            {
            //                int.TryParse(str, out inputX);
            //            }
            //            else
            //            {
            //                int.TryParse(str, out inputY);
            //            }
            //            Console.WriteLine(inputX + "," + inputY);
            //            if (inputX != 0 && inputY != 0)
            //            {
            //                ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(inputX, inputY);
            //            }
            //        }
            //        KeyCode[] inputKey = new KeyCode[] { KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2 , KeyCode.Alpha3 , KeyCode.Alpha4 , KeyCode.Alpha5 ,
            //        KeyCode.Alpha6, KeyCode.Alpha7 , KeyCode.Alpha8 , KeyCode.Alpha9, KeyCode.Minus };

            //        for (int i = 0; i < inputKey.Length; i++)
            //        {
            //            var key = inputKey[i];
            //            if (Input.GetKeyDown(key))
            //            {
            //                if (key == KeyCode.Minus)
            //                {
            //                    inputValue.Add("-");
            //                }
            //                else
            //                {
            //                    inputValue.Add(i.ToString());
            //                }
            //                Console.WriteLine("input:" + string.Join("", inputValue));
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.ToString());
            //    }
            //}), 1, true));
        }


        public void Init()
        {
            if (uiTown.imgBG.transform.Find("caveHome") != null)
            {
                GameObject.Destroy(uiTown.imgBG.transform.Find("caveHome").gameObject);
            }

            ui_bg = CreateUI.NewImage(SpriteTool.GetSprite("TownCommon", "Build2005"));
            ui_bg.name = "caveHome";
            ui_bg.transform.SetParent(uiTown.imgBG.transform, false);
            var areaid = g.data.grid.GetGridData(g.world.playerUnit.data.unitData.GetPoint()).areaBaseID;
            if (areaid >= 9)
            {
                ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(630, 1);
            }
            else if (areaid >= 8)
            {
                ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(660, -160);
            }
            else if (areaid >= 6)
            {
                ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(475, 85);
            }
            else
            {
                ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-50, 80);
            }
            //ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(CommonTool.Random(-900, -600), -239.5f);
            System.Action action = () =>
            {
                if (DataCave.ReadData().state == 1)
                {
                    Cave.OpenDrama(GameTool.LS("Cave_BuyDram1"));
                }
                else if (DataCave.ReadData().state == 2)
                {
                    Cave.OpenDrama(GameTool.LS("Cave_BuyDram2"), () =>
                    {
                        Action action2 = () =>
                        {
                            // 要搬家
                            MoveCave();
                        };
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_BuyDram3"), 2, action2);
                    });
                }
                else
                {
                    Cave.OpenDrama(GameTool.LS("Cave_BuyDram4"), () =>
                    {
                        Action action2 = () =>
                        {
                        // 要选购洞府
                        idx = minIdx;
                            OpenSelect();
                        };
                        g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_BuyDram5"), 2, action2);
                    });
                }
            };
            ui_bg.AddComponent<Button>().onClick.AddListener(action);

            var ui_namebg = CreateUI.NewImage(SpriteTool.GetSprite("Common", "jianzhumingzibg"));
            ui_namebg.transform.SetParent(ui_bg.transform, false);
            ui_namebg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-65.8f, 10.2f);
            ui_namebg.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(30, 101);

            var ui_name = CreateUI.NewText(GameTool.LS("Cave_BuyDram6"), fontID: 1);
            ui_name.transform.SetParent(ui_bg.transform, false);
            ui_name.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-64f, 10.2f);
            ui_name.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(25, 101);
            ui_name.GetComponent<UnityEngine.UI.Text>().lineSpacing = 0.65f;

            Tools.AddScale(ui_bg);
        }

        // 打开选择洞府
        public void OpenSelect()
        {
            if (canvas != null)
            {
                GameObject.Destroy(canvas);
            }
            canvas = CreateUI.NewCanvas();


            GameObject image = CreateUI.NewImage(SpriteTool.GetSprite("Common", "tuichu"));
            image.transform.SetParent(canvas.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(775, 370);
            Action btnTuichu = () =>
            {
                Close();
            };
            image.AddComponent<Button>().onClick.AddListener(btnTuichu);


            image = CreateUI.NewImage(SpriteTool.GetSprite("NPCInfoCommon", "daoxinmingzikuang"));
            image.transform.SetParent(canvas.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 250);


            GameObject textGo = CreateUI.NewText("", image.GetComponent<RectTransform>().sizeDelta);
            textGo.transform.SetParent(canvas.transform, false);
            textGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 250);
            uiName = textGo.GetComponent<Text>();
            uiName.alignment = TextAnchor.MiddleCenter;
            uiName.color = Color.black;


            textGo = CreateUI.NewText("", image.GetComponent<RectTransform>().sizeDelta);
            textGo.transform.SetParent(canvas.transform, false);
            textGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 200);
            uiPrice = textGo.GetComponent<Text>();
            uiPrice.alignment = TextAnchor.MiddleCenter;
            uiPrice.color = Color.black;


            image = CreateUI.NewImage(SpriteTool.GetSprite("Common", "youjian"));
            image.transform.SetParent(canvas.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 250);
            Action btnYou = () =>
            {
                idx++;
                if (idx > maxIdx)
                    idx = minIdx;
                CreateBg();
            };
            image.AddComponent<Button>().onClick.AddListener(btnYou);



            image = CreateUI.NewImage(SpriteTool.GetSprite("Common", "zuojian"));
            image.transform.SetParent(canvas.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-730, 250);
            Action btnZuo = () =>
            {
                idx--;
                if (idx < minIdx)
                    idx = maxIdx;
                CreateBg();
            };
            image.AddComponent<Button>().onClick.AddListener(btnZuo);




            image = CreateUI.NewImage(SpriteTool.GetSprite("SchoolCommon", "shapananniu_2"));
            image.transform.SetParent(canvas.transform, false);
            image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 150);





            textGo = CreateUI.NewText(GameTool.LS("Cave_BuyDram7"), image.GetComponent<RectTransform>().sizeDelta);
            textGo.transform.SetParent(canvas.transform, false);
            textGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-665, 150);
            Text t = textGo.GetComponent<Text>();
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.black;
            Action btnOk = () =>
            {
                Close();
                Action buyAction = () =>
                {
                    BuyCave();
                }; 
                string str = string.Format(GameTool.LS("Cave_BuyDram8"), price, uiName.text);
                Cave.Log("提示文本-"+ GameTool.LS("Cave_BuyDram8")+"-"+price+"-"+ uiName);
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), str, 2, buyAction);

            };
            textGo.AddComponent<Button>().onClick.AddListener(btnOk);


            CreateBg();
        }

        public void CreateBg()
        {
            string[] names = new string[] { GameTool.LS("Cave_beijingName1"), GameTool.LS("Cave_beijingName2"), GameTool.LS("Cave_beijingName3"),  "荒山1", "高山1", "荒山2", "高山2","荒山3", "高山3",  "荒山4" , "高山4"};
            int[] prices = new int[] { 5000, 50000, 500000, 50000, 50000, 100000, 100000, 500000, 500000, 1000000, 1000000 };
            price = prices[idx - 1];
            uiName.text = names[idx - 1];
            uiPrice.text = GameTool.LS("Cave_Lingshi") + GameTool.LS("maohao") + price;

            if (image != null)
            {
                GameObject.Destroy(image);
            }
            if (idx > 3)
            {
                int id = idx - 4;
                int stand = id % 2;
                int level = id / 4;
                CreateBg2(stand, level);
            }
            else
            {
                CreateBg1(idx);
            }
        }

        public void CreateBg1(int level)
        {
            string[] path = new string[] { "chengzhenbg", "chengzhen3bg", "chengzhen2bg" };
            image =  CreateUI.NewImage(SpriteTool.GetSpriteBigTex("BG/" + path[level - 1]));
            image.transform.SetParent(canvas.transform, false);
            image.transform.SetAsFirstSibling();
        }

        public void CreateBg2(int stand, int level)
        {
            string path = "SchoolMainPage/" + (stand == 1 ? "zhengdao" : "modao") + "_" + (level + 1)+"/";
            image = CreateUI.NewImage(SpriteTool.GetSpriteBigTex(path + "houjing"));
            image.transform.SetParent(canvas.transform, false);
            image.transform.SetAsFirstSibling();

            var qianjing= CreateUI.NewImage(SpriteTool.GetSpriteBigTex(path + "qianjing"));
            qianjing.transform.SetParent(image.transform, false);
        }

        // 关闭界面
        public void Close()
        {
            GameObject.Destroy(canvas);
        }

        // 购买洞府
        public void BuyCave()
        {
            int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
            if (money < price)
            {
                Cave.OpenDrama(GameTool.LS("Cave_BuyDram0"));
            }
            else
            {
                g.world.playerUnit.data.CostPropItem(PropsIDType.Money, price);
                Cave.OpenDrama(GameTool.LS("Cave_BuyDram10"));
                DataCave data = DataCave.ReadData();
                data.state = 1;
                data.bg = idx;
                data.name = g.world.playerUnit.data.unitData.propertyData.GetName() + GameTool.LS("Cave_BuyDram11");

                DataCave.SaveData(data);

                UIMapMain ui = g.ui.GetUI<UIMapMain>(UIType.MapMain);
                if (ui != null)
                {
                    new MapMainCave (ui);
                }
            }
        }

        public void MoveCave()
        {
            DataCave data = DataCave.ReadData();
            data.state = 1;
            DataCave.SaveData(data);
            MapEventBase mapEvent = g.world.mapEvent.GetGridEvent(new UnityEngine.Vector2Int(data.x, data.y));
            g.world.mapEvent.DelGridEvent(mapEvent, true);

            DramaFunction.UpdateMapAllUI();

            UIMapMain ui = g.ui.GetUI<UIMapMain>(UIType.MapMain);
            if (ui != null)
            {
                new MapMainCave(ui);
            }
        }
    }
}