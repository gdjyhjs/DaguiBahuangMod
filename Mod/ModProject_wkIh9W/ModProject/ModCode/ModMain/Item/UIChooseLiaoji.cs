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
    // 指定一个飘渺之力
    public class UIChooseLiaoji : MonoBehaviour
    {
        public UIChooseLiaoji(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符(飘渺之力soleid)

        public DataStruct<string, ConfWingmanBaseItem> selectItem;

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

            textTitle.text = "选择飘渺之力";
            leftTitle.text = "可从下面选择飘渺之力";
            rightTitle.text = "已选择的飘渺之力";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            var allAttr = g.conf.wingmanBase._allConfList;
            foreach (ConfWingmanBaseItem item in allAttr)
            {
                if (item.id == 1011)
                    continue;
                var selectItem = new DataStruct<string, ConfWingmanBaseItem>(item.id.ToString(), item);
                string name = GameTool.LS(item.name);

                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<Text>().text = name;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem = selectItem;
                    UpdateLeft();
                }));
                go.SetActive(true);
                AddTip(go, selectItem.t2);
            }
        }

        public void AddTip(GameObject go, ConfWingmanBaseItem conf)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                string tip = UIMartialInfoTool.GetDescRichText(GameTool.LS(conf.effectDesc), new BattleSkillValueData(g.world.playerUnit), 1)+"\n\n"+
                 UIMartialInfoTool.GetDescRichText(GameTool.LS(conf.desc), new BattleSkillValueData(g.world.playerUnit), 1);
                g.ui.OpenUI<UISkyTip>(UIType.SkyTip).InitData(tip, go.transform.position);
            });
            uiEvent.onMouseExitCall += (Action)(() => { g.ui.CloseUI(UIType.SkyTip); });
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void UpdateLeft()
        {
            UnityAPIEx.DestroyChild(leftRoot);
            string name = GameTool.LS(selectItem.t2.name);

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
                UITipItem.AddTip("请先选择飘渺之力！");
                return;
            }
            string name = GameTool.LS(selectItem.t2.name);
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
