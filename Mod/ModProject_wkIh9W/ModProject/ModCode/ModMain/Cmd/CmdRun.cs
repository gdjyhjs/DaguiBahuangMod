using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace MOD_wkIh9W
{
    public class CmdRun : MonoBehaviour
    {
        //必须有这行代码，否则无法MonoBehaviour类无法AddComponent
        public CmdRun(IntPtr ptr) : base(ptr) { }
        public bool isRun = false;

        void Update()
        {
            try
            {
                if (!isRun)
                {
                    if (g.game == null || g.ui == null || g.cache == null)
                    {
                        return;
                    }
                    if (g.ui.GetUI(UIType.Login) != null)
                    {
                        isRun = true;
                    }
                    return;
                }
                ModMain.OnUpdate();

                foreach (var item in ModMain.allCmdItems)
                {
                    if (item.key.IsKeyDown())
                    {
                        item.Run();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(name+" Update "+e.ToString()) ;
            }
        }



        #region

        public List<string> logs = new List<string>();
        public void AddLog(string log)
        {
            Console.WriteLine(log);
            logs.Add(log);
            UICostItemTool.AddTipText(log);
            while (logs.Count > 200)
            {
                logs.RemoveAt(0);
            }
            UIBase ui = g.ui.GetUI(new UIType.UITypeBase("DaguiTool", UILayer.UI));
            if (ui != null)
            {
                UIDaguiTool daguiTool = ui.GetComponent<UIDaguiTool>();
                if (daguiTool)
                {
                    daguiTool.AddLogItem(log);
                }
            }
        }

        public DataProps.MartialData GetMartial(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastAddMartial != null ? ModMain.lastAddMartial.t2 : null;
            }
            var props = unit.data.unitData.propData.CloneAllProps();
            foreach (var item in props)
            {
                if (item.propsType == DataProps.PropsDataType.Martial)
                {
                    if (item.soleID == soleId)
                        return item.To<DataProps.MartialData>();
                }
            }
            return null;
        }


        public DataUnit.ActionMartialData GetSkill(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastStudySkill != null ? ModMain.lastStudySkill.t2 : null;
            }
            return unit.data.unitData.GetActionMartial(soleId);
        }

        public DataUnit.ActionMartialData GetAbility(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastStudyAbility != null ? ModMain.lastStudyAbility.t2 : null;
            }
            return unit.data.unitData.GetActionMartial(soleId);
        }

        public DataProps.PropsData GetElder(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastAddElder != null ? ModMain.lastAddElder.t2 : null;
            }
            return unit.data.unitData.propData.GetProps(soleId);
        }

        public DataProps.PropsData GetRule(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastAddRule != null ? ModMain.lastAddRule.t2 : null;
            }
            return unit.data.unitData.propData.GetProps(soleId);
        }

        public DataProps.PropsData GetArtifact(WorldUnitBase unit, string soleId)
        {
            if (soleId == "last")
            {
                return ModMain.lastAddArtifact != null ? ModMain.lastAddArtifact.t2 : null;
            }
            return unit.data.unitData.propData.GetProps(soleId);
        }

        public WorldUnitBase GetUnit(string unit)
        {
            WorldUnitBase u = null;
            if (unit == "player" || unit == g.world.playerUnit.data.unitData.unitID)
            {
                u = g.world.playerUnit;
            }
            else if (unit == "unitA")
            {
                var ui = g.ui.GetUI<UIDramaDialogue>(UIType.DramaDialogue);
                if (ui != null)
                {
                    u = ui.dramaData.unitLeft;
                }
            }
            else if (unit == "unitB")
            {
                var ui = g.ui.GetUI<UIDramaDialogue>(UIType.DramaDialogue);
                if (ui != null)
                {
                    u = ui.dramaData.unitRight;
                }
            }
            else if (unit == "lookUnit")
            {
                var npcInfo = g.ui.GetUI<UINPCInfo>(UIType.NPCInfo);
                if (npcInfo != null)
                {
                    u = npcInfo.unit;
                }
            }
            else if (unit == "playerWife")
            {
                u = g.world.unit.GetUnit(g.world.playerUnit.data.unitData.relationData.married, true);
            }
            else
            {
                u = g.world.unit.GetUnit(unit, true);
            }
            ModMain.lastUnit = u;
            return u;
        }

        public string GetUnitName(WorldUnitBase unit)
        {
            if (unit == null)
            {
                return "查无此人";
            }
            string relationStr = g.conf.npcPartFitting.GetPartName(unit.data.unitData.unitID, g.world.playerUnit);
            string name = unit.data.unitData.propertyData.GetName();
            if (relationStr != "")
            {
                name += "(" + relationStr + ")";
            }
            return name;
        }



        string p1, p2, p3, p4;
        public void Cmd(string[] para)
        {
            p1 = para[1];
            p2 = para[2];
            p3 = para[3];
            p4 = para[4];
            AddLog("执行: " + string.Join(" ", para));
            MethodInfo method = typeof(CmdRun).GetMethod(para[0]);
            if (method != null)
            {
                method.Invoke(this, null);
            }
            else
            {
                Console.WriteLine("Method not found");
                AddLog("敬请期待！");
            }

            //Invoke(para[0], 0);
        }

        //提升境界
        public void UpGrade()
        {

        }

        public void SetRoleGrade()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }


            if (!int.TryParse(p2, out int value))
            {
                value = 0;
            }
            int grade = value / 3 + 1;
            int phase = value % 3 + 1;

            foreach (var item in g.conf.testAttr._allConfList)
            {
                if (item.grade == grade && item.phase == phase)
                {
                    unit.data.dynUnitData.hpMax.baseValue = item.hp;
                    unit.data.dynUnitData.mpMax.baseValue = item.mp;
                    unit.data.dynUnitData.spMax.baseValue = item.sp;
                    unit.data.dynUnitData.moveSpeed.baseValue = item.moveSpeed;
                    unit.data.dynUnitData.attack.baseValue = item.attack;
                    unit.data.dynUnitData.defense.baseValue = item.defense;
                    unit.data.dynUnitData.basisBlade.baseValue = item.basBlade;
                    unit.data.dynUnitData.basisSpear.baseValue = item.basSpear;
                    unit.data.dynUnitData.basisSword.baseValue = item.basSword;
                    unit.data.dynUnitData.basisFist.baseValue = item.basFist;
                    unit.data.dynUnitData.basisPalm.baseValue = item.basPalm;
                    unit.data.dynUnitData.basisFinger.baseValue = item.basFinger;
                    unit.data.dynUnitData.basisFire.baseValue = item.basisFire;
                    unit.data.dynUnitData.basisFroze.baseValue = item.basisFroze;
                    unit.data.dynUnitData.basisThunder.baseValue = item.basisThunder;
                    unit.data.dynUnitData.basisWind.baseValue = item.basisWind;
                    unit.data.dynUnitData.basisEarth.baseValue = item.basisEarth;
                    unit.data.dynUnitData.basisWood.baseValue = item.basisWood;
                    unit.data.dynUnitData.phycicalFree.baseValue = item.phycicalFree;
                    unit.data.dynUnitData.magicFree.baseValue = item.magicFree;
                    unit.data.dynUnitData.crit.baseValue = item.crit;
                    unit.data.dynUnitData.guard.baseValue = item.guard;
                    unit.data.dynUnitData.critValue.baseValue = item.critValue;
                    unit.data.dynUnitData.guardValue.baseValue = item.guardValue;
                    unit.data.dynUnitData.abilityPoint.baseValue = item.abilityPoint;

                    unit.data.dynUnitData.gradeID.baseValue = g.conf.roleGrade.GetGradeItem((int)item.grade, (int)item.phase).id;
                    unit.data.dynUnitData.exp.baseValue = g.conf.roleGrade.GetGradeItem((int)item.grade, (int)item.phase).exp;

                    AddLog("执行成功");
                    return;
                }
            }
            AddLog("执行失败：无法找到输入的境界：" + value + " " + grade + "-" + phase);
        }

        //设置逆天改命
        public void SetFate()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }


            if (!int.TryParse(p2, out int grade))
            {
                grade = unit.data.dynUnitData.GetGrade();
            }

            if (!int.TryParse(p3, out int fateId))
            {
                AddLog("执行失败：输入的气运id不合法：" + p3);
                return;
            }

            var fate = g.conf.fateFeature.GetItem(fateId);

            if (fate == null)
            {
                AddLog("执行失败：输入的气运id不存在：" + p3);
                return;
            }
            DelFate(unit, grade);

            Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade;
            if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
            {
                upGrade = g.data.world.playerLog.upGrade;
            }
            else
            {
                upGrade = unit.data.unitData.npcUpGrade;
            }
            // 添加对应境界的逆天改命
            var data = new DataWorld.World.PlayerLogData.GradeData();
            data.quality = 5;
            upGrade.Add(grade, data);

            UnitActionLuckAdd luckAdd = new UnitActionLuckAdd(fate.id) { fateFeatureGrade = grade };
            luckAdd.Init(unit);
            unit.CreateAction(luckAdd);

            if (unit != g.world.playerUnit && unit.data.unitData.heart.IsHeroes())
            {
                unit.data.unitData.heart.InitSkillHeart(unit);
            }
            AddLog("执行成功！");
        }

        void DelFate(WorldUnitBase unit, int grade)
        {
            Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade;
            if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
            {
                upGrade = g.data.world.playerLog.upGrade;
            }
            else
            {
                upGrade = unit.data.unitData.npcUpGrade;
            }
            if (upGrade.ContainsKey(grade))
            {
                var item = upGrade[grade];
                upGrade.Remove(grade);
                var conf = g.conf.fateFeature.GetItem(item.luck);
                WorldUnitLuckBase luck = g.world.playerUnit.GetLuck(item.luck);
                if (luck != null)
                {
                    unit.CreateAction(new UnitActionLuckDel(luck));
                }

                //逆天改命效果降级
                var fates = g.conf.fateFeature.GetGroupFeatureItems(conf.group);
                if (fates.Count > 0)
                {
                    ConfFateFeatureItem fate = null;
                    foreach (var v in fates)
                    {
                        if (g.conf.fateFeature.GetItemIndex(v.id) >= g.conf.fateFeature.GetItemIndex(item.luck))
                            continue;
                        if (fate == null || fate.id < v.id)
                        {
                           fate = v;
                        }
                    }

                    if (fate != null)
                    {
                        //新增气运 
                        ConfRoleCreateFeatureItem featureItem = g.conf.roleCreateFeature.GetItem(fates[fates.Count - 1].id);
                        DataUnit.LuckData luckData = new DataUnit.LuckData();
                        luckData.id = fates[fates.Count - 1].id;
                        luckData.duration = int.Parse(featureItem.duration);
                        unit.data.unitData.propertyData.AddAddLuck(luckData);
                        unit.CreateLuck(luckData);
                    }
                }
            }
        }

        //添加气运
        public void AddLuck()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int fateId))
            {
                AddLog("执行失败：输入的气运id不合法：" + p2);
                return;
            }

            var fate = g.conf.roleCreateFeature.GetItem(fateId);

            if (fate == null)
            {
                AddLog("执行失败：输入的气运id不存在：" + p2);
                return;
            }

            UnitActionLuckAdd luckAdd = new UnitActionLuckAdd(fateId);
            var err = unit.CreateAction(luckAdd);

            if (luckAdd.luck != null)
            {
                if (fate.type == 1 || fate.type == 11) {
                    List<DataUnit.LuckData> list = new List<DataUnit.LuckData>(unit.data.unitData.propertyData.bornLuck);
                    list.Add(luckAdd.luckData);
                    unit.data.unitData.propertyData.bornLuck = list.ToArray();
                }
                if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
                {
                    g.ui.OpenUI<UIGetReward>(UIType.GetReward).UpdateLuck(luckAdd.luck.luckData);
                }
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败：" + err);
            }
        }

        //删除气运
        public void DelLuck()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int fateId))
            {
                AddLog("执行失败：输入的气运id不合法：" + p2);
                return;
            }

            var fate = g.conf.roleCreateFeature.GetItem(fateId);

            if (fate == null)
            {
                AddLog("执行失败：输入的气运id不存在：" + p2);
                return;
            }
            int err = 1, err2  = 0;
            WorldUnitLuckBase luckBase = unit.GetLuck(fateId);
            if (luckBase != null)
            {
                err2 = unit.CreateAction(new UnitActionLuckDel(luckBase));
                if (err2 == 0)
                {
                    List<DataUnit.LuckData> list = new List<DataUnit.LuckData>(unit.data.unitData.propertyData.bornLuck);
                    var item = list.Find((v) => v.id == fateId);
                    if (item != null)
                    {
                        list.Remove(item);
                    }
                    unit.data.unitData.propertyData.bornLuck = list.ToArray();

                    err--;
                }
                else
                {
                }
            }
            else
            {
            }

            Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade;
            if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
            {
                upGrade = g.data.world.playerLog.upGrade;
            }
            else
            {
                upGrade = unit.data.unitData.npcUpGrade;
            }
            List<int> rmlist = new List<int>();
            foreach (var i in upGrade.Keys)
            {
                var item2 = upGrade[i];
                if (item2.luck == fateId)
                {
                    rmlist.Add(i);
                }
            }
            foreach (var item2 in rmlist)
            {
                upGrade.Remove(item2);
                err--;
            }

            if (err < 1)
            {
                AddLog("执行成功");
            }
            else
            {
                AddLog("执行失败：" + err2);
            }
        }

        //添加属性
        public void AddAttr()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }

            var vs = unit.data.dynUnitData.GetValues(p2);
            int index = 0;
            while (true)
            {
                try
                {
                    vs[index++].baseValue += value;
                }
                catch (Exception)
                {
                    break;
                }
            }
            AddLog("执行成功！");
        }

        //修改天骄道心
        public void FixHeart1()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.taoistHeart.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                value = 0;
            }
            if (!int.TryParse(p4, out int skill))
            {
                skill = 0;
            }
            unit.data.unitData.heart.state = DataUnit.TaoistHeart.HeartState.Complete;
            unit.data.unitData.heart.confID = id;
            unit.data.unitData.heart.hp = 100;
            unit.data.unitData.heart.seedGrowCool = value;
            unit.data.unitData.heart.heartEffectHP = 100;
            if (skill != 0)
            {
                unit.data.unitData.heart.heroesSkillGroupID = skill;
            }
            AddLog("执行成功！");
        }

        //修改人杰道种
        public void FixHeart2()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.taoistSeed.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                value = 0;
            }
            unit.data.unitData.heart.state = DataUnit.TaoistHeart.HeartState.Seed;
            unit.data.unitData.heart.confID = id;
            unit.data.unitData.heart.hp = 100;
            unit.data.unitData.heart.seedNeed = value;
            unit.data.unitData.heart.heartEffectHP = 100;
            AddLog("执行成功！");
        }

        //修改npc道心
        public void FixHeart3()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.taoistHeart.GetItem(id);

            if (conf == null && id != 0)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            unit.data.unitData.heart.state = DataUnit.TaoistHeart.HeartState.Empty;
            unit.data.unitData.heart.confID = id;
            unit.data.unitData.heart.hp = 100;
            unit.data.unitData.heart.seedNeed = 0;
            unit.data.unitData.heart.heartEffectHP = 100;
            if (id != 0)
            {
                unit.data.unitData.heart.heartEffectLevel = CommonTool.GetMinValue(g.conf.taoistHeartEffect.GetItemInID(id),
                    (Func<ConfTaoistHeartEffectItem, float>)((v) => { return v.level; })).level;
                unit.data.unitData.heart.heartEffectLastConfID = id;
                unit.data.unitData.heart.lastAttackUnitID = "";

                unit.data.unitData.heart.SetEmpty(unit, false);

            }
            AddLog("执行成功！");
        }

        //增加修为
        public void AddExp()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            if (unit.data.dynUnitData.curGrade >= 10 && value > 0)
            {
                unit.data.unitData.propertyData.AddFixExp(value);
            }
            else
            {
                unit.data.unitData.propertyData.AddExp(value);
            }
            AddLog("执行成功！");
        }

        //恢复所有状态
        public void Recover()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            unit.data.dynUnitData.hp.baseValue += unit.data.dynUnitData.hpMax.value;
            unit.data.dynUnitData.mp.baseValue += unit.data.dynUnitData.mpMax.value;
            unit.data.dynUnitData.sp.baseValue += unit.data.dynUnitData.spMax.value;
            unit.data.dynUnitData.health.baseValue += unit.data.dynUnitData.healthMax.value;
            unit.data.dynUnitData.energy.baseValue += unit.data.dynUnitData.energyMax.value;
            unit.data.dynUnitData.mood.baseValue += unit.data.dynUnitData.moodMax.value;
            AddLog("执行成功！");
        }

        //让A杀死B
        public void AKillB()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }

            var err = unit.CreateAction(new UnitActionRoleKill(unit2, false));

            if (err == 0)
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //修改名字
        public void FixName()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (p2.Length < 1)
            {
                AddLog("执行失败：名字至少1个字：" + p2);
                return;
            }

            string name1 = p2.Substring(0, 1);
            string name2 = p2.Substring(1, p2.Length - 1);

            unit.data.unitData.propertyData.name = new string[] { name1, name2 };
            AddLog("执行成功！");
        }

        //增加道心坚定值
        public void AddHeartHp()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }


            if (!int.TryParse(p2, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p2);
                return;
            }

            unit.data.unitData.heart.AddHeartEffectHP(unit, unit, value);
        }

        //让A传授道种给B
        public void AHeartB()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }


            UnitActionSetTaoistHeart roleRelation = new UnitActionSetTaoistHeart(unit2, unit.data.unitData.heart.HeartConf());
            var err = unit.CreateAction(roleRelation);

            if (err == 0)
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //增加道号
        public void AddTitle()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.appellationTitle.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            if (unit.appellation.AddAppellationID(id))
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！");
            }
        }

        //设置宗门职位
        public void SetSchoolPost()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (unit.data.school == null)
            {
                AddLog("执行失败：" + unit.data.unitData.propertyData.GetName() + "没有宗门：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int value))
            {
                AddLog("执行失败：职位id不合法：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value2))
            {
                value2 = 1;
            }

            SchoolPostType postType = (SchoolPostType)value;
            SchoolDepartmentType departmentType = (SchoolDepartmentType)value2;
            var school = unit.data.school;
            WorldUnitBase exitUnit = null;
            if (postType == SchoolPostType.SchoolMain)
            {
                exitUnit = g.world.unit.GetUnit(school.buildData.npcSchoolMain);
            }
            else if (postType == SchoolPostType.BigElders)
            {
                var bigEldersData = school.buildData.postData.GetBigElders(departmentType);
                if (bigEldersData != null)
                {
                    exitUnit = g.world.unit.GetUnit(bigEldersData.unitData.unitID);
                }
            }
            else if (postType == SchoolPostType.Elders)
            {
                exitUnit = g.world.unit.GetUnit(school.buildData.postData.postElders[departmentType].unitData.unitID);
            }
            else if (postType == SchoolPostType.Inherit)
            {
                if (school.buildData.postData.postElders[departmentType].manageInherit.Count > 0)
                    exitUnit = g.world.unit.GetUnit(school.buildData.postData.postElders[departmentType].manageInherit[0]);
            }
            else if (postType == SchoolPostType.In)
            {
                if (school.buildData.postData.postElders[departmentType].manageIn.Count > 0)
                    exitUnit = g.world.unit.GetUnit(school.buildData.postData.postElders[departmentType].manageIn[0]);
            }
            else if (postType == SchoolPostType.Out)
            {
                if (school.buildData.postData.postElders[departmentType].manageOut.Count > 0)
                    exitUnit = g.world.unit.GetUnit(school.buildData.postData.postElders[departmentType].manageOut[0]);
            }
            if (exitUnit != null && exitUnit.data.unitData.unitID != unit.data.unitData.unitID)
            {
                exitUnit.CreateAction(new UnitActionSchoolExit());
            }

            UnitActionSchoolSetPostType setPostAction = new UnitActionSchoolSetPostType(unit.data.school.buildData.id, postType, departmentType);
            setPostAction.Init(unit);
            setPostAction.isTest = true;
            var err = setPostAction.IsCreate(true);
            if (err == 0)
            {
                unit.CreateAction(setPostAction);
                if (value == 3)
                {
                    unit.data.school.GetBuildSub<MapBuildSchoolTaskHall>().SetDepartmentElder(departmentType, unit.data.unitData.unitID);
                }
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //加入宗门
        public void JoinSchool()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (unit.data.school != null)
            {
                AddLog("执行失败：" + unit.data.unitData.propertyData.GetName() + "已经有宗门：" + p1);
                return;
            }

            var school = g.world.build.GetBuild<MapBuildSchool>(p2);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p2);
                return;
            }

            UnitActionSchoolJoin action = new UnitActionSchoolJoin(p2, SchoolPostType.Out) { isTest = true };
            var err = unit.CreateAction(action, true);

            if (err == 0)
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //让A退出宗门
        public void ExitSchool()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (unit.data.school == null)
            {
                AddLog("执行失败：" + unit.data.unitData.propertyData.GetName() + "没有宗门：" + p1);
                return;
            }

            UnitActionSchoolExit action = new UnitActionSchoolExit();
            var err = unit.CreateAction(action, true);

            if (err == 0)
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //修改宗门属性
        public void FixSchoolAttr()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }
            int value = 0;
            if (p2 != "10" && p2 != "11")
            {
                if (!int.TryParse(p3, out value))
                {
                    AddLog("执行失败：输入的数字不合法：" + p3);
                    return;
                }
            }
            else
            {
                if (p3.Length < 1)
                {
                    AddLog("执行失败：输入的名字最少1个字：" + p3);
                    return;
                }
            }

            if (p2 == "1") //安定度
            {
                school.buildData.manorData.mainManor.AddStable(value);
            }
            else if (p2 == "2") //繁荣度
            {
                school.buildData.propertyData.AddProsperous(value);
            }
            else if (p2 == "3") //记名弟子
            {
                school.buildData.AddTotalMember(school, value);
            }
            else if (p2 == "4") //荣誉值
            {
                school.buildData.AddReputation(value);
            }
            else if (p2 == "5") //忠诚度
            {
                school.buildData.propertyData.AddLoyal(value);
            }
            else if (p2 == "7") //灵石
            {
                school.buildData.AddMoney(value);
            }
            else if (p2 == "8") //药材
            {
                school.buildData.propertyData.AddMedicina(value);
            }
            else if (p2 == "9") //矿材
            {
                school.buildData.propertyData.AddMine(value);
            }
            else if (p2 == "10") //宗门名
            {
                var fixSchools = school.GetAllSchools(true);
                foreach (var fixSchool in fixSchools)
                {
                    fixSchool.buildData.propertyData.fixName = p3;
                }
            }
            else if (p2 == "11") //分舵名
            {
                school.buildData.propertyData.fixAreaName = p3;
            }
            //else if (p2 == "12") //立场
            //{
            //}
            //else if (p2 == "13") //灵阁逆天改命
            //{
            //}
            AddLog("执行成功！");
        }

        //修改功法库
        public void FixSchoolSkill()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }
            List<string> fixBas = new List<string>(p2.Split(','));
            Console.WriteLine("旧的数据：" + school.schoolData.buildSchoolData.values[0]);
            var data = school.schoolData.buildSchoolData;
            SchoolInitScaleData scaleData = CommonTool.JsonToObject<SchoolInitScaleData>(data.values[0]);
            scaleData.basTypes = new Il2CppSystem.Collections.Generic.List<string>();
            school.buildData.propertyData.fixAddBasTypes = new Il2CppSystem.Collections.Generic.List<string>();
            var subData = school.GetBuildSub<MapBuildSchoolLibrary>().data;
            subData.weaponType.Clear();
            subData.magicType.Clear();
            foreach (var item in fixBas)
            {
                scaleData.basTypes.Add(item);
                var basType = g.conf.roleEffect.GetBasTypeID(item);
                if (basType != null)
                {
                    if (basType.t1)
                    {
                        subData.weaponType.Add(item);
                    }
                    else
                    {
                        subData.magicType.Add(item);
                    }
                }
            }
            //data.ValuesToData(data.values);
            //data.values[0] = CommonTool.ObjectToJson(scaleData).Trim();
            data.SetData(scaleData);
            Console.WriteLine("新的数据：" + school.schoolData.buildSchoolData.values[0]);

            AddLog("执行成功！");
        }

        //修改功法库
        public void FixSchoolUseSkill()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }
            List<string> fixBas = new List<string>(p2.Split(','));
            var subData = school.GetBuildSub<MapBuildSchoolLibrary>().data;
            subData.weaponType.Clear();
            subData.magicType.Clear();
            foreach (var item in fixBas)
            {
                var basType = g.conf.roleEffect.GetBasTypeID(item);
                if (basType != null)
                {
                    if (basType.t1)
                    {
                        subData.weaponType.Add(item);
                    }
                    else
                    {
                        subData.magicType.Add(item);
                    }
                }
            }

            AddLog("执行成功！");
        }

        //修改宗门逆天改命
        public void FixSchoolFate()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int fateId))
            {
                AddLog("执行失败：输入的宗门逆天改命id不合法：" + p2);
                return;
            }

            var fate = g.conf.schoolFate.GetItem(fateId);

            if (fate == null)
            {
                AddLog("执行失败：输入的宗门逆天改名id不存在：" + p2);
                return;
            }

            Console.WriteLine("旧的数据 " + school.buildData.propertyData.fixFateID);
            school.buildData.propertyData.fixFateID = fateId;
            Console.WriteLine("新的数据 " + school.buildData.propertyData.fixFateID);
            AddLog("执行成功！");
        }

        //修改宗门宗旨
        public void FixSchoolTenet()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }

            List<int> values = new List<int>();
            var p = p2.Split(',');
            if (p.Length != 2)
            {
                AddLog("执行失败：输入的宗旨数量必须是2个：" + p2);
                return;
            }
            foreach (var item in p)
            {
                if (!int.TryParse(item, out int value))
                {
                    AddLog("执行失败：输入的id不合法：" + item + "/" + p2);
                    return;
                }

                var conf = g.conf.schoolSlogan.GetItem(value);

                if (conf == null)
                {
                    AddLog("执行失败：输入的id不存在：" + item + "/" + p2);
                    return;
                }
                values.Add(value);
            }

            var fixSchools = school.GetAllSchools(true);
            foreach (var fixSchool in fixSchools)
            {
                fixSchool.buildData.propertyData.fixSloganItem1Type1 = values[0];
                fixSchool.buildData.propertyData.fixSloganItem1Type2 = values[1];
            }
            AddLog("执行成功！");
        }

        //修改宗门门规
        public void FixSchoolGauge()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }

            List<int> values = new List<int>();
            var p = p2.Split(',');
            if (p.Length != 2)
            {
                AddLog("执行失败：输入的门规数量必须是2个：" + p2);
                return;
            }
            foreach (var item in p)
            {
                if (!int.TryParse(item, out int value))
                {
                    AddLog("执行失败：输入的id不合法：" + item + "/" + p2);
                    return;
                }

                var conf = g.conf.schoolSlogan.GetItem(value);

                if (conf == null)
                {
                    AddLog("执行失败：输入的id不存在：" + item + "/" + p2);
                    return;
                }
                values.Add(value);
            }

            var fixSchools = school.GetAllSchools(true);
            foreach (var fixSchool in fixSchools)
            {
                fixSchool.buildData.propertyData.fixSloganItem2Type1 = values[0];
                fixSchool.buildData.propertyData.fixSloganItem2Type2 = values[1];
            }
            AddLog("执行成功！");
        }

        //修改宗门立场
        public void FixSchoolStand()
        {
            var school = g.world.build.GetBuild<MapBuildSchool>(p1);
            if (school == null)
            {
                AddLog("执行失败：无法获取宗门：" + p1);
                return;
            }

            var stand = p2 == "down" ? 2 : 1;

            var fixSchools = school.GetAllSchools(true);
            foreach (var fixSchool in fixSchools)
            {
                fixSchool.buildData.propertyData.fixStand = stand;
                MapBuildSchool.UpdateSchoolRoundDecorate(fixSchool);
            }
            DramaFunction.UpdateMapAllUI();
            AddLog("执行成功！");
        }

        //设置关系
        public void SetRelation()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }

            UnitRelationType relationType = (UnitRelationType)value;
            if (relationType < UnitRelationType.Parent || relationType > UnitRelationType.Student)
            {
                AddLog("执行失败：输入的关系不合法：" + p3);
                return;
            }
            switch (relationType)
            {
                case UnitRelationType.Parent:
                    if (!unit.data.unitData.relationData.children.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.children.Add(unit2.data.unitData.unitID);
                    unit2.data.unitData.relationData.parent[0] = unit.data.unitData.unitID;
                    break;
                case UnitRelationType.Children:
                    if (!unit2.data.unitData.relationData.children.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.children.Add(unit.data.unitData.unitID);
                    unit.data.unitData.relationData.parent[0] = unit2.data.unitData.unitID;
                    break;
                case UnitRelationType.ChildrenPrivate:
                    if (!unit2.data.unitData.relationData.childrenPrivate.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.childrenPrivate.Add(unit.data.unitData.unitID);
                    unit.data.unitData.relationData.parent[0] = unit2.data.unitData.unitID;
                    break;
                case UnitRelationType.Brother:
                    if (!unit.data.unitData.relationData.brother.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.brother.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.brother.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.brother.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.ParentBack:
                    if (!unit.data.unitData.relationData.childrenBack.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.childrenBack.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.parentBack.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.parentBack.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.ChildrenBack:
                    if (!unit.data.unitData.relationData.parentBack.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.parentBack.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.childrenBack.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.childrenBack.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.BrotherBack:
                    if (!unit.data.unitData.relationData.brotherBack.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.brotherBack.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.brotherBack.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.brotherBack.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.Married:
                    unit.data.unitData.relationData.married = unit2.data.unitData.unitID;
                    unit2.data.unitData.relationData.married = unit.data.unitData.unitID;
                    break;
                case UnitRelationType.Lover:
                    if (!unit.data.unitData.relationData.lover.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.lover.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.lover.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.lover.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.Master:
                    if (!unit.data.unitData.relationData.student.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.student.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.master.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.master.Add(unit.data.unitData.unitID);
                    break;
                case UnitRelationType.Student:
                    if (!unit.data.unitData.relationData.master.Contains(unit2.data.unitData.unitID))
                        unit.data.unitData.relationData.master.Add(unit2.data.unitData.unitID);
                    if (!unit2.data.unitData.relationData.student.Contains(unit.data.unitData.unitID))
                        unit2.data.unitData.relationData.student.Add(unit.data.unitData.unitID);
                    break;
            }

            unit.data.UpdateAllUnitRelation();
            unit2.data.UpdateAllUnitRelation();
            AddLog("执行成功！");
        }

        //增加双向亲密度
        public void SetOneLove()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            unit.data.unitData.relationData.AddIntim(unit2.data.unitData.unitID, value);
            unit2.data.unitData.relationData.AddIntim(unit.data.unitData.unitID, value);
            AddLog("执行成功！");
        }

        //增加单向亲密度
        public void SetTwoLove()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            unit.data.unitData.relationData.AddIntim(unit2.data.unitData.unitID, value);
            AddLog("执行成功！");
        }

        //增加双向仇恨度
        public void SetOneHate()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            unit.data.unitData.relationData.AddHate(unit2.data.unitData.unitID, value);
            unit2.data.unitData.relationData.AddHate(unit.data.unitData.unitID, value);
            AddLog("执行成功！");
        }

        //增加单向仇恨度
        public void SetTwoHate()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var unit2 = GetUnit(p2);
            if (unit2 == null)
            {
                AddLog("执行失败：无法获取单位b：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            unit.data.unitData.relationData.AddHate(unit2.data.unitData.unitID, value);
            AddLog("执行成功！");
        }

        //秘籍词条满级
        public void MartialBookMax()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var martial = GetMartial(unit, p2);
            if (martial == null)
            {
                AddLog("执行失败：无法获取单位的秘籍：" + p1 + " " + p2);
                return;
            }

            var data = martial.martialInfo.prefixs;
            int i = 0;
            while (true)
            {
                DataProps.MartialData.Prefix d;
                try
                {
                    d = data[i];
                }
                catch (Exception)
                {
                    break;
                }
                martial.SetPrefixLevel(i, g.conf.battleSkillPrefixValue.GetPrefixMaxLevel(martial, d.prefixValueItem) * 100);
                i++;
            }
            martial.martialInfo.initPrefixs = false;
            AddLog("执行成功！");
        }

        //功法词条满级
        public void MartialSkillMax()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var skill = GetSkill(unit, p2);
            if (skill == null)
            {
                AddLog("执行失败：无法获取单位的功法：" + p1 + " " + p2);
                return;
            }
            var martial = skill.data.To<DataProps.MartialData>();

            var data = martial.martialInfo.prefixs;
            int i = 0;
            while (true)
            {
                DataProps.MartialData.Prefix d;
                try
                {
                    d = data[i];
                }
                catch (Exception)
                {
                    break;
                }
                martial.SetPrefixLevel(i, g.conf.battleSkillPrefixValue.GetPrefixMaxLevel(martial, d.prefixValueItem) * 100);
                i++;
            }
            martial.martialInfo.initPrefixs = false;
            AddLog("执行成功！");
        }

        //学习背包秘籍
        public void MartialBookStudy()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var martial = GetMartial(unit, p2);
            if (martial == null)
            {
                AddLog("执行失败：无法获取单位的秘籍：" + p1 + " " + p2);
                return;
            }
            //UnitActionPropMartialStudy martialStudy = new UnitActionPropMartialStudy(martial.data) { isOneStudy = true, isCost = false };
            //martialStudy.Init(unit);
            //martialStudy.day = 0;
            //var err = unit.CreateAction(martialStudy, true);
            //if (err == 0)
            //{
            //    AddLog("执行成功！");
            //}
            //else
            //{
            //    AddLog("执行失败！" + err);
            //}

            StudyAndEquip(unit, martial.data);

        }

        //遗忘已学功法
        public void DelSkill()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var skill = GetSkill(unit, p2);
            if (skill == null)
            {
                AddLog("执行失败：无法获取单位的功法：" + p1 + " " + p2);
                return;
            }
            var martial = skill.data.To<DataProps.MartialData>();
            MartialType martialType = martial.martialType;
            var soleID = skill.data.soleID;
            if (martialType == MartialType.Ability)
            {
                for (int i = 0; i < unit.data.unitData.abilitys.Length; i++)
                {
                    if (soleID == unit.data.unitData.abilitys[i])
                    {
                        unit.CreateAction(new UnitActionMartialUnequip(MartialType.Ability, i), true);
                    }
                }
            }
            if (soleID == unit.data.unitData.skillLeft)
            {
                unit.CreateAction(new UnitActionMartialUnequip(MartialType.SkillLeft, 0), true);
            }
            if (soleID == unit.data.unitData.skillRight)
            {
                unit.CreateAction(new UnitActionMartialUnequip(MartialType.SkillRight, 0), true);
            }
            if (soleID == unit.data.unitData.step)
            {
                unit.CreateAction(new UnitActionMartialUnequip(MartialType.Step, 0), true);
            }
            if (soleID == unit.data.unitData.ultimate)
            {
                unit.CreateAction(new UnitActionMartialUnequip(MartialType.Ultimate, 0), true);
            }
            var err = unit.CreateAction(new UnitActionMartialDel(skill), true);

            if (err == 0)
            {
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        //遗忘已学功法
        public void DelAllSkill()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            List<DataUnit.ActionMartialData> list = new List<DataUnit.ActionMartialData>();
            var allActionMartial = unit.data.unitData.allActionMartial;

            foreach (var item in allActionMartial)
            {
                list.Add(item.value);
            }

            foreach (var skill in list)
            {
                var martial = skill.data.To<DataProps.MartialData>();
                MartialType martialType = martial.martialType;
                var soleID = skill.data.soleID;
                if (martialType == MartialType.Ability)
                {
                    for (int i = 0; i < unit.data.unitData.abilitys.Length; i++)
                    {
                        if (soleID == unit.data.unitData.abilitys[i])
                        {
                            unit.CreateAction(new UnitActionMartialUnequip(MartialType.Ability, i), true);
                        }
                    }
                }
                if (soleID == unit.data.unitData.skillLeft)
                {
                    unit.CreateAction(new UnitActionMartialUnequip(MartialType.SkillLeft, 0), true);
                }
                if (soleID == unit.data.unitData.skillRight)
                {
                    unit.CreateAction(new UnitActionMartialUnequip(MartialType.SkillRight, 0), true);
                }
                if (soleID == unit.data.unitData.step)
                {
                    unit.CreateAction(new UnitActionMartialUnequip(MartialType.Step, 0), true);
                }
                if (soleID == unit.data.unitData.ultimate)
                {
                    unit.CreateAction(new UnitActionMartialUnequip(MartialType.Ultimate, 0), true);
                }
                var err = unit.CreateAction(new UnitActionMartialDel(skill), true);

            }

            AddLog("执行成功！");
        }

        // 丢弃所有背包道具
        public void DelAllItem()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!float.TryParse(p2, out float value))
            {
                value = 99;
            }
            var list = unit.data.unitData.propData._allShowProps.ToArray();
            for (int i = 0; i < list.Count; i++)
            {
                var propData = list[i];
                if (propData.propsInfoBase.level > value)
                        continue;
                unit.data.CostPropItem(propData.soleID, propData.propsCount);
            }
        }

        //修炼功法满级
        public void UpSkill()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var skill = GetSkill(unit, p2);
            if (skill == null)
            {
                AddLog("执行失败：无法获取单位的功法：" + p1 + " " + p2);
                return;
            }
            var soleID = skill.data.soleID;
            unit.data.unitData.AddMartialExp(soleID, 100000000);
            AddLog("执行成功！");
        }

        //游戏加速
        public void GameSpeed()
        {
            if (!float.TryParse(p1, out float value))
            {
                AddLog("执行失败：输入的数字不合法：" + p1);
                return;
            }

            ModMain.FixGameSpeed(value);
            AddLog("执行成功！");
        }

        //增加背包大小
        public void BagSize()
        {
            if (!int.TryParse(p1, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p1);
                return;
            }

            g.world.playerUnit.data.dynUnitData.propGridNum.AddValue((Func<int>)(() => { return value; }));
            AddLog("执行成功！");
        }

        //增加器灵
        public void AddSprite()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.artifactSprite.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }
            unit.data.unitData.artifactSpriteData.AddSprite(id);
            AddLog("执行成功！");
        }

        //删除器灵
        public void DelSprite()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.artifactSprite.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }
            for (int i = 0; i < unit.data.unitData.artifactSpriteData.sprites.Count; i++)
            {
                var sprite = unit.data.unitData.artifactSpriteData.sprites[i];
                if (sprite.spriteID == id)
                {
                    unit.data.unitData.artifactSpriteData.DelSpriteInSoleID(sprite.soleID);
                }
            }
            AddLog("执行成功！");

        }

        //解锁器灵天赋盘
        public void LuckSpriteSkill()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.artifactSprite.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }
            var qilings = unit.data.unitData.artifactSpriteData.GetSpriteInSpriteID(id);
            if (qilings.Count > 0)
            {
                var qiling = qilings[0];
                var items = g.conf.artifactSpriteTalent.GetItemsInSpriteID(id);
                foreach (var item in items)
                {
                    if (item.isShow == 0)
                    {
                        continue;
                    }
                    unit.data.unitData.artifactSpriteData.ActiveTanlet(qiling.soleID, item.number);
                }
                // 如果有融合器灵的法宝则刷新法宝属性
                var propList = unit.data.unitData.propData.CloneAllProps();
                DataProps.PropsData shapeProp = propList.Find(
                    (Func<DataProps.PropsData, bool>)((v) =>
                    g.conf.artifactShape.GetItem(v.propsID) != null && v.To<DataProps.PropsArtifact>().spriteSoleID == qiling.soleID));
                if (shapeProp != null)
                {
                    WorldUnitEquipProps.UpdatePlayerArtifactPropEffect(unit, shapeProp.soleID);
                }
                AddLog("执行成功！");
                return;
            }
            AddLog("执行失败！");
        }

        //增加器灵亲密度
        public void AddSpriteLove()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.artifactSprite.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            for (int i = 0; i < unit.data.unitData.artifactSpriteData.sprites.Count; i++)
            {
                var sprite = unit.data.unitData.artifactSpriteData.sprites[i];
                if (sprite.spriteID == id)
                {
                    unit.data.unitData.artifactSpriteData.sprites[i].intimacy =
                        Mathf.Clamp(value, g.conf.artifactSpriteClose.initClose, g.conf.artifactSpriteClose.closeMax);

                }
            }
            AddLog("执行成功！");
        }

        //解锁昊天眼技能
        public void LuckEyeSkill()
        {
            if (!int.TryParse(p1, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p1);
                return;
            }
            DataWorld.World.GodEyeData data = g.data.world.godEyeData;

            int skillID = value;
            if (!data.skillsID.Contains(skillID))
                data.skillsID.Add(skillID);
            var conf = g.conf.godEyeSkills.GetItem(skillID);
            if (!data.bossGrade.ContainsKey(conf.bossID))
            {
                data.bossGrade.Add(conf.bossID, 1);
            }
            AddLog("执行成功！");
        }

        //设置心法套装
        public void SetSuit()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            var skill = GetAbility(unit, p2);
            if (skill == null)
            {
                AddLog("执行失败：无法获取单位的功法：" + p1 + " " + p2);
                return;
            }
            var martial = skill.data.To<DataProps.MartialData>();
            MartialType martialType = martial.martialType;
            var soleID = skill.data.soleID;

            if (!int.TryParse(p3, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p3);
                return;
            }

            var conf = g.conf.battleAbilitySuitBase.GetItem(id);

            if (conf == null && id != 0)
            {
                AddLog("执行失败：输入的id不存在：" + p3);
                return;
            }

            if (conf.suitMember.Split('|').Contains(martial.baseID.ToString()))
            {
                martial.data.To<DataProps.PropsAbilityData>().suitID = id;
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败：该功法与设置的套装不匹配  功法ID=" + martial.baseID + "  套装ID=" + id + "  该套装支持功法=" + conf.suitMember);
            }

        }

        //解锁所有仙法
        public void UnlockImmortalSkill()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.immortalHuman.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            List<int> skillIds = new List<int>();
            for (int i = 0; i < g.conf.immortalHuman._allConfList.Count; i++)
            {
                if (g.conf.immortalHuman.allConfList[i].id == id)
                {
                    var activeSkillsID = g.conf.immortalHuman.allConfList[i].activeSkills.Split('|');
                    for (int j = 0; j < activeSkillsID.Length; j++)
                    {
                        int skillid = int.Parse(activeSkillsID[j]);
                        unit.data.unitData.immortalCard.AddImmortalCard(skillid, g.conf.immortalHuman.allConfList[i].id);
                        skillIds.Add(skillid);
                    }

                    var passiveSkillsID = g.conf.immortalHuman.allConfList[i].passiveSkills.Split('|');
                    for (int j = 0; j < passiveSkillsID.Length; j++)
                    {
                        int skillid = int.Parse(passiveSkillsID[j]);
                        unit.data.unitData.immortalCard.AddImmortalCard(skillid, g.conf.immortalHuman.allConfList[i].id);
                        skillIds.Add(skillid);
                    }
                }
            }
            UIGetImmortalSkill.OpenAddImmortalCardUI(unit, skillIds.ToArray());
            AddLog("执行成功！");
        }

        //获得道具
        public void AddItem()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.itemProps.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                AddLog("执行失败：输入的数字不合法：" + p3);
                return;
            }
            AddItemId(unit, id, value);
            AddLog("执行成功！");
        }

        private void AddItemId(WorldUnitBase unit, int id, int value)
        {
            List<int> propIds = new List<int> { 2823001, 2823002, 2823003, 2823004 };
            if (propIds.Contains(id))
            {
                if (value > 0)
                {
                    var props = g.world.devilDemonMgr.dataProps.AddProps(id, value);
                    if (props.Count > 0)
                    {
                        ModMain.lastAddProp = new DataStruct<WorldUnitBase, DataProps.PropsData>(unit, props[0]);
                    }
                }
                else
                {
                    g.world.devilDemonMgr.dataProps.DelProps(id, value);
                }
            }
            else
            {
                if (value > 0)
                {
                    Il2CppSystem.Collections.Generic.List<GameItemRewardData> rewards = new Il2CppSystem.Collections.Generic.List<GameItemRewardData>();
                    rewards.Add(new GameItemRewardData(1, new int[] { id }, value));
                    WorldUnitData.RewardItemData reward = unit.data.RewardItem(rewards);

                    var props = reward.succeedPropsDatas;
                    foreach (DataProps.PropsData data in props)
                    {
                        var item = unit.data.unitData.propData.GetProps(data.soleID);
                        Console.WriteLine("添加道具：" + data.soleID + " >> " + item + " " + data.propsID);
                        if (item == null)
                            continue;
                        ModMain.lastAddProp = new DataStruct<WorldUnitBase, DataProps.PropsData>(unit, item);

                        if (item.propsID == 5081996) // 化神之气
                        {
                            DataProps.PropsElderData Prop = item.To<DataProps.PropsElderData>();
                            int rCount = UnityEngine.Random.Range(g.conf.npcGrade7.pieceMin, g.conf.npcGrade7.pieceMax + 1);
                            for (int i = 0; i < rCount; i++)
                            {
                                DataProps.PropsElderData elDerProps2 = DataProps.PropsData.NewProps(5082005, 1).To<DataProps.PropsElderData>();
                                Prop.atk = Prop.atk + elDerProps2.atk;
                                Prop.def = Prop.def + elDerProps2.def;
                                Prop.hpMax = Prop.hpMax + elDerProps2.hpMax;
                                Prop.mpMax = Prop.mpMax + elDerProps2.mpMax;
                                Prop.spMax = Prop.spMax + elDerProps2.spMax;
                            }
                            ModMain.lastAddElder = new DataStruct<WorldUnitBase, DataProps.PropsData>(unit, item);
                        }
                        if (item.propsID == 5082066) // 道魂
                        {
                            Func<int> getRand = () =>
                            {
                                var result = UnityEngine.Random.Range(1, 12);
                                if (result > 6)
                                {
                                    result += 4;
                                }
                                return result;
                            };

                            int mainType = getRand();
                            int viceType1 = 0, viceType2 = 0;
                            do
                            {
                                viceType1 = getRand();
                            } while (viceType1 == mainType);
                            do
                            {
                                viceType2 = getRand();
                            } while (viceType1 == viceType2 || viceType2 == mainType);
                            int purity = 0;
                            if (UnityEngine.Random.Range(0, 100) < 90)
                            {
                                purity = UnityEngine.Random.Range(85, 101);
                            }
                            if (UnityEngine.Random.Range(0, 100) < 50)
                            {
                                purity = UnityEngine.Random.Range(50, 101);
                            }
                            else
                            {
                                purity = UnityEngine.Random.Range(0, 101);
                            }
                            List<int> values = new List<int>(item.values);
                            values[3] = mainType;
                            values[4] = viceType1;
                            values[5] = viceType2;
                            values[6] = purity;
                            item.SetValues(values.ToArray());
                            ModMain.lastAddRule = new DataStruct<WorldUnitBase, DataProps.PropsData>(unit, item);
                        }
                        if (!(item.propsType != DataProps.PropsDataType.Props || item.propsItem.className != 401))
                        {
                            ModMain.lastAddArtifact = new DataStruct<WorldUnitBase, DataProps.PropsData>(unit, item);
                        }
                        data.SetValues(item.values);
                        Console.WriteLine("道具数据 " + string.Join(",", data.values));
                    }
                }
                else
                {
                    unit.data.CostPropItem(id, -value);
                }
            }
        }

        //获得秘籍
        public void AddMartial()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            string[] list = p2.Split(',');
            if (list.Length != 2)
            {
                AddLog("执行失败：输入的秘籍不合法：" + p2);
                return;
            }
            if (!int.TryParse(list[1], out int skillId))
            {
                AddLog("执行失败：输入的秘籍id不合法：" + p2);
                return;
            }
            if (!int.TryParse(list[0], out int mType))
            {
                AddLog("执行失败：输入的秘籍类型不合法：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                value = unit.data.dynUnitData.GetGrade();
            }
            else
            {
                if (value < 0 && value > 10)
                {
                    AddLog("执行失败：输入的境界不在1-10之间：" + p2);
                    return;
                }
            }

            MartialType martialType = (MartialType)(mType);
            if (martialType == MartialType.SkillLeft)
            {
                if (g.conf.battleSkillAttack.GetItem(skillId) == null)
                {
                    AddLog("执行失败：无法获取该武技/灵技秘籍：" + p2);
                    return;
                }
            }
            else if (martialType == MartialType.SkillRight)
            {
                if (g.conf.battleSkillAttack.GetItem(skillId) == null)
                {
                    AddLog("执行失败：无法获取该绝技秘籍：" + p2);
                    return;
                }
            }
            else if (martialType == MartialType.Ultimate)
            {
                if (g.conf.battleSkillAttack.GetItem(skillId) == null)
                {
                    AddLog("执行失败：无法获取该神通秘籍：" + p2);
                    return;
                }
            }
            else if (martialType == MartialType.Step)
            {
                if (g.conf.battleStepBase.GetItem(skillId) == null)
                {
                    AddLog("执行失败：无法获取该心法秘籍：" + p2);
                    return;
                }
            }
            else if (martialType == MartialType.Ability)
            {
                if (g.conf.battleAbilityBase.GetItem(skillId) == null)
                {
                    AddLog("执行失败：无法获取该心法秘籍：" + p2);
                    return;
                }
            }
            else
            {
                AddLog("执行失败：无法获取该类型秘籍：" + p2);
                return;
            }

            RandomGetRewardData rewardData = new RandomGetRewardData(martialType, 6, value);
            rewardData.baseID = skillId;
            WorldUnitData.RewardItemData rewardItemData = unit.data.RewardItem(RewardTool.GetReward(rewardData), false);
            DataProps.MartialData martialData = unit.data.unitData.propData.GetProps(rewardItemData.succeedPropsDatas[0].soleID).To<DataProps.MartialData>();
            if (martialData != null)
            {
                ModMain.lastAddMartial = new DataStruct<WorldUnitBase, DataProps.MartialData>(unit, martialData);
                Console.WriteLine("设置上次添加的秘籍 " + martialData.data.soleID + " >> " + ModMain.lastAddMartial.t2);
                if (p4 == "true")
                {
                    StudyAndEquip(unit, martialData.data);
                }
                AddLog("执行成功！");
            }
            else
            {
                AddLog("执行失败！");
            }


        }

        //获得套装
        public void AddSuit()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.battleAbilitySuitBase.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }

            if (!int.TryParse(p3, out int value))
            {
                value = unit.data.dynUnitData.GetGrade();
            }
            else
            {
                if (value < 0 && value > 10)
                {
                    AddLog("执行失败：输入的境界不在1-10之间：" + p2);
                    return;
                }
            }

            ConfBattleAbilitySuitBaseItem suitItem = g.conf.battleAbilitySuitBase.GetItem(id);
            var list = suitItem.suitMember.Split('|');
            List<DataProps.PropsData> props = new List<DataProps.PropsData>();
            int ok = 0;
            for (int i = 0; i < list.Length; i++)
            {
                int baseID = int.Parse(list[i]);
                int grade = unit.data.dynUnitData.GetGrade();
                string unitID = unit.data.unitData.unitID;
                RandomGetRewardData rewardData = new RandomGetRewardData(MartialType.Ability, 6, grade);
                rewardData.baseID = baseID;
                WorldUnitData.RewardItemData rewardItemData = unit.data.RewardItem(RewardTool.GetReward(rewardData), false);
                DataProps.PropsData prop = unit.data.unitData.propData.GetProps(rewardItemData.succeedPropsDatas[0].soleID);
                if (prop != null)
                {
                    prop.To<DataProps.PropsAbilityData>().suitID = id;
                    if (p4 == "true")
                    {
                        StudyAndEquip(unit, prop);
                    }
                    ok++;
                }
            }
            if (ok == list.Length)
            {
                AddLog("执行成功！");
            }
            else if (ok == 0)
            {
                AddLog("执行失败！");
            }
            else
            {
                AddLog("部分执行成功 " + ok + "/" + list.Length);
            }
        }

        //修改秘籍词条
        public void FixMartialPrefix()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var martial = GetMartial(unit, p2);
            if (martial == null)
            {
                AddLog("执行失败：无法获取单位的秘籍：" + p1 + " " + p2);
                return;
            }

            var list = p3.Split(',');
            List<int> prefix = new List<int>();
            foreach (var item in list)
            {
                if (!int.TryParse(item, out int value))
                {
                    AddLog("执行失败：输入的词条不合法：" + item + "/" + p3);
                    return;
                }
                prefix.Add(value);
            }

            FixPrefix(martial, prefix);
            AddLog("执行成功！");
        }

        //修改功法词条
        public void FixSkillPrefix()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var skill = GetSkill(unit, p2);
            if (skill == null)
            {
                AddLog("执行失败：无法获取单位的功法：" + p1 + " " + p2);
                return;
            }
            var martial = skill.data.To<DataProps.MartialData>();

            var list = p3.Split(',');
            List<int> prefix = new List<int>();
            foreach (var item in list)
            {
                if (!int.TryParse(item, out int value))
                {
                    AddLog("执行失败：输入的词条不合法：" + item + "/" + p3);
                    return;
                }
                prefix.Add(value);
            }

            FixPrefix(martial, prefix);
            AddLog("执行成功！");

        }
        private void FixPrefixMaxLevel(DataProps.MartialData martialData)
        {
            DataProps.PropsData prop = martialData.data;
            // 词条数据
            var values = prop.values;
            Console.WriteLine("旧的词条数据 " + string.Join(", ", values));
            List<int> data = new List<int>();
            bool isLevel = false;
            for (int i = 0; i < values.Length; i++)
            {
                if (isLevel)
                {
                    data.Add(600);
                }
                else
                {
                    data.Add(values[i]);
                }
                if (values[i] == -1)
                {
                    isLevel = true;
                }
            }
            Console.WriteLine("新的词条数据 " + string.Join(", ", data));
            prop.SetValues(data.ToArray());
            martialData.martialInfo.initPrefixs = false;
        }
        private void FixPrefix(DataProps.MartialData martialData, List<int> prefix)
        {
            Console.WriteLine("修改词条 " + string.Join(", ", prefix));
            DataProps.PropsData prop = martialData.data;
            // 词条数据
            var values = prop.values;
            Console.WriteLine("旧的词条数据 " + string.Join(", ", values));
            List<int> data = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                data.Add(values[i]);
            }
            for (int i = 0; i < prefix.Count; i++)
            {
                data.Add(prefix[i]);
            }
            data.Add(-1);
            for (int i = 0; i < prefix.Count; i++)
            {
                data.Add(600);
            }
            Console.WriteLine("新的词条数据 " + string.Join(", ", data));
            prop.SetValues(data.ToArray());
            martialData.martialInfo.initPrefixs = false;
        }

        private void StudyAndEquip(WorldUnitBase unit, DataProps.PropsData propsData)
        {
            DataProps.MartialData martialData = propsData.To<DataProps.MartialData>();

            //补充属性
            string basType = martialData.martialInfo.basType;

            if (basType != "")
            {
                int basRequire = martialData.martialInfo.basRequire;
                basRequire = Mathf.Clamp(basRequire, 0, g.conf.roleAttributeLimit.basisBladeMax);
                var basValues = unit.data.dynUnitData.GetValues(basType);

                if (basType == "basAllAny")
                {
                    int curV = CommonTool.GetItemsAdding(basValues, (Func<DynInt, int>)((v) => { return v.value; })) / 12;
                    int addV = (basRequire - curV);

                    if (addV > 0)
                    {
                        int i = 0;
                        while (true)
                        {
                            try
                            {
                                basValues[i++].baseValue += addV;
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int i = 0;
                    while (true)
                    {
                        DynInt basValue;
                        try
                        {
                            basValue = basValues[i];
                        }
                        catch (Exception)
                        {
                            break;
                        }
                        if (basValue.value < basRequire)
                        {
                            basValue.baseValue += (basRequire - basValue.value) + CommonTool.Random(0, 10);
                        }
                        i++;
                    }
                }
            }

            //补充道点
            if (martialData.martialType == MartialType.Ability)
            {
                int abilityPoint = martialData.martialInfo.abilityPoint;
                if ((abilityPoint + unit.data.dynUnitData.GetCurUseAbilityPoint()) > unit.data.dynUnitData.abilityPoint.value)
                {
                    unit.data.dynUnitData.abilityPoint.baseValue += (abilityPoint + unit.data.dynUnitData.GetCurUseAbilityPoint()) - unit.data.dynUnitData.abilityPoint.value + UnityEngine.Random.Range(0, 10);
                }
            }

            var study = new UnitActionPropMartialStudy(martialData.data);
            study.isCost = false;
            study.isOneStudy = true;
            var err = unit.CreateAction(study);
            if (err != 0)
            {
                Console.WriteLine("自动学习失败 " + err + $"  day={study.day} isCost={study.isCost} money={study.money} costMood={study.costMood}");
            }
            else
            {
                Console.WriteLine("自动学习成功 " + err + $"  day={study.day} isCost={study.isCost} money={study.money} costMood={study.costMood}");

                ModMain.lastStudySkill = new DataStruct<WorldUnitBase, DataUnit.ActionMartialData>
                    (unit, unit.data.unitData.GetActionMartial(martialData.data.soleID));
                Console.WriteLine("设置上次学习的功法 " + martialData.data.soleID + " >> " + ModMain.lastStudySkill.t2);
                if (martialData.martialType == MartialType.Ability)
                {
                    ModMain.lastStudyAbility = new DataStruct<WorldUnitBase, DataUnit.ActionMartialData>
                       (unit, unit.data.unitData.GetActionMartial(martialData.data.soleID));
                }
            }
            DataUnit.ActionMartialData actionMartialData = unit.data.unitData.GetActionMartial(martialData.data.soleID);
            if (actionMartialData != null)
            {
                if (unit.data.unitData.heart.IsHeroes())
                {
                    WorldUnitAIAction1035.RandomUnlockPrefix(unit, actionMartialData.data.To<DataProps.MartialData>());
                }

                //取消装备中的心法
                int index = 0;
                if (martialData.martialType == MartialType.Ability)
                {
                    index = UnitActionMartialUnequip.GetAbilityIndexInType(unit, martialData);
                    if (index != -1)
                    {
                        unit.CreateAction(new UnitActionMartialUnequip(MartialType.Ability, index));
                    }
                    else
                    {
                        index = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            try
                            {
                                if (string.IsNullOrWhiteSpace(unit.data.unitData.abilitys[i]))
                                {
                                    index = i;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                else
                {
                    unit.CreateAction(new UnitActionMartialUnequip(null) { type = martialData.martialType });
                }

                var err2 = unit.CreateAction(new UnitActionMartialEquip(actionMartialData, index));
                unit.data.unitData.AddMartialExp(actionMartialData.data.soleID, 10000000);
                if (err2 != 0)
                {
                    Console.WriteLine("自动装备失败 " + err2);
                }
            }
        }

        public void OpenMapLight()
        {
            GridBaseUI.debugView = true;
            DramaFunction.UpdateMapAllUI();
            AddLog("执行成功！");
        }
        public void CloseMapLight()
        {
            GridBaseUI.debugView = false;
            DramaFunction.UpdateMapAllUI();
            AddLog("执行成功！");
        }

        // 打开地图
        public void OpenMap()
        {
            for (int x = 0; x < g.data.grid.mapWidth; x++)
            {
                for (int y = 0; y < g.data.grid.mapHeight; y++)
                {
                    var p = new Vector2Int(x, y);
                    g.data.map.AddOpenGrid(p);
                }
            }
            DramaFunction.UpdateMapAllUI();
            AddLog("执行成功！");
        }

        // 关闭地图
        public void CloseMap()
        {
            for (int x = 0; x < g.data.grid.mapWidth; x++)
            {
                for (int y = 0; y < g.data.grid.mapHeight; y++)
                {
                    var p = new Vector2Int(x, y);
                    g.data.map.DelOpenGrid(p);
                }
            }
            DramaFunction.UpdateMapAllUI();
            AddLog("执行成功！");
        }

        // 移动角色
        public void MoveChar()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!int.TryParse(p2, out int valueX))
            {
                AddLog("执行失败：输入的X坐标不合法：" + p2);
                return;
            }
            if (!int.TryParse(p3, out int valueY))
            {
                AddLog("执行失败：输入的Y坐标不合法：" + p3);
                return;
            }

            var err = unit.CreateAction(new UnitActionSetPoint(new Vector2Int(valueX, valueY)));
            if (err == 0)
            {

                AddLog("执行成功！");
                DramaFunction.UpdateMapAllUI();
            }
            else
            {
                AddLog("执行失败！" + err);
            }
        }

        // 添加壶妖
        public void AddPotmon()
        {

            if (!int.TryParse(p1, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p1);
                return;
            }

            var conf = g.conf.potmonBase.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p1);
                return;
            }

            if (!int.TryParse(p2, out int value))
            {
                value = g.world.playerUnit.data.dynUnitData.GetGrade();
            }
            else
            {
                if (value < 0 && value > 10)
                {
                    AddLog("执行失败：输入的境界不在1-10之间：" + p2);
                    return;
                }
            }
            try
            {
                var list = g.world.devilDemonMgr.potMonMgr.potMonDatas;
                PotMonMgr.PotMonData potmon = g.world.devilDemonMgr.potMonMgr.AddPotMon(id, value);
                var ui = g.ui.GetUI<UIPotmonList>(UIType.PotmonList);
                if (ui != null)
                {
                    ui.UpdateUI();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败！");
                return;
            }
            AddLog("执行成功！");
        }

        // 修复神器
        public void FixShenqi()
        {
            try
            {
                if (g.data.world.resurgeCount < g.conf.gameDifficultyValue.curItem.fishJade){
                    g.data.world.resurgeCount = g.conf.gameDifficultyValue.curItem.fishJade;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString()); ;
            }
            try
            {
                if (g.data.world.godEyeData.isDamage && g.data.world.gameLevel != GameLevelType.CrueltyShura)
                {
                    g.data.world.godEyeData.Repair();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                if (g.world.devilDemonMgr.devilDemonData.isDamage)
                {
                    g.world.devilDemonMgr.devilDemonData.isDamage = false;
                    g.world.devilDemonMgr.devilDemonData.repairMonth = 0;
                    g.world.devilDemonMgr.devilDemonData.brokenCount = 0;
                    var list = g.world.devilDemonMgr.potMonMgr.potMonDatas;
                    foreach (var item in list)
                    {
                        if (item.healthState != PotMonHealthState.Normal)
                        {
                            item.SetHealthState(PotMonHealthState.Normal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            AddLog("执行成功！");
        }

        public void FixElder()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var propData = GetElder(unit, p2);
            if (unit == null)
            {
                AddLog("执行失败：无法获取化神之气：" + p2);
                return;
            }
            try
            {
                var attr = p3.Split(',');
                int.TryParse(attr[0], out int atk);
                int.TryParse(attr[1], out int def);
                int.TryParse(attr[2], out int hpMax);
                int.TryParse(attr[3], out int mpMax);
                int.TryParse(attr[4], out int spMax);

                var data = propData.To<DataProps.PropsElderData>();
                data.atk = atk;
                data.def = def;
                data.hpMax = hpMax;
                data.mpMax = mpMax;
                data.spMax = spMax;
                propData.initValuesHashIndex = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败！");
                return;
            }
            AddLog("执行成功！");

        }

        public void FixRule()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var propData = GetRule(unit, p2);
            if (unit == null)
            {
                AddLog("执行失败：无法获取道魂：" + p2);
                return;
            }
            var dddd = new Dictionary<string, int>() { { "basBlade", 1 }, { "basSpear", 2 }, { "basSword", 3 }, { "basFist", 4 }, { "basPalm", 5 }, { "basFinger", 6 },
                { "basFire", 11 },{ "basFroze", 12 },{ "basThunder", 13 },{ "basWind", 14 },{ "basEarth", 15 },{ "basWood", 16 }};
            int[] list = new int[3];

            try
            {
                var data = p3.Split(',');
                list[0] = dddd[data[0]];
                try
                {
                    list[1] = dddd[data[1]];
                    list[2] = dddd[data[2]];
                }
                catch (Exception)
                {
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败：需要输入正确的属性：" + p3);
                return;
            }

            int.TryParse(p4, out int value);
            value = Mathf.Clamp(value, 0, 100);

            try
            {
                Console.WriteLine("jiu的数据 " + string.Join(",", propData.values));
                var data = propData.To<DataProps.PropsRuleData>();
                data.mainType = list[0];
                data.viceType1 = list[1];
                data.viceType2 = list[2];
                data.purity = value;
                propData.initValuesHashIndex = false;
                Console.WriteLine("新的数据 " + string.Join(",", propData.values));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败！");
                return;
            }

            AddLog("执行成功！");
        }

        public void FixArtifact()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var propData = GetArtifact(unit, p2);
            if (unit == null)
            {
                AddLog("执行失败：无法获取法宝：" + p2);
                return;
            }

            try
            {
                var data = p3.Split(',');
                var oldValues = propData.values;
                Console.WriteLine("旧的数据 "+string.Join(",", propData.values));
                List<int> values = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    values.Add(oldValues[i]);
                }
                for (int i = 3; i < 11; i++)
                {
                    if (int.TryParse(data[i - 3], out int value))
                    {
                        values.Add(value);
                    }
                    else
                    {
                        values.Add(oldValues[i]);
                    }
                }
                values.Add(oldValues[11]);
                int count = data.Length - 8;
                int addCount = 0;
                for (int i = 0; i < count; i++)
                {
                    if (int.TryParse(data[i + 8], out int value))
                    {
                        values.Add(value);
                        addCount++;
                    }
                }
                values.Add(-1);
                for (int i = 0; i < addCount; i++)
                {
                    values.Add(600);
                }
                propData.SetValues(values.ToArray());
                Console.WriteLine("新的数据 " + string.Join(",", propData.values));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败：" + p3);
                return;
            }

            AddLog("执行成功！");
        }

        public void FixShenhun()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }
            unit.data.unitData.elderData.id = id;
            AddLog("执行成功");
        }

        public void FixDaoJie()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            var dddd = new Dictionary<string, int>() { { "basBlade", 1 }, { "basSpear", 2 }, { "basSword", 3 }, { "basFist", 4 }, { "basPalm", 5 }, { "basFinger", 6 },
                { "basFire", 11 },{ "basFroze", 12 },{ "basThunder", 13 },{ "basWind", 14 },{ "basEarth", 15 },{ "basWood", 16 }};
            List<int> list = new List<int>();

            try
            {
                var data = p2.Split(',');
                for (int i = 0; i < 9; i++)
                {
                    list.Add(dddd[data[i% data.Length]]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败：需要输入正确的属性：" + p2);
                return;
            }

            unit.data.unitData.fieldSkillData.mainType1 = list[0];
            unit.data.unitData.fieldSkillData.mainType2 = list[1];
            unit.data.unitData.fieldSkillData.mainType3 = list[2];
            unit.data.unitData.fieldSkillData.viceType1A = list[3];
            unit.data.unitData.fieldSkillData.viceType1B = list[4];
            unit.data.unitData.fieldSkillData.viceType2A = list[5];
            unit.data.unitData.fieldSkillData.viceType2B = list[6];
            unit.data.unitData.fieldSkillData.viceType3A = list[7];
            unit.data.unitData.fieldSkillData.viceType3B = list[8];
            AddLog("执行成功");
        }

        public void FixLiaoji()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!int.TryParse(p2, out int id))
            {
                AddLog("执行失败：输入的id不合法：" + p2);
                return;
            }

            var conf = g.conf.wingmanBase.GetItem(id);

            if (conf == null)
            {
                AddLog("执行失败：输入的id不存在：" + p2);
                return;
            }
            List<int> wingmanEffectList = new List<int>();
            List<int> wingmanPrefixList = new List<int>();
            List<int> wingmanPrefixUp = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(p3))
                {
                    var data = p3.Split(',');
                    foreach (var item in data)
                    {
                        int id2 = int.Parse(item.Substring(1, item.Length - 1));
                        if (item.StartsWith("e"))
                        {
                            if (g.conf.wingmanEffect.GetItem(id2) == null)
                            {
                                AddLog("执行失败：无法获取效果：" + p3);
                                return;
                            }
                            wingmanEffectList.Add(id2);
                        }
                        if (item.StartsWith("f"))
                        {
                            if (g.conf.wingmanFixValue.GetItem(id2) == null)
                            {
                                AddLog("执行失败：无法获取词条：" + p3);
                                return;
                            }
                            wingmanPrefixList.Add(id2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                if (!string.IsNullOrEmpty(p4))
                {
                    var data = p4.Split(',');
                    foreach (var item in data)
                    {
                        int id2 = int.Parse(item);
                        if (g.conf.wingmanFixValue.GetItem(id2) == null)
                        {
                            AddLog("执行失败：无法获取升级：" + p4);
                            return;
                        }
                        wingmanPrefixUp.Add(id2);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine(string.Join(",", wingmanEffectList));
            Console.WriteLine(string.Join(",", wingmanPrefixList));
            Console.WriteLine(string.Join(",", wingmanPrefixUp));
            unit.data.unitData.reborn.wingManID = id;
            unit.data.unitData.reborn.wingmanEffectList.Clear();
            foreach (var item in wingmanEffectList)
            {
                unit.data.unitData.reborn.wingmanEffectList.Add(item);
            }
            unit.data.unitData.reborn.wingmanPrefixList.Clear();
            foreach (var item in wingmanPrefixList)
            {
                unit.data.unitData.reborn.wingmanPrefixList.Add(item);
            }
            unit.data.unitData.reborn.wingmanPrefixUp.Clear();
            foreach (var item in wingmanPrefixUp)
            {
                unit.data.unitData.reborn.wingmanPrefixUp.Add(item);
            }


            AddLog("执行成功");
        }

        public void AddUpGradeProps()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!int.TryParse(p2, out int value))
            {
                value = 0;
            }

            for (int grade = 1; grade <= 10; grade++)
            {
                if (value == 0 || value == grade)
                {
                    if (grade == 1)
                    {
                        continue;
                    }
                    if (grade == 7)
                    {
                        int[] items = new int[] { 1011945,1011955,5082016,5082026,5082036,5082046,5082056, 5081996 };
                        foreach (var item in items)
                        {
                            AddItemId(unit, item, 1);
                        }
                        continue;
                    }
                    if (grade == 8)
                    {
                        int[] items = new int[] { 1011965,1011975,1011605,1011606};
                        foreach (var item in items)
                        {
                            AddItemId(unit, item, 1);
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            AddItemId(unit, 5082066, 1);
                        }
                        continue;
                    }

                    AddPropForGrade(grade);
                    if (grade == 10)
                    {
                        AddPropForGrade(1);
                    }
                }
            }

            AddLog("执行成功");
        }

        private static void AddPropForGrade(int grade)
        {
            ConfRoleGradeItem upGradeItem = g.conf.roleGrade.GetGradeUpQuality(grade)[g.conf.roleGrade.GetGradeUpQuality(grade).Count - 1];

            int[] itemShowA = CommonTool.StrSplitInt(g.conf.roleGrade.GetGradeItem(grade - 1, 2).itemShowA, '_');
            for (int i = 0; i < itemShowA.Length; i++)
            {
                g.world.playerUnit.data.RewardPropItem(DataProps.PropsData.NewProps(itemShowA[i], 1));
            }
            itemShowA = CommonTool.StrSplitInt(g.conf.roleGrade.GetGradeItem(grade - 1, 3).itemShowA, '_');
            for (int i = 0; i < itemShowA.Length; i++)
            {
                g.world.playerUnit.data.RewardPropItem(DataProps.PropsData.NewProps(itemShowA[i], 1));
            }
            itemShowA = CommonTool.StrSplitInt(upGradeItem.itemShowA, '_');
            for (int i = 0; i < itemShowA.Length; i++)
            {
                g.world.playerUnit.data.RewardPropItem(DataProps.PropsData.NewProps(itemShowA[i], 1));
            }

            //突破材料
            int[] itemShowB = CommonTool.StrSplitInt(upGradeItem.itemShowB, '_');
            for (int i = 0; i < itemShowB.Length; i++)
            {
                g.world.playerUnit.data.RewardPropItem(DataProps.PropsData.NewProps(itemShowB[i], 1));
            }
        }

        // 解锁所有技艺图谱
        public void LuckArt()
        {
            foreach (var item in g.conf.runeFormula._allConfList)
            {
                if (item.grade <= 10 && g.conf.itemProps.GetItem(item.formulaId) != null)
                {
                    if (!g.data.world.allArtistrySkillID.Contains(item.formulaId))
                        g.data.world.allArtistrySkillID.Add(item.formulaId);
                }
            }

            foreach (var item in g.conf.makePillFormula._allConfList)
            {
                if (item.grade <= 10 && g.conf.itemProps.GetItem(item.id) != null)
                {
                    if (g.data.world.pillFormulas.Find((Func< DataWorld.World.PillFormulaData, bool>)((v) => v.id == item.id)) == null)
                    {
                        DataWorld.World.PillFormulaData data = new DataWorld.World.PillFormulaData();
                        data.id = item.id;
                        g.data.world.pillFormulas.Add(data);
                    }
                }
            }



            foreach (var item in g.conf.geomancySkill._allConfList)
            {
                if (item.grade == 9 && g.conf.itemProps.GetItem(item.id) != null)
                {
                    if (!g.data.world.allArtistrySkillID.Contains(item.id))
                        g.data.world.allArtistrySkillID.Add(item.id);
                }
            }
            foreach (var item in g.conf.worldBlockHerb._allConfList)
            {
                if (g.conf.itemProps.GetItem(item.id) != null)
                {
                    if (!g.data.world.allArtistrySkillID.Contains(item.id))
                        g.data.world.allArtistrySkillID.Add(item.id);
                }
            }
            foreach (var item in g.conf.worldBlockMineBook._allConfList)
            {
                if (item.conceal != 1 && g.conf.itemProps.GetItem(item.id) != null)
                {
                    if (!g.data.world.allArtistrySkillID.Contains(item.id))
                        g.data.world.allArtistrySkillID.Add(item.id);
                }
            }
            foreach (var item in g.conf.artifactShapeMaterial._allConfList)
            {
                if (g.conf.itemProps.GetItem(item.id) != null)
                {
                    if (!g.data.world.allArtistrySkillID.Contains(item.id))
                        g.data.world.allArtistrySkillID.Add(item.id);
                }
            }


            AddLog("执行成功");
        }

        public void StudySkillGroup()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (!int.TryParse(p2, out int value))
            {
                AddLog("执行失败：没有选择套路：" + p2);
                return;
            }
            if (value == 0)
            {
                var list = g.conf.taoistHeartSkillGroupName._allConfList;
                value = list[0].id;
            }
            if (g.conf.taoistHeartSkillGroupName.GetItem(value) == null)
            {
                AddLog("执行失败：没有的套路无效：" + p2);
                return;
            }
            try
            {
                DelAllSkill();
                unit.data.unitData.heart.heroesSkillGroupID = value;
                if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
                {
                    var u = unit;
                    unit = new WorldUnitBase();
                    unit.conf = u.conf;
                    unit.data = u.data;
                    unit.luckDyn = u.luckDyn;
                    unit.behavior = u.behavior;
                    unit.appellation = u.appellation;
                    unit.allHobby = u.allHobby;
                    unit.allLuck = u.allLuck;
                    unit.allTask = u.allTask;
                    unit.allTroubles = u.allTroubles;
                    unit.allEquips = u.allEquips;
                    unit.allEffects = u.allEffects;
                    unit.allEffectsCustom = u.allEffectsCustom;
                    unit._allWorldUnitEffectBaseCache = u._allWorldUnitEffectBaseCache;
                    unit._isDie = u._isDie;
                    unit.onDieCall = u.onDieCall;
                    unit.isInitOneEquip = u.isInitOneEquip;
                }
                try
                {
                    g.conf.npcHeroes.InitMartial(unit, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    AddLog("执行失败");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败!");
                return;
            }


            AddLog("执行成功");
        }

        public void PlayNpc()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if(unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID){
                AddLog("执行失败：无法夺舍自己：" + p1);
                return;
            }
            bool mainUi = false;
            var ui = g.ui.GetUI<UIMapMain>(UIType.MapMain);
            if (ui != null)
            {
                mainUi = true;
                g.ui.OpenUI<UIMapMain>(UIType.MapMain);
                g.ui.CloseAllUI();
            }
            else
            {
                AddLog("执行失败：当前无法进行夺舍!" );
                return;
            }
            g.data.world.playerUnitID = unit.data.unitData.unitID;
            g.world.playerUnit = g.world.unit.GetUnit(g.data.world.playerUnitID);
            g.world.playerUnit.RoundStart(g.world.run.roundDayResidue);
            g.world.playerUnit.UpdateConf();
            if (mainUi)
            {
                g.ui.OpenUI<UIMapMain>(UIType.MapMain);
            }


            AddLog("执行成功");
        }

        public void FixXingge()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            int outTrait1, outTrait2, inTrait;
            try
            {
                var data = p2.Split(',');
                outTrait1 = int.Parse(data[0]);
                outTrait2 = int.Parse(data[1]);
                inTrait = int.Parse(data[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败：选择的性格不合法："+p2);
                return;
            }

            unit.data.unitData.propertyData.outTrait1 = outTrait1;
            unit.data.unitData.propertyData.outTrait2 = outTrait2;
            unit.data.unitData.propertyData.inTrait = inTrait;
            AddLog("执行成功");
        }

        public void RunGameCmd()
        {
            try
            {
                DramaFunctionTool.OptionsFunction(p1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败");
                return;
            }
            AddLog("执行成功");
        }
        public void OpenGameDrama()
        {

            try
            {
                int.TryParse(p1, out int id);

                var unit1 = GetUnit(p2);
                var unit2 = GetUnit(p3);

                DramaTool.OpenDrama(id, new DramaData
                {
                    unitLeft = unit1,
                    unitRight = unit2,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败");
                return;
            }
            AddLog("执行成功");
        }
        public void Tc85YzAddNpc()
        {
            var unit = GetUnit(p1);
            if (unit == null)
            {
                AddLog("执行失败：无法获取单位：" + p1);
                return;
            }
            if (unit.data.unitData.unitID == g.world.playerUnit.data.unitData.unitID)
            {
                AddLog("执行失败：不能卖自己：" + p1);
                return;
            }
            if (IsJinvOrBangjia(unit.data.unitData.unitID))
            {
                AddLog("执行失败：此人已经是妓女或被绑架：" + p1);
                return;
            }

            try
            {
                MOD_Tc85Yz.Brothel.Sell(g.world.playerUnit, unit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败");
                return;
            }
            AddLog("执行成功");
        }
        public void Tc85YzAutoNpc()
        {
            int.TryParse(p1, out int value);


            int sell = 0;
            try
            {
                List<string> units = new List<string>();
                foreach (var item in g.world.unit.allUnit)
                {
                    var unit = item.Value;
                    if (unit.data.unitData.propertyData.sex == UnitSexType.Woman)
                    {
                        units.Add(unit.data.unitData.unitID);
                    }
                }

                while (value > 0)
                {
                    if (units.Count == 0) {
                        break;
                    }
                    var unitid = units[UnityEngine.Random.Range(0, units.Count)];
                    units.Remove(unitid);
                    if (IsJinvOrBangjia(unitid))
                    {
                        continue;
                    }
                    if (unitid == g.world.playerUnit.data.unitData.unitID)
                    {
                        continue;
                    }
                    var unit = g.world.unit.GetUnit(unitid);
                    if (unit == null)
                    {
                        continue;
                    }
                    var err = MOD_Tc85Yz.Brothel.Sell(g.world.playerUnit, unit);
                    if (err != -1)
                    {
                        sell++;
                        value--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                AddLog("执行失败");
                return;
            }
            AddLog("执行成功!卖出数量" + sell);
        }


        bool IsJinvOrBangjia(string id)
        {
            Type type = typeof(MOD_Tc85Yz.Brothel);
            FieldInfo field1 = type.GetField("Bitchs", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, List<string>> bitchs = (Dictionary<string, List<string>>)field1.GetValue(null);
            FieldInfo field2 = type.GetField("Kidnapping", BindingFlags.NonPublic | BindingFlags.Static);
            List<string> kidnapping = (List<string>)field2.GetValue(null);

            foreach (var item in bitchs)
            {
                foreach (var unitid in item.Value)
                {
                    if (unitid == id)
                    {
                        return true;
                    }
                }
            }
            foreach (var unitid in kidnapping)
            {
                if (unitid == id)
                {
                    return true;
                }
            }
            return false;

        }

        void LogJinv()
        {
            Type type = typeof(MOD_Tc85Yz.Brothel);
            FieldInfo field1 = type.GetField("Bitchs", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, List<string>> bitchs = (Dictionary<string, List<string>>)field1.GetValue(null);
            foreach (var item in bitchs)
            {
                string name = item.Key;
                var town = g.world.build.GetBuild(item.Key);
                if (town != null)
                {
                    name += town.name;
                }
                Console.WriteLine(name+" 妓女数量>> "+item.Value);
            }
        }


        #endregion
    }
}
