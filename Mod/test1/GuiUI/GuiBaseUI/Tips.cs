using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBaseUI
{
    public class Tips
    {
        public Tips(string title, string content, int type = 2, Action onYesCall = null, Action onNoCall = null)
        {
            g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(title, content, type, onYesCall, onNoCall);
        }
    }
}
