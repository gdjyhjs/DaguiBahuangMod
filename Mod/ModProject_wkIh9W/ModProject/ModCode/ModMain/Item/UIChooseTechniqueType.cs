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
    // 选择功法类型
    public class UIChooseTechniqueType : MonoBehaviour
    {
        public UIChooseTechniqueType(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符，

        public static DataStruct<string, string>[] allAttr = new DataStruct<string, string>[]{
            new DataStruct<string, string>("basBlade", "刀"),
            new DataStruct<string, string>("basSpear", "枪"),
            new DataStruct<string, string>("basSword", "剑"),
            new DataStruct<string, string>("basFist", "拳"),
            new DataStruct<string, string>("basPalm", "掌"),
            new DataStruct<string, string>("basFinger", "指"),
            new DataStruct<string, string>("basFire", "火"),  
            new DataStruct<string, string>("basFroze", "水"),
            new DataStruct<string, string>("basThunder", "雷"),
            new DataStruct<string, string>("basWind", "风"),
            new DataStruct<string, string>("basEarth", "土"),
            new DataStruct<string, string>("basWood", "木"),
        };
        public static string GetAttrName(string attr)
        {
            foreach (var item in allAttr)
            {
                if (item.t1 == attr)
                {
                    return item.t2;
                }
            }
            return "";
        }
        public List<DataStruct<string, string>> selectItem = new List<DataStruct<string, string>>();



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

            textTitle.text = "选择功法类型";
            leftTitle.text = "可从下面选择功法类型";
            rightTitle.text = "已选择的功法类型";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            foreach (var item in allAttr)
            {
                var selectItem = item;
                var para = item.t1;
                var name = GameTool.LS(item.t2);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    if (this.selectItem.Contains(selectItem))
                    {
                        UITipItem.AddTip("这个属性已经选择过了！");
                        return;
                    }
                    this.selectItem.Add(selectItem);
                    UpdateLeft();
                }));
                go.SetActive(true);
            }

            var goType = GameObject.Instantiate(typeItem, typeRoot);
            goType.GetComponentInChildren<Text>().text = "所有功法类型";
            goType.SetActive(true);
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            for (int i = 0; i < selectItem.Count; i++)
            {
                var index = i;
                var leftItem = selectItem[i];
                var name = leftItem.t2;
                var go = GameObject.Instantiate(goItem, leftRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    selectItem.RemoveAt(index);
                    UpdateLeft();
                }));
                go.SetActive(true);

            }
        }

        public void OnBtnOk()
        {
            if (selectItem.Count < 1)
            {
                UITipItem.AddTip("至少选择1个功法类型！");
                return;
            }
            List<string> data1 = new List<string>();
            List<string> data2 = new List<string>();
            foreach (var item in selectItem)
            {
                data1.Add(item.t1);
                data2.Add(item.t2);
            }
            call(string.Join(",", data1), string.Join(",", data2));
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
