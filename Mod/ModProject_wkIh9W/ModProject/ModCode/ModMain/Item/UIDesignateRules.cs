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
    // 指定门规
    public class UIDesignateRules : MonoBehaviour
    {
        public UIDesignateRules(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 门规id


        public ConfSchoolSloganItem[] allItems
        {
            get
            {
                return g.conf.schoolSlogan._allConfList.ToArray();
            }
        }
        public List<ConfSchoolSloganItem> selectItem = new List<ConfSchoolSloganItem>();



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

            textTitle.text = "选择门规";
            leftTitle.text = "可从下面选择门规";
            rightTitle.text = "已选择的门规";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            foreach (var item in allItems)
            {
                if (item.type != 2 || item.effect == "0" ||
                    string.IsNullOrWhiteSpace(GameTool.LS(item.desc)) || string.IsNullOrWhiteSpace(GameTool.LS(item.slogan)))
                {
                    continue;
                }

                var selectItem = item;
                var name = GameTool.LS(item.slogan) + ":" + GameTool.LS(item.desc);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    if (this.selectItem.Count >= 2)
                    {
                        UITipItem.AddTip("最多选择2个门规！");
                        return;
                    }
                    if (this.selectItem.Contains(selectItem))
                    {
                        UITipItem.AddTip("最多选择不同的门规！");
                    }
                    this.selectItem.Add(selectItem);
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
            for (int i = 0; i < selectItem.Count; i++)
            {
                var index = i;
                var leftItem = selectItem[i];
                var name = GameTool.LS(leftItem.slogan) + ":" + GameTool.LS(leftItem.desc);
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
            if (selectItem.Count < 2)
            {
                UITipItem.AddTip("至少选择2个门规！");
                return;
            }
            List<string> data1 = new List<string>();
            List<string> data2 = new List<string>();
            foreach (var item in selectItem)
            {
                data1.Add(item.id.ToString());
                data2.Add(GameTool.LS(item.slogan));
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
