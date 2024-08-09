using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Cave.Patch;
using Cave;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.IO;

namespace Cave.BuildFunction
{
    /*
     
     */
    // 后山
    public class MountainBuild : ClassBase
    {
        private Vector2 playerBronPoint = new Vector2(2.6f, 9.2f); // 玩家出生点
        private Vector2 tranPoint = new Vector2(1.0f, 9.2f); // 传送阵点
        UIBattleInfo uiBase;

        // 初始化副本
        public override void Init(string param)
        {
            MainCave.Close();
            DataCave dataCave = DataCave.ReadData();
            

            Action<ETypeData> onBattleStart = OnBattleStart;
            Action<ETypeData> onBattleEnd = OnBattleEnd;

            // 监听战斗开始一次
            g.events.On(EBattleType.BattleStart, onBattleStart, 1);
            // 监听战斗结束
            g.events.On(EBattleType.BattleEnd, onBattleEnd, 1);

            // 进入副本
            g.world.battle.IntoBattle(new DataMap.MonstData() { id = 42685183, level = 1 });
        }

        private void OnBattleStart(ETypeData e)
        {
            uiBase = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
            //初始化副本

            Action onIntoRoomEnd = () => OnIntoRoomEnd();
            SceneType.battle.battleMap.onIntoRoomEnd += onIntoRoomEnd;

            Action onStart = () =>
            {
                SceneType.battle.timer.Frame(new Action(OnUpdate), 1, true);
            };
            SceneType.battle.battleMap.AddStartBattleCall(onStart);

            // 自动点击开始战斗按钮
            TimerCoroutine autoStartCor = null;
            Action action = () =>
            {
                uiBase.uiStartBattle.btnStart.onClick.Invoke();
                if (!uiBase.uiStartBattle.goGroupRoot.activeSelf)
                {
                    SceneType.battle.timer.Stop(autoStartCor);
                    InitDungeonUI();
                }
            };
            autoStartCor = SceneType.battle.timer.Frame(action, 1, true);

            InitDungeon();
        }

        // 初始化副本UI
        private void InitDungeonUI()
        {
        }

        // 初始化副本
        private void InitDungeon()
        {
        }

        // 每帧执行
        private void OnUpdate()
        {
        }


        // 清空玩家效果 
        private void ClearPlayerEffects(UnitCtrlHuman unit)
        {
            // 清空玩家状态
            unit.AddState(UnitStateType.NotUserAllSkill, -1);
            // 清除蛟龙等效果
            new SchoolBigFight.UnitEffectSpecHandler().Close(unit);
            // 固定移动速度
            unit.data.moveSpeed.baseValue = 80;
            // 清除僚机
            unit.WingmanEnable(false);
            // 清除战斗效果
            var list = unit.effects;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                unit.DelEffect(list[i]);
            }
            var player = unit.TryCast<UnitCtrlPlayer>();
            if (player != null)
            {
                player.eyeSkillBase = null;
                player.piscesSkillBase = null;
            }
        }

        // 进入房间结束
        private void OnIntoRoomEnd()
        {
            // 设置玩家初始位置
            SceneType.battle.battleMap.playerUnitCtrl.move.SetPosition(playerBronPoint);
            Cave.Log("设置玩家初始位置:" + SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x+","+ SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.y);

            float intoTime = 0;
            //初始化离开副本的法阵
            Action exitAction = () =>
            {
                if (Time.time < intoTime)
                {
                    return;
                }
                Action exitDungeon = () =>
                {
                    SceneType.battle.battleEnd.BattleEnd(true);
                    intoTime = Time.time + 1;
                };
                Action exitCancel = () =>
                {
                    intoTime = Time.time + 1;
                };
                intoTime = float.MaxValue;
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), "是否确定离开地牢？", 2, exitDungeon, exitCancel);
            };
            // 设置离开传送法阵的位置
            SceneType.battle.battleMap.CreatePassLevelEffect(tranPoint, exitAction, null).isRepet = true;
            UIMask.GradientEffectToWhite();
        }

        private void OnBattleEnd(ETypeData e)
        {
            g.events.On(EBattleType.BattleExit, new Action(() =>
            {
                GameTool.SetCursor(g.data.globle.gameSetting.cursorMap, g.data.globle.gameSetting.cursorMapLed);
                new MainCave();
            }), 1, true);

        }
    }
}
