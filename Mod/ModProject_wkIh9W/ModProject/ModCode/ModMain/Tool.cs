using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MOD_wkIh9W
{
    public class Tool
    {
        public static string UnitTip(string unitId)
        {
            var unit = ModMain.cmdRun.GetUnit(unitId);
            return UnitTip(unit);
        }
        public static string UnitTip(WorldUnitBase unit)
        {
            if (unit == null)
            {
                return "";
            }
            List<string> tip = new List<string>();
            try
            {
                var data = unit.data;
                var unitData = data.unitData;
                var dynUnitData = data.dynUnitData;
                var propertyData = unitData.propertyData;
                var unitID = unitData.unitID;
                try
                {
                    var ta = dynUnitData.sex.value == 1 ? "他" : "她";
                    try
                    {
                        string relationStr = g.conf.npcPartFitting.GetPartName(unitID, g.world.playerUnit);
                        string name = unit.data.unitData.propertyData.GetName();
                        if (relationStr != "")
                        {
                            name += "(" + relationStr + ")";
                        }
                        name += "[" + unitID + "]";
                        tip.Add(name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e1 " + e.ToString());
                    }
                    try
                    {
                        var p = unit.data.unitData.GetPoint();
                        var s = g.data.grid.GetNamePoint(p);
                        tip.Add($"{s}({p.x},{p.y})");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e666 " + e.ToString());
                    }
                    try
                    {
                        tip.Add((string.IsNullOrEmpty(unitData.relationData.married) ? "未婚" : "已婚") + "\t" +
                            (unitData.relationData.lover.Count + "个道侣"));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e2 " + e.ToString());
                    }
                    try
                    {
                        tip.Add(GameTool.LS(g.conf.roleBeauty.GetItemInBeauty(dynUnitData.beauty.value).text)
                            + "\t" + (dynUnitData.sex.value == 1 ? GameTool.LS("common_nan") : GameTool.LS("common_nv")));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e3 " + e.ToString());
                    }
                    try
                    {
                        var gradeItem = g.conf.roleGrade.GetItem(dynUnitData.gradeID.value);
                        tip.Add(GameTool.LS(g.conf.roleRace.GetItem(dynUnitData.race.value).race)
                            + "\t" + GameTool.LS(gradeItem.gradeName) + GameTool.LS(gradeItem.phaseName));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e4 " + e.ToString());
                    }
                    try
                    {
                        SchoolPostType postType = (data.school != null ? data.school.buildData.GetPostType(unitID) : SchoolPostType.None);
                        tip.Add((postType == SchoolPostType.None ? GameTool.LS("player_shanxiu") : g.conf.schoolPost.GetPostName(unit))
                            + "\t" + (data.school == null ? "" : data.school.name));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e5 " + e.ToString());
                    }
                    try
                    {
                        List<string> xingqu = new List<string>();
                        for (int i = 0; i < propertyData.hobby.Length; i++)
                        {
                            var item = g.conf.roleCreateHobby.GetItem(propertyData.hobby[i]);
                            if (item.show == 0)
                            {
                                continue;
                            }
                            xingqu.Add(GameTool.LS(item.name));
                        }
                        tip.Add(GameTool.LS("playerInfo_xingquaihao") + ":" + string.Join("、", xingqu));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e6 " + e.ToString());
                    }
                    try
                    {
                        tip.Add(GameTool.LS(g.conf.roleCreateCharacter.GetItem(dynUnitData.inTrait.value).sc5asd_sd34)
                            + "  " + GameTool.LS(g.conf.roleCreateCharacter.GetItem(dynUnitData.outTrait1.value).sc5asd_sd34)
                            + "、" + GameTool.LS(g.conf.roleCreateCharacter.GetItem(dynUnitData.outTrait2.value).sc5asd_sd34));
                        float intim1 = Mathf.RoundToInt(unitData.relationData.intimToPlayerUnit); // ta对我的好感
                        float intim2 = g.world.playerUnit.data.unitData.relationData.GetIntim(unitID); // 我对ta的好感
                        int oneIntimValue = int.Parse(g.conf.gameParameter.closeIconValueB);
                        float a = intim1 / oneIntimValue;
                        float b = intim2 / oneIntimValue;
                        tip.Add($"{ta}对我{a:F1}心  我对{ta}{b:F1}心");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("e7 " + e.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("a " + e.ToString());
                }
                try
                {
                    string hp = "<space=0.3em><voffset=-0.1em><size=130%><sprite name=\"tili\"><size=100%></voffset><space=0.3em>" + dynUnitData.hp.valueIngoreMinClamp;
                    string atk = "<space=0.3em><voffset=-0.1em><size=130%><sprite name=\"gongji\"><size=100%></voffset><space=0.3em>" + dynUnitData.attack.value;
                    string def = "<space=0.3em><voffset=-0.1em><size=130%><sprite name=\"fangyu\"><size=100%></voffset><space=0.3em>" + dynUnitData.defense.value;

                    tip.Add($"{hp}  {atk}  {def}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("b " + e.ToString());
                }
                try
                {

                    if (unitData.heart.IsHeroes())
                    {
                        tip.Add("道心：" + GameTool.LS(g.conf.taoistHeart.GetItem(unitData.heart.confID).heartName));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("c " + e.ToString());
                }
                try
                {
                    //武技
                    DataUnit.ActionMartialData skillLeftData = unitData.GetActionMartial(unitData.skillLeft);
                    if (skillLeftData != null)
                    {
                        var martialData = skillLeftData.data.To<DataProps.MartialData>();
                        tip.Add("武技：" + GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 2));
                    }

                    //绝技
                    DataUnit.ActionMartialData skillRightData = unitData.GetActionMartial(unitData.skillRight);
                    if (skillRightData != null)
                    {
                        var martialData = skillRightData.data.To<DataProps.MartialData>();
                        tip.Add("绝技：" + GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 2));
                    }

                    //身法
                    DataUnit.ActionMartialData stepData = unitData.GetActionMartial(unitData.step);
                    if (stepData != null)
                    {
                        var martialData = stepData.data.To<DataProps.MartialData>();
                        tip.Add("身法：" + GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 2));
                    }

                    //神通
                    DataUnit.ActionMartialData ultimateData = unitData.GetActionMartial(unitData.ultimate);
                    if (ultimateData != null)
                    {
                        var martialData = ultimateData.data.To<DataProps.MartialData>();
                        tip.Add("神通：" + GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 2));
                    }

                    // 心法
                    List<string> xinfa = new List<string>();
                    foreach (string item in unit.data.unitData.abilitys)
                    {
                        DataUnit.ActionMartialData actionMartialData = unitData.GetActionMartial(item);
                        if (actionMartialData != null)
                        {
                            var martialData = actionMartialData.data.To<DataProps.MartialData>();
                            xinfa.Add(GameTool.SetTextReplaceColorKey(martialData.martialInfo.name, GameTool.LevelToColorKey(martialData.martialInfo.level), 2));
                        }
                    }
                    tip.Add("心法:" + string.Join(" ", xinfa));

                }
                catch (Exception e)
                {
                    Console.WriteLine("d " + e.ToString());
                }
                try
                {

                    tip.Add("近期经历:");
                    DataUnitLog.LogData logs = g.world.unitLog.GetLogDataSync(unit.data.unitData.unitID);
                    int index = 0;
                    for (int i = logs.allLogData.Count - 1; i >= 0; i--)
                    {
                        var logData = logs.allLogData[i];
                        DataUnitLog.LogData.Data[] log = logData.logs.ToArray();
                        var text = GetLogText(log);

                        tip.Add($"{text}");
                        index++;
                        if (index >= 3)
                        {
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("e " + e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("aaaa " + e.ToString());
            }


            return string.Join("\n", tip);
        }
        private static string GetLogText(DataUnitLog.LogData.Data[] logs)
        {
            string logStr = "";
            for (int i = 0; i < logs.Length; i++)
            {
                if (logs[i].id[0] == 0)
                {
                    logStr += "\n";
                }
                else
                {
                    logStr += logs[i].GetLogString();
                }
            }
            logStr = TextDelLineFeed(logStr);
            logStr = logStr.Replace("\n\n", "\n");

            //logStr = GameTool.LSTextReplaceColor(logStr, 1);

            return HretHandler(logStr, 1);
        }
        private static string HretHandler(string text, int bgType)
        {
            var hretData = new UITextHretTool.HretData();
            int index = 0;
            int hretCount = 0;
            string oldText = text;
            string newText = "";
            while (index < oldText.Length)
            {
                index = oldText.IndexOf("@");
                if (index == -1)
                {
                    break;
                }

                string hretInfo = oldText.Substring(index + 1, oldText.IndexOf("@", index + 1) - index - 1);

                string[] values = hretInfo.Split('_');
                if (values.Length < 2)
                {
                    oldText = oldText.Remove(0, index + hretInfo.Length + 2);
                    index = 0;
                    hretCount++;
                    continue;
                }

                string type = values[0];
                string typeKey = type + hretCount;
                string data = values[1];
                UITextHretTool.HretHandlerBase hretHandler = GetHretHandler(type);
                hretHandler.Init(data, bgType, hretData);

                if (index > 0)
                {
                    newText += oldText.Substring(0, index);
                }
                oldText = oldText.Remove(0, index + hretInfo.Length + 2);
                newText += hretHandler.GetHretText();
                index = 0;
                hretCount++;
            }
            newText += oldText;

            text = newText;

            text = text.Replace("</color>", "");
            text = text.Replace("<color=#FF5A1C>", "");
            text = text.Replace("<color=#004FCA>", "");
            text = text.Replace("<color=#057600>", "");


            return text;
        }
        private static string TextDelLineFeed(string logStr)
        {
            for (int i = 0; i < logStr.Length; i++)
            {
                if (logStr[i] == '\n')
                {
                    logStr = logStr.Remove(i, 1);
                    i--;
                    continue;
                }
                break;
            }
            for (int i = logStr.Length - 1; i >= 0; i--)
            {
                if (logStr[i] == '\n')
                {
                    logStr = logStr.Remove(i, 1);
                    continue;
                }
                break;
            }
            return logStr;
        }
        private static UITextHretTool.HretHandlerBase GetHretHandler(string type)
        {
            if (type == "q") return new UITextHretTool.HretHandlerUnit();
            else if (type == "w") return new UITextHretTool.HretHandlerProps();
            else if (type == "e") return new UITextHretTool.HretHandlerLuck();
            else if (type == "r") return new UITextHretTool.HretHandlerSchoolMap();
            else if (type == "t") return new UITextHretTool.HretHandlerString();
            else if (type == "ps") return new UITextHretTool.HretHandlerPotmonSkill();
            else if (type == "p") return new UITextHretTool.HretHandlerPotmon();
            return null;
        }

        /// <summary>
        /// 获取指令中文字符串
        /// </summary>
        /// <returns></returns>
        public static string GetCmdCnStr(string cmd)
        {
            return GetCmdCnStr(cmd.Split(' '));
        }
        public static string GetCmdCnStr(string[] cmd)
        {
            try
            {
                List<string> result = new List<string>();
                ConfDaguiToolItem item = ModMain.confTool.allItems[cmd[0]];
                result.Add(item.funccn);
                result.Add(item.funccn);




                return string.Join(" ", result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return string.Join(" ", cmd);
            }
        }
    }
}
