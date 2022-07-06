﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GuiBaseUI;

namespace Cave
{
    public class CaveOnWorleRunEnd
    {

        public CaveOnWorleRunEnd()
        {
            DataCave data = DataCave.ReadData();

            if (data.state != 2)
            {
                return;
            }

            var point = data.GetPoint();
            int idx = 0;

            Dictionary<string, CaveNpcData> npcDatas = new Dictionary<string, CaveNpcData>(data.npcDatas);
            foreach (KeyValuePair<string, CaveNpcData> item in npcDatas)
            {
                Cave.Log(" 处理洞府居民 "+ (idx++)+"/"+ data.npcDatas.Count);
                var npc = item.Value;
                Cave.Log(" 处理洞府居民：unitID=" + npc.unitID + " state=" + npc.state);
                var unit = g.world.unit.GetUnit(npc.unitID);
                if (unit == null)
                {
                    DataUnitDie.DieData unitData = g.data.unitDie.GetUnitOrDie(npc.unitID);
                    Cave.Log("洞府居民不存在：" + npc.unitID +" 死亡数据="+(unitData != null));
                    if (unitData != null && npc.state == 2)
                    {
                        data.AddLog($"{unitData.GetName()}已身死道消，离开了洞府。");
                    }
                    data.SetNpcIntoState(npc.unitID, 0);
                }
                else
                {
                    int intim = Mathf.RoundToInt(unit.data.unitData.relationData.intimToPlayerUnit);
                    if (npc.state == 2)
                    {
                        // 检查是否离开洞府
                        bool isOk = CommonTool.Random(0, 100) < (-intim);
                        if (isOk || intim < 0)
                        {
                            data.SetNpcIntoState(npc.unitID, 0);
                            data.AddLog($"{unit.data.unitData.propertyData.GetName()}由于个人原因，离开了{data.name}。");
                        }
                        else
                        {
                            if (data.GetNpcIntoState(npc.unitID) == 2 && point != GameTool.StrToPoint(npc.lastPoint) && GameTool.StrToPoint(npc.lastPoint) != unit.data.unitData.GetPoint())
                            {
                                MoveNpc(unit, point);
                            }
                            npc.lastPoint = GameTool.PointToStr(unit.data.unitData.GetPoint());
                        }
                    }
                    else if (npc.state == 1)
                    {
                        // 被邀请入住洞府 需要做出回复
                        bool isOk = CommonTool.Random(0, 100) < intim;
                        if (isOk || intim > 0)
                        {
                            data.SetNpcIntoState(npc.unitID, 2);
                            data.AddLog($"<color=#{CaveStateData.blud}>{unit.data.unitData.propertyData.GetName()}</color>同意了邀请，住进了<color=#{CaveStateData.blud}>{data.name}</color>。");
                            MoveNpc(unit, point);
                            unit.data.unitData.relationData.AddIntim(g.world.playerUnit.data.unitData.unitID, CommonTool.Random(20, 30));
                        }
                        else
                        {
                            data.SetNpcIntoState(npc.unitID, 1);
                            data.AddLog($"<color=#{CaveStateData.blud}>{unit.data.unitData.propertyData.GetName()}</color>拒绝了<color=#{CaveStateData.blud}>{data.name}</color>邀请。");
                        }
                    }

                    npc.lastPoint = GameTool.PointToStr(unit.data.unitData.GetPoint());
                    if (npc.state == 2) {
                        if (unit.data.unitData.heart.IsHeroes()) {
                            unit.data.unitData.relationData.AddIntim(g.world.playerUnit.data.unitData.unitID, CommonTool.Random(5, 15)); // 天骄入住洞府随机增加对洞主的好感度
                        } else {
                            unit.data.unitData.relationData.AddIntim(g.world.playerUnit.data.unitData.unitID, CommonTool.Random(10, 30)); // 非天骄入住洞府随机增加对洞主的好感度
                        }
                    }
                }
            }
            DataCave.SaveData(data);
        }

        private void MoveNpc(WorldUnitBase unit, Vector2Int point)
        {
            WorldUnitLuckBase luckInvite = unit.GetLuck(102); // 邀约中不能移动
            if (luckInvite == null && g.world.unit.GetUnit(unit, false) == null) // 隐藏了并且没有死亡气运，则不能移动
                return;

            unit.CreateAction(new UnitActionSetPoint(point));
        }
    }
}