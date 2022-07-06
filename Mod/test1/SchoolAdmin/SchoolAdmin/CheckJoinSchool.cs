﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    // 检查是否可以加入宗门
    public class CheckJoinSchool
    {
        private static SchoolAdminData m_SchoolAdminData;
        public static SchoolAdminData schoolAdminData
        {
            get
            {
                if (m_SchoolAdminData == null)
                {
                    m_SchoolAdminData = SchoolAdminData.ReadData();
                }
                return m_SchoolAdminData;
            }
            set
            {
                m_SchoolAdminData = value;
            }
        }
        MapBuildSchool school;
        WorldUnitBase unit;
        public CheckJoinSchool(MapBuildSchool school, WorldUnitBase unit)
        {
            this.school = school;
            this.unit = unit;
        }

        public int GetResult()
        {
            WorldUnitDynData unitData = unit.data.dynUnitData;
            string unitName = unit.data.unitData.propertyData.GetName();
            SchoolAdminData schoolAdminData = CheckJoinSchool.schoolAdminData;
            for (int k = 0; k < schoolAdminData.data.Count; k++)
            {
                SchoolAdminData.PageData data = schoolAdminData.data[k];
                if (data.IsCheckSchool(school)) // 判断是否应用该宗门
                {
                    for (int i = 0; i < data.mainTog.Count; i++)
                    {
                        if (data.IsCheckMainTog(i))
                        {
                            List<string> value = data.subTog[i];
                            switch (i)
                            {
                                case 4://性格
                                    ConfRoleCreateCharacterItem inTrait = g.conf.roleCreateCharacter.GetItem(unitData.inTrait.value);
                                    ConfRoleCreateCharacterItem outTrait1 = g.conf.roleCreateCharacter.GetItem(unitData.outTrait1.value);
                                    ConfRoleCreateCharacterItem outTrait2 = g.conf.roleCreateCharacter.GetItem(unitData.outTrait2.value);
                                    List<ConfRoleCreateCharacterItem> characters = new List<ConfRoleCreateCharacterItem>() { inTrait, outTrait1, outTrait2 };

                                    bool characterOk = false;
                                    for (int j = 0; j < characters.Count; j++)
                                    {
                                        if (data.GetSubTog(i, characters[j].id - 1) == "1")
                                        {
                                            characterOk = true;
                                        }
                                    }
                                    if (!characterOk)
                                    {
                                        if (SchoolAdmin.isDebug)
                                        {
                                            SchoolAdmin.Log("性格不符   ");
                                        }
                                        return 1004;
                                    }
                                    break;
                                case 0://性别
                                    var sex = unit.data.unitData.propertyData.sex;
                                    if (sex == UnitSexType.Man && data.GetSubTog(i, 0) != "1")
                                    {
                                        SchoolAdmin.Log("性别不符 不收 男性");
                                        return 1000;
                                    }
                                    if (sex == UnitSexType.Woman && data.GetSubTog(i, 1) != "1")
                                    {
                                        SchoolAdmin.Log("性别不符 不收 女性");
                                        return 1000;
                                    }
                                    break;
                                case 1://魅力
                                    ConfRoleBeautyItem beautyItem = g.conf.roleBeauty.GetItemInBeauty(unitData.beauty.value);
                                    if (data.GetSubTog(i, beautyItem.id - 1) != "1")
                                    {
                                        SchoolAdmin.Log("魅力不符 不收 " + GameTool.LS(beautyItem.text));
                                        return 1001;
                                    }
                                    break;
                                case 2://兴趣
                                    UnhollowerBaseLib.Il2CppStructArray<int> hobby = unit.data.unitData.propertyData.hobby;
                                    bool hobbyOk = false;
                                    for (int j = 0; j < hobby.Count; j++)
                                    {
                                        if (data.GetSubTog(i, hobby[j]-201) == "1")
                                        {
                                            hobbyOk = true;
                                        }
                                    }
                                    if (!hobbyOk)
                                    {
                                        if (SchoolAdmin.isDebug)
                                        {
                                            List<string> xx = new List<string>();
                                            foreach (var item in hobby)
                                            {
                                                xx.Add(GameTool.LS(g.conf.roleCreateHobby.GetItem(hobby[i]).name));
                                            }
                                            SchoolAdmin.Log("兴趣不符   " + string.Join("，", xx));
                                        }
                                        return 1002;
                                    }
                                    break;
                                case 3://声望
                                    ConfRoleReputationItem reputationItem = g.conf.roleReputation.GetItemInReputation(unitData.reputation.value);
                                    if (data.GetSubTog(i, reputationItem.id - 1) != "1")
                                    {
                                        SchoolAdmin.Log("声望不符 不收 " + GameTool.LS(reputationItem.text));
                                        return 1003;
                                    }
                                    break;
                                case 5://境界
                                    ConfRoleGradeItem gradeItem = g.conf.roleGrade.GetItem(unitData.gradeID.value);
                                    if (data.GetSubTog(i, gradeItem.id - 1) != "1")
                                    {
                                        SchoolAdmin.Log("境界不符 不收 " + GameTool.LS(gradeItem.gradeName) + GameTool.LS(gradeItem.phaseName));
                                        return 1005;
                                    }
                                    break;
                                case 6://情感
                                    var childrenPrivate = unit.data.unitData.relationData.childrenPrivate;
                                    var children = unit.data.unitData.relationData.children;
                                    var lover = unit.data.unitData.relationData.lover;
                                    var married = unit.data.unitData.relationData.married;
                                    if (data.GetSubTog(i, 0) == "1") // 
                                    {

                                    }
                                    else if (data.GetSubTog(i, 1) == "1") // 要求未婚
                                    {
                                        if (!string.IsNullOrEmpty(married))
                                        {
                                            SchoolAdmin.Log("婚姻不符 只收 未婚");
                                            return 1006;
                                        }
                                    }
                                    else if (data.GetSubTog(i, 2) == "1") // 要求已婚
                                    {
                                        if (string.IsNullOrEmpty(married))
                                        {
                                            SchoolAdmin.Log("婚姻不符 只收 已婚");
                                            return 1006;
                                        }
                                    }

                                    if (data.GetSubTog(i, 3) == "1") //
                                    {

                                    }
                                    else if (data.GetSubTog(i, 4) == "1") // 要求无子女
                                    {
                                        if ((childrenPrivate.Count + children.Count) > 0)
                                        {
                                            SchoolAdmin.Log("子女不符 只收 无子女 "+"子女数量 "+ (childrenPrivate.Count + children.Count));
                                            return 1006;
                                        }
                                    }
                                    else if (data.GetSubTog(i, 5) == "1") // 要求有子女
                                    {
                                        if ((childrenPrivate.Count + children.Count) < 1)
                                        {
                                            SchoolAdmin.Log("子女不符 只收 有子女" + "子女数量 " + (childrenPrivate.Count + children.Count));
                                            return 1006;
                                        }
                                    }

                                    if (data.GetSubTog(i, 6) == "1")
                                    {

                                    }
                                    else if (data.GetSubTog(i, 7) == "1") // 要求无道侣
                                    {
                                        if ((lover.Count) > 0)
                                        {
                                            SchoolAdmin.Log("道侣不符 只收 无道侣");
                                            return 1006;
                                        }
                                    }
                                    else if (data.GetSubTog(i, 8) == "1") // 要求有道侣
                                    {
                                        if ((lover.Count) < 1)
                                        {
                                            SchoolAdmin.Log("道侣不符 只收 有道侣");
                                            return 1006;
                                        }
                                    }

                                    break;
                                case 7://姓氏
                                    string needName = data.GetSubTog(i, 0);
                                    if (!unitName.StartsWith(needName))
                                    {
                                        SchoolAdmin.Log(unitName+"  姓氏不符 只收 " + needName);
                                        return 1007;
                                    }
                                    break;
                                case 8://道心
                                    var heart = unit.data.unitData.heart;
                                    if (heart.IsHeroes()) // 是天骄
                                    {
                                        if (data.GetSubTog(i, 0) == "1")
                                        {
                                            SchoolAdmin.Log("招收 天骄");
                                        }
                                        else
                                        {
                                            SchoolAdmin.Log("不收 天骄");
                                            return 1008;
                                        }
                                    }
                                    else if (heart.IsReserveHeroes()) // 是人豪
                                    {
                                        if (data.GetSubTog(i, 1) == "1")
                                        {
                                            SchoolAdmin.Log("招收 人豪");
                                        }
                                        else
                                        {
                                            SchoolAdmin.Log("不收 人豪");
                                            return 1008;
                                        }
                                    }
                                    else // 是普通NPC
                                    {
                                        if (data.GetSubTog(i, 2) == "1")
                                        {
                                            SchoolAdmin.Log("招收 咸鱼");
                                        }
                                        else
                                        {
                                            SchoolAdmin.Log("不收 咸鱼");
                                            return 1008;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}