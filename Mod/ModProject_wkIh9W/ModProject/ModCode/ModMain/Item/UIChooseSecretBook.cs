using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace MOD_wkIh9W.Item
{
    // 选择秘籍
    public class UIChooseSecretBook : MonoBehaviour
    {
        public UIChooseSecretBook(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 秘籍

        public static DataStruct<string, string, string>[] cache_allItems;
        public static DataStruct<string, string, string>[] allItems
        {
            get
            {
                if (cache_allItems == null)
                {
                    List<DataStruct<string, string, string>> allMartialItems =
                        new List<DataStruct<string, string, string>>();

                    foreach (var item in g.conf.battleSkillAttack._allConfList)
                    {
                        if (item.className.Split('|').Contains("1"))
                        {
                            MartialType martialType = ConfBattleSkillAttackItemEx.martialType(item);
                            allMartialItems.Add(new DataStruct<string, string, string>
                                ((int)martialType + "", item.id + "", $"{GameTool.LS(item.name)}"));
                        }
                    }

                    foreach (var item in g.conf.battleStepBase._allConfList)
                    {
                        if (item.className.Split('|').Contains("1"))
                        {
                            allMartialItems.Add(new DataStruct<string, string, string>
                                ((int)MartialType.Step + "", item.id + "", $"{GameTool.LS(item.name)}"));
                        }
                    }

                    foreach (var item in g.conf.battleAbilityBase._allConfList)
                    {
                        if (item.className.Contains(1))
                        {
                            string name = $"{GameTool.LS(item.name)}";
                            if (item.className.Contains(101))
                            {
                                name += $"（刀）";
                            }
                            else if (item.className.Contains(102))
                            {
                                name += $"（枪）";
                            }
                            else if (item.className.Contains(103))
                            {
                                name += $"（剑）";
                            }
                            else if (item.className.Contains(104))
                            {
                                name += $"（券）";
                            }
                            else if (item.className.Contains(105))
                            {
                                name += $"（掌）";
                            }
                            else if (item.className.Contains(106))
                            {
                                name += $"（指）";
                            }
                            else if (item.className.Contains(111))
                            {
                                name += $"（火）";
                            }
                            else if (item.className.Contains(112))
                            {
                                name += $"（水）";
                            }
                            else if (item.className.Contains(113))
                            {
                                name += $"（雷）";
                            }
                            else if (item.className.Contains(114))
                            {
                                name += $"（风）";
                            }
                            else if (item.className.Contains(115))
                            {
                                name += $"（土）";
                            }
                            else if (item.className.Contains(116))
                            {
                                name += $"（木）";
                            }

                            allMartialItems.Add(new DataStruct<string, string, string>
                                ((int)MartialType.Ability + "", item.id + "", $"{name}"));
                        }
                    }

                    cache_allItems = allMartialItems.ToArray();
                }
                return cache_allItems;
            }
        }
        public DataStruct<string, string, string> selectItem;



        public Transform leftRoot;
        public Transform rightRoot;
        public Transform typeRoot;
        public GameObject goItem;
        public GameObject typeItem;
        public Text textTitle;
        public Text leftTitle;
        public Text rightTitle;
        public Button btnClose;
        public Button btnOk;
        void Awake()
        {
            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");
            typeRoot = transform.Find("Root/typeRoot");

            goItem = transform.Find("Item").gameObject;
            typeItem = transform.Find("typeItem").gameObject;

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();
            leftTitle = transform.Find("Root/Text2").GetComponent<Text>();
            rightTitle = transform.Find("Root/Text3").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk.onClick.AddListener((Action)OnBtnOk);

            gameObject.AddComponent<UIFastClose>();

            goItem.GetComponent<Text>().color = Color.black;

            textTitle.text = "选择秘籍";
            leftTitle.text = "可从下面选择秘籍";
            rightTitle.text = "已选择的秘籍";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            for (int i = 0; i < allItems.Length; i++)
            {
                var selectItem = allItems[i];
                var name = selectItem.t3;

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
                AddTip(go, selectItem);
            }

            var goType = GameObject.Instantiate(typeItem, typeRoot);
            goType.GetComponentInChildren<Text>().text = "所有秘籍";
            goType.SetActive(true);
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = selectItem.t3;
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(leftRoot);
                selectItem = null;
            }));
            go.SetActive(true);
            AddTip(go, selectItem);
        }

        public void AddTip(GameObject go, DataStruct<string, string, string> selectItem)
        {
            try
            {
                var uiEvent = go.AddComponent<UIEventListener>();
                uiEvent.onMouseEnterCall += (Action)(() => {
                    int type = int.Parse(selectItem.t1);
                    int baseID = int.Parse(selectItem.t2);
                    int[] data = new int[] { baseID, g.world.playerUnit.data.dynUnitData.GetGrade() };
                    var propsData = DataProps.PropsData.NewMartial((MartialType)type, data);
                    g.ui.OpenUI<UIMartialPropPreview>(UIType.MartialPropPreview).InitData(g.world.playerUnit,
                        propsData.To<DataProps.MartialData>(), go.transform.position);
                });
                uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.MartialPropPreview); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择秘籍！");
                return;
            }
            call(selectItem.t1 + "," + selectItem.t2, selectItem.t3);
            CloseUI();
        }


        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());
        }
    }
}
