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
    // 指定套装
    public class UIDesignateSuit : MonoBehaviour
    {
        public UIDesignateSuit(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 套装id


        public ConfBattleAbilitySuitBaseItem[] allItems
        {
            get
            {
                return g.conf.battleAbilitySuitBase._allConfList.ToArray();
            }
        }
        public ConfBattleAbilitySuitBaseItem selectItem;



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

            textTitle.text = "指定一个套装";
            leftTitle.text = "可从下面选择套装";
            rightTitle.text = "已选择的套装";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, int abilityId)
        {
            foreach (var item in allItems)
            {
                string[] list = item.suitMember.Split('|');
                if (!list.Contains(abilityId.ToString()))
                    continue;

                var selectItem = item;
                var name = GameTool.LS(item.suitName);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);

                go.AddComponent<UISkyTipEffect>().InitData(UIMartialInfoTool.GetDescRichText(GameTool.LS(selectItem.suitDesc1), new BattleSkillValueData() { grade = g.world.playerUnit.data.dynUnitData.GetGrade(), level = 1 }, 1));
            }

        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            var name = GameTool.LS(selectItem.suitName);
            var go = GameObject.Instantiate(goItem, leftRoot);
            go.GetComponent<Text>().text = name;
            go.AddComponent<Button>().onClick.AddListener((Action)(() =>
            {
                UnityAPIEx.DestroyChild(leftRoot);
                selectItem = null;
            }));
            go.SetActive(true);
            go.AddComponent<UISkyTipEffect>().InitData(UIMartialInfoTool.GetDescRichText(GameTool.LS(selectItem.suitDesc1), new BattleSkillValueData() { grade = g.world.playerUnit.data.dynUnitData.GetGrade(), level = 1 }, 1));
        }

        public void OnBtnOk()
        {
            if (selectItem == null)
            {
                UITipItem.AddTip("请先选择套装！");
                return;
            }
            call(selectItem.id.ToString(), GameTool.LS(selectItem.suitName) );
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
