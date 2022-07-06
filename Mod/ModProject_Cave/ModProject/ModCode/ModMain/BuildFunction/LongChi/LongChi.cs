using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave
{
    class LongChi
    {
        Il2CppSystem.Action<ETypeData> onHaloSummonMonst;
        public void Init()
        {
            onHaloSummonMonst = new Action<ETypeData>(OnHaloSummonMonst);
            g.events.On(EBattleType.HaloSummonMonst, onHaloSummonMonst);
        }

        public void Destroy()
        {
            g.events.Off(EBattleType.HaloSummonMonst, onHaloSummonMonst);
        }

        void OnHaloSummonMonst(ETypeData edata)
        {
            try
            {
                if (g.world.playerUnit.data.school != null)
                {
                    if (!g.world.playerUnit.data.school.GetBuildSub<MapBuildSchoolDragonDoor>().IsShowBuildUI())
                    {
                        Cave.Log("调用自己宗门 召唤单位");
                        g.world.playerUnit.data.school.GetBuildSub<MapBuildSchoolDragonDoor>().OnHaloSummonMonst(edata);
                    }
                }
                else
                {
                    var schools = g.world.build.GetBuilds<MapBuildSchool>();
                    if (schools.Count > 0)
                    {
                        var school = schools[0];
                        Cave.Log("调用其他宗门 召唤单位");
                        g.world.playerUnit.data.unitData.schoolID = school.buildData.id;
                        school.GetBuildSub<MapBuildSchoolDragonDoor>().OnHaloSummonMonst(edata);
                        g.world.playerUnit.data.unitData.schoolID = "";
                    }
                }
            }
            catch (Exception e)
            {
                Cave.Log(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
