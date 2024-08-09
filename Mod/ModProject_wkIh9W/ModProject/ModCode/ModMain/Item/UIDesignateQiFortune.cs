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
    // 指定气运
    public class UIDesignateQiFortune : MonoBehaviour
    {
        public UIDesignateQiFortune(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 气运id


        public WorldUnitBase unit;
        public ConfRoleCreateFeatureItem[] allItems
        {
            get
            {
                List<ConfRoleCreateFeatureItem> list = new List<ConfRoleCreateFeatureItem>();
                var xx = unit.data.unitData.propertyData.bornLuck;
                var yy = unit.data.unitData.propertyData.addLuck;
                foreach (var item in xx)
                {
                    ConfRoleCreateFeatureItem featureItem = g.conf.roleCreateFeature.GetItem(item.id);
                    list.Add(featureItem);
                }
                foreach (var item in yy)
                {
                    ConfRoleCreateFeatureItem featureItem = g.conf.roleCreateFeature.GetItem(item.id);
                    if (list.Find((v)=>v.id == featureItem.id) == null)
                    {
                        list.Add(featureItem);
                    }
                }
                Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade;
                if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
                {
                    upGrade = g.data.world.playerLog.upGrade;
                }
                else
                {
                    upGrade = unit.data.unitData.npcUpGrade;
                }
                foreach (var item in upGrade)
                {
                    ConfRoleCreateFeatureItem featureItem = g.conf.roleCreateFeature.GetItem(item.Value.luck);
                    if (featureItem != null)
                    {
                        if (list.Find((v) => v.id == featureItem.id) == null)
                        {
                            list.Add(featureItem);
                        }
                    }
                }
                return list.ToArray();
            }
        }
        public ConfRoleCreateFeatureItem selectItem;



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

            textTitle.text = "选择气运";
            leftTitle.text = "可从下面选择气运";
            rightTitle.text = "已选择的气运";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, WorldUnitBase unit)
        {
            this.unit = unit;
            foreach (var item in allItems)
            {
                var selectItem = item;
                var name = CommonTool.SetTextColor(GameTool.LS(selectItem.name), GameTool.LevelToColor(selectItem.level, 1)) + "(" + selectItem.id + ")";

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
            }

        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = CommonTool.SetTextColor(GameTool.LS(selectItem.name), GameTool.LevelToColor(selectItem.level, 1)) + "(" + selectItem.id + ")";
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
                UITipItem.AddTip("请先选择气运！");
                return;
            }
            call(selectItem.id.ToString(), CommonTool.SetTextColor(GameTool.LS(selectItem.name), GameTool.LevelToColor(selectItem.level, 1)) + "(" + selectItem.id + ")");
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
