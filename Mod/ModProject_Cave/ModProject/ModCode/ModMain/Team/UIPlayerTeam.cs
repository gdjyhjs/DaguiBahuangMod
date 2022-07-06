using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Cave.Team
{
    public class UIPlayerTeam
    {
        static GameObject unitItemPrefab;
        public UIPlayerTeam(Transform parent, Vector2 pos)
        {
            if (DataTeam.teamUnits.Count < 1)
            {
                return;
            }
            GameObject bg = null;
            try
            {
                // 滚动背景
                bg = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("PlayerInfo", "daojukubg"));
                bg.transform.SetParent(parent.transform, false);
                bg.GetComponent<RectTransform>().anchoredPosition = pos;
                bg.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 620);
                bg.GetComponent<Image>().type = Image.Type.Tiled;

                // 滚动框
                var tmpGo = GuiBaseUI.CreateUI.NewScrollView(new Vector2(200, 600), spacing: new Vector2(0, 8));
                tmpGo.transform.SetParent(bg.transform, false);
                tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                tmpGo.name = "scr_createBuilds";
                var tmpScroll = tmpGo.GetComponent<ScrollRect>();

                var list = DataTeam.teamUnits;
                List<WorldUnitBase> units = new List<WorldUnitBase>(list.Count);
                foreach (var item in list)
                {
                    WorldUnitBase unit = g.world.unit.GetUnit(item);
                    if (unit != null)
                    {
                        units.Add(unit);
                    }
                }

                units.Sort((a, b) =>
                {
                    int aa = a.data.dynUnitData.GetGrade();
                    int bb = b.data.dynUnitData.GetGrade();
                    if (aa == bb)
                        return 0;
                    return aa < bb ? 1 : -1;
                });

                Action<GuiBaseUI.ItemCell, int> itemAction = (cellItem, i) =>
                {
                    int idx = i - 1;
                    if (idx >= 0 && idx < units.Count)
                    {
                        cellItem.gameObject.SetActive(true);
                        WorldUnitBase unit = units[idx];
                        UpdateUnitItem(cellItem.gameObject, unit, idx);
                    }
                    else
                    {
                        cellItem.gameObject.SetActive(false);
                    }
                };

                var goItem = CreateUnitItem();
                var bigDtaa = new GuiBaseUI.BigDataScroll();
                var itemCell = goItem.AddComponent<GuiBaseUI.ItemCell>();
                bigDtaa.Init(tmpScroll, itemCell, itemAction, goItem.GetComponent<RectTransform>().sizeDelta.y);
                bigDtaa.cellHeight = 122;
                bigDtaa.cellCount = units.Count;
                tmpScroll.verticalNormalizedPosition = 0.9999f;

            }
            catch (Exception e)
            {
                if (bg != null)
                {
                    GameObject.Destroy(bg);
                }
                Cave.LogError(e.Message + "\n" + e.StackTrace);
            }
        }

        private GameObject CreateUnitItem()
        {
            if (unitItemPrefab != null)
                return unitItemPrefab;

            unitItemPrefab = GuiBaseUI.CreateUI.NewImage(SpriteTool.GetSprite("Common", "qilingxuanzekuang1_1_1")); // 297 122
            unitItemPrefab.GetComponent<Image>().enabled = false;
            unitItemPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 122);
            unitItemPrefab.name = "item";

            // npc单位
            GameObject tmpGo = GuiBaseUI.CreateUI.NewRawImage();
            tmpGo.name = "imgHead";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(15, 0);
            tmpGo.AddComponent<UISkyTipEffect>();

            tmpGo = GuiBaseUI.CreateUI.NewButton(null, SpriteTool.GetSprite("SchoolCommon", "jubaoanniu_3"));
            tmpGo.name = "btnOperate";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 20);
            tmpGo.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 56);
            Transform tmpParent = tmpGo.transform;

            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(18, 50));
            tmpGo.name = "textState";
            tmpGo.transform.SetParent(tmpParent, false);
            Text tmpText2 = tmpGo.GetComponent<Text>();
            tmpText2.alignment = TextAnchor.MiddleCenter;
            tmpText2.lineSpacing = 0.65f;
            tmpText2.fontSize = 16;

            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(25, 100));
            tmpGo.name = "textName";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-90, 0);
            Text tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 22;
            tmpText.color = Color.black;
            tmpText.lineSpacing = 0.75f;


            tmpGo = GuiBaseUI.CreateUI.NewText("", new Vector2(25, 100));
            tmpGo.name = "textRelation";
            tmpGo.transform.SetParent(unitItemPrefab.transform, false);
            tmpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -15);
            tmpText = tmpGo.GetComponent<Text>();
            tmpText.alignment = TextAnchor.MiddleCenter;
            tmpText.fontSize = 15;
            tmpText.color = Color.black;
            tmpText.lineSpacing = 0.7f;
            tmpText.fontStyle = FontStyle.Italic;

            return unitItemPrefab;
        }

        private void UpdateUnitItem(GameObject go, WorldUnitBase unit, int idx)
        {
            try
            {
                Transform root = go.transform;
                // npc单位
                string name = unit.data.unitData.propertyData.GetName();
                string relation = g.conf.npcPartFitting.GetPartName(unit.data.unitData.unitID, g.world.playerUnit);
                ConfRoleGradeItem gradeItem = g.conf.roleGrade.GetItem(unit.data.dynUnitData.gradeID.value);
                string grade = GameTool.LS(gradeItem.gradeName);
                bool isBattle = DataTeam.battleUnits.Contains(unit.data.unitData.unitID);
                var imgHead = root.Find("imgHead").GetComponent<RawImage>();
                var btnOperate = root.Find("btnOperate").GetComponent<Button>();
                var textState = root.Find("btnOperate/textState").GetComponent<Text>();
                var textName = root.Find("textName").GetComponent<Text>();
                var textRelation = root.Find("textRelation").GetComponent<Text>();
                try
                {
                    PortraitModel.CreateTexture(unit, imgHead, new Vector2(0, -21.5f), 3f, true);
                    imgHead.enabled = true;
                }
                catch (Exception e)
                {
                    Debug.Log("加载头像错误\n" + e.Message + "\n" + e.StackTrace);
                    imgHead.enabled = false;
                }





                Action updateText = () =>
                {
                    string stateStr = DataTeam.battleUnits.Contains(unit.data.unitData.unitID) ? "<color=red>出战中</color>" : "备战中";
                    textState.text = stateStr;

                    List<string> lucks = new List<string>();
                    for (int i = 0; i < unit.allLuck.Count; i++)
                    {
                        WorldUnitLuckBase luckBase = unit.allLuck[i];

                        if (luckBase.luckConf.type == 2 && luckBase.luckConf.conceal == 0)
                        {
                            if (luckBase.luckConf.level < 0)
                            {
                                lucks.Add("<color=red>"+GameTool.LS(luckBase.luckConf.name)+"</color>");
                            }
                            else
                            {
                                lucks.Add("<color=green>" + GameTool.LS(luckBase.luckConf.name) + "</color>");
                            }
                        }
                    }

                    imgHead.GetComponent<UISkyTipEffect>().InitData(
                        $"<size=22>{name}</size>({relation})\n{GameTool.LS("Cave_State")}{GameTool.LS("maohao")}{stateStr}\n{GameTool.LS(gradeItem.gradeName) + GameTool.LS(gradeItem.phaseName)}\n"
                        + string.Join("\n", lucks)
                        , new Vector3(-5, 0, 0));
                };
                updateText();

                Action changeBattle = () =>
                {
                    isBattle = DataTeam.battleUnits.Contains(unit.data.unitData.unitID);
                    if (isBattle)
                    {
                        DataTeam.battleUnits.Remove(unit.data.unitData.unitID);
                    }
                    else
                    {
                        DataTeam.battleUnits.Add(unit.data.unitData.unitID);
                    }
                    isBattle = DataTeam.battleUnits.Contains(unit.data.unitData.unitID);
                    updateText();
                };
                btnOperate.onClick.RemoveAllListeners();
                btnOperate.onClick.AddListener(changeBattle);


                textName.text = name;
                textRelation.text = grade;
            }
            catch (Exception e)
            {
                Cave.LogError("创建单位Item失败：\n" + e.Message + "\n" + e.StackTrace);
            }
        }

    }
}
