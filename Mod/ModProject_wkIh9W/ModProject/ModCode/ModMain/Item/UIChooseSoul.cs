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
    // 指定一个神魂
    public class UIChooseSoul : MonoBehaviour
    {
        public UIChooseSoul(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符(神魂soleid)


        public static DataStruct<string, string>[] allAttr = new DataStruct<string, string>[]{
            new DataStruct<string, string>("1", "蛟龙"),
            new DataStruct<string, string>("2", "勾陈"),
            new DataStruct<string, string>("3", "玄龟"),
            new DataStruct<string, string>("4", "陆吾"),
            new DataStruct<string, string>("5", "重明"),
        };
        public DataStruct<string, string> selectItem;

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

            textTitle.text = "选择神魂";
            leftTitle.text = "可从下面选择神魂";
            rightTitle.text = "已选择的神魂";
        }


        public void InitData(UIDaguiToolItem toolItem, int index)
        {
            foreach (var item in allAttr)
            {
                var selectItem = item;
                string name = selectItem.t2;

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

        public void AddTip(GameObject go, DataStruct<string, string> data)
        {
            var uiEvent = go.AddComponent<UIEventListener>();
            uiEvent.onMouseEnterCall += (Action)(() => {
                int id = int.Parse(data.t1);
                ConfElderBaseItem conf = g.conf.elderBase.GetItem(id);
                Console.WriteLine(id+" "+conf);
                string tip = UIMartialInfoTool.GetDescRichText(GameTool.LS(conf.desc), new BattleSkillValueData(g.world.playerUnit), 2);
                try
                {
                    var confs = g.conf.elderLevel.GetItemInElderID(id);
                    confs.Sort((Func<ConfElderLevelItem, ConfElderLevelItem, int>)((a, b) => a.level - b.level));
                    for (int i = 0; i < confs.Count; i++)
                    {
                        tip += "\n  " + confs[i].level + "." + UIMartialInfoTool.GetDescRichText(GameTool.LS(confs[i].desc), new BattleSkillValueData(g.world.playerUnit), 2);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(tip);
                    Console.WriteLine(e.ToString());
                }
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
            string name = selectItem.t2;
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
                UITipItem.AddTip("请先选择神魂！");
                return;
            }
            string name = selectItem.t2;
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
