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
    // 选择壶妖
    public class UIChoosePotmon : MonoBehaviour
    {
        public UIChoosePotmon(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 壶妖id


        public ConfPotmonBaseItem[] allItems;
        public ConfPotmonBaseItem[] findItems;
        public ConfPotmonBaseItem selectItem;


        public Transform leftRoot;
        public Transform rightRoot;
        public Transform typeRoot;
        public GameObject goItem;
        public GameObject typeItem;
        public Text textTitle;
        public Text leftTitle;
        public Text rightTitle;
        public Button btnOk;
        public Button btnClose;

        public Text textPage;
        public Button btnLastPage;
        public Button btnNextPage;
        public InputField inputFind;

        public string lastFinxStr = "";
        public string finxStr = "";
        public int pageIndex = 0;
        public int pageShowCount = 36;
        public int pageMax = 0;
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

            textTitle.text = "选择壶妖";
            leftTitle.text = "可从下面选择壶妖";
            rightTitle.text = "已选择的壶妖";

            textPage = transform.Find("Root/Text4").GetComponent<Text>();
            btnLastPage = transform.Find("Root/BtnL").GetComponent<Button>();
            btnNextPage = transform.Find("Root/BtnN").GetComponent<Button>();
            inputFind = transform.Find("Root/InputField").GetComponent<InputField>();


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
        }

        void Update()
        {
            try
            {
                if (finxStr != inputFind.text && lastFinxStr != inputFind.text)
                {
                    CancelInvoke();
                    finxStr = inputFind.text;
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
            if (lastFinxStr != inputFind.text && finxStr == inputFind.text)
            {
                UpdateFind();
                UpdateUI();
            }
        }

        public void Init(bool reRead)
        {
            if (reRead || allItems == null)
            {
                var items = g.conf.potmonBase._allConfList.ToArray();
                List<ConfPotmonBaseItem> list = new List<ConfPotmonBaseItem>(items);
                allItems = list.ToArray();
            }
        }

        public void UpdateFind()
        {
            lastFinxStr = inputFind.text;
            finxStr = inputFind.text;
            FindTool findTool = new FindTool();
            findTool.SetFindStr(inputFind.text);
            if (findTool.findStr.Length == 0)
            {
                findItems = allItems;
            }
            else
            {
                List<ConfPotmonBaseItem> list = new List<ConfPotmonBaseItem>(allItems);
                list.RemoveAll((v) => !findTool.CheckFind(GameTool.LS(v.name)));
                findItems = list.ToArray();
            }
        }

        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            Init(false);
            UpdateFind();
            UpdateUI();
        }

        void UpdateUI()
        {
            var list = findItems;
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
                var name = GameTool.LS(selectItem.name);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
            }
            textPage.text = $"{pageIndex + 1}/{pageMax}";
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = GameTool.LS(selectItem.name);
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(leftRoot);
                selectItem = null;
            }));
            go.SetActive(true);
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择壶妖！");
                return;
            }
            call(selectItem.id.ToString(), GameTool.LS(selectItem.name));
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
