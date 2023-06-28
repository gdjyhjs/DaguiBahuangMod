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
    // 选择正魔
    public class UIChooseRighteousnessOrEvil : MonoBehaviour
    {
        public UIChooseRighteousnessOrEvil(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符



        public Text textTitle;
        public Button btnClose;
        public Button btnOk1;
        public Button btnOk2;
        void Awake()
        {

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk1 = transform.Find("Root/BtnOk1").GetComponent<Button>();
            btnOk2 = transform.Find("Root/BtnOk2").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk1.onClick.AddListener((Action)(()=>
            {
                call?.Invoke("up", "正");
                CloseUI();
            }));
            btnOk2.onClick.AddListener((Action)(() =>
            {
                call?.Invoke("down", "魔");
                CloseUI();
            }));

            gameObject.AddComponent<UIFastClose>();

            textTitle.text = "选择正魔";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
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
