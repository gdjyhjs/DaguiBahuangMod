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
    // 指定一个功法
    public class UIDesignateOneTechnique : MonoBehaviour
    {
        public UIDesignateOneTechnique(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符(功法soleid)

        public WorldUnitBase unit;

        public DataStruct<string, DataProps.MartialData>[] allItems
        {
            get
            {
                List<DataStruct<string, DataProps.MartialData>> list = new List<DataStruct<string, DataProps.MartialData>>();
                list.Add(new DataStruct<string, DataProps.MartialData>("last", ModMain.lastStudySkill != null ? ModMain.lastStudySkill.t2.data.To<DataProps.MartialData>() : null));
                var allActionMartial = unit.data.unitData.allActionMartial;

                foreach (var item in allActionMartial)
                {
                    list.Add(new DataStruct<string, DataProps.MartialData>(item.value.data.soleID,  item.value.data.To<DataProps.MartialData>()));
                }
                return list.ToArray();  
            }
        }
        public DataStruct<string, DataProps.MartialData> selectItem;




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

            textTitle.text = "选择功法";
            leftTitle.text = "可从下面选择功法";
            rightTitle.text = "已选择的功法";
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
                    name = "上一次学习的心法:";

                }
                if (selectItem.t2 != null)
                {
                    name += GameTool.SetTextReplaceColorKey(selectItem.t2.martialInfo.name, GameTool.LevelToColorKey(selectItem.t2.martialInfo.level), 1);
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

        public void AddTip(GameObject go, DataStruct<string, DataProps.MartialData> data)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                if (data.t2 != null)
                {
                    DataUnit.ActionMartialData actionMartialData = unit.data.unitData.GetActionMartial(data.t2.data.soleID);
                    if (actionMartialData != null)
                    {
                        g.ui.OpenUI<UIMartialInfo>(UIType.MartialInfo).InitData(unit, actionMartialData, go.transform.position);
                    }
                }
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.MartialInfo); });
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
                name = "上一次学习的心法:";

            }
            if (selectItem.t2 != null)
            {
                name += GameTool.SetTextReplaceColorKey(selectItem.t2.martialInfo.name, GameTool.LevelToColorKey(selectItem.t2.martialInfo.level), 1);
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
                UITipItem.AddTip("请先选择功法！");
                return;
            }
            string name = "";
            if (selectItem.t1 == "last")
            {
                name = "上一次学习的心法";

            }
            else if (selectItem.t2 != null)
            {
                name = GameTool.SetTextReplaceColorKey(selectItem.t2.martialInfo.name, GameTool.LevelToColorKey(selectItem.t2.martialInfo.level), 1);
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
