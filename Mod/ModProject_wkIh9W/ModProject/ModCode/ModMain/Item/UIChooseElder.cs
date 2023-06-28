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
    // 指定一个化神之气
    public class UIChooseElder : MonoBehaviour
    {
        public UIChooseElder(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符(化神之气soleid)

        public WorldUnitBase unit;

        public DataStruct<string, DataProps.PropsData>[] allItems
        {
            get
            {
                int propId = 5081996;
                List<DataStruct<string, DataProps.PropsData>> list = new List<DataStruct<string, DataProps.PropsData>>();

                var lastProp = new DataStruct<string, DataProps.PropsData>("last", (ModMain.lastAddElder != null) ? ModMain.lastAddElder.t2 : null);
                list.Add(lastProp);

                List<DataProps.PropsData> propsDatas = new List<DataProps.PropsData>(g.world.playerUnit.data.unitData.propData._allShowProps.ToArray());
                foreach (var item in propsDatas)
                {
                    if (item.propsID == propId)
                        list.Add(new DataStruct<string, DataProps.PropsData>(item.soleID, item));
                }
                return list.ToArray();
            }
        }
        public DataStruct<string, DataProps.PropsData> selectItem;




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

            textTitle.text = "选择化神之气";
            leftTitle.text = "可从下面选择化神之气";
            rightTitle.text = "已选择的化神之气";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, WorldUnitBase unit)
        {
            this.unit = unit;
            foreach (var item in allItems)
            {
                var selectItem = item;
                string name = "";
                if (selectItem.t1 == "last")
                {
                    name = "上一次添加的化神之气:";

                }
                if (selectItem.t2 != null)
                {
                    name += GameTool.SetTextReplaceColorKey(GameTool.LS(selectItem.t2.propsItem.name), GameTool.LevelToColorKey(selectItem.t2.propsItem.level), 1);
                }

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
                AddTip(go, selectItem);
            }
        }

        public void AddTip(GameObject go, DataStruct<string, DataProps.PropsData> data)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {

                var dataProp = data == null ? null :data.t2;
                if (dataProp == null) {
                    dataProp = DataProps.PropsData.NewProps(5081996, 1);
                }
                g.ui.OpenUI<UIPropInfo>(UIType.PropInfo).InitData(g.world.playerUnit, dataProp, go.transform.position, false);
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.PropInfo); });
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            string name = "";
            if (selectItem.t1 == "last")
            {
                name = "上一次添加的化神之气:";

            }
            if (selectItem.t2 != null)
            {
                name += GameTool.SetTextReplaceColorKey(GameTool.LS(selectItem.t2.propsItem.name), GameTool.LevelToColorKey(selectItem.t2.propsItem.level), 1);
            }
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
                UITipItem.AddTip("请先选择化神之气！");
                return;
            }
            string name = "";
            if (selectItem.t1 == "last")
            {
                name = "上一次添加的化神之气";

            }
            else if (selectItem.t2 != null)
            {
                name = GameTool.SetTextReplaceColorKey(GameTool.LS(selectItem.t2.propsItem.name), GameTool.LevelToColorKey(selectItem.t2.propsItem.level), 1);
            }
            call(selectItem.t1, name);
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
