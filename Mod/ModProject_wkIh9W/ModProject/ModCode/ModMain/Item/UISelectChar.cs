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
    // 选择气运
    public class UISelectChar : MonoBehaviour
    {
        public UISelectChar(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 角色id

        public static DataStruct<string, string>[] allItems;
        public static DataStruct<string, string>[] findItems;
        public static DataStruct<string, string>[] selItems;
        public DataStruct<string, string> selectItem;

        public Transform leftRoot;
        public Transform rightRoot;
        public GameObject goItem;
        public Text textTitle;
        public Text leftTitle;
        public Text rightTitle;
        public Button btnOk;
        public Button btnClose;

        public Text textPage;
        public Button btnLastPage;
        public Button btnNextPage;

        public int pageIndex = 0;
        public int pageShowCount = 36;
        public int pageMax = 0;

        public static InputField inputFind;

        public static string lastFinxStr = "";
        public static string finxStr = "";

        void Awake()
        {
            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");

            goItem = transform.Find("Item").gameObject;

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();
            leftTitle = transform.Find("Root/Text2").GetComponent<Text>();
            rightTitle = transform.Find("Root/Text3").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk.onClick.AddListener((Action)OnBtnOk);

            gameObject.AddComponent<UIFastClose>();

            goItem.GetComponent<Text>().color = Color.black;

            textTitle.text = "选择角色";
            leftTitle.text = "可从下面选择角色";
            rightTitle.text = "已选择的角色";

            textPage = transform.Find("Root/Text4").GetComponent<Text>();
            btnLastPage = transform.Find("Root/BtnL").GetComponent<Button>();
            btnNextPage = transform.Find("Root/BtnN").GetComponent<Button>();
            UISelectChar.inputFind = transform.Find("Root/InputField").GetComponent<InputField>();


            btnLastPage.onClick.AddListener((Action)(() =>
            {
                if (pageMax > 1)
                {
                    pageIndex--;
                    if (pageIndex < 0)
                    {
                        pageIndex = pageMax - 1;
                    }
                    UpdateUI();
                }
            }));
            btnNextPage.onClick.AddListener((Action)(() =>
            {
                if (pageMax > 1)
                {
                    pageIndex++;
                    if (pageIndex >= pageMax)
                    {
                        pageIndex = 0;
                    }
                    UpdateUI();
                }
            }));

            UISelectChar.finxStr = PlayerPrefs.GetString( "SelectOneCharfindStr", "");
            UISelectChar.lastFinxStr = UISelectChar.finxStr;
            UISelectChar.inputFind.text = UISelectChar.finxStr;
            pageIndex = PlayerPrefs.GetInt( "SelectOneCharpageIndex", 0);

            transform.Find("Root/BtnUpdate").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UISelectChar.findItems = null;
                UISelectChar.allItems = null;
                UISelectChar.Init(true);
                UpdateUI();
            }));


            UINpcSelectClass.InitSel(transform, () =>
            {
                UpdateSelect();
                UpdateUI();
            });
        }

        public static void UpdateSelect()
        {
            if (UINpcSelectClass.isOpenSel && !UINpcSelectClass.CheckAll())
            {
                List<DataStruct<string, string>> list = new List<DataStruct<string, string>>();
                foreach (var item in findItems)
                {
                    var unit = ModMain.cmdRun.GetUnit(item.t1);
                    if (unit != null)
                    {
                        if (UINpcSelectClass.CheckUnit(unit))
                        {
                            list.Add(item);
                        }
                    }
                }
                selItems = list.ToArray();
            }
            else
            {
                selItems = findItems;
            }
        }

        public static void Init(bool reRead)
        {
            if (reRead || UISelectChar.allItems == null)
            {
                var units = g.world.unit.allUnit;
                int count = units.Count + 5;
                Console.WriteLine("应获取角色 " + count);

                DataStruct<string, string>[] items = new DataStruct<string, string>[count];
                int index = 0;
                items[index++] = new DataStruct<string, string>("player", "玩家");
                items[index++] = new DataStruct<string, string>("unitA", "当前剧情单位A");
                items[index++] = new DataStruct<string, string>("unitB", "当前剧情单位B");
                items[index++] = new DataStruct<string, string>("lookUnit", "正在查看的单位");
                items[index++] = new DataStruct<string, string>("playerWife", "玩家的结婚对象");
                foreach (var item in units)
                {
                    items[index++] = new DataStruct<string, string>(item.Key, item.value.data.unitData.propertyData.GetName());
                }
                UISelectChar.allItems = items.ToArray();

                UpdateFind();
            }
        }

        public static void UpdateFind()
        {
            lastFinxStr = inputFind.text;
            finxStr = inputFind.text;
            FindTool findTool = new FindTool();
            findTool.SetFindStr(inputFind.text);
            if (findTool.findStr.Length == 0)
            {
                UISelectChar.findItems = UISelectChar.allItems;
            }
            else
            {
                List<DataStruct<string, string>> list = new List<DataStruct<string, string>>(UISelectChar.allItems);
                list.RemoveAll((v) => !findTool.CheckFind(v.t2) && !v.t1.Contains(finxStr));
                UISelectChar.findItems = list.ToArray();
            }
            UpdateSelect();
        }

        void Update()
        {
            try
            {
                if (UISelectChar.finxStr != UISelectChar.inputFind.text && UISelectChar.lastFinxStr != UISelectChar.inputFind.text)
                {
                    CancelInvoke();
                    UISelectChar.finxStr = UISelectChar.inputFind.text;
                    Invoke("DelayUpdateUI", 0.5f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(name + " Update " + e.ToString());
            }

        }

        void DelayUpdateUI()
        {
            if (UISelectChar.lastFinxStr != UISelectChar.inputFind.text && UISelectChar.finxStr == UISelectChar.inputFind.text)
            {
                UISelectChar.UpdateFind();
                UpdateUI();
            }
        }

        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            UISelectChar.Init(false);
            UpdateUI();
        }

        void UpdateUI()
        {
            var list = UISelectChar.selItems;
            UnityAPIEx.DestroyChild(rightRoot);
            pageMax = Mathf.CeilToInt(list.Length * 1f / pageShowCount);
            if (pageIndex >= pageMax)
            {
                pageIndex = 0;
            }
            for (int i = pageIndex * pageShowCount; i < (pageIndex + 1) * pageShowCount; i++)
            {
                if (i >= list.Length)
                    break;
                var selectItem = list[i];
                var name = GameTool.LS(selectItem.t2);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
                go.AddComponent<UISkyTipEffect>().InitData(Tool.UnitTip(selectItem.t1));
            }
            textPage.text = $"{pageIndex + 1}/{pageMax}";
        }


        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = GameTool.LS(selectItem.t2);
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(leftRoot);
                selectItem = null;
            }));
            go.SetActive(true);
            go.AddComponent<UISkyTipEffect>().InitData(Tool.UnitTip(selectItem.t1));
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择单位！");
                return;
            }
            call(selectItem.t1, selectItem.t2);
            CloseUI();
        }


        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());

            PlayerPrefs.SetString( "SelectOneCharfindStr", UISelectChar.finxStr);
            PlayerPrefs.SetInt( "SelectOneCharpageIndex", pageIndex);

            PlayerPrefs.SetInt("SelectOneCharselTgl", UINpcSelectClass.isOpenSel ? 1 : 0);
        }
    }
}
