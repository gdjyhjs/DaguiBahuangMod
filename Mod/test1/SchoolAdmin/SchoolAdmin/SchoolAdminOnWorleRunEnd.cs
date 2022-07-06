using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    public class SchoolAdminOnWorleRunEnd
    {
        public SchoolAdminOnWorleRunEnd()
        {

            SchoolAdmin.Log("有人加入宗门");
            var school = g.world.playerUnit.data.school;
            if (school != null && school.buildData.GetPostType(g.world.playerUnit.data.unitData.unitID) == SchoolPostType.SchoolMain)
            {
                var allUnits = g.world.unit.GetUnit(school.buildData.GetPostUnit(SchoolPostType.Out), false);
                foreach (var unit in allUnits)
                {
                    int err = unit.CreateAction(new UnitActionSchoolExit());
                    SchoolAdmin.Log(unit.data.unitData.propertyData.GetName() + " 退出宗门 " + err);
                }
            }
        }
    }
}
