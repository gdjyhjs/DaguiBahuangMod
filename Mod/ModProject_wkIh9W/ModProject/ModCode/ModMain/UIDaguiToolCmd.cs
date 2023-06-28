using Newtonsoft.Json;
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
    public class UIDaguiToolCmd : MonoBehaviour
    {
        //必须有这行代码，否则无法MonoBehaviour类无法AddComponent
        public UIDaguiToolCmd(IntPtr ptr) : base(ptr) { }

        public Transform leftRoot;
        public Transform rightRoot;
        public GameObject leftItem;
        public GameObject rightItem;
        public InputField inputName;
        public Button btnSetKey;

        public CmdItem showCmdItem;
        public int selectIndex;

        void Awake()
        {
            selectIndex = PlayerPrefs.GetInt(name + "curIndex", 0);

            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");
            leftItem = transform.Find("LeftItem").gameObject;
            rightItem = transform.Find("RightItem").gameObject;

            inputName = transform.Find("Root/Right/InputName").GetComponent<InputField>();
            btnSetKey = transform.Find("Root/Right/BtnKey").GetComponent<Button>();
            var btnCopy = transform.Find("Root/Right/BtnCopy").GetComponent<Button>();
            btnCopy.onClick.AddListener((Action)(() =>
            {
                if (showCmdItem != null)
                {
                    CmdItem[] all = new CmdItem[] { showCmdItem };
                    string str = JsonConvert.SerializeObject(all);
                    GUIUtility.systemCopyBuffer = str;
                    UITipItem.AddTip("复制成功！");
                }
                else
                {
                    UITipItem.AddTip("复制失败！");
                }
            }));
            btnSetKey.onClick.AddListener((Action)(() =>
            {
                ModMain.OpenUI<UIDaguiToolSetKey>("DaguiToolSetKey").InitData((key)=>
                {
                    selectLeft.t2.text = key.ToString();
                    showCmdItem.key = key;
                    SetKeyTip();
                    ModMain.SaveCmdItems();
                });
            }));

            transform.Find("Root/BtnCopyAll").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                // 复制所有
                try
                {
                    CmdItem[] all = ModMain.allCmdItems.ToArray();
                    string str = JsonConvert.SerializeObject(all);
                    GUIUtility.systemCopyBuffer = str;
                    UITipItem.AddTip("复制成功！");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    UITipItem.AddTip("复制失败！");
                }
            
            }));
            transform.Find("Root/BtnZTAll").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                try
                {
                    // 粘贴所有
                    var str = GUIUtility.systemCopyBuffer;
                    CmdItem[] all = JsonConvert.DeserializeObject<CmdItem[]>(str);
                    ModMain.allCmdItems.AddRange(all);
                    UpdateUI();
                    UITipItem.AddTip("导入成功！");
                    ModMain.SaveCmdItems();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    UITipItem.AddTip("导入失败！");
                }

            }));
        }

        private void SetKeyTip()
        {
            var keyStr = showCmdItem.key.ToString();
            if (string.IsNullOrEmpty(keyStr))
            {
                keyStr = "点击可设置快捷键";
            }
            btnSetKey.GetComponent<Text>().text = "当前快捷键：" + keyStr;
        }

        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
            transform.Find("Root/btnClose").GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                g.ui.CloseUI(GetComponent<UIBase>());
            }));

            //右键快速关闭
            gameObject.AddComponent<UIFastClose>();

            UpdateUI();
        }

        DataStruct<Text, Text> selectLeft;
        void UpdateUI()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var list = ModMain.allCmdItems;
            for (int i = 0; i < list.Count; i++)
            {
                CmdItem cmd = list[i];
                var go = GameObject.Instantiate(leftItem, leftRoot);
                var textName = go.transform.Find("Name").GetComponent<Text>();
                var textKey = go.transform.Find("Key").GetComponent<Text>();
                textName.text = cmd.name;
                textKey.text = cmd.key.ToString();
                var btnDel = go.transform.Find("btnDel").GetComponent<Button>();
                var btnRun = go.transform.Find("btnRun").GetComponent<Button>();
                btnDel.onClick.AddListener((Action)(() =>
                {
                    list.Remove(cmd);
                    GameObject.DestroyImmediate(go);
                    ModMain.SaveCmdItems();
                }));
                btnRun.onClick.AddListener((Action)(() =>
                {
                    cmd.Run();
                }));
                DataStruct<Text, Text> left = new DataStruct<Text, Text>(textName, textKey);
                go.GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    selectLeft = left;
                    selectIndex = go.transform.GetSiblingIndex();
                    showCmdItem = cmd;
                    UpdateCmd();
                }));
                go.SetActive(true);
            }

            if (leftRoot.childCount > 0)
            {
                if (selectIndex < 0 || selectIndex >= leftRoot.childCount)
                {
                    selectIndex = 0;
                }
                leftRoot.GetChild(selectIndex).GetComponent<Button>().onClick.Invoke();
            }
        }

        public void UpdateCmd()
        {
            inputName.onValueChanged.RemoveAllListeners();
            inputName.text = showCmdItem.name;
            inputName.onValueChanged.AddListener((Action<string>)((v)=>
            {
                selectLeft.t1.text = v;
                showCmdItem.name = v;
                ModMain.SaveCmdItems();
            }));
            SetKeyTip();

            UnityAPIEx.DestroyChild(rightRoot);
            foreach (var item in showCmdItem.cmds)
            {
                var go = GameObject.Instantiate(rightItem, rightRoot);
                go.GetComponent<Text>().text = item;
                go.SetActive(true);
            }
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());
            PlayerPrefs.SetInt(name + "curIndex", selectIndex);
        }
    }
}
