using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace FixData
{
    [HarmonyPatch(typeof(WorldMgr), "Init")]
    class Patch_WorldMgr_Init
    {
        [HarmonyPrefix]
        private static bool Prefix()
        {
            try
            {
                Console.WriteLine("是否尝试修复无效数据 " + ModMain.autoFixData);
                if (ModMain.autoFixData)
                {
                    FixData();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("尝试清除无效数据失败\n" + e.Message + "\n" + e.StackTrace);
            }
            return true;
        }

        //static List<string> message = new List<string>();
        static List<string> dressMessage = new List<string>();
        static List<string> schoolMessage = new List<string>();
        static List<string> propMessage = new List<string>();
        static List<string> martialPropMessage = new List<string>();
        static List<string> eventMessage = new List<string>();
        static List<string> monstMessage = new List<string>();
        static List<string> taskMessage = new List<string>();
        static List<string> luckMessage = new List<string>();
        static List<string> logMessage = new List<string>();
        static List<string> gradeMessage = new List<string>();
        static List<string> heartMessage = new List<string>();
        static List<string> delSchoolMessage = new List<string>();
        static List<string> traitMessage = new List<string>();
        static List<string> spriteMessage = new List<string>();
        static List<string> titleMessage = new List<string>();
        static List<string> letterMessage = new List<string>();
        static List<string> potmonMessage = new List<string>();
        static List<string> skillMessage = new List<string>();
        static List<string> townMarketMessage = new List<string>();
        static List<string> pillFormulasMessage = new List<string>();
        static List<string> tmpPropsMessage = new List<string>();
        static List<string> monthLogMessage = new List<string>();
        static double costTime;

        private static void FixData()
        {
            dressMessage = new List<string>();
            schoolMessage = new List<string>();
            propMessage = new List<string>();
            martialPropMessage = new List<string>();
            eventMessage = new List<string>();
            monstMessage = new List<string>();
            taskMessage = new List<string>();
            luckMessage = new List<string>();
            logMessage = new List<string>();
            gradeMessage = new List<string>();
            heartMessage = new List<string>();
            delSchoolMessage = new List<string>();
            traitMessage = new List<string>();
            spriteMessage = new List<string>();
            titleMessage = new List<string>();
            letterMessage = new List<string>();
            potmonMessage = new List<string>();
            skillMessage = new List<string>();
            townMarketMessage = new List<string>();
            pillFormulasMessage = new List<string>();
            tmpPropsMessage = new List<string>();
            monthLogMessage = new List<string>();

            List<int> badLuckId = new List<int>() { 9071, 9072, 9073, 9074, 9075, 9076 }; // 无法使用的气运ID

            DateTime start = DateTime.Now;
            CheckBuildData();
            CheckUnitData(badLuckId);
            CheckUnitLogData();
            CheckEventData();
            CheckMonstData();
            CheckLetterData();
            CheckPotmonData();
            CheckTownData();
            CheckFormulasData();
            CheckTmpPropData();
            CheckMonthLog();
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            costTime = duration.TotalMilliseconds;
            Console.WriteLine("代码耗时：" + costTime + "ms");


            FixDataEnd();
        }

        private static void FixDataEnd()
        {
            int msgCount = 0;
            Dictionary<string, List<string>> allMssage = new Dictionary<string, List<string>>();
            {
                if (delSchoolMessage.Count > 0)
                {
                    msgCount += delSchoolMessage.Count;
                    allMssage.Add($"\n删除了{delSchoolMessage.Count}个无法修复的建筑", delSchoolMessage);
                }
                if (traitMessage.Count > 0)
                {
                    msgCount += traitMessage.Count;
                    allMssage.Add($"\n修复{traitMessage.Count}条性格数据", traitMessage);
                }
                if (spriteMessage.Count > 0)
                {
                    msgCount += spriteMessage.Count;
                    allMssage.Add($"\n修复{spriteMessage.Count}条器灵错误", spriteMessage);
                }
                if (gradeMessage.Count > 0)
                {
                    msgCount += gradeMessage.Count;
                    allMssage.Add($"\n修复{gradeMessage.Count}条境界错误", gradeMessage);
                }
                if (heartMessage.Count > 0)
                {
                    msgCount += heartMessage.Count;
                    allMssage.Add($"\n修复{heartMessage.Count}天骄错误", heartMessage);
                }
                if (schoolMessage.Count > 0)
                {
                    msgCount += schoolMessage.Count;
                    allMssage.Add($"\n修复{schoolMessage.Count}条宗门数据", schoolMessage);
                }
                if (propMessage.Count > 0)
                {
                    msgCount += propMessage.Count;
                    allMssage.Add($"\n清除{propMessage.Count}个道具", propMessage);
                }
                if (martialPropMessage.Count > 0)
                {
                    msgCount += martialPropMessage.Count;
                    allMssage.Add($"\n清除{martialPropMessage.Count}个秘籍", martialPropMessage);
                }
                if (luckMessage.Count > 0)
                {
                    msgCount += luckMessage.Count;
                    allMssage.Add($"\n清除{luckMessage.Count}条气运", luckMessage);
                }
                if (taskMessage.Count > 0)
                {
                    msgCount += taskMessage.Count;
                    allMssage.Add($"\n清除{taskMessage.Count}个任务", taskMessage);
                }
                if (titleMessage.Count > 0)
                {
                    msgCount += titleMessage.Count;
                    allMssage.Add($"\n删除了{titleMessage.Count}个不存在的道号", titleMessage);
                }
                if (letterMessage.Count > 0)
                {
                    msgCount += letterMessage.Count;
                    allMssage.Add($"\n删除了{letterMessage.Count}个无效信件", letterMessage);
                }
                if (potmonMessage.Count > 0)
                {
                    msgCount += potmonMessage.Count;
                    allMssage.Add($"\n修复了{potmonMessage.Count}个壶妖数据", potmonMessage);
                }
                if (skillMessage.Count > 0)
                {
                    msgCount += skillMessage.Count;
                    allMssage.Add($"\n修复了{skillMessage.Count}个功法数据", skillMessage);
                }
                if (townMarketMessage.Count > 0)
                {
                    msgCount += townMarketMessage.Count;
                    allMssage.Add($"\n删除了{townMarketMessage.Count}个坊市无效道具", townMarketMessage);
                }
                if (eventMessage.Count > 0)
                {
                    msgCount += eventMessage.Count;
                    allMssage.Add($"\n清除{eventMessage.Count}个事件", eventMessage);
                }
                if (monstMessage.Count > 0)
                {
                    msgCount += monstMessage.Count;
                    allMssage.Add($"\n清除{monstMessage.Count}个副本", monstMessage);
                }
                if (logMessage.Count > 0)
                {
                    msgCount += logMessage.Count;
                    allMssage.Add($"\n清除{logMessage.Count}条错误生平记事", logMessage);
                }
                if (dressMessage.Count > 0)
                {
                    msgCount += dressMessage.Count;
                    allMssage.Add($"\n修复{dressMessage.Count}条立绘数据错误", dressMessage);
                }
                if (pillFormulasMessage.Count > 0)
                {
                    msgCount += pillFormulasMessage.Count;
                    allMssage.Add($"\n清除{pillFormulasMessage.Count}条丹方错误", pillFormulasMessage);
                }
                if (tmpPropsMessage.Count > 0)
                {
                    msgCount += tmpPropsMessage.Count;
                    allMssage.Add($"\n删除{tmpPropsMessage.Count}条临时背包无效道具", tmpPropsMessage);
                }
                if (monthLogMessage.Count > 0)
                {
                    msgCount += monthLogMessage.Count;
                    allMssage.Add($"\n删除{monthLogMessage.Count}个无效月结日志", monthLogMessage);
                }
            }
            ModMain.FixTip($"检测数据完成，累计修复{msgCount}处错误");
            //Console.WriteLine("存档检查完毕，修补数=" + fixID);
            g.world.system.AddSystemInMap(new Action<Il2CppSystem.Action>((call) =>
            {
                CallQueue cq = new CallQueue();
                cq.Add(new Action(() =>
                {
                    if (msgCount > 0)
                    {
                        var str = string.Join("\n", allMssage);
                        Console.WriteLine(str);
                        var ui = g.ui.OpenUI<UITextInfoLong>(UIType.TextInfoLong);
                        ui.InitData("大鬼修复存档MOD", "");
                        var prefab = ui.ptextInfo.gameObject;

                        ui.ptextInfo.text = $"存档修复完成，总共修补了{msgCount}处错误。\n本次修复存档耗时{(costTime * 0.001f).ToString("F2")}秒。\n八荒大鬼为你的存档保驾护航！";
                        {
                            var gameObject = new GameObject();
                            var rtf = gameObject.AddComponent<RectTransform>();
                            gameObject.transform.SetParent(ui.goTableDataRoot.transform, false);
                            rtf.sizeDelta = new Vector2(1298, 90);
                            {
                                var btn = GameObject.Instantiate(ui.btnOK, rtf);
                                btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-540, 0);
                                btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "前往大鬼首页";

                                btn.onClick.RemoveAllListeners();
                                btn.onClick.AddListener(new Action(() =>
                                {
                                    Application.OpenURL("http://www.yellowshange.com/");
                                }));
                            }
                            {
                                var btn = GameObject.Instantiate(ui.btnOK, rtf);
                                btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-240, 0);
                                btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "前往哔哩哔哩";

                                btn.onClick.RemoveAllListeners();
                                btn.onClick.AddListener(new Action(() =>
                                {
                                    Application.OpenURL("https://space.bilibili.com/16445976");
                                }));
                            }
                            {
                                var btn = GameObject.Instantiate(ui.btnOK, rtf);
                                btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, 0);
                                btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "加入大鬼QQ群";

                                btn.onClick.RemoveAllListeners();
                                btn.onClick.AddListener(new Action(() =>
                                {
                                    Application.OpenURL("https://jq.qq.com/?_wv=1027&k=H4EvfwOi");
                                }));
                            }
                            {
                                var btn = GameObject.Instantiate(ui.btnOK, rtf);
                                btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(360, 0);
                                btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "充电支持大鬼";

                                btn.onClick.RemoveAllListeners();
                                btn.onClick.AddListener(new Action(() =>
                                {
                                    Application.OpenURL("https://jq.qq.com/?_wv=1027&k=H4EvfwOi");
                                }));
                            }
                        }

                        GameObject.DestroyImmediate(ui.goTableDataRoot.GetComponent<UITargetSizeDelta>());
                        var ver = ui.goTableDataRoot.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
                        ver.childControlHeight = false;
                        ver.childControlWidth = false;
                        ver.childForceExpandHeight = false;
                        ver.childForceExpandWidth = false;
                        var fitter = ui.goTableDataRoot.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                        fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

                        foreach (var item in allMssage)
                        {
                            var title = item.Key;
                            var data = item.Value;
                            var gameObject = new GameObject();
                            var rtf = gameObject.AddComponent<RectTransform>();
                            gameObject.transform.SetParent(ui.goTableDataRoot.transform, false);
                            rtf.sizeDelta = new Vector2(1298, 90);

                            GameObject textGo = GameObject.Instantiate(prefab, rtf);
                            textGo.GetComponent<TMPro.TextMeshProUGUI>().text = title;
                            textGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -20);


                            var btn = GameObject.Instantiate(ui.btnOK, rtf);
                            btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-540, -20);
                            btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "展开修复详情";
                            GameObject messageObj = null;

                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(new Action(() =>
                            {
                                if (messageObj == null)
                                {
                                    messageObj = GameObject.Instantiate(prefab, ui.goTableDataRoot.transform);
                                    messageObj.GetComponent<TMPro.TextMeshProUGUI>().text = string.Join("\n", data);
                                    messageObj.transform.SetSiblingIndex(rtf.GetSiblingIndex() + 1);
                                }
                                else if (messageObj.activeSelf)
                                {
                                    messageObj.SetActive(false);
                                    btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "展开修复详情";
                                }
                                else
                                {
                                    messageObj.SetActive(true);
                                    btn.GetComponentInChildren<UnityEngine.UI.Text>().text = "关闭修复详情";
                                }
                            }));

                        }


                        ui.onCloseCall += new Action(() =>
                        {
                            cq.Next();
                        });
                    }
                    else
                    {
                        if (ModMain.openFixTip)
                        {
                            cq.Next();
                        }
                        else
                        {
                            var ui = g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup);
                            ui.InitData("大鬼修复存档MOD",
                                $"本次检测存档没有需要修复的数据，检测耗时{(costTime * 0.001f).ToString("F2")}秒,如果存档依然有问题可添加八荒大鬼的QQ996776472人工协助修复。存档没问题时可再设置界面暂时关闭" +
                                $"自动修复功能，可加快读档进度游戏的速度，等需要时再打开即可。",
                                2);
                            ui.textBtn1.text = "大鬼首页";
                            ui.textBtn2.text = "不再提示";
                            ui.textBtn3.text = "我知道了";
                            ui.btn1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-115, 50);
                            ui.btn3.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 50);
                            ui.btn1.onClick.RemoveAllListeners();
                            ui.btn2.onClick.RemoveAllListeners();
                            ui.btn3.onClick.RemoveAllListeners();
                            ui.btn1.onClick.AddListener(new Action(() =>
                            {
                                Application.OpenURL("http://www.yellowshange.com/");
                            }));
                            ui.btn2.onClick.AddListener(new Action(() =>
                            {
                                ModMain.openFixTip = false;
                                g.ui.CloseUI(ui);
                                cq.Next();
                            }));
                            ui.btn3.onClick.AddListener(new Action(() =>
                            {
                                g.ui.CloseUI(ui);
                                cq.Next();
                            }));
                            ui.btn2.gameObject.SetActive(true);
                        }
                    }
                }));

                cq.Add(new Action(() =>
                {
                    Il2CppSystem.Collections.Generic.List<TaskBase> allTask = g.world.playerUnit.allTask;
                    var kuafuTask = allTask.Find(new Func< TaskBase, bool>((v) => v.data.taskBaseItem.id == 2110102));
                    if (kuafuTask != null && UnitConditionTool.Condition("dialogue_760101_0"))
                    {
                        var ui = g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup);
                        ui.InitData("大鬼修复存档MOD",
                            $"检测到【阿夸之忧】任务可能无法正常进行，是否进行修复？",
                            2);
                        ui.textBtn1.text = "大鬼首页";
                        ui.textBtn2.text = "确认修复";
                        ui.textBtn3.text = "取消修复";
                        ui.btn1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-115, 50);
                        ui.btn3.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 50);
                        ui.btn1.onClick.RemoveAllListeners();
                        ui.btn2.onClick.RemoveAllListeners();
                        ui.btn3.onClick.RemoveAllListeners();
                        ui.btn1.onClick.AddListener(new Action(() =>
                        {
                            Application.OpenURL("http://www.yellowshange.com/");
                        }));
                        ui.btn2.onClick.AddListener(new Action(() =>
                        {
                            g.ui.CloseUI(ui);
                            UICustomDramaDyn dramaDyn = new UICustomDramaDyn(760101);
                            dramaDyn.dramaData.onDramaEndCall += new Action(() =>
                            {
                                cq.Next();
                            });
                            dramaDyn.OpenUI();
                        }));
                        ui.btn3.onClick.AddListener(new Action(() =>
                        {
                            g.ui.CloseUI(ui);
                            cq.Next();
                        }));
                        ui.btn2.gameObject.SetActive(true);
                    }
                    else
                    {
                        cq.Next();
                    }
                }));
                cq.Run();

                cq.Add(new Action(() =>
                {
                    IntoWorldFixData(call);
                }));
                cq.Run();
            }));
        }

        private static void IntoWorldFixData(Il2CppSystem.Action call)
        {
            CallQueue cq = new CallQueue();
            List<Action> list = new List<Action>();

            Console.WriteLine("进入世界后检查逆天改命数据");
            List<int> delLuck = new List<int>();
            Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade = g.data.world.playerLog.upGrade;
            foreach (var item in upGrade)
            {
                if (g.conf.roleCreateFeature.GetItem(item.Value.luck) == null)
                {
                    Action<int, DataWorld.World.PlayerLogData.GradeData> resetFunc = (grade, gradeData) =>
                    {
                        UIUpGradeAttr uiAttr = g.ui.OpenUI<UIUpGradeAttr>(UIType.UpGradeAttr);
                        uiAttr.textTip.text = "八荒大鬼修复坏档删除逆天改命补偿";
                        ConfRoleGradeItem gradeItem = g.conf.roleGrade.GetGradeItemInQuality(grade, gradeData.quality);
                        if (gradeItem == null)
                        {
                            gradeItem = g.conf.roleGrade.allConfList[0];
                            uiAttr.textTip.text = "八荒大鬼修复坏档删除逆天改命补偿!";
                        }
                        uiAttr.InitData(gradeItem, grade);
                        uiAttr.onCloseRewardCall += new Action<Il2CppSystem.Action>((func) => cq.Next());
                    };
                    int tmpGrade = item.Key;
                    DataWorld.World.PlayerLogData.GradeData tmpGradeData = item.Value;
                    list.Add(() => resetFunc(tmpGrade, tmpGradeData));
                    delLuck.Add(item.Value.luck);
                }
            }



            // 执行异步修复
            if (list.Count > 0)
            {
                cq.Add(new Action(() =>
                {
                    g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), $"修复坏档删除{delLuck.Count}个逆天改命：" + string.Join(",", delLuck) + "，请重新选择逆天改命", 1, new Action(() => cq.Next()));

                }));
                foreach (var item in list)
                {
                    cq.Add(item);
                }
                cq.Add(call);
                cq.Run();
            }
            else
            {
                call?.Invoke();
            }
        }

        private static void CheckLetterData()
        {
            ModMain.NextFixTip();
            float max = g.data.world.allLetter.Count;
            int index = 0;
            Console.WriteLine("检查信件");
            g.data.world.allLetter.RemoveAll(new Func<DataWorld.World.LetterData, bool>((v) =>
            {
                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测信件数据 {value.ToString("F2")}% 修复:信件-{letterMessage.Count}");
                if (g.conf.letterBase.GetItem(v.letterID) == null)
                {
                    letterMessage.Add((letterMessage.Count + 1) + "删除无效信件：" + v.letterID);
                    return true;
                }
                return false;
            }));
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测信件数据 {value.ToString("F2")}% 修复:信件-{letterMessage.Count}");
            }
        }

        private static void CheckPotmonData()
        {
            Console.WriteLine("检查壶妖");
            Il2CppSystem.Collections.Generic.List<PotMonUnitData> list = g.data.world.devilDemonData.potmonData.potMonList;
            ModMain.NextFixTip();
            float max = list.Count * 2;
            int index = 0;
            list.RemoveAll(new Func<PotMonUnitData, bool>((v) =>
            {
                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测壶妖数据 {value.ToString("F2")}% 修复:壶妖-{potmonMessage.Count}");

                if (g.conf.potmonBase.GetItem(v.id) == null)
                {
                    potmonMessage.Add($"{potmonMessage.Count + 1} 删除不存在的壶妖 {v.id}({v.soleId})");
                    return true;
                }
                return false;
            }));
            for (int i = 0; i < list.Count; i++)
            {
                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测壶妖数据 {value.ToString("F2")}% 修复:壶妖-{potmonMessage.Count}");

                if (list[i].unitId != "" && !g.data.unit.allUnit.ContainsKey(list[i].unitId))
                {
                    potmonMessage.Add($"{potmonMessage.Count + 1} 修复化形死亡的壶妖 {list[i].id}({list[i].soleId})");
                    g.data.world.devilDemonData.potmonData.unitRace.Remove(list[i].unitId);
                    list[i].unitId = "";
                    list[i].converState = 0;
                    list[i].furryGradeId = 0;
                }
            }
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测壶妖数据 {value.ToString("F2")}% 修复:壶妖-{potmonMessage.Count}");
            }
        }

        private static void CheckMonthLog()
        {
            Debug.Log("检查月结日志");
            Il2CppSystem.Collections.Generic.List<DataUnitLog.LogData.LogItemData> logs = g.data.world.monthLog;
            for (int i = 0; i < logs.Count; i++)
            {
                Il2CppSystem.Collections.Generic.List<DataUnitLog.LogData.Data> logDatas = logs[i].logs;
                logDatas.RemoveAll(new Func<DataUnitLog.LogData.Data, bool>((v) =>
                {
                    ConfRoleLogLocalItem logItem = g.conf.roleLogLocal.GetItem(v.id[0]);
                    bool isRemove = logItem == null;
                    monthLogMessage.Add((monthLogMessage.Count + 1) + "删除不存在的月结日志 " + v.id[0]);
                    return isRemove;
                }));
            }
        }

        private static void CheckTmpPropData()
        {
            Il2CppSystem.Collections.Generic.List<DataProps.PropsData> allProps = g.data.world.tempProps.allProps;
            ModMain.NextFixTip();
            float max = allProps.Count;
            int index = 0;
            if (max > 0)
            {
                // 删除背包不存在的道具
                allProps.RemoveAll(new Func<DataProps.PropsData, bool>((v) =>
                {
                    index++;
                    float value = index / max * 100;
                    ModMain.FixTip($"检测临时背包数据 {value.ToString("F2")}% 修复:临时道具-{tmpPropsMessage.Count}");
                    if (v.propsType == DataProps.PropsDataType.Props)
                    {
                        if (v.propsItem == null)
                        {
                            tmpPropsMessage.Add((tmpPropsMessage.Count + 1) + "临时背包" + " 删除不存在的道具 " + v.propsID);
                            return true;
                        }
                    }
                    else if (v.propsType == DataProps.PropsDataType.Martial)
                    {
                        DataProps.MartialData martialData = v.To<DataProps.MartialData>();
                        if (martialData.martialType == MartialType.Ability)
                        {
                            if (g.conf.battleAbilityBase.GetItem(martialData.baseID) == null)
                            {
                                tmpPropsMessage.Add((tmpPropsMessage.Count + 1) + "临时背包" + " 删除不存在的秘籍 心法 " + martialData.baseID);
                                return true;
                            }
                            else
                            {
                                FixMartialPrefix("临时背包" + " 修复秘籍词条 ", v);
                            }
                        }
                        else if (martialData.martialType == MartialType.Step)
                        {
                            if (g.conf.battleStepBase.GetItem(martialData.baseID) == null)
                            {
                                tmpPropsMessage.Add((tmpPropsMessage.Count + 1) + "临时背包" + " 删除不存在的秘籍 身法 " + martialData.baseID);
                                return true;
                            }
                            else
                            {
                                FixMartialPrefix("临时背包" + " 修复秘籍词条 ", v);
                            }
                        }
                        else if (martialData.martialType != MartialType.None)
                        {
                            if (g.conf.battleSkillAttack.GetItem(martialData.baseID) == null)
                            {
                                tmpPropsMessage.Add((tmpPropsMessage.Count + 1) + "临时背包" + " 删除不存在的秘籍 技能 " + martialData.baseID);
                                return true;
                            }
                            else
                            {
                                FixMartialPrefix("临时背包" + " 修复秘籍词条 ", v);
                            }
                        }
                        else
                        {
                            tmpPropsMessage.Add((tmpPropsMessage.Count + 1) + "临时背包" + " 删除不存在的秘籍 ?? " + martialData.baseID);
                            return true;
                        }
                    }
                    return false;

                }));
                {
                    float value = index / max * 100;
                    ModMain.FixTip($"检测临时背包数据 {value.ToString("F2")}% 修复:临时道具-{tmpPropsMessage.Count}");
                }
            }
        }

        private static void CheckFormulasData()
        {

            Il2CppSystem.Collections.Generic.List<DataWorld.World.PillFormulaData> formulas = g.data.world.pillFormulas;
            ModMain.NextFixTip();
            float max = formulas.Count;
            int index = 0;
            formulas.RemoveAll(new Func<DataWorld.World.PillFormulaData,bool>((v) =>
            {

                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测丹方 {value.ToString("F2")}% 修复:丹方-{pillFormulasMessage.Count}");

                if (g.conf.makePillFormula.GetItem(v.id) == null)
                {
                    pillFormulasMessage.Add($"{pillFormulasMessage.Count + 1} 删除不存在的丹方 {v.id}");
                    return true;
                }
                else
                {
                    return false;
                }
            }));
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测丹方 {value.ToString("F2")}% 修复:丹方-{pillFormulasMessage.Count}");
            }
        }
        private static void CheckTownData()
        {
            ModMain.NextFixTip();
            float max = g.data.grid.mapWidth * g.data.grid.mapHeight;
            int index = 0;
            for (int x = 0; x < g.data.grid.mapWidth; x++)
            {
                for (int y = 0; y < g.data.grid.mapHeight; y++)
                {
                    index++;
                    float value = index / max * 100;
                    ModMain.FixTip($"检测坊市 {value.ToString("F2")}% 修复:坊市道具-{townMarketMessage.Count}");

                    DataGrid.GridData gridData = g.data.grid.GetGridData(new Vector2Int(x, y));


                    if (gridData.IsBuild() && gridData.isOrigi)
                    {
                        DataBuildBase.BuildData build = WorldFactory.GetBuildData(gridData.typeInt).GetBuild(new Vector2Int(x, y)).TryCast<DataBuildBase.BuildData>();
                        DataBuildBase.BuildSubData subBuild = build.GetBuildSub(MapBuildSubType.TownMarketPill);
                        if (subBuild != null)
                        {
                            if (subBuild.objData.ContainsKey("data"))
                            {
                                MapBuildTownMarket.Data data = CommonTool.JsonToObject<MapBuildTownMarket.Data>(subBuild.objData.GetString("data"));

                                data.props.RemoveAll(new Func<int, bool>((v) =>
                                {
                                    if (g.conf.townMarketItem.GetItem(v) == null || g.conf.itemProps.GetItem(v) == null)
                                    {
                                        townMarketMessage.Add($"{townMarketMessage.Count + 1} 删除坊市{x}，{y}无效道具 props: {v}");
                                        return true;
                                    }
                                    return false;
                                }));

                                subBuild.objData.SetString("data", CommonTool.ObjectToJson(data));

                            }
                        }
                    }
                }
            }
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测坊市 {value.ToString("F2")}% 修复:坊市道具-{townMarketMessage.Count}");
            }
        }

        private static void CheckUnitLogData()
        {
            ModMain.NextFixTip();
            Console.WriteLine("检查角色日志");
            CallQueue cq6 = new CallQueue();
            float max = g.data.unitLog.allLog.Count;
            int index = 0;
            foreach (var item in g.data.unitLog.allLog)
            {
                var k = item.key;
                var v = item.value;

                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测所有角色生平日志数据 {value.ToString("F2")}% 修复:日志-{logMessage.Count}");


                foreach (var item2 in v.allLog)
                {
                    var cacheItem = item2;
                    cq6.Add(new Action(() =>
                    {
                        DataUnitLog.LogData.LogItemData data = new DataUnitLog.LogData.LogItemData();
                        data.StringToData(cacheItem[1]);
                        int destroyId = 0;
                        foreach (var log in data.logs)
                        {
                            if (log.id[0] != 0 && g.conf.roleLogLocal.GetItem(log.id[0]) == null)
                            {
                                destroyId = log.id[0];
                                break;
                            }
                        }
                        if (destroyId != 0)
                        {
                            logMessage.Add((logMessage.Count) + "删除不存在生平记事：" + destroyId);
                            v.allLog.Remove(cacheItem);
                        }
                    }));
                }
                foreach (var item2 in v.allVitalLog)
                {
                    var cacheItem = item2;
                    cq6.Add(new Action(() =>
                    {
                        DataUnitLog.LogData.LogItemData data = new DataUnitLog.LogData.LogItemData();
                        data.StringToData(cacheItem[1]);
                        int destroyId = 0;
                        foreach (var log in data.logs)
                        {
                            if (log.id[0] != 0 && g.conf.roleLogLocal.GetItem(log.id[0]) == null)
                            {
                                destroyId = log.id[0];
                                break;
                            }
                        }
                        if (destroyId != 0)
                        {
                            logMessage.Add((logMessage.Count) + "删除不存在重大生平记事：" + destroyId);
                            v.allVitalLog.Remove(cacheItem);
                        }
                    }));
                }

            }
            cq6.RunAllCall();
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测所有角色生平日志数据 {value.ToString("F2")}% 修复:日志-{logMessage.Count}");
            }
        }

        private static void CheckMonstData()
        {
            ModMain.NextFixTip();
            float max = g.data.map.allGridMonst.Count;
            int index = 0;

            Console.WriteLine("检查怪物");
            CallQueue cq5 = new CallQueue();
            foreach (var v in g.data.map.allGridMonst)
            {

                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测大地图副本 {value.ToString("F2")}% 修复:副本-{monstMessage.Count}");

                var data = v;
                if (v.value.id != 0 && g.conf.dungeonBase.GetItem(v.value.id) == null)
                {
                    cq5.Add(new Action(() =>
                    {
                        monstMessage.Add((monstMessage.Count) + "删除不存在怪物：" + data.value.id);
                        g.data.map.allGridMonst.Remove(data.key);
                    }));
                }
            }
            cq5.RunAllCall();
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测大地图副本 {value.ToString("F2")}% 修复:副本-{monstMessage.Count}");
            }
        }

        private static void CheckEventData()
        {
            ModMain.NextFixTip();
            float max = g.data.map.allGridEventID.Count;
            int index = 0;

            Console.WriteLine("检查事件");
            CallQueue cq4 = new CallQueue();
            foreach (var v in g.data.map.allGridEventID)
            {
                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测大地图事件 {value.ToString("F2")}% 修复:事件-{eventMessage.Count}");

                var data = v;
                if (v.value.id != 0 && g.conf.worldFortuitousEventBase.GetItem(v.value.id) == null)
                {
                    cq4.Add(new Action(() =>
                    {
                        eventMessage.Add((eventMessage.Count+1) + "删除不存在事件：" + data.value.id);
                        g.data.map.allGridEventID.Remove(data.key);
                    }));
                }
            }
            cq4.RunAllCall();
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测大地图事件 {value.ToString("F2")}% 修复:事件-{eventMessage.Count}");
            }
        }

        private static void CheckUnitData(List<int> badLuckId)
        {
            ModMain.NextFixTip();
            Console.WriteLine("检查角色数据");
            int max = g.data.unit.allUnit.Count;
            int index = 0;
            foreach (var item in g.data.unit.allUnit)
            {
                index++;
                float value = index / max * 100;
                ModMain.FixTip($"检测角色数据 {index}/{max} 修复:境界-{gradeMessage.Count} 天骄-{heartMessage.Count}" +
                    $" 性格-{traitMessage.Count} 道具-{propMessage.Count} 秘籍-{martialPropMessage.Count}" +
                    $" 气运-{badLuckId.Count} 任务-{taskMessage.Count} 器灵-{spriteMessage.Count}" +
                    $" 道号-{titleMessage.Count} 立绘-{dressMessage.Count} 技能-{skillMessage.Count}");
                DataUnit.UnitInfoData unitData;
                bool isPlayer;
                string unitName;
                try
                {
                    unitData = item.Value;
                    isPlayer = g.data.world.playerUnitID == unitData.unitID;
                    unitName = unitData.propertyData.GetName();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "。\n" + e.StackTrace);
                    continue;
                }
                int i = 0;
                try
                {
                    FixUnitGrade(unitData, unitName);
                    i = 11;
                    FixUnitHero(unitData, unitName);
                    i = 22;
                    FixUnitInTrait(unitData, unitName);
                    i = 33;
                    FixUnitOutTrait1(unitData, unitName);
                    i = 44;
                    FixUnitProp(unitData, unitName);
                    i = 55;
                    FixUnitEquip(unitData, unitName);
                    i = 66;
                    FixUnitPostnatalLuck(badLuckId, unitData, unitName);
                    i = 77;
                    FixUnitBrodLuck(badLuckId, unitData, unitName);
                    i = 88;
                    FixUnitTask(unitData, unitName);
                    i = 99;
                    FixUnitSprite(unitData, unitName);
                    i = 111;
                    FixUnitTitle(unitData, unitName);
                    i = 122;
                    FixUnitModel(unitData, unitName);
                    i = 132;
                    FixUnitSkill(unitData, unitName);
                    i = 142;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "！" + i + "\n" + e.StackTrace);
                    continue;
                }
            }
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测角色数据 {index}/{max} 修复:境界-{gradeMessage.Count} 天骄-{heartMessage.Count}" +
                    $" 性格-{traitMessage.Count} 道具-{propMessage.Count} 秘籍-{martialPropMessage.Count}" +
                    $" 气运-{badLuckId.Count} 任务-{taskMessage.Count} 器灵-{spriteMessage.Count}" +
                    $" 道号-{titleMessage.Count} 立绘-{dressMessage.Count} 技能-{skillMessage.Count}");
            }
        }

        private static void FixUnitProp(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 删除背包不存在的道具

            unitData.propData.allProps.RemoveAll(new Func<DataProps.PropsData, bool>((v) =>
            {
                if (v.propsType == DataProps.PropsDataType.Props)
                {
                    if (v.propsItem == null)
                    {
                        propMessage.Add((propMessage.Count + 1) + unitName + " 删除不存在的道具 " + v.propsID);
                        return true;
                    }
                }
                else if (v.propsType == DataProps.PropsDataType.Martial)
                {
                    DataProps.MartialData martialData = v.To<DataProps.MartialData>();
                    if (martialData.martialType == MartialType.Ability)
                    {
                        if (g.conf.battleAbilityBase.GetItem(martialData.baseID) == null)
                        {
                            martialPropMessage.Add((martialPropMessage.Count + 1) + unitName + " 删除不存在的秘籍 心法 " + martialData.baseID);
                            return true;
                        }
                        else
                        {
                            FixMartialPrefix(unitName + " 修复功法词条 ", v);
                        }
                    }
                    else if (martialData.martialType == MartialType.Step)
                    {
                        if (g.conf.battleStepBase.GetItem(martialData.baseID) == null)
                        {
                            martialPropMessage.Add((martialPropMessage.Count + 1) + unitName + " 删除不存在的秘籍 身法 " + martialData.baseID);
                            return true;
                        }
                        else
                        {
                            FixMartialPrefix(unitName + " 修复功法词条 ", v);
                        }
                    }
                    else if (martialData.martialType != MartialType.None)
                    {
                        if (g.conf.battleSkillAttack.GetItem(martialData.baseID) == null)
                        {
                            martialPropMessage.Add((martialPropMessage.Count + 1) + unitName + " 删除不存在的秘籍 技能 " + martialData.baseID);
                            return true;
                        }
                        else
                        {
                            FixMartialPrefix(unitName + " 修复功法词条 ", v);
                        }
                    }
                    else
                    {
                        martialPropMessage.Add((martialPropMessage.Count + 1) + unitName + " 删除不存在的秘籍 None " + martialData.baseID);
                        return true;
                    }
                }
                return false;

            }));
        }

        private static void FixUnitTitle(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 修复道号
            bool fixTitle = false;
            Il2CppSystem.Collections.Generic.List<DataUnit.AppellationTitle.AppellationItem> appellationItems = new Il2CppSystem.Collections.Generic.List<DataUnit.AppellationTitle.AppellationItem>();
            for (int i = 0; i < unitData.appellationTitle.appellationItems.Count; i++)
            {
                if (g.conf.appellationTitle.GetItem(unitData.appellationTitle.appellationItems[i].id) == null)
                {
                    titleMessage.Add((titleMessage.Count + 1) +" "+unitName + " 删除不存在的道号 " + unitData.appellationTitle.appellationItems[i].id);
                    fixTitle = true;
                }
                else
                {
                    appellationItems.Add(unitData.appellationTitle.appellationItems[i]);
                }
            }
            if (fixTitle)
            {
                unitData.appellationTitle.appellationItems = appellationItems;
                fixTitle = false;
            }

            Il2CppSystem.Collections.Generic.List<int> equipAppellationID = new Il2CppSystem.Collections.Generic.List<int>();
            for (int i = 0; i < unitData.appellationTitle.equipAppellationID.Count; i++)
            {
                if (g.conf.appellationTitle.GetItem(unitData.appellationTitle.equipAppellationID[i]) == null)
                {
                    titleMessage.Add((titleMessage.Count + 1)+" "+unitName + " 卸下装备的不存在的道号 " + unitData.appellationTitle.equipAppellationID[i]);
                    fixTitle = true;
                }
                else
                {
                    equipAppellationID.Add(unitData.appellationTitle.equipAppellationID[i]);
                }
            }
            if (fixTitle)
            {
                unitData.appellationTitle.equipAppellationID = equipAppellationID;
                fixTitle = false;
            }
        }


        private static void FixUnitSkill(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 装备的技能
            if (unitData.skillLeft != "")
            {
                var m = unitData.GetActionMartial(unitData.skillLeft);
                if (g.conf.battleSkillAttack.GetItem(m.data.values[4]) == null)
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + " 卸下装备的不存在的武技 " + unitData.skillLeft);
                    unitData.skillLeft = "";
                }
                else
                {
                    FixMartialPrefix(unitName + " 修复功法词条 ", m.data);
                }
            }
            if (unitData.skillRight != "")
            {
                var m = unitData.GetActionMartial(unitData.skillRight);
                if (g.conf.battleSkillAttack.GetItem(m.data.values[4]) == null)
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + " 卸下装备的不存在的绝技 " + unitData.skillRight);
                    unitData.skillRight = "";
                }
                else
                {
                    FixMartialPrefix(unitName + " 修复功法词条 ", m.data);
                }
            }
            if (unitData.ultimate != "")
            {
                var m = unitData.GetActionMartial(unitData.ultimate);
                if (g.conf.battleSkillAttack.GetItem(m.data.values[4]) == null)
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + " 卸下装备的不存在的神通 " + unitData.ultimate);
                    unitData.ultimate = "";
                }
                else
                {
                    FixMartialPrefix(unitName + " 修复功法词条 ", m.data);
                }
            }
            if (unitData.step != "")
            {
                var m = unitData.GetActionMartial(unitData.step);
                if (g.conf.battleStepBase.GetItem(m.data.values[4]) == null)
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + " 卸下装备的不存在的身法 " + unitData.step);
                    unitData.step = "";
                }
                else
                {
                    FixMartialPrefix(unitName + " 修复功法词条 ", m.data);
                }
            }
            for (int i = 0; i < unitData.abilitys.Length; i++)
            {
                if (unitData.abilitys[i] != "")
                {
                    var m = unitData.GetActionMartial(unitData.abilitys[i]);
                    if (g.conf.battleAbilityBase.GetItem(m.data.values[4]) == null)
                    {
                        skillMessage.Add((skillMessage.Count + 1) + unitName + " 卸下装备的不存在的心法 " + unitData.abilitys[i]);
                        unitData.abilitys[i] = "";
                    }
                    else
                    {
                        DataProps.MartialData martialData = m.data.To<DataProps.MartialData>();
                        DataProps.PropsAbilityData abilityData = martialData.data.To<DataProps.PropsAbilityData>();
                        if (abilityData.suitID != 0)
                        {
                            if (g.conf.battleAbilitySuitBase.GetItem(abilityData.suitID) == null)
                            {
                                skillMessage.Add((skillMessage.Count + 1) + unitName + " 修复装备的心法套装错误 " + abilityData.suitID);
                                abilityData.suitID = 0;
                            }
                        }
                        FixMartialPrefix(unitName + " 修复功法词条 ", m.data);
                    }
                }
            }

            Dictionary<string, DataUnit.ActionMartialData> allActionMartial = new Dictionary<string, DataUnit.ActionMartialData>();
            foreach (var item in unitData.allActionMartial)
            {
                allActionMartial.Add(item.key, item.value);
            }
            foreach (var item in allActionMartial)
            {
                var key = item.Key;
                var m = item.Value;

                var type = m.data.To<DataProps.MartialData>().martialType;
                if (type == MartialType.SkillLeft || type == MartialType.SkillRight || type == MartialType.Ultimate)
                {
                    if (g.conf.battleSkillAttack.GetItem(m.data.values[4]) == null)
                    {
                        if (type == MartialType.SkillLeft)
                        {
                            skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的武技 " + m.data.values[4]);
                        }
                        else if (type == MartialType.SkillRight)
                        {
                            skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的绝技 " + m.data.values[4]);
                        }
                        else if (type == MartialType.Ultimate)
                        {
                            skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的神通 " + m.data.values[4]);
                        }
                    }
                    else
                    {
                        FixMartialPrefix(unitName + " 修复秘籍词条 ", m.data);
                        continue;
                    }
                }
                else if (type == MartialType.Step)
                {
                    if (g.conf.battleStepBase.GetItem(m.data.values[4]) == null)
                    {
                        skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的身法 " + m.data.values[4]);
                    }
                    else
                    {
                        FixMartialPrefix(unitName + " 修复秘籍词条 ", m.data);
                        continue;
                    }
                }
                else if (type == MartialType.Ability)
                {
                    if (g.conf.battleAbilityBase.GetItem(m.data.values[4]) == null)
                    {
                        skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的心法 " + m.data.values[4]);
                    }
                    else
                    {
                        FixMartialPrefix(unitName + " 修复秘籍词条 ", m.data);
                        continue;
                    }
                }
                else
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + " 删除不存在的功法？ " + m.data.values[4]);
                }
                unitData.allActionMartial.Remove(key);
            }

        }

        private static void FixMartialPrefix(string unitName, DataProps.PropsData prop)
        {
            DataProps.MartialData martialData = prop.To<DataProps.MartialData>();

            for (int i = 6; i < prop.values.Length; i++)
            {
                if (prop.values[i] == -1)
                {
                    break;
                }
                ConfBattleSkillPrefixValueItem item;
                try
                {
                    item = g.conf.battleSkillPrefixValue.GetItem((int)martialData.martialType, martialData.baseID, prop.values[i]); ;
                }
                catch (Exception)
                {
                    item = null;
                }
                if (item == null)
                {
                    skillMessage.Add((skillMessage.Count + 1) + unitName + prop.values[i] + " "
                        + new Func<string>(() =>
                        {
                            string str;
                            if (martialData.martialType == MartialType.Ability)
                            {
                                str = $"心法{martialData.baseID} {GameTool.LS(g.conf.battleAbilityBase.GetItem(martialData.baseID).name)} 的第{i - 5}个词条";
                            }
                            else if (martialData.martialType == MartialType.SkillLeft)
                            {
                                str = $"武技{martialData.baseID} {GameTool.LS(g.conf.battleSkillAttack.GetItem(martialData.baseID).name)} 的第{i - 5}个词条";
                            }
                            else if (martialData.martialType == MartialType.SkillRight)
                            {
                                str = $"绝技{martialData.baseID} {GameTool.LS(g.conf.battleSkillAttack.GetItem(martialData.baseID).name)} 的第{i - 5}个词条";
                            }
                            else if (martialData.martialType == MartialType.Step)
                            {
                                str = $"身法{martialData.baseID} {GameTool.LS(g.conf.battleStepBase.GetItem(martialData.baseID).name)} 的第{i - 5}个词条";
                            }
                            else if (martialData.martialType == MartialType.Ultimate)
                            {
                                str = $"神通{martialData.baseID} {GameTool.LS(g.conf.battleSkillAttack.GetItem(martialData.baseID).name)} 的第{i - 5}个词条";
                            }
                            else
                            {
                                str = $"未知功法{martialData.baseID} 的第{i - 5}个词条";
                            }
                            return str;
                        })());
                    prop.values[i] = 1;
                }
            }
        }

        private static void FixUnitModel(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 修复角色立绘
            PortraitModelData modelData = unitData.propertyData.modelData;
            var humanData = unitData.propertyData.battleModelData;
            // 后背
            if (modelData.back != 0 && g.conf.roleDress.GetItem(modelData.back) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "houbei");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 后背 {modelData.back} >> {id}");
                modelData.back = id;
            }
            // 帽子
            if (modelData.hat != 0 && g.conf.roleDress.GetItem(modelData.hat) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "maozi");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 帽子 {modelData.hat} >> {id}");
                modelData.hat = id;
            }
            // 头发前
            if (modelData.hairFront != 0 && g.conf.roleDress.GetItem(modelData.hairFront) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "toufaqian");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 头发前 {modelData.hairFront} >> {id}");
                modelData.hairFront = id;
            }
            // 衣服
            if (modelData.body != 0 && g.conf.roleDress.GetItem(modelData.body) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "yifu");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 头发前 {modelData.body} >> {id}");
                modelData.body = id;
            }
            // 眉毛
            if (modelData.eyebrows != 0 && g.conf.roleDress.GetItem(modelData.eyebrows) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "meimao");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 眉毛 {modelData.eyebrows} >> {id}");
                modelData.eyebrows = id;
            }
            // 眼睛
            if (modelData.eyes != 0 && g.conf.roleDress.GetItem(modelData.eyes) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "yanjing");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 眼睛 {modelData.eyes} >> {id}");
                modelData.eyes = id;
            }
            // 头发
            if (modelData.hair != 0 && g.conf.roleDress.GetItem(modelData.hair) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "toufa");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 头发 {modelData.hair} >> {id}");
                modelData.hair = id;
            }
            // 脸
            if (modelData.head != 0 && g.conf.roleDress.GetItem(modelData.head) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "lian");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 脸 {modelData.head} >> {id}");
                modelData.head = id;
            }
            // 嘴巴
            if (modelData.mouth != 0 && g.conf.roleDress.GetItem(modelData.mouth) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "zuiba");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 嘴巴 {modelData.mouth} >> {id}");
                modelData.mouth = id;
            }
            // 鼻子
            if (modelData.nose != 0 && g.conf.roleDress.GetItem(modelData.nose) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "bizi");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 鼻子 {modelData.nose} >> {id}");
                modelData.nose = id;
            }
            // 全脸
            if (modelData.faceFull != 0 && g.conf.roleDress.GetItem(modelData.faceFull) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "quanlian");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 全脸 {modelData.faceFull} >> {id}");
                modelData.faceFull = id;
            }
            // 左脸
            if (modelData.faceLeft != 0 && g.conf.roleDress.GetItem(modelData.faceLeft) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "zuolian");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 左脸 {modelData.faceLeft} >> {id}");
                modelData.faceLeft = id;
            }
            // 右脸
            if (modelData.faceRight != 0 && g.conf.roleDress.GetItem(modelData.faceRight) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "youlian");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 右脸 {modelData.faceRight} >> {id}");
                modelData.faceRight = id;
            }
            // 眉心
            if (modelData.forehead != 0 && g.conf.roleDress.GetItem(modelData.forehead) == null)
            {
                var list = g.conf.roleDress.GetDressItem((int)unitData.propertyData.sex, "meixin");
                var id = list[CommonTool.Random(0, list.Count)].id;
                dressMessage.Add($"{unitName} 修复立绘 眉心 {modelData.forehead} >> {id}");
                modelData.forehead = id;
            }

            // 战斗立绘
            if (humanData.back != 0 && g.conf.roleDress.GetItem(humanData.back) == null)
            {
                dressMessage.Add($"{unitName} 修复战斗立绘 后背 {humanData.back} >> {modelData.back}");
                humanData.back = modelData.back;
            }
            if (humanData.hat != 0 && g.conf.roleDress.GetItem(humanData.hat) == null)
            {
                dressMessage.Add($"{unitName} 修复战斗立绘 帽子 {humanData.hat} >> {modelData.hat}");
                humanData.hat = modelData.hat;
            }
            if (humanData.body != 0 && g.conf.roleDress.GetItem(humanData.body) == null)
            {
                dressMessage.Add($"{unitName} 修复战斗立绘 身体 {humanData.body} >> {modelData.body}");
                humanData.body = modelData.body;
            }
            if (humanData.hair != 0 && g.conf.roleDress.GetItem(humanData.hair) == null)
            {
                dressMessage.Add($"{unitName} 修复战斗立绘 头发 {humanData.hair} >> {modelData.hair}");
                humanData.hair = modelData.hair;
            }
            if (humanData.head != 0 && g.conf.roleDress.GetItem(humanData.head) == null)
            {
                dressMessage.Add($"{unitName} 修复战斗立绘 脸 {humanData.head} >> {modelData.head}");
                humanData.head = modelData.head;
            }
        }

        private static void FixUnitSprite(DataUnit.UnitInfoData unitData, string unitName)
        {

            // 器灵
            CallQueue delSpriteCq = new CallQueue();
            foreach (var sprite in unitData.artifactSpriteData.sprites)
            {
                ConfArtifactSpriteItem spriteItem = g.conf.artifactSprite.GetItem(sprite.spriteID);
                if (spriteItem == null)
                {
                    var delSprite = sprite;
                    delSpriteCq.Add(new Action(() =>
                    {
                        if (unitData.artifactSpriteData.firstSpriteData.firstSpriteSoleID == delSprite.soleID)
                        {
                            unitData.artifactSpriteData.firstSpriteData.firstSpriteSoleID = 0;
                            spriteMessage.Add((spriteMessage.Count + 1) + unitName + " 删除不存在的本命器灵 " + delSprite.spriteID);
                        }
                        else
                        {
                            spriteMessage.Add((spriteMessage.Count + 1) + " 删除不存在的器灵 " + delSprite.spriteID);
                        }
                        var propList = unitData.propData.allProps;
                        foreach (var prop in propList)
                        {
                            if (prop.propsType == DataProps.PropsDataType.Props && prop.propsItem.className == 401)
                            {
                                DataProps.PropsArtifact artifactData = prop.To<DataProps.PropsArtifact>();
                                if (artifactData.spriteSoleID == delSprite.soleID)
                                {
                                    titleMessage.Add((spriteMessage.Count + 1) + " 法宝取消融合器灵 " + prop.propsID);
                                    artifactData.spriteSoleID = 0;
                                }
                            }
                        }
                        unitData.artifactSpriteData.sprites.Remove(delSprite);
                    }));
                }
            }
            delSpriteCq.RunAllCall();
        }

        private static void FixUnitTask(DataUnit.UnitInfoData unitData, string unitName)
        {

            // 任务
            CallQueue cq3 = new CallQueue();
            foreach (var v in unitData.allTask)
            {
                var data = v;
                if (g.conf.taskBase.GetItem(v.id) == null)
                {
                    cq3.Add(new Action(() =>
                    {
                        taskMessage.Add((taskMessage.Count) + unitName + " 删除不存在的任务=" + data.id);
                        unitData.allTask.Remove(data);
                    }));
                }
            }
            cq3.RunAllCall();
        }

        private static void FixUnitBrodLuck(List<int> badLuckId, DataUnit.UnitInfoData unitData, string unitName)
        {
            List<DataUnit.LuckData> bornLuck = new List<DataUnit.LuckData>();
            foreach (var v in unitData.propertyData.bornLuck)
            {
                var data = v;
                if (g.conf.roleCreateFeature.GetItem(v.id) == null)
                {
                    logMessage.Add((logMessage.Count) + unitName + " 删除不存在的先天天气运=" + data.id);
                }
                else if (badLuckId.Contains(v.id))
                {
                    logMessage.Add((logMessage.Count) + unitName + " 删除错误的先天气运=" + v.id + "(" + GameTool.LS(g.conf.roleCreateFeature.GetItem(v.id).name) + ")");
                }
                else
                {
                    bornLuck.Add(v);
                }
            }
            unitData.propertyData.bornLuck = bornLuck.ToArray();
        }

        private static void FixUnitPostnatalLuck(List<int> badLuckId, DataUnit.UnitInfoData unitData, string unitName)
        {
            // 后天气运 broth
            CallQueue cq2 = new CallQueue();
            foreach (var v in unitData.propertyData.addLuck)
            {
                var data = v;
                if (g.conf.roleCreateFeature.GetItem(v.id) == null)
                {
                    cq2.Add(new Action(() =>
                    {
                        logMessage.Add((logMessage.Count) + unitName + " 删除不存在的后天气运=" + data.id);
                        unitData.propertyData.addLuck.Remove(data);
                    }));
                }
                else if (badLuckId.Contains(v.id))
                {
                    cq2.Add(new Action(() =>
                    {
                        logMessage.Add((logMessage.Count) + unitName + " 删除错误的后天气运=" + v.id + "(" + GameTool.LS(g.conf.roleCreateFeature.GetItem(v.id).name) + ")");
                        unitData.propertyData.addLuck.Remove(data);
                    }));
                }
            }
            cq2.RunAllCall();
        }

        private static void FixUnitEquip(DataUnit.UnitInfoData unitData, string unitName)
        {

            // 装备的道具
            for (int i = 0; i < unitData.props.Length; i++)
            {
                if (unitData.props[i] != "")
                {
                    DataProps.PropsData propsData = unitData.propData.GetProps(unitData.props[i]);
                    if (propsData == null || propsData.propsItem == null)
                    {
                        propMessage.Add((propMessage.Count) + unitName + " 删除不存在的装备中的道具 " + unitData.props[i]);
                        unitData.props[i] = "";
                    }
                }
            }
            // 装备的装备
            for (int i = 0; i < unitData.equips.Length; i++)
            {
                if (unitData.equips[i] != "")
                {
                    DataProps.PropsData propsData = unitData.propData.GetProps(unitData.equips[i]);
                    if (propsData == null || propsData.propsItem == null)
                    {
                        propMessage.Add((propMessage.Count) + unitName + " 删除不存在的装备中的装备 " + unitData.equips[i]);
                        unitData.equips[i] = "";
                    }
                }
            }
        }

        private static void FixUnitOutTrait1(DataUnit.UnitInfoData unitData, string unitName)
        {

            // 外在性格1
            while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.outTrait1) == null)
            {
                var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                if (g.conf.roleCreateCharacter.GetItem(id).type == 2 && id != unitData.propertyData.outTrait2)
                {
                    traitMessage.Add((traitMessage.Count) + unitName + " 修复外在性格1错误 " + unitData.propertyData.outTrait1);
                    unitData.propertyData.outTrait1 = id;
                }
            }

            // 外在性格2
            while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.outTrait2) == null)
            {
                var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                if (g.conf.roleCreateCharacter.GetItem(id).type == 2 && id != unitData.propertyData.outTrait1)
                {
                    traitMessage.Add((traitMessage.Count) + unitName + " 修复外在性格2错误 " + unitData.propertyData.outTrait2);
                    unitData.propertyData.outTrait2 = id;
                }
            }
        }

        private static void FixUnitInTrait(DataUnit.UnitInfoData unitData, string unitName)
        {

            // 内在性格
            while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.inTrait) == null)
            {
                var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                if (g.conf.roleCreateCharacter.GetItem(id).type == 1)
                {
                    traitMessage.Add((traitMessage.Count) + unitName + " 修复内在性格错误 " + unitData.propertyData.inTrait);
                    unitData.propertyData.inTrait = id;
                }
            }
        }

        private static void FixUnitHero(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 天骄
            if (unitData.heart.state == DataUnit.TaoistHeart.HeartState.Complete && g.conf.npcHeroesBas.GetItem(unitData.heart.heroesSkillGroupID) == null)
            {
                heartMessage.Add((heartMessage.Count) + unitName + " 修复天骄错误 " + unitData.heart.heroesSkillGroupID);
                unitData.heart.heroesSkillGroupID = g.conf.npcHeroesBas.allConfBase[CommonTool.Random(0, g.conf.npcHeroesBas.allConfBase.Count)].id;
            }
        }

        private static void FixUnitGrade(DataUnit.UnitInfoData unitData, string unitName)
        {
            // 境界
            if (g.conf.roleGrade.GetItem(unitData.propertyData.gradeID) == null)
            {
                gradeMessage.Add((gradeMessage.Count) + unitName + " 修复境界错误 " + unitData.propertyData.gradeID);
                unitData.propertyData.gradeID = 1;
            }
        }

        private static void CheckBuildData()
        {
            ModMain.NextFixTip();
            Action checkTownStorage = null;
            int min = int.MaxValue;
            Console.WriteLine("检查建筑数据：宗门和建木");
            float max = g.data.grid.mapWidth * g.data.grid.mapHeight;
            int index = 0;
            for (int x = 0; x < g.data.grid.mapWidth; x++)
            {
                for (int y = 0; y < g.data.grid.mapHeight; y++)
                {
                    // 以下代码如何让value保留两位小数显示
                    index++;
                    float value = index / max * 100;
                    ModMain.FixTip($"检测建筑数据 {value.ToString("F2")}% 修复:宗门-{schoolMessage.Count + delSchoolMessage.Count} 建木-{propMessage.Count}");
                    DataGrid.GridData gridData = g.data.grid.GetGridData(new Vector2Int(x, y));

                    if (gridData.IsBuild() && gridData.isOrigi)
                    {
                        var a = WorldFactory.GetBuildData(gridData.typeInt);
                        var b = a.GetBuild(new Vector2Int(x, y));
                        if (b == null || b.Cast<DataBuildBase.BuildData>() == null)
                        {
                            Console.WriteLine("删除无法修复的建筑 " + gridData.typeInt + "(" + x + ", " + y + ")");
                            delSchoolMessage.Add("删除无法修复的建筑 " + gridData.typeInt + "(" + x + ", " + y + ")");

                            DataGrid.GridData grid = new DataGrid.GridData();
                            g.data.grid.allGrid[GameTool.PointToInt(new Vector2Int(x, y))] = new DataGrid.GridData();
                            continue;
                        }
                    }

                    if (gridData != null && gridData.IsBuild() && gridData.isOrigi && gridData.typeInt == (int)MapTerrainType.School)
                    {
                        var area = gridData.areaBaseID;
                        DataBuildSchool.School schoolData = WorldFactory.GetBuildData(gridData.typeInt).Cast<DataBuildSchool.School>();
                        DataBuildBase.BuildData data = schoolData.GetBuild(new Vector2Int(x, y)).Cast<DataBuildBase.BuildData>();
                        if (g.data.world.fixPatch > 50)
                        {
                            try
                            {
                                // {"scale":5,"branch":0,"stand":2,"mainSchoolPoint":null,"schoolSloganItem1Type1":522,"schoolSloganItem1Type2":525,"schoolSloganItem2Type1":524,"schoolSloganItem2Type2":527,"typeID":8,"basTypes":["basFinger","basFroze","basWood","basPalm","basBlade","basSpear"],"name1ID":54,"name2ID":53,"schoolInitScaleID":5101,"fateID":1023,"heritID":12,"stopRun":true}
                                SchoolInitScaleData scaleData = CommonTool.JsonToObject<SchoolInitScaleData>(data.values[0]);
                                bool isChange = false;
                                if (g.conf.schoolName.GetItem(scaleData.name1ID) == null)
                                {
                                    var newId = g.conf.schoolName.allConfBase[CommonTool.Random(0, g.conf.schoolName.allConfBase.Count)].id;
                                    schoolMessage.Add((schoolMessage.Count + 1) + "重设不存在的宗门第一个字：" + scaleData.name1ID + "→" + newId);
                                    scaleData.name1ID = newId;
                                    isChange = true;
                                }
                                if (g.conf.schoolName.GetItem(scaleData.name2ID) == null)
                                {
                                    var newId = g.conf.schoolName.allConfBase[CommonTool.Random(0, g.conf.schoolName.allConfBase.Count)].id;
                                    schoolMessage.Add((schoolMessage.Count + 1) + "重设不存在的宗门第二个字：" + scaleData.name2ID + "→" + newId);
                                    scaleData.name2ID = newId;
                                    isChange = true;
                                }
                                if (g.conf.schoolType.GetItem(scaleData.typeID) == null)
                                {
                                    var newId = g.conf.schoolType.allConfBase[CommonTool.Random(0, g.conf.schoolType.allConfBase.Count)].id;
                                    schoolMessage.Add((schoolMessage.Count + 1) + "重设不存在的宗门类型：" + scaleData.typeID + "→" + newId);
                                    scaleData.typeID = newId;
                                    isChange = true;
                                }
                                var schoolName = GameTool.LS(g.conf.schoolName.GetItem(scaleData.name1ID).name1) + GameTool.LS(g.conf.schoolName.GetItem(scaleData.name2ID).name2) + GameTool.LS(g.conf.schoolType.GetItem(scaleData.typeID).name);
                                if (isChange)
                                {
                                    schoolMessage[schoolMessage.Count - 1] += "\n修改后的宗门名字为：" + schoolName;
                                }
                                if (g.conf.schoolInitScale.GetItem(scaleData.schoolInitScaleID) == null)
                                {
                                    List<ConfSchoolInitScaleItem> list = new List<ConfSchoolInitScaleItem>();
                                    foreach (var item in g.conf.schoolInitScale.allConfBase)
                                    {
                                        if (g.conf.schoolSmall.GetItem(item.id) == null)
                                        {
                                            var conf = g.conf.schoolInitScale.GetItem(item.id);
                                            if (conf.area == area || conf.area + 1 == area)
                                            {
                                                list.Add(conf);
                                            }
                                        }
                                    }
                                    int newId = list.Count > 0 ? list[CommonTool.Random(0, list.Count)].id : g.conf.schoolInitScale.allConfBase[CommonTool.Random(0, g.conf.schoolInitScale.allConfBase.Count)].id;
                                    schoolMessage.Add((schoolMessage.Count + 1) + $" {schoolName} 区域{area}修复的宗门规模：" + scaleData.schoolInitScaleID + "→" + newId);
                                    scaleData.schoolInitScaleID = newId;
                                    isChange = true;
                                }
                                if (isChange)
                                {
                                    data.values[0] = CommonTool.ObjectToJson(scaleData);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("损坏的宗门数据=" + data.values[0]);
                                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                                try
                                {
                                    SchoolInitScaleData scaleData = CommonTool.JsonToObject<SchoolInitScaleData>(data.values[0]);
                                    Console.WriteLine("scale=" + scaleData.scale);
                                    Console.WriteLine("branch=" + scaleData.branch);
                                    Console.WriteLine("stand=" + scaleData.stand);
                                    Console.WriteLine("mainSchoolPoint=" + scaleData.mainSchoolPoint);
                                    Console.WriteLine("schoolSloganItem1Type1=" + scaleData.schoolSloganItem1Type1);
                                    Console.WriteLine("schoolSloganItem1Type2=" + scaleData.schoolSloganItem1Type2);
                                    Console.WriteLine("schoolSloganItem2Type1=" + scaleData.schoolSloganItem2Type1);
                                    Console.WriteLine("schoolSloganItem2Type2=" + scaleData.schoolSloganItem2Type2);
                                    Console.WriteLine("basTypes=" + string.Join(",", scaleData.basTypes) + "(" + scaleData.basTypes.Count + ")");
                                    Console.WriteLine("name1ID=" + scaleData.name1ID);
                                    Console.WriteLine("name2ID=" + scaleData.name2ID);
                                    Console.WriteLine("schoolInitScaleID=" + scaleData.schoolInitScaleID);
                                    Console.WriteLine("fateID=" + scaleData.fateID);
                                    Console.WriteLine("heritID=" + scaleData.heritID);
                                    Console.WriteLine("stopRun=" + scaleData.stopRun);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("宗门数据无法转json");
                                }
                                delSchoolMessage.Add((delSchoolMessage.Count) + " 删除无法修复的宗门 " + x + "," + y);

                                MapCreate mapCreate = new MapCreate(g.data.grid);
                                ConfWorldCreateCmd.Cmd c = new ConfWorldCreateCmd.Cmd();
                                c.Init(mapCreate);
                                MapCreateGridData dataGrid = mapCreate.GetGridData(new Vector2Int(x, y));
                                c.ClearBuildSchoolDecorate(new Vector2Int(x, y), dataGrid.decorateID);

                                schoolData.DelBuild(data.id);

                                CallQueue cq = new CallQueue();
                                for (int i = 0; i < data.points.Length; i++)
                                {
                                    DataGrid.GridData grid = g.data.grid.GetGridData(GameTool.StrToPoint(data.points[i]));
                                    if (grid != null)
                                    {
                                        cq.Add(new Action(() =>
                                        {
                                            grid.decorateID = 0;
                                            grid.isOrigi = false;
                                        }));
                                    }
                                }
                                cq.RunAllCall();
                            }
                        }
                        else
                        {
                            Console.WriteLine("旧宗门数据 " + string.Join(", ", data.values));
                        }
                    }


                    if (gridData.isOrigi && gridData.typeInt == (int)MapTerrainType.Town && gridData.areaBaseID == 1)
                    {
                        DataBuildTown.Town townData = WorldFactory.GetBuildData(gridData.typeInt).Cast<DataBuildTown.Town>();
                        DataBuildBase.BuildData data = townData.GetBuild(new Vector2Int(x, y)).Cast<DataBuildBase.BuildData>();
                        var point = GameTool.StrToPoint(data.points[0]);
                        if (point.x * point.y < min)
                        {
                            min = point.x * point.y;
                            checkTownStorage = () =>
                            {
                                var storage = data.GetBuildSub(MapBuildSubType.TownStorage); // MapBuildTownStorage
                                                                                             // 删除建木不存在的道具
                                string dataStr = storage.objData.GetString("data");
                                MapBuildTownStorage.Data townStorageData = CommonTool.JsonToObject<MapBuildTownStorage.Data>(dataStr);
                                if (null != townStorageData)
                                {
                                    bool needFix = false;
                                    townStorageData.propData.allProps.RemoveAll(new Func<DataProps.PropsData, bool>((v) =>
                                    {
                                        var itemId = v.propsID;
                                        if (itemId != 0 && v.propsItem == null)
                                        {
                                            propMessage.Add((propMessage.Count + 1) + " 从建木中 删除不存在的道具 " + itemId);
                                            needFix = true;
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }));
                                    if (needFix)
                                    {
                                        storage.objData.SetString("data", CommonTool.ObjectToJson(townStorageData));
                                    }
                                }
                            };
                        }
                    }
                }
            }
            checkTownStorage?.Invoke();
            {
                float value = index / max * 100;
                ModMain.FixTip($"检测建筑数据 {value.ToString("F2")}% 修复:宗门-{schoolMessage.Count + delSchoolMessage.Count} 建木-{propMessage.Count}");
            }
        }
    }

}
