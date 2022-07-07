using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.BuildFunction
{
    class ShowTips : GuiBaseUI.ClassBase
    {
        public ShowTips()
        {

        }

        public override void Init(string param)
        {
            var skyTip = g.ui.OpenUI<UISkyTip>(UIType.SkyTip);
            var pos = GameEffectTool.GetMouseWorldPosi(g.ui.uiCamera);
            skyTip.InitData("<size=22>"+GameTool.LS(param)+"</size>", pos, false);
        }
    }
}
