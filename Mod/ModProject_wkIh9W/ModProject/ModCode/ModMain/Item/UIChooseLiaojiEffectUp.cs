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
    // 选择升级飘渺之力效果
    public class UIChooseLiaojiEffectUp : MonoBehaviour
    {
        public UIChooseLiaojiEffectUp(IntPtr ptr) : base(ptr) { }

        public Action<string, string> call; // 参数字符， 飘渺之力效果id

        public List<DataStruct<string, string>> selectItem = new List<DataStruct<string, string>>(); // 最多11


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

            var prefab = g.res.Load<GameObject>("UI/Item/CostArrtItem");
            var prefabText = prefab.transform.Find("Root/Item/Text/Text").GetComponent<TMPro.TextMeshProUGUI>();
            var pText = goItem.GetComponent<TMPro.TextMeshProUGUI>();
            pText.font = prefabText.font;
            pText.spriteAsset = prefabText.spriteAsset;

            textTitle.text = "选择升级的飘渺之力效果";
            leftTitle.text = "可从下面选择飘渺之力效果";
            rightTitle.text = "已选择的飘渺之力效果";
        }

        public void InitData(UIDaguiToolItem toolItem, int index, int id)
        {
            foreach (var item in g.conf.wingmanFixValue._allConfList)
            {
                var selectItem = item;
                if (item.name == "0" || item.wingManID != id)
                    continue;
                string tips;
                try
                {
                    tips = GameTool.LS(selectItem.name) + ":" + UIMartialInfoTool.GetDescRichText(GameTool.LS(item.desc), new BattleSkillValueData() { grade = 1, level = 1 }, 2, "y", "e");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    tips = GameTool.LS(selectItem.name) + ":" + GameTool.LS(selectItem.desc);
                }
                var go = GameObject.Instantiate(goItem, rightRoot);
                go.GetComponent<TMPro.TextMeshProUGUI>().text = tips;
                go.AddComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    if (this.selectItem.Count >= 99)
                    {
                        UITipItem.AddTip("最多选择99个飘渺之力效果！");
                        return;
                    }
                    this.selectItem.Add(new DataStruct<string, string>(selectItem.id.ToString(), tips));
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
            for (int i = 0; i < selectItem.Count; i++)
            {
                var index = i;
                var leftItem = selectItem[i];
                var go = GameObject.Instantiate(goItem, leftRoot);
                go.GetComponent<TMPro.TextMeshProUGUI>().text = leftItem.t2;
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
                UITipItem.AddTip("至少选择1个飘渺之力效果！");
                return;
            }
            List<string> data1 = new List<string>();
            foreach (var item in selectItem)
            {
                data1.Add(item.t1.ToString());
            }
            call(string.Join(",", data1), "已选择"+ data1.Count+"个飘渺之力效果");
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
