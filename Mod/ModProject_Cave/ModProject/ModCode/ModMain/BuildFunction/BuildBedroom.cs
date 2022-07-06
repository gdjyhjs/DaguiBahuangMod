using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.BuildFunction
{
    // 卧室
    public class BuildBedroom : GuiBaseUI.ClassBase
    {
        public BuildBedroom()
        {
        }

        public override void Init(string param)
        {
            Cave.Log("打开闲楼");
            new CreateFace().InitData(g.world.playerUnit);
        }
    }
}
