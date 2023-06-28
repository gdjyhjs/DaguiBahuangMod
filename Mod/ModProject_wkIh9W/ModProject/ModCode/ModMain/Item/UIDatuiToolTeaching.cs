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
    // 使用教程
    public class UIDatuiToolTeaching : MonoBehaviour
    {
        public UIDatuiToolTeaching(IntPtr ptr) : base(ptr) { }

        public Text textTitle;
        void Awake()
        {
            textTitle = transform.Find("Root/Text1").GetComponent<Text>();
            textTitle.text = "工具盒使用教程";

            gameObject.AddComponent<UIFastClose>();
            var btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnClose.onClick.AddListener((Action)CloseUI);
        }


        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }
    }
}
