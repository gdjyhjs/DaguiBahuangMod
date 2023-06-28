using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace MOD_wkIh9W
{
    internal class UIDaguiToolSetKey : MonoBehaviour
    {
        //必须有这行代码，否则无法MonoBehaviour类无法AddComponent
        public UIDaguiToolSetKey(IntPtr ptr) : base(ptr) { }

        public CmdKey key = new CmdKey();
        public GameObject goOk;
        public Button btnOk;
        public GameObject goRe;
        public Button btnRe;
        public Text textMsg;
        public Action<CmdKey> okCall;
        public bool waitInpit = true;

        void CallOk()
        {
            okCall?.Invoke(key);
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        void Awake()
        {
            goOk = transform.Find("Root/BtnOk").gameObject;
            btnOk = goOk.GetComponent<Button>();
            goRe = transform.Find("Root/BtnRe").gameObject;
            btnRe = goRe.GetComponent<Button>();
            textMsg = transform.Find("Root/teztKey").GetComponent<Text>();
            btnOk.onClick.AddListener((Action)(()=>
            {
                string tip;
                if (!string.IsNullOrEmpty(key.key))
                {
                    tip = "确定将"+key.ToString()+"设置为快捷键吗？";
                }
                else
                {
                    tip = "确当取消快捷键吗？";
                }

                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("[大鬼]工具盒", 
                    tip, 2, (Action)(()=>
                    {
                        CallOk();
                    }), null);

            }));
            btnRe.onClick.AddListener((Action)(() =>
            {
                key = new CmdKey();
                waitInpit = true;
                btnOk.gameObject.SetActive(true);
                btnRe.gameObject.SetActive(true);
            }));
            btnOk.gameObject.SetActive(true);
            btnRe.gameObject.SetActive(true);
        }

        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
            transform.Find("Root/btnClose").GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                g.ui.CloseUI(GetComponent<UIBase>());
            }));
        }

        public void InitData(Action<CmdKey> okCall)
        {
            this.okCall = okCall;
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());
        }

        void Update()
        {

            try
            {
                if (!waitInpit)
                {
                    return;
                }
                key.ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                key.alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt);
                key.shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                UpdateText();
                foreach (var item in keys)
                {
                    if (Input.GetKeyDown(item))
                    {
                        waitInpit = false;
                        key.key = item.ToString();
                        UpdateText();
                        btnOk.gameObject.SetActive(true);
                        btnRe.gameObject.SetActive(true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(name + " Update " + e.ToString());
            }

        }

        private void UpdateText()
        {
            string msg = key.GetState();
            if (string.IsNullOrEmpty(msg))
            {
                textMsg.text = "等待输入快捷键...";
            }
            else
            {
                textMsg.text = msg;
            }
        }

        public KeyCode[] keys = new KeyCode[]{
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12,
        KeyCode.Q,
        KeyCode.W,
        KeyCode.E,
        KeyCode.R,
        KeyCode.T,
        KeyCode.Y,
        KeyCode.U,
        KeyCode.I,
        KeyCode.O,
        KeyCode.P,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.Z,
        KeyCode.X,
        KeyCode.C,
        KeyCode.V,
        KeyCode.B,
        KeyCode.N,
        KeyCode.M,

        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,

        KeyCode.Keypad0,
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9,

        };
    }
}