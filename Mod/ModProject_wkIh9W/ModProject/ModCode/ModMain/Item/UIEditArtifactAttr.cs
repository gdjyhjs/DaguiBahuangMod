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
    // 选择增加词条
    public class UIEditArtifactAttr : MonoBehaviour
    {
        public UIEditArtifactAttr(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 词条id

        public List<ConfBattleSkillPrefixValueItem> selectItem = new List<ConfBattleSkillPrefixValueItem>(); // 最多11


        public Transform leftRoot;
        public Transform rightRoot;
        public Transform AttrRoot;
        public GameObject goItem;
        public GameObject attrItem;
        public Text textTitle;
        public Text leftTitle;
        public Text rightTitle;
        public Button btnClose;
        public Button btnOk;
        void Awake()
        {
            leftRoot = transform.Find("Root/Left/View/Root");
            rightRoot = transform.Find("Root/Right/View/Root");
            AttrRoot = transform.Find("Root/AttrList");

            goItem = transform.Find("Item").gameObject;
            attrItem = transform.Find("AttrItem").gameObject;

            textTitle = transform.Find("Root/Text1").GetComponent<Text>();
            leftTitle = transform.Find("Root/Text2").GetComponent<Text>();
            rightTitle = transform.Find("Root/Text3").GetComponent<Text>();

            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk.onClick.AddListener((Action)OnBtnOk);

            gameObject.AddComponent<UIFastClose>();

            var prefab = g.res.Load<GameObject>("UI/Item/CostArrtItem");
            var prefabText = prefab.transform.Find("Root/Item/Text/Text").GetComponent<TMPro.TextMeshProUGUI>();
            var pText = goItem.GetComponent<TMPro.TextMeshProUGUI>();
            pText.font = prefabText.font;
            pText.spriteAsset = prefabText.spriteAsset;

            textTitle.text = "编辑法宝词条和属性";
            leftTitle.text = "可从下面选择法宝词条，最多选择11条";
            rightTitle.text = "已选择的词条";
        }


        public void InitData(UIDaguiToolItem toolItem, int index, DataProps.PropsArtifact propData)
        {
            ConfArtifactShapeItem shapeConf = g.conf.artifactShape.GetItem(propData.data.propsID);
            var allItems = g.conf.battleSkillPrefixValue.items[11][shapeConf.skillID];
            foreach (ConfBattleSkillPrefixValueItem item in allItems)
            {
                var selectItem = item;
                string tips;
                try
                {
                    tips = UIMartialInfoTool.GetDescRichText(GameTool.LS(item.desc), new BattleSkillValueData() { grade = 1, level = 1 }, 2, "y", "e");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    tips = GameTool.LS(item.desc);
                }
                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<TMPro.TextMeshProUGUI>().text = tips;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    this.selectItem.Add(selectItem);
                    UpdateLeft();
                }));
                go.SetActive(true);
            }
            var values = propData.data.values;
            string[] attrName = new string[] { "境界（1-10）", "耐久度", "品质（1-6）", "生命比例", "攻击比例", "防御比例", "魂力比例", "SP", };
            for (int i = 0; i < attrName.Length; i++)
            {
                var idx = i + 3;

                var go = GameObject.Instantiate(attrItem, AttrRoot);
                go.transform.Find("Placeholder").GetComponent<Text>().text = attrName[i];
                go.transform.Find("Name").GetComponent<Text>().text = attrName[i]+":";
                var input = go.GetComponentInChildren<InputField>();
                input.text = values[idx].ToString();
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
            for (int i = 0; i < selectItem.Count; i++)
            {
                var index = i;
                var leftItem = selectItem[i];
                string tips;
                try
                {
                    tips = UIMartialInfoTool.GetDescRichText(GameTool.LS(leftItem.desc), new BattleSkillValueData() { grade = 1, level = 1 }, 2, "y", "e");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    tips = GameTool.LS(leftItem.desc);
                }
                var go = GameObject.Instantiate(goItem, leftRoot);
                go.GetComponent<TMPro.TextMeshProUGUI>().text = tips;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    selectItem.RemoveAt(index);
                    UpdateLeft();
                }));
                go.SetActive(true);

            }

        }

        public void OnBtnOk()
        {
            if (selectItem.Count < 1)
            {
                UITipItem.AddTip("至少选择1个词条！");
                return;
            }
            List<string> data1 = new List<string>();
            List<int> data2 = new List<int>();
            foreach (var item in selectItem)
            {
                data1.Add(item.number.ToString());
                data2.Add(item.id);
            }
            List<string> data = new List<string>();
            for (int i = 0; i < AttrRoot.childCount; i++)
            {
                var child = AttrRoot.GetChild(i);
                var input = child.GetComponentInChildren<InputField>();
                data.Add(input.text);
            }
            data.AddRange(data1);
            call(string.Join(",", data), "已选择"+data2.Count+"个词条");
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
