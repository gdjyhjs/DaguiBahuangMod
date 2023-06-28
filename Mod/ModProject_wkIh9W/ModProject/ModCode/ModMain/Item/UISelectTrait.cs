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
    // 指定性格
    public class UISelectTrait : MonoBehaviour
    {
        public UISelectTrait(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 性格id


        public ConfRoleCreateCharacterItem[] allItems
        {
            get
            {
                return g.conf.roleCreateCharacter._allConfList.ToArray();
            }
        }
        public List<ConfRoleCreateCharacterItem> selectItem1 = new List<ConfRoleCreateCharacterItem>();
        public List<ConfRoleCreateCharacterItem> selectItem2 = new List<ConfRoleCreateCharacterItem>();



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
            Console.WriteLine("777");
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

            textTitle.text = "选择性格";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            foreach (var item in allItems)
            {
                var selectItem = item;
                var name = GameTool.LS(item.sc5asd_sd34);
                var root = item.type == 1 ? leftRoot : rightRoot;
                var go = GameObject.Instantiate(goItem, root);
                var list = item.type == 1 ? selectItem1 : selectItem2;

                go.GetComponentInChildren<Text>().text = name;
                go.GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        list.Add(selectItem);
                    }
                    else
                    {
                        list.Remove(selectItem);
                    }
                }));
                go.SetActive(true);
            }
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void OnBtnOk()
        {
            if (selectItem1.Count != 1)
            {
                UITipItem.AddTip("请选择1个内在性格！不能多也不能少！");
                return;
            }
            if (selectItem2.Count != 1)
            {
                UITipItem.AddTip("请选择2个外在性格！不能多也不能少！");
                return;
            }
            List<string> data1 = new List<string>();
            List<string> data2 = new List<string>();
            foreach (var item in selectItem1)
            {
                data1.Add(item.id.ToString());
                data2.Add(GameTool.LS(item.sc5asd_sd34));
            }
            foreach (var item in selectItem2)
            {
                data1.Add(item.id.ToString());
                data2.Add(GameTool.LS(item.sc5asd_sd34));
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
