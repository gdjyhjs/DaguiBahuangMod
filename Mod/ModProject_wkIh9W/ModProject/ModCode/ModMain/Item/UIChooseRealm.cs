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
    // 选择境界
    public class UIChooseRealm : MonoBehaviour
    {
        public UIChooseRealm(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符，

        public static DataStruct<string, string>[] allAttr = new DataStruct<string, string>[]{
            new DataStruct<string, string>("0", "练气前期"),
            new DataStruct<string, string>("1", "练气中期"),
            new DataStruct<string, string>("2", "练气后期"),
            new DataStruct<string, string>("3", "筑基前期"),
            new DataStruct<string, string>("4", "筑基中期"),
            new DataStruct<string, string>("5", "筑基后期"),
            new DataStruct<string, string>("6", "结晶前期"),
            new DataStruct<string, string>("7", "结晶中期"),
            new DataStruct<string, string>("8", "结晶后期"),
            new DataStruct<string, string>("9", "金丹前期"),
            new DataStruct<string, string>("10", "金丹中期"),
            new DataStruct<string, string>("11", "金丹后期"),
            new DataStruct<string, string>("12", "具灵前期"),
            new DataStruct<string, string>("13", "具灵中期"),
            new DataStruct<string, string>("14", "具灵后期"),
            new DataStruct<string, string>("15", "元婴前期"),
            new DataStruct<string, string>("16", "元婴中期"),
            new DataStruct<string, string>("17", "元婴后期"),
            new DataStruct<string, string>("18", "化神前期"),
            new DataStruct<string, string>("19", "化神中期"),
            new DataStruct<string, string>("20", "化神后期"),
            new DataStruct<string, string>("21", "悟道前期"),
            new DataStruct<string, string>("22", "悟道中期"),
            new DataStruct<string, string>("23", "悟道后期"),
            new DataStruct<string, string>("24", "羽化前期"),
            new DataStruct<string, string>("25", "羽化中期"),
            new DataStruct<string, string>("26", "羽化后期"),
            new DataStruct<string, string>("27", "登仙前期"),
            new DataStruct<string, string>("28", "登仙中期"),
            new DataStruct<string, string>("29", "登仙后期"),
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

            textTitle.text = "选择境界";
            leftTitle.text = "可从下面选择境界";
            rightTitle.text = "已选择的境界";
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
                    this.selectItem=(selectItem);
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
            var name = selectItem.t2;
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                selectItem=null;
                UpdateLeft();
            }));
            go.SetActive(true);
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请选择1个境界！");
                return;
            }
            call(selectItem.t1.ToString(), selectItem.t2);
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
