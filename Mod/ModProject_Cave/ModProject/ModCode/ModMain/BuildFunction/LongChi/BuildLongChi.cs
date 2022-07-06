using Cave.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.BuildFunction
{
    class BuildLongChi : GuiBaseUI.ClassBase
    {

        public override void Init(string param)
        {
            if (g.ui.GetUI(UIType.DragonDoorUpgrade) != null)
                return;
            Patch_UIDragonDoorUpgrade_Init.openCaveLongChi = true;
            UIDragonDoorUpgrade ui = g.ui.OpenUI<UIDragonDoorUpgrade>(UIType.DragonDoorUpgrade);
        }
    }
}
