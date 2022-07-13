using System;
using System.Reflection;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace FixData
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
        private static HarmonyLib.Harmony harmony;
        public static bool autoFixData;
        /// <summary>
        /// MOD初始化，进入游戏时会调用此函数
        /// </summary>
        public void Init()
        {
            //使用了Harmony补丁功能的，需要手动启用补丁。
            //启动当前程序集的所有补丁
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
            if (harmony == null)
            {
                harmony = new HarmonyLib.Harmony("MOD_M60S96");
            }
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (PlayerPrefs.HasKey("GuiFixDataItem"))
            {
                autoFixData = PlayerPrefs.GetInt("GuiFixDataItem") == 1;
            }
            else
            {
                autoFixData = true;
                PlayerPrefs.SetInt("GuiFixDataItem", 1);
            }
        }
    }


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

        private static void FixData()
        {
            List<string> message = new List<string>();
            int fixID = 0, schoolCount = 0, propCount = 0, eventCount = 0, monstCount = 0, taskCount = 0, luckCount = 0, logCount = 0, gradeCount = 0, heartCountCount = 0, delSchool = 0, traitCount = 0, spriteCount = 0;
            {
                Console.WriteLine("检查的宗门数据");
                for (int x = 0; x < g.data.grid.mapWidth; x++)
                {
                    for (int y = 0; y < g.data.grid.mapHeight; y++)
                    {
                        DataGrid.GridData gridData = g.data.grid.GetGridData(new Vector2Int(x, y));
                        if (gridData != null && gridData.IsBuild() && gridData.isOrigi && gridData.typeInt == (int)MapTerrainType.School)
                        {
                            DataBuildSchool.School schoolData = WorldFactory.GetBuildData(gridData.typeInt).Cast<DataBuildSchool.School>();
                            DataBuildBase.BuildData data = schoolData.GetBuild(new Vector2Int(x, y)).Cast<DataBuildBase.BuildData>();
                            try
                            {
                                SchoolInitScaleData scaleData = CommonTool.JsonToObject<SchoolInitScaleData>(data.values[0]);
                                bool isChange = false;
                                if (g.conf.schoolType.GetItem(scaleData.typeID) == null)
                                {
                                    var newId = g.conf.schoolType.allConfBase[CommonTool.Random(0, g.conf.schoolType.allConfBase.Count)].id;
                                    message.Add((++fixID) + "重设不存在的宗门类型：" + scaleData.typeID + "→" + newId);
                                    scaleData.typeID = newId;
                                    isChange = true;
                                    schoolCount++;
                                }
                                if (g.conf.schoolName.GetItem(scaleData.name1ID) == null)
                                {
                                    var newId = g.conf.schoolName.allConfBase[CommonTool.Random(0, g.conf.schoolName.allConfBase.Count)].id;
                                    message.Add((++fixID) + "重设不存在的宗门第一个字：" + scaleData.name1ID + "→" + newId);
                                    scaleData.name1ID = newId;
                                    isChange = true;
                                    schoolCount++;
                                }
                                if (g.conf.schoolName.GetItem(scaleData.name2ID) == null)
                                {
                                    var newId = g.conf.schoolName.allConfBase[CommonTool.Random(0, g.conf.schoolName.allConfBase.Count)].id;
                                    message.Add((++fixID) + "重设不存在的宗门第二个字：" + scaleData.name2ID + "→" + newId);
                                    scaleData.name2ID = newId;
                                    isChange = true;
                                    schoolCount++;
                                }
                                if (isChange)
                                {
                                    data.values[0] = CommonTool.ObjectToJson(scaleData);
                                }
                            }
                            catch (Exception)
                            {
                                message.Add((++fixID) + " 删除无法修复的宗门 " + x + "," + y + " 异常数据：" + data.values[0]);
                                schoolCount++;

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
                    }
                }

                Console.WriteLine("检查角色数据");
                foreach (var item in g.data.unit.allUnit)
                {
                    DataUnit.UnitInfoData unitData = item.Value;
                    bool isPlayer = g.data.world.playerUnitID == unitData.unitID;
                    string unitName = unitData.propertyData.GetName();
                    // 境界
                    if (g.conf.roleGrade.GetItem(unitData.propertyData.gradeID) == null)
                    {
                        message.Add((++fixID) + unitName + " 修复境界错误 " + unitData.propertyData.gradeID);
                        gradeCount++;
                        unitData.propertyData.gradeID = 1;
                    }
                    // 天骄
                    if (unitData.heart.state == DataUnit.TaoistHeart.HeartState.Complete && g.conf.npcHeroesBas.GetItem(unitData.heart.heroesSkillGroupID) == null)
                    {
                        message.Add((++fixID) + unitName + " 修复天骄错误 " + unitData.heart.heroesSkillGroupID);
                        heartCountCount++;
                        unitData.heart.heroesSkillGroupID = g.conf.npcHeroesBas.allConfBase[CommonTool.Random(0, g.conf.npcHeroesBas.allConfBase.Count)].id;
                    }

                    // 内在性格
                    while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.inTrait) == null)
                    {
                        var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                        if (g.conf.roleCreateCharacter.GetItem(id).type == 1)
                        {
                            message.Add((++fixID) + unitName + " 修复内在性格错误 " + unitData.propertyData.inTrait);
                            traitCount++;
                            unitData.propertyData.inTrait = id;
                        }
                    }

                    // 外在性格1
                    while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.outTrait1) == null)
                    {
                        var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                        if (g.conf.roleCreateCharacter.GetItem(id).type == 2 && id != unitData.propertyData.outTrait2)
                        {
                            message.Add((++fixID) + unitName + " 修复外在性格1错误 " + unitData.propertyData.outTrait1);
                            traitCount++;
                            unitData.propertyData.outTrait1 = id;
                        }
                    }

                    // 外在性格2
                    while (g.conf.roleCreateCharacter.GetItem(unitData.propertyData.outTrait2) == null)
                    {
                        var id = g.conf.roleCreateCharacter.allConfBase[CommonTool.Random(0, g.conf.roleCreateCharacter.allConfBase.Count)].id;
                        if (g.conf.roleCreateCharacter.GetItem(id).type == 2 && id != unitData.propertyData.outTrait1)
                        {
                            message.Add((++fixID) + unitName + " 修复外在性格2错误 " + unitData.propertyData.outTrait2);
                            traitCount++;
                            unitData.propertyData.outTrait2 = id;
                        }
                    }

                    // 删除背包不存在的道具
                    CallQueue cq1 = new CallQueue();
                    foreach (var v in unitData.propData.allProps)
                    {
                        var data = v;
                        var itemId = v.propsID;
                        if (itemId != 0 && v.propsItem == null)
                        {
                            cq1.Add(new Action(() =>
                            {
                                message.Add((++fixID) + unitName + " 删除不存在的道具 " + itemId);
                                propCount++;
                                unitData.propData.allProps.Remove(data);
                            }));
                        }
                    }
                    cq1.RunAllCall();

                    // 装备的道具
                    for (int i = 0; i < unitData.props.Length; i++)
                    {
                        if (unitData.props[i] != "")
                        {
                            DataProps.PropsData propsData = unitData.propData.GetProps(unitData.props[i]);
                            if (propsData == null || propsData.propsItem == null)
                            {
                                message.Add((++fixID) + unitName + " 删除不存在的装备中的道具 " + unitData.props[i]);
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
                                message.Add((++fixID) + unitName + " 删除不存在的装备中的装备 " + unitData.equips[i]);
                                unitData.equips[i] = "";
                            }
                        }
                    }
                    // 后天气运
                    CallQueue cq2 = new CallQueue();
                    foreach (var v in unitData.propertyData.addLuck)
                    {
                        var data = v;
                        if (g.conf.roleCreateFeature.GetItem(v.id) == null)
                        {
                            cq2.Add(new Action(() =>
                            {
                                message.Add((++fixID) + unitName + " 删除不存在的后天气运=" + data.id);
                                luckCount++;
                                unitData.propertyData.addLuck.Remove(data);
                            }));
                        }
                    }
                    cq2.RunAllCall();
                    List<DataUnit.LuckData> bornLuck = new List<DataUnit.LuckData>();
                    foreach (var v in unitData.propertyData.bornLuck)
                    {
                        var data = v;
                        if (g.conf.roleCreateFeature.GetItem(v.id) == null)
                        {
                            message.Add((++fixID) + unitName + " 删除不存在的先天天气运=" + data.id);
                            luckCount++;
                        }
                        else
                        {
                            bornLuck.Add(v);
                        }
                    }
                    unitData.propertyData.bornLuck = bornLuck.ToArray();

                    // 任务
                    CallQueue cq3 = new CallQueue();
                    foreach (var v in unitData.allTask)
                    {
                        var data = v;
                        if (g.conf.taskBase.GetItem(v.id) == null)
                        {
                            cq3.Add(new Action(() =>
                            {
                                message.Add((++fixID) + unitName + " 删除不存在的任务=" + data.id);
                                taskCount++;
                                unitData.allTask.Remove(data);
                            }));
                        }
                    }
                    cq3.RunAllCall();

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
                                    message.Add((++fixID) + unitName + " 删除不存在的本命器灵 " + delSprite.spriteID);
                                    spriteCount++;
                                }
                                else
                                {
                                    message.Add((++fixID) + unitName + " 删除不存在的器灵 " + delSprite.spriteID);
                                    spriteCount++;
                                }
                                var propList = unitData.propData.allProps;
                                foreach (var prop in propList)
                                {
                                    if (prop.propsType == DataProps.PropsDataType.Props && prop.propsItem.className == 401)
                                    {
                                        DataProps.PropsArtifact artifactData = prop.To<DataProps.PropsArtifact>();
                                        if (artifactData.spriteSoleID == delSprite.soleID)
                                        {
                                            message.Add(unitName + " 法宝取消融合器灵 " + prop.propsID);
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

                Console.WriteLine("检查事件");
                CallQueue cq4 = new CallQueue();
                foreach (var v in g.data.map.allGridEventID)
                {
                    var data = v;
                    if (v.value.id != 0 && g.conf.worldFortuitousEventBase.GetItem(v.value.id) == null)
                    {
                        cq4.Add(new Action(() =>
                        {
                            message.Add((++fixID) + "删除不存在事件：" + data.value.id);
                            eventCount++;
                            g.data.map.allGridEventID.Remove(data.key);
                        }));
                    }
                }
                cq4.RunAllCall();
            }
            Console.WriteLine("检查怪物");
            CallQueue cq5 = new CallQueue();
            foreach (var v in g.data.map.allGridMonst)
            {
                var data = v;
                if (v.value.id != 0 && g.conf.dungeonBase.GetItem(v.value.id) == null)
                {
                    cq5.Add(new Action(() =>
                    {
                        message.Add((++fixID) + "删除不存在怪物：" + data.value.id);
                        monstCount++;
                        g.data.map.allGridMonst.Remove(data.key);
                    }));
                }
            }
            cq5.RunAllCall();

            Console.WriteLine("检查角色日志");
            CallQueue cq6 = new CallQueue();
            foreach (var item in g.data.unitLog.allLog)
            {
                var k = item.key;
                var v = item.value;
                foreach (var item2 in v.allLog)
                {
                    var cacheItem = item2;
                    cq5.Add(new Action(() =>
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
                            message.Add((++fixID) + "删除不存在生平记事：" + destroyId);
                            logCount++;
                            v.allLog.Remove(cacheItem);
                        }
                    }));
                }
                foreach (var item2 in v.allVitalLog)
                {
                    var cacheItem = item2;
                    cq5.Add(new Action(() =>
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
                            message.Add((++fixID) + "删除不存在重大生平记事：" + destroyId);
                            logCount++;
                            v.allVitalLog.Remove(cacheItem);
                        }
                    }));
                }

            }
            cq6.RunAllCall();

            if (message.Count > 0)
            {
                string str = $"检测到存档已损坏，自动修补了{fixID}条数据："
                    + $"\n删除了{delSchool}个无法修复的宗门\n修复{traitCount}条性格数据"
                    + $"\n修复{spriteCount}条器灵错误\n修复{gradeCount}条境界错误\n修复{heartCountCount}天骄错误"
                    + $"\n修复{schoolCount}条宗门数据\n清除{propCount}个道具\n清除{luckCount}条气运\n清除{taskCount}个任务\n清除{eventCount}个事件\n清除{monstCount}个副本\n清除{logCount}条错误生平记事\n修复详情如下：";
                Console.WriteLine(str);
                message.Insert(0, str);
                g.world.system.AddSystemInMap(new Action<Il2CppSystem.Action>((call) =>
                {
                    var ui = g.ui.OpenUI<UITextInfoLong>(UIType.TextInfoLong);
                    ui.InitData("修补存档", string.Join("\n", message));
                    ui.onCloseCall += call;
                }));
            }
        }
    }


    [HarmonyPatch(typeof(UIGameSetting), "Init")]
    class Patch_UIGameSetting_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UIGameSetting __instance)
        {
            try
            {
                AddFixToggle(__instance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        private static void AddFixToggle(UIGameSetting ui)
        {
            var root = ui.goSystem.transform.Find("Scroll View/Root");
            var oldItem = root.Find("GuiFixDataItem");
            if (oldItem != null)
            {
                GameObject.Destroy(oldItem.gameObject);
            }
            var item= GameObject.Instantiate( root.Find("Item (7)").gameObject);
            item.name = "GuiFixDataItem";
            item.transform.SetParent(root, false);
            item.transform.SetSiblingIndex(1);
            var textItem = item.transform.GetChild(0).GetComponent<Text>();
            textItem.text = "大鬼自动修复存档";
            var toggleItem = item.transform.GetChild(1).GetComponent<Toggle>();
            toggleItem.onValueChanged.RemoveAllListeners();
            toggleItem.isOn = ModMain.autoFixData;
            toggleItem.onValueChanged.AddListener(new Action<bool>((isOn)=>
            {
                ModMain.autoFixData = isOn;
                if (ModMain.autoFixData)
                {
                    UITipItem.AddTip("开启自动修复存档！");
                }
                else
                {
                    UITipItem.AddTip("关闭自动修复存档！");
                }
            }));
            GameObject btnObj = GameObject.Instantiate(ui.btnSystemOK.gameObject);
            btnObj.transform.SetParent(item.transform, false);
            btnObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, 0);
            btnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 28);
            btnObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(45, 28);
            btnObj.transform.GetChild(1).GetComponent<Text>().text = "赞助"; 
            var btn = btnObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(new Action(()=>
            {
                Application.OpenURL("http://www.yellowshange.com/about.php");
            }));
        }
    }

}
