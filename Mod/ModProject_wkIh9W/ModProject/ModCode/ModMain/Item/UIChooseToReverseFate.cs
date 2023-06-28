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
    // 选择逆天改命
    public class UIChooseToReverseFate : MonoBehaviour
    {
        public UIChooseToReverseFate(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 逆天改命id
        public ConfFateFeatureItem[] allItems
        {
            get
            {
                return g.conf.fateFeature._allConfList.ToArray();
            }
        }
        public ConfFateFeatureItem selectItem;



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

            textTitle.text = "选择逆天改命";
            leftTitle.text = "可从下面选择逆天改命";
            rightTitle.text = "已选择的逆天改命";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            foreach (var item in allItems)
            {
                var selectItem = item;
                var name = GameTool.LS(g.conf.roleCreateFeature.GetItem(item.id).name);
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var conf = g.conf.roleCreateFeature.GetItem(selectItem.id);
                name = CommonTool.SetTextColor(name, GameTool.LevelToColor(conf.level, 2));
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
            var conf = g.conf.roleCreateFeature.GetItem(selectItem.id);
            var name = CommonTool.SetTextColor(GameTool.LS(conf.name), GameTool.LevelToColor(conf.level, 2));
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
                UITipItem.AddTip("请先选择逆天改命！");
                return;
            }
            var conf = g.conf.roleCreateFeature.GetItem(selectItem.id);
            var name = CommonTool.SetTextColor(GameTool.LS(conf.name), GameTool.LevelToColor(conf.level, 2));
            call(selectItem.id.ToString(), name);
            CloseUI();
        }

        public void AddTip(GameObject go, ConfFateFeatureItem selectItem)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                ConfRoleCreateFeatureItem conf = g.conf.roleCreateFeature.GetItem(selectItem.id);
                var tips = UIMartialInfoTool.GetDescRichText(GameTool.LS(conf.tips), new BattleSkillValueData() { grade = 1, level = 1 }, 1,"y","e");
                g.ui.OpenUI<UISkyTip>(UIType.SkyTip).InitData(tips, go.transform.position);
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.SkyTip); });
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
