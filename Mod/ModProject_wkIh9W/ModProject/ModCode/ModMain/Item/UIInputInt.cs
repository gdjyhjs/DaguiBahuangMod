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
    // 输入属性
    public class UIInputInt : MonoBehaviour
    {
        public UIInputInt(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 词条id

        public Transform AttrRoot;
        public GameObject attrItem;
        public Text textTitle;
        public Button btnClose;
        public Button btnOk;
        void Awake()
        {
            AttrRoot = transform.Find("Root/AttrList");

            attrItem = transform.Find("AttrItem").gameObject;

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk.onClick.AddListener((Action)OnBtnOk);

            gameObject.AddComponent<UIFastClose>();

            var prefab = g.res.Load<GameObject>("UI/Item/CostArrtItem");
            var prefabText = prefab.transform.Find("Root/Item/Text/Text").GetComponent<TMPro.TextMeshProUGUI>();

            textTitle.text = "输入属性";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, string[] attrName)
        {
            for (int i = 0; i < attrName.Length; i++)
            {
                var idx = i + 3;

                var go = GameObject.Instantiate(attrItem, AttrRoot);
                go.transform.Find("Placeholder").GetComponent<Text>().text = attrName[i];
                go.transform.Find("Name").GetComponent<Text>().text = attrName[i]+":";
                go.SetActive(true);
            }
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void OnBtnOk()
        {
            List<string> data = new List<string>();
            for (int i = 0; i < AttrRoot.childCount; i++)
            {
                var child = AttrRoot.GetChild(i);
                var input = child.GetComponentInChildren<InputField>();
                data.Add(input.text);
            }
            call(string.Join(",", data), data.Count.ToString());
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
