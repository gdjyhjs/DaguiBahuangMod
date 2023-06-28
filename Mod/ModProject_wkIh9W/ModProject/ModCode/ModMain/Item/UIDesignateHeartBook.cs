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
    // 指定心法秘籍
    public class UIDesignateHeartBook : MonoBehaviour
    {
        public UIDesignateHeartBook(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 秘籍

        public WorldUnitBase unit;

        public DataProps.MartialData[] allItems
        {
            get
            {
                List<DataProps.MartialData> items = new List<DataProps.MartialData>();

                var props = unit.data.unitData.propData.CloneAllProps();
                foreach (var item in props)
                {
                    if (item.propsType == DataProps.PropsDataType.Martial)
                    {
                        DataProps.MartialData martialData = item.To<DataProps.MartialData>();
                        if (martialData.martialType == MartialType.Ability)
                        {
                            items.Add(martialData);
                        }
                    }
                }

                return items.ToArray();
            }
        }
        public DataProps.MartialData selectItem;

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

            textTitle.text = "选择心法秘籍";
            leftTitle.text = "可从下面选择心法秘籍";
            rightTitle.text = "已选择的心法秘籍";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, WorldUnitBase unit)
        {
            this.unit = unit;
            foreach (var item in allItems)
            {
                var selectItem = item;
                var name = GameTool.SetTextReplaceColorKey(selectItem.martialInfo.name, GameTool.LevelToColorKey(selectItem.martialInfo.level), 1);

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

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = GameTool.SetTextReplaceColorKey(selectItem.martialInfo.name, GameTool.LevelToColorKey(selectItem.martialInfo.level), 1);
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

        public void AddTip(GameObject go, DataProps.MartialData data)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                //DataUnit.ActionMartialData actionMartialData = unit.data.unitData.GetActionMartial(data.data.soleID);
                //g.ui.OpenUI<UIMartialInfo>(UIType.MartialInfo).InitData(unit, actionMartialData, go.transform.position);
                g.ui.OpenUI<UIMartialPropPreview>(UIType.MartialPropPreview).InitData(unit, data, go.transform.position);
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.MartialPropPreview); });
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择秘籍！");
                return;
            }
            call(selectItem.data.soleID, GameTool.SetTextReplaceColorKey(selectItem.martialInfo.name, GameTool.LevelToColorKey(selectItem.martialInfo.level), 1));
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
