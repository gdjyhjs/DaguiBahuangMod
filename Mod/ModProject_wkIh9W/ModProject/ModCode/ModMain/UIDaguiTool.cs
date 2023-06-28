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
    public class UIDaguiTool : MonoBehaviour
    {
        //必须有这行代码，否则无法MonoBehaviour类无法AddComponent
        public UIDaguiTool(IntPtr ptr) : base(ptr) { }

        public Transform leftRoot;
        public Transform rightRoot;
        public GameObject leftItem;
        public GameObject leftItemFind;
        public GameObject rightItem;
        public GameObject rightTitleItem;
        public GameObject rightButtonItem;
        public GameObject rightInputItem;

        public Transform recordRoot;
        public Transform cmdListRoot;
        public Button btnCmd;
        public Button btnNew;
        public Button btnSave;
        public Button btnZT; // 粘贴指令集
        public Button btnExitCmd;
        public Button btnClearRecord;
        public GameObject itemRecord;
        public GameObject itemCmd;
        public InputField inputCmdName;

        public static CmdItem tmpCmdItem;

        public int openCmdSet = 0; // 是否打开指令集

        private void Awake()
        {
            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");
            leftItem = transform.Find("LeftItem").gameObject;
            leftItemFind = transform.Find("LeftItemFind").gameObject;
            rightButtonItem = transform.Find("RightButtonItem").gameObject;
            rightTitleItem = transform.Find("RightTitleItem").gameObject;
            rightInputItem = transform.Find("RightInputItem").gameObject;
            rightItem = transform.Find("RightItem").gameObject;

            recordRoot = transform.Find("Root/Record/View/Root");
            cmdListRoot = transform.Find("Root/CmdList/View/Root");
            itemRecord = transform.Find("ItemRecord").gameObject;
            itemCmd = transform.Find("ItemCmd").gameObject;
            btnCmd = transform.Find("Root/BtnCmd").GetComponent<Button>();
            btnNew = transform.Find("Root/BtnNew").GetComponent<Button>();
            btnSave = transform.Find("Root/BtnSave").GetComponent<Button>();
            btnZT = transform.Find("Root/BtnZT").GetComponent<Button>();
            inputCmdName = transform.Find("Root/CmdList/InpitCmdName").GetComponent<InputField>();
            btnExitCmd = transform.Find("Root/CmdList/BtnExit").GetComponent<Button>();
            btnClearRecord = transform.Find("Root/Record/BtnClear").GetComponent<Button>();

            btnClearRecord.onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(recordRoot);
                ModMain.cmdRun.logs.Clear();
            }));
            btnCmd.onClick.AddListener((Action)(()=>
            {
                ModMain.OpenUI<UIDaguiToolCmd>("DaguiToolCmd");
            }));
            btnNew.onClick.AddListener((Action)(() =>
            {
                openCmdSet = 1;
                tmpCmdItem = new CmdItem();
                OpenCmdList();
            }));
            btnSave.onClick.AddListener((Action)(() =>
            {
                tmpCmdItem.name = inputCmdName.text;
                ModMain.allCmdItems.Add(tmpCmdItem);
                ModMain.SaveCmdItems();
                openCmdSet = 0;
                while (cmdListRoot.childCount > 0)
                {
                    GameObject.DestroyImmediate(cmdListRoot.GetChild(0).gameObject);
                }
                tmpCmdItem = new CmdItem();
                inputCmdName.text = "";
                OpenRecord();
            }));
            btnZT.onClick.AddListener((Action)(() =>
            {
                openCmdSet = 1;
                try
                {
                    var str = GUIUtility.systemCopyBuffer;
                    CmdItem[] all = JsonConvert.DeserializeObject<CmdItem[]>(str);
                    if (all != null && all.Length > 0)
                    {
                        tmpCmdItem = all[0];
                        OpenCmdList();
                        UITipItem.AddTip("导入成功！");
                    }
                    else
                    {
                        UITipItem.AddTip("导入失败！");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    UITipItem.AddTip("导入失败！");
                }
            }));
            btnExitCmd.onClick.AddListener((Action)(() =>
            {
                openCmdSet = 0;
                OpenRecord();
            }));


            var text5 = transform.Find("Root/Text5").GetComponent<Text>();
            var btn5 = text5.GetComponent<Button>();
            Action updateTest5 = () =>
            {
                var str = ModMain.cmdData.key.ToString();
                if (string.IsNullOrEmpty(str))
                {
                    text5.text = "点击设置打开/关闭界面的快捷键";
                }
                else
                {
                    text5.text = string.Format("使用{0}可快速打开或关闭界面", str);
                }
            };
            btn5.onClick.AddListener((Action)(() =>
            {
                ModMain.OpenUI<UIDaguiToolSetKey>("DaguiToolSetKey").InitData((key) =>
                {
                    ModMain.cmdData.key = key;
                    updateTest5();
                    ModMain.SaveCmdItems();
                });
            }));
            updateTest5();
            text5.gameObject.AddComponent<UISkyTipEffect>().InitData("点击可以修改快捷键");

            transform.Find("Root/BtnAllUnit").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                ModMain.OpenUI<MOD_wkIh9W.Item.UILookChar>("LookChar");
            }));


            transform.Find("Root/BtnTeaching").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                ModMain.OpenUI<MOD_wkIh9W.Item.UIDatuiToolTeaching>("DatuiToolTeaching");
            }));
        }


        void OnEnable()
        {
            UpdateUI();
            UIMask.disableFastKey++;
            openCmdSet = PlayerPrefs.GetInt(name + "openCmdSet", 0);
            if (openCmdSet == 1)
            {
                if (tmpCmdItem == null)
                {
                    tmpCmdItem = CmdItem.FromString(PlayerPrefs.GetString(name + "tmpCmdItem"));
                }
                OpenCmdList();
            }
            else
            {
                OpenRecord();
                UpdateRecord();
            }
        }

        void OnDisable()
        {
            UIMask.disableFastKey--;
            PlayerPrefs.SetInt(name + "openCmdSet", openCmdSet);
            if (openCmdSet == 1)
            {
                PlayerPrefs.SetString(name + "tmpCmdItem", tmpCmdItem.ToString());
            }
        }

        public void OpenRecord()
        {
            btnCmd.gameObject.SetActive(true);
            btnNew.gameObject.SetActive(true);
            btnSave.gameObject.SetActive(false);
            btnZT.gameObject.SetActive(false);
            transform.Find("Root/Record").gameObject.SetActive(true);
            transform.Find("Root/CmdList").gameObject.SetActive(false);
            UpdateListBtn();
        }
        public void OpenCmdList()
        {
            btnCmd.gameObject.SetActive(false);
            btnNew.gameObject.SetActive(false);
            btnSave.gameObject.SetActive(true);
            btnZT.gameObject.SetActive(true);
            transform.Find("Root/Record").gameObject.SetActive(false);
            transform.Find("Root/CmdList").gameObject.SetActive(true);
            UpdateCmdList();
        }

        public void UpdateRecord()
        {
            UnityAPIEx.DestroyChild(recordRoot);
            foreach (var item in ModMain.cmdRun.logs)
            {
                AddLogItem(item);
            }
        }

        public void UpdateCmdList()
        {
            inputCmdName.text = tmpCmdItem.name;
            while (cmdListRoot.childCount > 0)
            {
                GameObject.DestroyImmediate(cmdListRoot.GetChild(0).gameObject);
            }
            for (int i = 0; i < tmpCmdItem.cmds.Count; i++)
            {
                string cmd = tmpCmdItem.cmds[i];
                AddCmdItem(cmd);
            }
            UpdateCmdListButton();
            UpdateListBtn();
        }

        public void AddCmdItem(string cmd)
        {
            GameObject go = GameObject.Instantiate(itemCmd, cmdListRoot);
            go.GetComponent<Text>().text = cmd;
            var btns = go.transform.Find("btns").gameObject;
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                btns.SetActive(true);
            });
            uiEvent.onMouseExitCall += (Action)(() => { btns.SetActive(false); });
            go.SetActive(true);
        }

        public void UpdateCmdListButton()
        {
            return;
            //Console.WriteLine("UpdateCmdListButtonchildCount " + cmdListRoot.childCount);
            //for (int i = 0; i < cmdListRoot.childCount; i++)
            //{
            //    int index = i;
            //    var child = cmdListRoot.GetChild(i);
            //    var btnDel = child.Find("BtnDel").GetComponent<Button>();
            //    var btnUp = child.Find("BtnUp").GetComponent<Button>();
            //    var btnDown = child.Find("BtnDown").GetComponent<Button>();
            //    btnDel.onClick.RemoveAllListeners();
            //    btnUp.onClick.RemoveAllListeners();
            //    btnDown.onClick.RemoveAllListeners();
            //    btnDel.onClick.AddListener((Action)(() =>
            //    {
            //        tmpCmdItem.cmds.RemoveAt(index);
            //        GameObject.DestroyImmediate(child.gameObject);
            //        UpdateCmdListButton();
            //    }));
            //    btnUp.onClick.AddListener((Action)(() =>
            //    {
            //        if (index > 0)
            //        {
            //            child.SetSiblingIndex(child.GetSiblingIndex() - 1);
            //            var item = tmpCmdItem.cmds[index];
            //            tmpCmdItem.cmds.RemoveAt(index);
            //            tmpCmdItem.cmds.Insert(index - 1, item);
            //            UpdateCmdListButton();
            //        }
            //    }));
            //    btnDown.onClick.AddListener((Action)(() =>
            //    {
            //        if (index < cmdListRoot.childCount - 1)
            //        {
            //            child.SetSiblingIndex(child.GetSiblingIndex() + 1);
            //            var item = tmpCmdItem.cmds[index];
            //            tmpCmdItem.cmds.RemoveAt(index);
            //            tmpCmdItem.cmds.Insert(index + 1, item);
            //            UpdateCmdListButton();
            //        }
            //    }));
            //    btnUp.gameObject.SetActive(index > 0);
            //    btnDown.gameObject.SetActive(index < cmdListRoot.childCount - 1);

            //    btnUp.gameObject.SetActive(false);
            //    btnDown.gameObject.SetActive(false);
            //    btnDel.gameObject.SetActive(false);
            //}
        }

        public static List<UIBase> allDaguiUI = new List<UIBase>();
        public static void DelScroll(UIBase ui)
        {
            allDaguiUI.Remove(ui);
            var srs = ui.GetComponentsInChildren<ScrollRect>();
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                var key = ui.gameObject.name + i;
                PlayerPrefs.SetFloat(key, sr.verticalNormalizedPosition);
            }
        }
        public static void InitScroll(UIBase ui)
        {
            allDaguiUI.Add(ui);
            var srs = ui.GetComponentsInChildren<ScrollRect>();
            for (int i = 0; i < srs.Length; i++)
            {
                var sr = srs[i];
                sr.horizontal = false;
                sr.movementType = ScrollRect.MovementType.Clamped;
                sr.scrollSensitivity = ModMain.scrollSpeed;
                var key = ui.gameObject.name + i;
                if (PlayerPrefs.HasKey(key))
                {
                    var value = PlayerPrefs.GetFloat(key);
                    sr.verticalNormalizedPosition = value;
                }
            }
        }

        void OnDestroy()
        {
            var ui = GetComponent<UIBase>();
            List<UIBase> list = new List<UIBase>(allDaguiUI);
            foreach (var item in list)
            {
                if (item != ui)
                {
                    g.ui.CloseUI(ui);
                }
            }
            allDaguiUI.Clear();
            var srs = ui.GetComponentsInChildren<ScrollRect>();
            for (int i = 0; i < srs.Length; i++)
            {
                var key = gameObject.name + i;
                var sr = srs[i];
                PlayerPrefs.SetFloat(key, sr.verticalNormalizedPosition);
            }
        }

        void Start()
        {
            InitScroll(GetComponent<UIBase>());
            transform.Find("Root/btnClose").GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                g.ui.CloseUI(GetComponent<UIBase>());
            }));

            //右键快速关闭
            gameObject.AddComponent<UIFastClose>();

            //if (ModMain.chinaInit == 1)
            //{
            //    ModMain.chinaInit = 3;
            //    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(
            //        "[大鬼]工具盒", 
            //        "已自动下载拼音搜索需要的文件，拼音搜索需要重启游戏才生效，其他功能不影响使用。", 1, null, null);
            //}
        }
        public void InitData()
        {
        }

        public void UpdateListBtn()
        {
            bool isOpenCmdSet = openCmdSet == 1;
            Console.WriteLine(isOpenCmdSet+" rightRoot " + rightRoot.childCount);
            for (int i = 0; i < rightRoot.childCount; i++)
            {
                var child = rightRoot.GetChild(i);
                child.Find("BtnRun").gameObject.SetActive(!isOpenCmdSet);
                child.Find("BtnAdd").gameObject.SetActive(isOpenCmdSet);
            }
        }

        void UpdateUI()
        {
            Dictionary<string, List<ConfDaguiToolItem>> items = ModMain.confTool.items;
            UnityAPIEx.DestroyChild(leftRoot);
            List<Button> allBtn = new List<Button>();
            Button curBtn = null;
            int i = 0;
            foreach (var item in items)
            {
                int index = i;
                var type = item.Key;
                List<ConfDaguiToolItem> allitems = item.Value;
                var left = GameObject.Instantiate(leftItem, leftRoot);
                left.gameObject.SetActive(true);
                var btnLeft = left.GetComponent<Button>();
                allBtn.Add( btnLeft);
                btnLeft.onClick.AddListener((Action)(()=>
                {
                    Console.WriteLine(index +" "+ type+ " 该分类命令数："+ allitems.Count);
                    UpdateRight(allitems);
                    PlayerPrefs.SetInt(name + "BtnIndex", index);
                    if (curBtn != null)
                    {
                        curBtn.transform.Find("Mark").gameObject.SetActive(false);
                    }
                    curBtn = btnLeft;
                    curBtn.transform.Find("Mark").gameObject.SetActive(true);
                }));
                left.transform.Find("Text").GetComponent<Text>().text = allitems[0].typeName;
                i++;
            }
            {
                var left = GameObject.Instantiate(leftItemFind, leftRoot);
                left.gameObject.SetActive(true);
                var btnLeft = left.GetComponent<Button>();
                var inputField = left.transform.Find("InputField").GetComponent<InputField>();
                allBtn.Add(btnLeft);

                Action findItems = () =>
                {
                    List<ConfDaguiToolItem> finds = new List<ConfDaguiToolItem>();
                    finds.AddRange(ModMain.confTool.allItems.Values);
                    FindTool findTool = new FindTool();
                    findTool.SetFindStr(inputField.text);
                    if (findTool.findStr.Length == 0)
                    {

                    }
                    else
                    {
                        finds.RemoveAll((v) => !findTool.CheckFind(GameTool.LS(v.funccn)));
                    }

                    UpdateRight(finds);
                };
                btnLeft.onClick.AddListener((Action)(() =>
                {
                    PlayerPrefs.SetInt(name + "BtnIndex", items.Count);
                    if (curBtn != null)
                    {
                        curBtn.transform.Find("Mark").gameObject.SetActive(false);
                    }
                    curBtn = btnLeft;
                    curBtn.transform.Find("Mark").gameObject.SetActive(true);
                    findItems();
                }));
                inputField.text = PlayerPrefs.GetString(name+ "leftItemFind", "");
                inputField.onValueChanged.AddListener((Action<string>)((v)=>
                {
                    PlayerPrefs.SetString(name + "leftItemFind", v);
                    btnLeft.onClick.Invoke();
                }));
            }
            
            int defindex = PlayerPrefs.GetInt(name + "BtnIndex", 0);
            if (defindex >= 0 && defindex < allBtn.Count)
            {
                allBtn[defindex].onClick.Invoke();
            }


        }

        void UpdateRight(List<ConfDaguiToolItem> items)
        {
            while (rightRoot.childCount > 0)
            {
                GameObject.DestroyImmediate(rightRoot.GetChild(0).gameObject);
            }
            foreach (var item in items)
            {
                try
                {
                    var right = GameObject.Instantiate(rightItem, rightRoot);
                    var toolItem = new UIDaguiToolItem();
                    toolItem.InitData(this, item, right.transform);
                    right.gameObject.SetActive(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(item.func + ">>" + UIDaguiToolItem.debugItemPara + "\n" + e.ToString());
                }
            }
            UpdateListBtn();
        }

        public void AddLogItem(string log)
        {
            var go = GameObject.Instantiate(itemRecord, recordRoot);
            go.GetComponent<Text>().text = log;
            go.SetActive(true);
        }
    }
}
