using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace YellowMod
{
    public class Class1 : MelonMod
    {
        void test()
        {
            // 测试监听
            Action<ETypeData> testData = TestData;
            g.events.On(EGameType.OpenUIEnd, testData);
        }
        // 设置他人的关系时
        private void TestData(ETypeData e)
        {
            var edata = e.Cast<EGameTypeData.OpenUIEnd>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.K))
            {
                MelonDebug.Msg("K = ");
                test();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                MelonLogger.Msg("更换神器 昊天眼");
                g.data.world.animaWeapons.Clear();
                g.data.world.animaWeapons.Add(GameAnimaWeapon.HootinEye);
                DataWorld.World.GodEyeData data = g.data.world.godEyeData;
                if (data == null)
                {
                    data = new DataWorld.World.GodEyeData();
                }
                var list1 = g.conf.godEyeSkills.allConfBase;
                foreach (var item in list1)
                {
                    try
                    {
                        if (!data.skillsID.Contains(item.id))
                        {
                            data.skillsID.Add(item.id);
                        }
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Msg("Ae=" + e.Message + "\n" + e.StackTrace);
                        throw;
                    }
                    try
                    {
                        ConfGodEyeSkillsItem item1 = g.conf.godEyeSkills.GetItem(item.id);
                        if (data.bossGrade.ContainsKey(item1.bossID))
                        {
                            data.bossGrade[item1.bossID] = g.world.playerUnit.data.dynUnitData.GetGrade();
                        }
                        else
                        {
                            data.bossGrade.Add(item1.bossID, g.world.playerUnit.data.dynUnitData.GetGrade());
                        }

                    }
                    catch (Exception e)
                    {
                        MelonLogger.Msg("Be=" + e.Message + "\n" + e.StackTrace);
                    }
                }
                MelonLogger.Msg("昊天眼神器添加完毕 已解锁所有技能 请保存游戏重进！");
            }
        }
    }
}