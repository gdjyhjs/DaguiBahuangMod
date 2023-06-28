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
    // 查看角色
    public class UILookChar : MonoBehaviour
    {
        public UILookChar(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 角色id

        public Transform rightRoot;
        public GameObject goItem;
        public Text textTitle;
        public Button btnOk;
        public Button btnClose;

        public Text textPage;
        public Button btnLastPage;
        public Button btnNextPage;

        public int pageIndex = 0;
        public int pageShowCount = 36;
        public int pageMax = 0;
        void Awake()
        {
            rightRoot = transform.Find("Root/Right/View/Root");

            goItem = transform.Find("Item").gameObject;

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            //btnOk.onClick.AddListener((Action)OnBtnOk);
            btnOk.gameObject.SetActive(false);
            gameObject.AddComponent<UIFastClose>();

            goItem.GetComponent<Text>().color = Color.black;

            textTitle.text = "查看所有角色";

            textPage = transform.Find("Root/Text4").GetComponent<Text>();
            btnLastPage = transform.Find("Root/BtnL").GetComponent<Button>();
            btnNextPage = transform.Find("Root/BtnN").GetComponent<Button>();
            UISelectChar.inputFind = transform.Find("Root/InputField").GetComponent<InputField>();


            btnLastPage.onClick.AddListener((Action)(() =>
            {
                if (pageMax > 1)
                {
                    pageIndex--;
                    if (pageIndex < 0)
                    {
                        pageIndex = pageMax - 1;
                    }
                    UpdateUI();
                }
            }));
            btnNextPage.onClick.AddListener((Action)(() =>
            {
                if (pageMax > 1)
                {
                    pageIndex++;
                    if (pageIndex >= pageMax)
                    {
                        pageIndex = 0;
                    }
                    UpdateUI();
                }
            }));

            UISelectChar.finxStr = PlayerPrefs.GetString( "SelectOneCharfindStr", "");
            UISelectChar.lastFinxStr = UISelectChar.finxStr;
            UISelectChar.inputFind.text = UISelectChar.finxStr;
            pageIndex = PlayerPrefs.GetInt( "SelectOneCharpageIndex", 0);

            transform.Find("Root/BtnUpdate").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UISelectChar.findItems = null;
                UISelectChar.allItems = null;
                UISelectChar.Init(true);
                UpdateUI();
            }));

            var tglKey = MOD_wkIh9W.Patch.Patch_UINpcInfo.openKey;
            var tglOpen = transform.Find("Root/TglOpen").GetComponent<Toggle>();
            tglOpen.isOn = PlayerPrefs.GetInt(tglKey, 0) == 1;
            tglOpen.onValueChanged.AddListener((Action<bool>)((v) =>
            {
                PlayerPrefs.SetInt(tglKey, v ? 1 : 0);
            }));

            tglOpen.gameObject.SetActive(false); // 还没做好先屏蔽


            UINpcSelectClass.InitSel(transform, () =>
            {
                UISelectChar.UpdateSelect();
                UpdateUI();
            });
        }

        void Update()
        {
            try
            {
                if (UISelectChar.finxStr != UISelectChar.inputFind.text && UISelectChar.lastFinxStr != UISelectChar.inputFind.text)
                {
                    CancelInvoke();
                    UISelectChar.finxStr = UISelectChar.inputFind.text;
                    Invoke("DelayUpdateUI", 0.5f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(name + " Update " + e.ToString());
            }

        }

        void DelayUpdateUI()
        {
            if (UISelectChar.lastFinxStr != UISelectChar.inputFind.text && UISelectChar.finxStr == UISelectChar.inputFind.text)
            {
                UISelectChar.UpdateFind();
                UpdateUI();
            }
        }

        public void InitData()
        {
            UISelectChar.Init(false);
            UpdateUI();
        }

        void UpdateUI()
        {
            var list = UISelectChar.selItems;
            UnityAPIEx.DestroyChild(rightRoot);
            pageMax = Mathf.CeilToInt(list.Length * 1f / pageShowCount);
            if (pageIndex >= pageMax)
            {
                pageIndex = 0;
            }
            for (int i = pageIndex * pageShowCount; i < (pageIndex + 1) * pageShowCount; i++)
            {
                if (i >= list.Length)
                    break;
                var selectItem = list[i];
                var name = GameTool.LS(selectItem.t2);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.transform.Find("BtnLook").GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    var unit = ModMain.cmdRun.GetUnit(selectItem.t1);
                    if (unit != null)
                        g.ui.OpenUI<UINPCInfo>(UIType.NPCInfo).InitData(unit, true);
                }));
                go.transform.Find("BtnFace").GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    var unit = ModMain.cmdRun.GetUnit(selectItem.t1);
                    if (unit != null)
                        new MOD_wkIh9W.Patch.CreateFace().InitData(unit);
                }));
                go.SetActive(true);
                go.AddComponent<UISkyTipEffect>().InitData(Tool.UnitTip(selectItem.t1));
            }
            textPage.text = $"{pageIndex + 1}/{pageMax}";
        }


        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }


        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
            InitData();
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());

            PlayerPrefs.SetString( "SelectOneCharfindStr", UISelectChar.finxStr);
            PlayerPrefs.SetInt( "SelectOneCharpageIndex", pageIndex);

            PlayerPrefs.SetInt("SelectOneCharselTgl", UINpcSelectClass.isOpenSel ? 1 : 0);
        }
    }
}
