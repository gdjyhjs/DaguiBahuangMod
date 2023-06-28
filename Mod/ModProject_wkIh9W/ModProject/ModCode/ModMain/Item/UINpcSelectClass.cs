using Newtonsoft.Json;
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
    // 选择一个角色
    public class UINpcSelectClass : MonoBehaviour
    {
            public UINpcSelectClass(IntPtr ptr) : base(ptr) { }
        public class SelectData
        {
            public List<int> selectSex = new List<int>(); // 性别
            public List<int> selectMeili = new List<int>(); // 魅力
            public List<int> selectShengwang = new List<int>(); // 声望
            public List<int> selectXingge0 = new List<int>(); // 内在性格
            public List<int> selectXingge1 = new List<int>(); // 外在性格1
            public List<int> selectXingge2 = new List<int>(); // 外在性格2
            public List<int> selectJingjie = new List<int>(); // 境界
            public List<int> selectJieduan = new List<int>(); // 境界阶段
            public List<int> selectQinggan = new List<int>(); // 情感
            public List<int> selectHero = new List<int>(); // 英雄
            public List<int> selectDaoxin = new List<int>(); // 道心
            public List<string> selectSchool = new List<string>(); // 宗门
            public bool selectSexAll = true; // 性别
            public bool selectMeiliAll = true; // 魅力
            public bool selectShengwangAll = true; // 声望
            public bool selectXingge0All = true; // 内在性格
            public bool selectXingge1All = true; // 外在性格1
            public bool selectXingge2All = true; // 外在性格2
            public bool selectJingjieAll = true; // 境界
            public bool selectJieduanAll = true; // 境界阶段
            public bool selectQingganAll = true; // 情感
            public bool selectHeroAll = true; // 英雄
            public bool selectDaoxinAll = true; // 道心
            public bool selectSchoolAll = true; // 宗门
        }
        private static SelectData m_selectData;
        public static SelectData selectData
        {
            get
            {
                if (m_selectData == null)
                {
                    bool isOk = false;
                    if (PlayerPrefs.HasKey("UINpcSelectClass_m_selectData"))
                    {
                        try
                        {
                            var str = PlayerPrefs.GetString("UINpcSelectClass_m_selectData");
                            var data = JsonConvert.DeserializeObject<SelectData>(str);
                            if (data != null)
                            {
                                m_selectData = data;
                                isOk = true;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (!isOk)
                    {
                        m_selectData = new SelectData();
                    }
                }
                return m_selectData;
            }set
            {
                m_selectData = value;
                var str = JsonConvert.SerializeObject(m_selectData);
                PlayerPrefs.SetString("UINpcSelectClass_m_selectData", str);
            }
        }
        public static SelectData copyData
        {
            get
            {
                var str = JsonConvert.SerializeObject(selectData);
                return JsonConvert.DeserializeObject<SelectData>(str);
            }
        }

        public Transform rightRoot;
        public Button btnOk;
        public Button btnClose;
        public GameObject itemTitle;
        public GameObject itemToggle;

        public Action okCall;

        void Awake()
        {
            rightRoot = transform.Find("Root/Right/View/Root");
            gameObject.AddComponent<UIFastClose>();
            btnClose = transform.Find("Root/btnClose").GetComponent<Button>();
            btnOk = transform.Find("Root/BtnOk").GetComponent<Button>();

            btnClose.onClick.AddListener((Action)CloseUI);
            btnOk.onClick.AddListener((Action)OnBtnOk);

            itemTitle = transform.Find("ItemTitle").gameObject;
            itemToggle = transform.Find("itemToggle").gameObject;

            transform.Find("Root/BtnRest").GetComponent<Button>().onClick.AddListener((Action)(() =>
            {
                tmpData = new SelectData();
                UpdateUI();
            }));
        }

        public void CloseUI()
        {
            g.ui.CloseUI(GetComponent<UIBase>());
        }

        public void OnBtnOk()
        {
            selectData = tmpData;
            okCall?.Invoke();
            CloseUI();
        }

        void Start()
        {
            tmpData = copyData;
            UpdateUI();
        }

        SelectData tmpData;
        void UpdateUI()
        {
            UnityAPIEx.DestroyChild(rightRoot);
            int lineCount = 8;
            int y = -20;
            var allSexs = new int[] { 1, 2 };
            var title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "性别:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allSexs.Length; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                int sex = allSexs[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = (sex == 1) ? GameTool.LS("common_nan") : GameTool.LS("common_nv");
                toggle.isOn = tmpData.selectSex.Contains(sex) || tmpData.selectSexAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectSex.Add(sex);
                        bool selAll = true;
                        foreach (var data in allSexs)
                        {
                            if (!tmpData.selectSex.Contains(data))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectSexAll = true;
                            tmpData.selectSex.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectSexAll)
                        {
                            tmpData.selectSexAll = false;
                            tmpData.selectSex.Clear();
                            foreach (var data in allSexs)
                            {
                                tmpData.selectSex.Add(data);
                            }
                        }
                        tmpData.selectSex.Remove(sex);
                    }
                    Console.WriteLine(isOn+" " +string.Join(",", tmpData.selectSex));
                }));
                go.SetActive(true);
            }

            y -= 40;
            var allMeili = g.conf.roleBeauty._allConfList;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "魅力:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allMeili.Count; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                ConfRoleBeautyItem item = allMeili[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.text);
                toggle.isOn = tmpData.selectMeili.Contains(item.id) || tmpData.selectMeiliAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectMeili.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allMeili)
                        {
                            if (!tmpData.selectMeili.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectMeiliAll = true;
                            tmpData.selectMeili.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectMeiliAll)
                        {
                            tmpData.selectMeiliAll = false;
                            tmpData.selectMeili.Clear();
                            foreach (var data in allMeili)
                            {
                                tmpData.selectMeili.Add(data.id);
                            }
                        }
                        tmpData.selectMeili.Remove(item.id);
                    }
                }));
                go.SetActive(true);
            }

            y -= 40;
            var allShengwang = g.conf.roleReputation._allConfList;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "声望:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allShengwang.Count; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allShengwang[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.text);
                toggle.isOn = tmpData.selectShengwang.Contains(item.id) || tmpData.selectShengwangAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectShengwang.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allShengwang)
                        {
                            if (!tmpData.selectShengwang.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectShengwangAll = true;
                            tmpData.selectShengwang.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectShengwangAll)
                        {
                            tmpData.selectShengwangAll = false;
                            tmpData.selectShengwang.Clear();
                            foreach (var data in allShengwang)
                            {
                                tmpData.selectShengwang.Add(data.id);
                            }
                        }
                        tmpData.selectShengwang.Remove(item.id);
                    }
                }));
                go.SetActive(true);
            }

            var allXingge = g.conf.roleCreateCharacter._allConfList;
            y -= 40;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "内在性格:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            int index = 0;
            for (int i = 0; i < allXingge.Count; i++)
            {
                var item = allXingge[i];
                if (item.type != 1)
                {
                    continue;
                }
                if (index > 0 && index % lineCount == 0)
                {
                    y -= 40;
                }
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + index % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.sc5asd_sd34);
                toggle.isOn = tmpData.selectXingge0.Contains(item.id) || tmpData.selectXingge0All;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectXingge0.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allXingge)
                        {
                            if (item.type != 1)
                            {
                                continue;
                            }
                            if (!tmpData.selectXingge0.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectXingge0All = true;
                            tmpData.selectXingge0.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectXingge0All)
                        {
                            tmpData.selectXingge0All = false;
                            tmpData.selectXingge0.Clear();
                            foreach (var data in allXingge)
                            {
                                if (item.type != 1)
                                {
                                    continue;
                                }
                                tmpData.selectXingge0.Add(data.id);
                            }
                        }
                        tmpData.selectXingge0.Remove(item.id);
                    }
                }));
                go.SetActive(true);
                index++;
            }

            y -= 40;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "外在性格1:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            index = 0;
            for (int i = 0; i < allXingge.Count; i++)
            {
                var item = allXingge[i];
                if (item.type != 2)
                {
                    continue;
                }
                if (index > 0 && index % lineCount == 0)
                {
                    y -= 40;
                }
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + index % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.sc5asd_sd34);
                toggle.isOn = tmpData.selectXingge1.Contains(item.id) || tmpData.selectXingge1All;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectXingge1.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allXingge)
                        {
                            if (item.type != 2)
                            {
                                continue;
                            }
                            if (!tmpData.selectXingge1.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectXingge1All = true;
                            tmpData.selectXingge1.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectXingge1All)
                        {
                            tmpData.selectXingge1All = false;
                            tmpData.selectXingge1.Clear();
                            foreach (var data in allXingge)
                            {
                                if (item.type != 2)
                                {
                                    continue;
                                }
                                tmpData.selectXingge1.Add(data.id);
                            }
                        }
                        tmpData.selectXingge1.Remove(item.id);
                    }
                }));
                go.SetActive(true);
                index++;
            }

            y -= 40;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "外在性格2:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            index = 0;
            for (int i = 0; i < allXingge.Count; i++)
            {
                var item = allXingge[i];
                if (item.type != 2)
                {
                    continue;
                }
                if (index > 0 && index % lineCount == 0)
                {
                    y -= 40;
                }
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + index % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.sc5asd_sd34);
                toggle.isOn = tmpData.selectXingge2.Contains(item.id) || tmpData.selectXingge2All;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectXingge2.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allXingge)
                        {
                            if (item.type != 2)
                            {
                                continue;
                            }
                            if (!tmpData.selectXingge2.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectXingge2All = true;
                            tmpData.selectXingge2.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectXingge2All)
                        {
                            tmpData.selectXingge2All = false;
                            tmpData.selectXingge2.Clear();
                            foreach (var data in allXingge)
                            {
                                if (item.type != 2)
                                {
                                    continue;
                                }
                                tmpData.selectXingge2.Add(data.id);
                            }
                        }
                        tmpData.selectXingge2.Remove(item.id);
                    }
                }));
                go.SetActive(true);
                index++;
            }



            y -= 40;
            DataStruct<string, string>[] allAttr = new DataStruct<string, string>[]{
                new DataStruct<string, string>("1", "练气"),
                new DataStruct<string, string>("2", "筑基"),
                new DataStruct<string, string>("3", "结晶"),
                new DataStruct<string, string>("4", "金丹"),
                new DataStruct<string, string>("5", "具灵"),
                new DataStruct<string, string>("6", "元婴"),
                new DataStruct<string, string>("7", "化神"),
                new DataStruct<string, string>("8", "悟道"),
                new DataStruct<string, string>("9", "羽化"),
                new DataStruct<string, string>("10", "登仙"),
            };
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "大境界:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allAttr.Length; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allAttr[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = item.t2;
                toggle.isOn = tmpData.selectJingjie.Contains(int.Parse(item.t1)) || tmpData.selectJingjieAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectJingjie.Add(int.Parse(item.t1));
                        bool selAll = true;
                        foreach (var data in allAttr)
                        {
                            if (!tmpData.selectJingjie.Contains(int.Parse(data.t1)))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectJingjieAll = true;
                            tmpData.selectJingjie.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectJingjieAll)
                        {
                            tmpData.selectJingjieAll = false;
                            tmpData.selectJingjie.Clear();
                            foreach (var data in allAttr)
                            {
                                tmpData.selectJingjie.Add(int.Parse(data.t1));
                            }
                        }
                        tmpData.selectJingjie.Remove(int.Parse(item.t1));
                    }
                }));
                go.SetActive(true);
            }

            y -= 40;
            DataStruct<string, string>[] allAttr2 = new DataStruct<string, string>[]{
                new DataStruct<string, string>("1", "初期"),
                new DataStruct<string, string>("2", "中期"),
                new DataStruct<string, string>("3", "后期"),
            };
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "境界阶段:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allAttr2.Length; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allAttr2[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = item.t2;
                toggle.isOn = tmpData.selectJieduan.Contains(int.Parse(item.t1)) || tmpData.selectJieduanAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectJieduan.Add(int.Parse(item.t1));
                        bool selAll = true;
                        foreach (var data in allAttr2)
                        {
                            if (!tmpData.selectJieduan.Contains(int.Parse(data.t1)))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectJieduanAll = true;
                            tmpData.selectJieduan.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectJieduanAll)
                        {
                            tmpData.selectJieduanAll = false;
                            tmpData.selectJieduan.Clear();
                            foreach (var data in allAttr2)
                            {
                                tmpData.selectJieduan.Add(int.Parse(data.t1));
                            }
                        }
                        tmpData.selectJieduan.Remove(int.Parse(item.t1));
                    }
                }));
                go.SetActive(true);
            }


            y -= 40;
            DataStruct<string, string>[] allAttr3 = new DataStruct<string, string>[]{
                new DataStruct<string, string>("0", "咸鱼[无道心]"),
                new DataStruct<string, string>("1", "人豪[有道种]"),
                new DataStruct<string, string>("2", "天骄[有道心]"),
            };
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "道的追求:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allAttr3.Length; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allAttr3[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = item.t2;
                toggle.isOn = tmpData.selectHero.Contains(int.Parse(item.t1)) || tmpData.selectHeroAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectHero.Add(int.Parse(item.t1));
                        bool selAll = true;
                        foreach (var data in allAttr3)
                        {
                            if (!tmpData.selectHero.Contains(int.Parse(data.t1)))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectHeroAll = true;
                            tmpData.selectHero.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectHeroAll)
                        {
                            tmpData.selectHeroAll = false;
                            tmpData.selectHero.Clear();
                            foreach (var data in allAttr3)
                            {
                                tmpData.selectHero.Add(int.Parse(data.t1));
                            }
                        }
                        tmpData.selectHero.Remove(int.Parse(item.t1));
                    }
                }));
                go.SetActive(true);
            }

            y -= 40;
            var allDaoxin = g.conf.taoistHeart._allConfList;
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "道心:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allDaoxin.Count; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allDaoxin[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = GameTool.LS(item.heartName);
                toggle.isOn = tmpData.selectDaoxin.Contains(item.id) || tmpData.selectDaoxinAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectDaoxin.Add(item.id);
                        bool selAll = true;
                        foreach (var data in allDaoxin)
                        {
                            if (!tmpData.selectDaoxin.Contains(data.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectDaoxinAll = true;
                            tmpData.selectDaoxin.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectDaoxinAll)
                        {
                            tmpData.selectDaoxinAll = false;
                            tmpData.selectDaoxin.Clear();
                            foreach (var data in allDaoxin)
                            {
                                tmpData.selectDaoxin.Add(data.id);
                            }
                        }
                        tmpData.selectDaoxin.Remove(item.id);
                    }
                }));
                go.SetActive(true);
            }


            y -= 40;
            DataStruct<string, string>[] allAttr4 = new DataStruct<string, string>[]{
                new DataStruct<string, string>("0", "未婚"),
                new DataStruct<string, string>("1", "已婚"),
            };
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "婚姻:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            for (int i = 0; i < allAttr4.Length; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = allAttr4[i];
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = item.t2;
                toggle.isOn = tmpData.selectQinggan.Contains(int.Parse(item.t1)) || tmpData.selectQingganAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectQinggan.Add(int.Parse(item.t1));
                        bool selAll = true;
                        foreach (var data in allAttr4)
                        {
                            if (!tmpData.selectQinggan.Contains(int.Parse(data.t1)))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectQingganAll = true;
                            tmpData.selectQinggan.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectQingganAll)
                        {
                            tmpData.selectQingganAll = false;
                            tmpData.selectQinggan.Clear();
                            foreach (var data in allAttr4)
                            {
                                tmpData.selectQinggan.Add(int.Parse(data.t1));
                            }
                        }
                        tmpData.selectQinggan.Remove(int.Parse(item.t1));
                    }
                }));
                go.SetActive(true);
            }

            y -= 40;
            List<MapBuildSchool> builds = new List<MapBuildSchool>(g.world.build.GetBuilds<MapBuildSchool>().ToArray());
            title = GameObject.Instantiate(itemTitle, rightRoot);
            title.GetComponent<Text>().text = "宗门:";
            title.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, y);
            title.SetActive(true);
            builds.Insert(0, null);
            for (int i = 0; i < builds.Count; i++)
            {
                if (i > 0 && i % lineCount == 0)
                {
                    y -= 40;
                }
                var item = builds[i];
                string name = item == null ? "散修" : item.name;
                string id = item == null ? "" : item.buildData.id;
                var go = GameObject.Instantiate(itemToggle, rightRoot);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 + i % lineCount * 180, y);
                var toggle = go.GetComponent<Toggle>();
                go.GetComponentInChildren<Text>().text = name;
                toggle.isOn = tmpData.selectSchool.Contains(id) || tmpData.selectSchoolAll;
                toggle.onValueChanged.AddListener((Action<bool>)((isOn) =>
                {
                    if (isOn)
                    {
                        tmpData.selectSchool.Add(id);
                        bool selAll = true;
                        foreach (var data in builds)
                        {
                            if (!tmpData.selectSchool.Contains(data == null ? "" : data.buildData.id))
                            {
                                selAll = false;
                                break;
                            }
                        }
                        if (selAll)
                        {
                            tmpData.selectSchoolAll = true;
                            tmpData.selectSchool.Clear();
                        }
                    }
                    else
                    {
                        if (tmpData.selectSchoolAll)
                        {
                            tmpData.selectSchoolAll = false;
                            tmpData.selectSchool.Clear();
                            foreach (var data in builds)
                            {
                                tmpData.selectSchool.Add(data == null ? "" : data.buildData.id);
                            }
                        }
                        tmpData.selectSchool.Remove(id);
                    }
                }));
                go.SetActive(true);
            }
            y -= 40;

            rightRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Abs(y));
        }

        public static bool isOpenSel;
        public static void InitSel(Transform root, Action updateCall)
        {
            var tgl = root.Find("Root/selToggle").GetComponent<Toggle>();
            var btn = root.Find("Root/selBtn").GetComponent<Button>();
            tgl.GetComponentInChildren<Text>().text = "启动筛选功能";
            isOpenSel = PlayerPrefs.GetInt("SelectOneCharselTgl", 0) == 1;
            tgl.isOn = isOpenSel;
            btn.gameObject.SetActive(isOpenSel);
            btn.GetComponentInChildren<Text>().text = "选择筛选条件";
            tgl.onValueChanged.AddListener((Action<bool>)((isOn)=>
            {
                isOpenSel = isOn;
                btn.gameObject.SetActive(isOpenSel);
                updateCall?.Invoke();
            }));
            btn.onClick.AddListener((Action)(() =>
            {
                ModMain.OpenUI<UINpcSelectClass>("NpcSelectClass").okCall = updateCall;
            }));
        }

        public static bool CheckAll()
        {
            var data = selectData;
            return data.selectSexAll && data.selectMeiliAll && data.selectShengwangAll && data.selectXingge0All && data.selectXingge1All && data.selectXingge2All
                && data.selectJingjieAll && data.selectJieduanAll && data.selectJieduanAll && data.selectQingganAll && data.selectHeroAll
                && data.selectDaoxinAll && data.selectSchoolAll;
        }

        public static bool CheckUnit(WorldUnitBase unit)
        {
            WorldUnitDynData unitData = unit.data.dynUnitData;
            var data = selectData;

            if ((!data.selectSchoolAll))
            {
                string id = unit.data.unitData.schoolID;
                if (!data.selectSchool.Contains(id))
                {
                    return false;
                }
            }
            if ((!data.selectDaoxinAll))
            {
                int id = unit.data.unitData.heart.confID;
                if (!data.selectDaoxin.Contains(id))
                {
                    return false;
                }
            }
            if ((!data.selectHeroAll))
            {
                int state = (int)unit.data.unitData.heart.state;
                if (!data.selectHero.Contains(state))
                {
                    return false;
                }
            }
            if ((!data.selectQingganAll))
            {
                var state = string.IsNullOrEmpty(unit.data.unitData.relationData.married) ? 0 : 1;
                if (!data.selectQinggan.Contains(state))
                {
                    return false;
                }
            }
            if ((!data.selectJieduanAll))
            {
                ConfRoleGradeItem gradeItem = g.conf.roleGrade.GetItem(unitData.gradeID.value);
                if (!data.selectJieduan.Contains(gradeItem.phase))
                {
                    return false;
                }
            }
            if ((!data.selectJingjieAll))
            {
                if (!data.selectJingjie.Contains(unitData.GetGrade()))
                {
                    return false;
                }
            }
            if ((!data.selectXingge2All))
            {
                if (!data.selectXingge2.Contains(unitData.outTrait2.value))
                {
                    return false;
                }
            }
            if ((!data.selectXingge1All))
            {
                if (!data.selectXingge1.Contains(unitData.outTrait1.value))
                {
                    return false;
                }
            }
            if ((!data.selectXingge0All))
            {
                if (!data.selectXingge0.Contains(unitData.inTrait.value))
                {
                    return false;
                }
            }
            if ((!data.selectShengwangAll))
            {
                ConfRoleReputationItem reputationItem = g.conf.roleReputation.GetItemInReputation(unitData.reputation.value);
                if (!data.selectShengwang.Contains(reputationItem.id))
                {
                    return false;
                }
            }
            if ((!data.selectMeiliAll))
            {
                ConfRoleBeautyItem beautyItem = g.conf.roleBeauty.GetItemInBeauty(unitData.beauty.value);
                if (!data.selectMeili.Contains(beautyItem.id))
                {
                    return false;
                }
            }
            if ((!data.selectSexAll) && (!data.selectSex.Contains(unitData.sex.value)))
            {
                return false;
            }

            return true;
        }
    }
}
