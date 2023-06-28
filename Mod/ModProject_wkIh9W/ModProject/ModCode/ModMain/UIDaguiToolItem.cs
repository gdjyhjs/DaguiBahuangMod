using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using MOD_wkIh9W.Item;

namespace MOD_wkIh9W
{
    public class UIDaguiToolItem
    {
        public string func;
        public string[] allParams = new string[4];
        public string[] allParamsName = new string[4];
        public string[] list;
        public string[] listName;
        public GameObject[] listGo;

        public Func<WorldUnitBase> getUnit;
        public Func<DataProps.MartialData> getMatrialData;
        public Func<DataProps.PropsData> getArtifactData;
        public WorldUnitBase unit { get => getUnit?.Invoke(); }
        public DataProps.MartialData matrialData { get => getMatrialData?.Invoke(); }
        public DataProps.PropsData artifactData { get => getArtifactData?.Invoke(); }

        public int selectLiaoji;

        public UIDaguiTool tool;

        public void SaveData()
        {
            PlayerPrefs.SetString("UIDaguiToolItem_" + func, string.Join(" ", allParams));
            PlayerPrefs.SetString("UIDaguiToolItemName_" + func, string.Join(" ", allParamsName));
        }

        public void ReadData()
        {
            if (PlayerPrefs.HasKey("UIDaguiToolItem_" + func))
            {
                var str = PlayerPrefs.GetString("UIDaguiToolItem_" + func);
                var readParams = str.Split(' ');
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        allParams[i] = readParams[i];
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (PlayerPrefs.HasKey("UIDaguiToolItemName_" + func))
            {
                var str = PlayerPrefs.GetString("UIDaguiToolItemName_" + func);
                var readParams = str.Split(' ');
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        allParamsName[i] = readParams[i];
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void InitData(UIDaguiTool tool, ConfDaguiToolItem item, Transform transform)
        {
            this.tool = tool;
            var root = transform.Find("Root");
            var rightTitleItem = tool.rightTitleItem;
            var rightButtonItem = tool.rightButtonItem;
            var rightInputItem = tool.rightInputItem;

            var title = GameObject.Instantiate(rightTitleItem, root);
            title.GetComponentInChildren<Text>().text = item.titleName;
            title.gameObject.SetActive(true);

            list = new string[] {item.p1, item.p2, item.p3, item.p4 };
            listName = new string[] { item.para1, item.para2, item.para3, item.para4 };
            listGo = new GameObject[4];
            func = item.func;
            ReadData();
            for (int i = 0; i < list.Length; i++)
            {
                int index = i;
                var para = list[i];
                if (string.IsNullOrEmpty(para) || para == "UpGrade")
                {
                    continue;
                }

                int itemType = 0; // 0按钮 1输入框
                Action clickAction = null;
                GameObject goItem = null;
                SetItemData(listName, index, para, ref itemType, ref clickAction);

                if (itemType == 0)
                {
                    goItem = GameObject.Instantiate(rightButtonItem, root);
                    goItem.transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + allParams[index];
                    goItem.AddComponent<Button>().onClick.AddListener((Action)(() => clickAction?.Invoke()));
                }
                else if (itemType == 1)
                {
                    goItem = GameObject.Instantiate(rightInputItem, root);
                    goItem.transform.Find("Text").GetComponent<Text>().text = listName[index]+"：";
                    goItem.transform.Find("InputField/Placeholder").GetComponent<Text>().text = "";
                    var input = goItem.GetComponentInChildren<InputField>();
                    input.text = string.IsNullOrEmpty(allParamsName[index]) ? allParams[index] : allParamsName[index];
                    input.onValueChanged.AddListener((Action<string>)((v) =>
                    {
                        allParams[index] = v;
                    }));
                }
                else if (itemType == 3) // 多个输入框
                {
                    goItem = GameObject.Instantiate(rightButtonItem, root);
                    goItem.transform.Find("Text").GetComponent<Text>().text = "输入属性";
                    goItem.AddComponent<Button>().onClick.AddListener((Action)(() => clickAction?.Invoke()));
                }
                else
                {
                    Console.WriteLine("匹配不到项 " + itemType + " " + para);
                }
                goItem.gameObject.SetActive(true);
                listGo[i] = goItem;
            }

            var btnRun = transform.Find("BtnRun").GetComponent<Button>();
            btnRun.onClick.AddListener((Action)(()=>
            {
                SaveData();
                List<string> cmd = new List<string>();
                cmd.Add(item.func);
                cmd.AddRange(allParams);
                ModMain.cmdRun.Cmd(cmd.ToArray());
            }));
            var BtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            BtnAdd.onClick.AddListener((Action)(() =>
            {
                SaveData();
                List<string> cmd = new List<string>();
                cmd.Add(item.func);
                cmd.AddRange(allParams);
                string cmdStr = string.Join(" ", cmd);
                tool.AddCmdItem(cmdStr);
                UIDaguiTool.tmpCmdItem.cmds.Add(cmdStr);
                ModMain.cmdRun.Cmd(cmd.ToArray());
            }));
        }

        public static string debugItemPara;
        private void SetItemData(string[] listName, int index, string para, ref int itemType, ref Action clickAction)
        {
            debugItemPara = para;
            if (para == "ChooseQiFortune") // 选择气运 UIChooseQiFortune
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseQiFortune>(para);
                    ui.InitData(this, index);
                    ui.call = (p, attrName) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                    };
                };
            }
            else if (para == "DesignateQiFortune") // 指定气运 UIDesignateQiFortune
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateQiFortune>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (p, attrName) =>
                    {
                        allParams[index] = p;
                        allParamsName[index] = listName[index] + ":" + attrName;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                    };
                };
            }
            else if (para == "AttributeType") // 选择角色属性类型 UIAttributeType
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIAttributeType>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        allParamsName[index] = listName[index] + ":" + attrName;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                    };
                };
            }
            else if (para == "DaoHeartType") // 选择道心类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDaoHeartType>(para);
                    ui.InitData(this, index);
                    ui.call = (p, items) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistHeart.GetItem(items).heartName);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistHeart.GetItem(items).heartName);
                    };
                };
            }
            else if (para == "ChooseHeroSkill") // 选择套路类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseHeroSkill>(para);
                    ui.InitData(this, index);
                    ui.call = (p, name) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + name;
                        allParamsName[index] = listName[index] + ":" + name;
                    };
                };
            }
            else if (para == "DaoKindType") // 选择道种类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDaoKindType>(para);
                    ui.InitData(this, index);
                    ui.call = (p, items) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistSeed.GetItem(items).seedName);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistSeed.GetItem(items).seedName);
                    };
                };
            }
            else if (para == "ResidualHeartType") // 选择残心类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIResidualHeartType>(para);
                    ui.InitData(this, index);
                    ui.call = (p, items) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistHeart.GetItem(items).heartName);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(g.conf.taoistHeart.GetItem(items).heartName);
                    };
                };
            }
            else if (para == "ChooseToReverseFate") // 选择逆天改命 UIChooseToReverseFate
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseToReverseFate>(para);
                    ui.InitData(this, index);
                    ui.call = (p, attrName) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        attrName;
                        allParamsName[index] = listName[index] + ":" +
                        attrName;
                    };
                };
            }
            else if (para == "ChooseToSchoolFate") // 选择逆天改命
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseToSchoolFate>(para);
                    ui.InitData(this, index);
                    ui.call = (p, attrName) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        attrName;
                        allParamsName[index] = listName[index] + ":" +
                        attrName;
                    };
                };
            }
            else if (para == "ChooseDaoNumber") // 选择道号
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseDaoNumber>(para);
                    ui.InitData(this, index);
                    ui.call = (p, items) =>
                    {
                        allParams[index] = p;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(g.conf.appellationTitle.GetItem(items[0]).name);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(g.conf.appellationTitle.GetItem(items[0]).name);
                    };
                };
            }
            else if (para == "Position") // 选择宗门职位
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIPosition>(para);
                    ui.InitData(this, index);
                    ui.call = (item, name) =>
                    {
                        allParams[index] = item;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + name;
                        allParamsName[index] = listName[index] + ":" +
                       name;
                    };
                };
            }
            else if (para == "DesignateHall") // 选择宗门堂口
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateHall>(para);
                    ui.InitData(this, index);
                    ui.call = (item, name) =>
                    {
                        allParams[index] = item;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + name;
                        allParamsName[index] = listName[index] + ":" +
                       name;
                    };
                };
            }
            else if (para == "DesignateSect") // 指定宗门
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateSect>(para);
                    ui.InitData(this, index);
                    ui.call = (item, name) =>
                    {
                        allParams[index] = item;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + name;
                        allParamsName[index] = listName[index] + ":" +
                       name;
                    };
                };
            }
            else if (para == "DesignateAttribute") // 指定宗门属性
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateAttribute>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseTechniqueType") // 选择功法类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseTechniqueType>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseRealm") // 选择境界
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseRealm>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "Grade") // 选择大境界境界
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIGrade>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseSoul") // 选择神魂
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseSoul>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseLiaoji") // 选择飘渺之力
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseLiaoji>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        int.TryParse(attr, out selectLiaoji);
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
                int.TryParse(allParams[index], out selectLiaoji);
            }
            else if (para == "ChooseLiaojiEffect") // 选择飘渺之力效果
            {
                clickAction = () =>
                {
                    if (selectLiaoji == 0)
                    {
                        UITipItem.AddTip("请先选择飘渺之力！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseLiaojiEffect>(para);
                    ui.InitData(this, index, selectLiaoji);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseLiaojiEffectUp") // 选择飘渺之力升级效果
            {
                clickAction = () =>
                {
                    if (selectLiaoji == 0)
                    {
                        UITipItem.AddTip("请先选择飘渺之力！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseLiaojiEffectUp>(para);
                    ui.InitData(this, index, selectLiaoji);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignatePurpose") // 指定宗旨
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignatePurpose>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignateRules") // 指定门规
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateRules>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignateXingge") // 指定性格
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateTrait>("DesignateTrait");
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "ChooseRighteousnessOrEvil") // 选择正魔
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseRighteousnessOrEvil>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "RelationshipType") // 选择关系类型
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIRelationshipType>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignateSecretBook") // 指定秘籍
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateSecretBook>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignateTechnique") // 指定功法
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateTechnique>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                       attrName;
                    };
                };
            }
            else if (para == "DesignateInstrumentSpirit") // 指定器灵
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateInstrumentSpirit>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, selectItem) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(g.conf.artifactSprite.GetItem(selectItem.spriteID).name);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(g.conf.artifactSprite.GetItem(selectItem.spriteID).name);
                    };
                };
            }
            else if (para == "ChooseInstrumentSpirit") // 选择器灵
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseInstrumentSpirit>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, selectItem) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" +
                        GameTool.LS(selectItem.name);
                        allParamsName[index] = listName[index] + ":" +
                        GameTool.LS(selectItem.name);
                    };
                };
            }
            else if (para == "ChooseHaotianEyeSkill") // 指定选择昊天眼技能
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseHaotianEyeSkill>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "DesignateHeartTechniqueSecretBook") // 指定心法秘籍
            {
                if (unit == null)
                {
                    UITipItem.AddTip("请先选择一名有效角色！");
                    return;
                }
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIDesignateHeartBook>("DesignateHeartBook");
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseFairyTechnique") // 选择仙法
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseFairyTechnique>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseProps") // 选择道具
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseProps>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseSecretBook") // 选择秘籍
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseSecretBook>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseSuit") // 选择套装
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChooseSuit>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "DesignateSuit") // 选择套装
            {
                clickAction = () =>
                {
                    if (matrialData == null)
                    {
                        UITipItem.AddTip("请先选择功法秘籍");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateSuit>(para);
                    ui.InitData(this, index, matrialData.baseID);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "WhetherToLearnAutomatically") // 是否自动学习
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIWhetherToLearnAutomatically>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseToAddEntry") // 选择增加词条
            {
                clickAction = () =>
                {
                    if (matrialData == null)
                    {
                        UITipItem.AddTip("请先选择功法秘籍");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseToAddEntry>(para);
                    ui.InitData(this, index, matrialData.martialType, matrialData.baseID);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "charA") // 选择角色A
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UISelectChar>("SelectChar");
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        var unit = ModMain.cmdRun.GetUnit(attr);
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":"
                        + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "charB") // 选择角色B
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UISelectChar>("SelectChar");
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        var unit = ModMain.cmdRun.GetUnit(attr);
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":"
                        + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "charOneA") // 选择一个角色A
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UISelectOneChar>("SelectOneChar");
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":"
                        + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
                getUnit = () => ModMain.cmdRun.GetUnit(allParams[index]);
            }
            else if (para == "charOneB") // 选择一个角色B
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UISelectOneChar>("SelectOneChar");
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        var unit = ModMain.cmdRun.GetUnit(attr);
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" 
                        + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "DesignateOneSecretBook") // 指定一个秘籍
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateOneSecretBook>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
                getMatrialData = () => ModMain.cmdRun.GetMartial(unit, allParams[index]);
            }
            else if (para == "DesignateOneTechnique") // 指定一个功法
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateOneTechnique>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
                getMatrialData = () =>
                {
                    var skill = ModMain.cmdRun.GetSkill(unit, allParams[index]);
                    return skill != null ? skill.data.To<DataProps.MartialData>() : null;
                };
            }
            else if (para == "DesignateHeartTechnique") // 指定一个心法
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIDesignateHeartTechnique>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
                getMatrialData = () =>
                {
                    var skill = ModMain.cmdRun.GetAbility(unit, allParams[index]);
                    return skill != null ? skill.data.To<DataProps.MartialData>() : null;
                };
            }
            else if (para == "ChoosePotmon") // 指定选择壶妖
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIChoosePotmon>(para);
                    ui.InitData(this, index);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseElder") // 指定选择化神之气
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseElder>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseRule") // 指定选择道魂
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseRule>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "ChooseArtifact") // 指定选择法宝
            {
                clickAction = () =>
                {
                    if (unit == null)
                    {
                        UITipItem.AddTip("请先选择一名有效角色！");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIChooseArtifact>(para);
                    ui.InitData(this, index, unit);
                    ui.call = (attr, attrName) =>
                    {
                        getArtifactData = () => ModMain.cmdRun.GetArtifact(unit, allParams[index]);
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "EditArtifactAttr") // 指定法宝属性
            {
                clickAction = () =>
                {
                    if (artifactData == null)
                    {
                        UITipItem.AddTip("需要先指定一个法宝");
                        return;
                    }
                    var data = artifactData.To<DataProps.PropsArtifact>();
                    if (data == null)
                    {
                        UITipItem.AddTip("需要先指定一个法宝");
                        return;
                    }
                    var ui = ModMain.OpenUI<UIEditArtifactAttr>(para);
                    ui.InitData(this, index, data);
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = listName[index] + ":" + attrName;
                        allParamsName[index] = listName[index] + ":" +
                    attrName;
                    };
                };
            }
            else if (para == "InputInt") // 多个输入框
            {
                clickAction = () =>
                {
                    var ui = ModMain.OpenUI<UIInputInt>(para);
                    ui.InitData(this, index, listName[index].Split('|'));
                    ui.call = (attr, attrName) =>
                    {
                        allParams[index] = attr;
                        listGo[index].transform.Find("Text").GetComponent<Text>().text = "输入属性";
                    };
                };
                itemType = 3; // 多个输入框
            }
            else
            {
                itemType = 1; // 单输入框
            }
        }

    }
}
