using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cave.Team
{
    /// <summary>
    /// 玩家同行队伍
    /// </summary>
    public class PlayerTeam
    {
        private Il2CppSystem.Action<ETypeData> onBattleStart;
        private Il2CppSystem.Action<ETypeData> onBattleExit;
        private Il2CppSystem.Action<ETypeData> onCreateActionBack;
        private Il2CppSystem.Action<ETypeData> onSaveData;
        private Il2CppSystem.Action<ETypeData> onIntoWorld;

        private TimerCoroutine corOnUpdate;
        private List<Vector2Int> updatePoint = new List<Vector2Int>(); // 由于改变了移动，要刷新的格子
        private List<UnitCtrlHumanNPC> battleUnits = new List<UnitCtrlHumanNPC>();

        private Il2CppSystem.Action<ETypeData> onUIMapBattlePre;
        private Il2CppSystem.Action<ETypeData> onUIBattleInfo;
        public void Init()
        {
            // 战斗开始
            onBattleStart = (Il2CppSystem.Action<ETypeData>)OnBattleStart;
            g.events.On(EBattleType.BattleStart, onBattleStart);

            // 战斗结束
            onBattleExit = (Il2CppSystem.Action<ETypeData>)OnBattleExit;
            g.events.On(EBattleType.BattleExit, onBattleExit);

            //保存游戏时
            onSaveData = (Il2CppSystem.Action<ETypeData>)OnSaveData;
            g.events.On(EGameType.SaveData, onSaveData);

            // 进入游戏时
            onIntoWorld = (Il2CppSystem.Action<ETypeData>)OnIntoWorld;
            g.events.On(EGameType.IntoWorld, onIntoWorld);

            // 创建行为结束
            onCreateActionBack = (Il2CppSystem.Action<ETypeData>)OnCreateActionBack;
            g.events.On(EGameType.CreateActionBack(true), onCreateActionBack);
            g.events.On(EGameType.CreateActionBack(false), onCreateActionBack);

            // 每帧调用的函数
            corOnUpdate = g.timer.Frame(new Action(OnUpdate), 1, true);

            // 监听打开UI
            onUIMapBattlePre = (Il2CppSystem.Action<ETypeData>)OnUIMapBattlePre;
            onUIBattleInfo = (Il2CppSystem.Action<ETypeData>)OnUIBattleInfo;
            g.events.On(EGameType.OneOpenUIEnd(UIType.MapBattlePre), onUIMapBattlePre); // .Find("Left") -380 156
            g.events.On(EGameType.OneOpenUIEnd(UIType.BattleInfo), onUIBattleInfo); // StartBattle -380 156

        }

        private void OnUIMapBattlePre(ETypeData eData)
        {
            var ui = g.ui.GetUI<UIMapBattlePre>(UIType.MapBattlePre);
            if (ui != null)
            {
                new UIPlayerTeam(ui.transform.Find("Left"), new Vector2(-380, 156));
            }
        }

        private void OnUIBattleInfo(ETypeData eData)
        {
            var ui = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
            if (ui != null)
            {
                new UIPlayerTeam(ui.startBattle.goGroupRoot.transform, new Vector2(-580, 0));
            }
        }

        /// <summary>
        /// 每帧调用的函数
        /// </summary>
        private void OnUpdate()
        {
            if(SceneType.map != null && SceneType.map.world != null && updatePoint != null)
            {
                foreach (var point in updatePoint)
                {
                    var grid = SceneType.map.world.GetGrid(point);
                    if (grid != null)
                    {
                        var ui = grid.gridUI;
                        if (ui != null)
                        {
                            ui.UpdateUI();
                        }
                    }
                }
                updatePoint.Clear();
            }
        }

        // 当一个行为执行完毕
        private void OnCreateActionBack(ETypeData e)
        {
            EGameTypeData.CreateActionBack edata = e.TryCast<EGameTypeData.CreateActionBack>();
            if (edata.action == null)
            {
                return;
            }
            UnitActionSetPoint action = edata.action.TryCast<UnitActionSetPoint>();
            if (action != null)
            {
                if (action.unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID) // 如果玩家移动了，让同道之人一起移动
                {
                    var point = g.world.playerUnit.data.unitData.GetPoint();
                    foreach (string unitId in DataTeam.teamUnits)
                    {
                        WorldUnitBase unit = g.world.unit.GetUnit(unitId, false);
                        if (unit != null)
                        {
                            var p = unit.data.unitData.GetPoint();
                            if (!updatePoint.Contains(p))
                            {
                                updatePoint.Add(p);
                            }
                            unit.CreateAction(new UnitActionSetPoint(point));
                        }
                    }
                    for (int x = -2; x < 3; x++)
                    {
                        for (int y = -2; y < 3; y++)
                        {
                            var p = point + new Vector2Int(x, y);
                            if (!updatePoint.Contains(p))
                            {
                                updatePoint.Add(p);
                            }
                        }
                    }
                }
                else if (DataTeam.teamUnits.Contains(action.unit.data.unitData.unitID)) // 如果同道之人移动了，如果位置和玩家不同，将它移动回来
                {
                    var point = g.world.playerUnit.data.unitData.GetPoint();
                    if (action.point != point)
                    {
                        action.unit.CreateAction(new UnitActionSetPoint(point));
                        if (!updatePoint.Contains(point))
                        {
                            updatePoint.Add(point);
                        }
                        if (!updatePoint.Contains(action.point))
                        {
                            updatePoint.Add(action.point);
                        }
                    }
                }
            }
        }

        private void OnIntoWorld(ETypeData e)
        {
            DataTeam.InitData();
        }

        private void OnSaveData(ETypeData e)
        {
            DataTeam.SaveData();
        }

        // 战斗开始
        private void OnBattleStart(ETypeData eData)
        {
            Action onIntoRoomEnd = () => OnIntoRoomEnd();
            SceneType.battle.battleMap.onIntoRoomEnd += onIntoRoomEnd;
            Action onBattleStart = () => OnBattleStart();
            SceneType.battle.battleMap.onBattleStartCall += onBattleStart;
            SceneType.battle.battleMap.onIntoRoomStart += new Action(()=>
            {
                Cave.Log("进入房间开始 清楚单位" + battleUnits.Count);
                ClearBattleUnits();
            });
        }

        // 进入房间结束
        private void OnIntoRoomEnd()
        {
            Cave.Log("进入房间结束");
            InitRoomBattleUnits();
        }
        // 进入房间结束
        private void OnBattleStart()
        {
            Cave.Log("战斗开始");
            InitRoomBattleUnits();
        }
        private void OnBattleExit(ETypeData eData)
        {
            battleUnits.Clear();
        }

        private void InitRoomBattleUnits()
        {
            if (!SceneType.battle.battleMap.isStartBattle)
                return;
            ClearBattleUnits();

            List<string> units = DataTeam.battleUnits;
            foreach (var unitId in units)
            {
                WorldUnitBase unit = g.world.unit.GetUnit(unitId, false);
                if (unit != null)
                {
                    try
                    {
                        UnitCtrlHumanNPC humanNpc = SceneType.battle.unit.CreateUnitHuman<UnitCtrlHumanNPC>(unit.data, UnitType.PlayerNPC);
                        humanNpc.move.SetPosition((Vector2)SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi + CommonTool.RandomDireV2());
                        humanNpc.move.SetDire(CommonTool.Random(0, 100) < 50);
                        battleUnits.Add(humanNpc);
                        humanNpc.data.hp = humanNpc.data.maxHP.value;
                    }
                    catch (Exception e)
                    {
                        Cave.LogWarning(e.Message + "" + e.StackTrace);
                    }
                }
            }
        }

        private void ClearBattleUnits()
        {
            foreach (var humanNpc in battleUnits)
            {
                if (humanNpc != null && !humanNpc.isDie)
                {
                    try
                    {
                        if (humanNpc.isDie)
                        {
                            humanNpc.gameObject.SetActive(false);
                        }
                        else
                        {
                            humanNpc.Die(false);
                        }
                    }
                    catch (Exception e)
                    {
                        Cave.LogWarning(e.Message + "" + e.StackTrace);
                    }
                }
            }
            battleUnits.Clear();
        }

        public void Destroy()
        {
            g.events.Off(EGameType.CreateActionBack(true), onCreateActionBack);
            g.events.Off(EGameType.CreateActionBack(false), onCreateActionBack);
            g.events.Off(EBattleType.BattleStart, onBattleStart);
            g.events.Off(EBattleType.BattleExit, onBattleExit);
            g.events.Off(EGameType.SaveData, onSaveData);
            g.events.Off(EGameType.IntoWorld, onIntoWorld);
            g.timer.Stop(corOnUpdate);

            g.events.Off(EGameType.OneOpenUIEnd(UIType.MapBattlePre), onUIMapBattlePre); // .Find("Left") -380 156
            g.events.Off(EGameType.OneOpenUIEnd(UIType.BattleInfo), onUIBattleInfo); // StartBattle -380 156
        }

    }
}
