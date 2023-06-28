using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace MOD_wkIh9W.Item
{
    // 选择道具
    public class UIChooseProps : MonoBehaviour
    {
        public UIChooseProps(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 道具id
        public static Dictionary<string, ConfItemPropsItem[]> cacheFind = new Dictionary<string, ConfItemPropsItem[]>();
        public static ConfItemPropsItem[] allItems;
        public static ConfItemPropsItem[] findItems;
        public static ConfItemPropsItem[] selectItems;

        public ConfItemPropsItem selectItem;

        public Transform leftRoot;
        public Transform rightRoot;
        public Transform typeRoot;
        public Transform typeSubRoot;
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

        public int selectType;
        public List<string> selectSubType = new List<string>();

        public bool isInitData = false;

        void Awake()
        {
            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");
            typeRoot = transform.Find("Root/typeRoot");
            typeSubRoot = transform.Find("Root/typeSubRoot");

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

            textTitle.text = "选择道具";
            leftTitle.text = "可从下面选择道具";
            rightTitle.text = "已选择的道具";

            textPage = transform.Find("Root/Text4").GetComponent<Text>();
            btnLastPage = transform.Find("Root/BtnL").GetComponent<Button>();
            btnNextPage = transform.Find("Root/BtnN").GetComponent<Button>();
            inputFind = transform.Find("Root/InputField").GetComponent<InputField>();

            btnLastPage.onClick.AddListener((Action)(()=>
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

            finxStr = PlayerPrefs.GetString(name + "findStr", "");
            lastFinxStr = finxStr;
            inputFind.text = finxStr;
            pageIndex = PlayerPrefs.GetInt(name + "pageIndex", 0);

            transform.Find("Root/BtnUpdate").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UIChooseProps.findItems = null;
                UIChooseProps.cacheFind.Clear();
                UIChooseProps.allItems = null;
                UIChooseProps.selectItems = null;
                Init(true);
                UpdateUI();
            }));

            selectType = PlayerPrefs.GetInt(name + "selectType", 0);
            var list = PlayerPrefs.GetString(name + "selectSubType", "");
            selectSubType = new List<string>();
            if (!string.IsNullOrEmpty(list)) {
                selectSubType.AddRange(list.Split(','));
            }

            InitTypeRoot();
        }

        void InitTypeRoot()
        {
            int isInit = 0;
            List<int> types = new List<int> { 0 };
            foreach (var item in g.conf.itemType.types)
            {
                types.Add(item);
            }
            types.Remove(2);
            UnityAPIEx.DestroyChild(typeRoot);
            typeRoot.gameObject.SetActive(false);
            var group = typeRoot.GetComponent<ToggleGroup>();
            Toggle defToggle = null;
            foreach (var type in types)
            {
                int selType = type;
                GameObject go = GameObject.Instantiate(typeItem, typeRoot);
                go.GetComponentInChildren<Text>().text = GameTool.LS(type == 0 ? "playerInfo_quanbu" : g.conf.itemType.GetItem(type).name);
                var toggle = go.transform.Find("Toggle").GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(((Action<bool>)(b =>
                {
                    if (b && isInit > 0)
                    {
                        selectType = selType;
                        InitSubTypeRoot(isInit == 1);
                    }
                })));
                toggle.group = group;
                if (type == selectType)
                {
                    defToggle = toggle;
                }
                go.SetActive(true);
            }
            typeRoot.gameObject.SetActive(true);
            isInit = 1;
            if (defToggle)
            {
                defToggle.isOn = true;
            }
            isInit = 2;
        }

        void InitSubTypeRoot(bool isInit)
        {
            UnityAPIEx.DestroyChild(typeSubRoot);

            //创建子分类
            var items = new List<ConfItemTypeItem>();

            foreach (var value in g.conf.itemType._allConfList)
            {
                var conf = ItemTypeFixedEx.Fixed(value);

                if (conf.sort <= 0)
                    continue;
                if (conf.uiLabel == "0")
                    continue;
                if (conf.type != selectType)
                    continue;

                if (items.Exists(w => w.uiLabel == conf.uiLabel))
                    continue;

                items.Add(conf);
            }

            var allClass = new List<string>();

            allClass.AddRange(items.Select(w => w.uiLabel));
            if (!isInit)
            {
                selectSubType = new List<string>();
                selectSubType.AddRange(allClass);
            }

            foreach (var item in allClass)
            {
                string selSubType = item;
                GameObject go = GameObject.Instantiate(typeItem, typeSubRoot);
                go.GetComponentInChildren<Text>().text = GameTool.LS(selSubType);
                var toggle = go.transform.Find("Toggle").GetComponent<Toggle>();
                toggle.isOn = selectSubType.Contains(selSubType);
                toggle.onValueChanged.AddListener(((Action<bool>)(b =>
                {
                    if (b)
                    {
                        selectSubType.Add(selSubType);
                    }
                    else
                    {
                        selectSubType.Remove(selSubType);
                    }
                    UpdateSelectItems();
                    if (isInitData)
                    {
                        UpdateUI();
                    }
                })));
                go.SetActive(true);
            }
            UpdateSelectItems();
            if (isInitData)
            {
                UpdateUI();
            }
        }

        void UpdateSelectItems()
        {
            Console.WriteLine(selectType + ":" + string.Join(",", selectSubType));
            if (selectType == 0)
            {
                selectItems = findItems;
            }
            else
            {
                List<ConfItemPropsItem> list = new List<ConfItemPropsItem>();
                foreach (ConfItemPropsItem item in findItems)
                {
                    var className = item.className;
                    var conf = ItemTypeFixedEx.Fixed(g.conf.itemType.GetItem(className));
                    if ((selectType == 0 || selectType == conf.type) && selectSubType.Contains(conf.uiLabel))
                    {
                        list.Add(item);
                    }
                }
                selectItems = list.ToArray();
            }
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
                var items = g.conf.itemProps._allConfList.ToArray();
                List<ConfItemPropsItem> list = new List<ConfItemPropsItem>(items);
                list.RemoveAll((v)=> string.IsNullOrEmpty(GameTool.LS(v.name)));
                allItems = list.ToArray();
                UpdateFind();
            }
        }

        public void UpdateFind()
        {
            lastFinxStr = inputFind.text;
            finxStr = inputFind.text;
            FindTool findTool = new FindTool();
            findTool.SetFindStr(finxStr);
            if (findTool.findStr.Length == 0)
            {
                findItems = allItems;
            }
            else
            {
                if (cacheFind.ContainsKey(finxStr))
                {
                    findItems = cacheFind[finxStr];
                }
                else
                {
                    List<ConfItemPropsItem> list = new List<ConfItemPropsItem>(allItems);
                    list.RemoveAll((v) => !findTool.CheckFind(GameTool.LS(v.name)));
                    findItems = list.ToArray();
                    cacheFind.Add(finxStr, findItems);
                }
            }
            UpdateSelectItems();
        }

        public void AddTip(GameObject go, ConfItemPropsItem selectItem)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                try
                {
                    var propsData = DataProps.PropsData.NewPropsNotData(selectItem.id);

                    if (propsData.propsItem.className == 401)//显示法宝提示
                    {
                        DataProps.PropsData propsData2 = propsData.Clone();
                        if (propsData2.To<DataProps.PropsArtifact>().grade == 0)
                        {
                            propsData2.To<DataProps.PropsArtifact>().grade = 1;
                        }
                        g.ui.OpenUI<UIArtifactInfo>(UIType.ArtifactInfo).InitData(g.world.playerUnit, propsData2, go.transform.position, false);
                    }
                    else
                    {
                        g.ui.OpenUI<UIPropInfo>(UIType.PropInfo).InitData(g.world.playerUnit, propsData, go.transform.position, false);
                    }
                }
                catch (Exception)
                {

                }
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.PropInfo); g.ui.CloseUI(UIType.ArtifactInfo); });
        }

        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            isInitData = true;
            Init(false);
            UpdateUI();
        }

        void UpdateUI()
        {
            var list = selectItems;
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
                var itemName = GameTool.LS(selectItem.name);
                var name = CommonTool.SetTextColor(itemName, GameTool.LevelToColor(selectItem.level, 1));
                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = (i+1)+"."+name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
                AddTip(go, selectItem);
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
            var name = CommonTool.SetTextColor(GameTool.LS(selectItem.name), GameTool.LevelToColor(selectItem.level, 1));
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(leftRoot);
                selectItem = null;
            }));
            go.SetActive(true);
            AddTip(go, selectItem);
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择道具！");
                return;
            }
            call(selectItem.id.ToString(), CommonTool.SetTextColor(GameTool.LS(selectItem.name), GameTool.LevelToColor(selectItem.level, 1)));
            CloseUI();
        }


        void Start()
        {
            UIDaguiTool.InitScroll(GetComponent<UIBase>());
        }

        void OnDestroy()
        {
            UIDaguiTool.DelScroll(GetComponent<UIBase>());

            PlayerPrefs.SetString(name + "findStr", finxStr);
            PlayerPrefs.SetInt(name + "pageIndex", pageIndex);
            PlayerPrefs.SetInt(name + "selectType", selectType);
            PlayerPrefs.SetString(name + "selectSubType", string.Join(",", selectSubType));
        }
    }
}
