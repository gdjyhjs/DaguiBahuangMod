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
    // 指定宗门属性
    public class UIDesignateAttribute : MonoBehaviour
    {
        public UIDesignateAttribute(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符(宗门属性)

        public static DataStruct<string, string>[] allAttr = new DataStruct<string, string>[]{
            new DataStruct<string, string>("1", "安定度"),
            new DataStruct<string, string>("2", "繁荣度"),
            new DataStruct<string, string>("3", "记名弟子"),
            new DataStruct<string, string>("4", "名誉值"),
            new DataStruct<string, string>("5", "忠诚度"),
            new DataStruct<string, string>("7", "灵石"),
            new DataStruct<string, string>("8", "药材"),
            new DataStruct<string, string>("9", "矿材"),
            new DataStruct<string, string>("10", "宗门名"),
            new DataStruct<string, string>("11", "分舵名"),
            //new DataStruct<string, string>("12", "立场"),
            //new DataStruct<string, string>("13", "灵阁逆天改命"),
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
        public DataStruct<string, string> selectItem;


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

            textTitle.text = "选择宗门属性";
            leftTitle.text = "可从下面选择宗门属性";
            rightTitle.text = "已选择的宗门属性";
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
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
            }
            
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
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择宗门属性！");
                return;
            }
            call(selectItem.t1, selectItem.t2);
            CloseUI();
        }

        void Start()
        {

        }
    }
}
