using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD_atLeW5
{
    public static class StaticData
    {
        public class Data
        {
            public int fixCacheCount = 30;
            public int sortType = 1;
        }
        public static Data data = new Data();
        public static int fixCacheCount { get { return data.fixCacheCount; }
            set
            {
                if (data.fixCacheCount != value)
                {
                    data.fixCacheCount = value;
                    ModMain.SaveData();
                    var ui = g.ui.GetUI<UILogin>(UIType.Login);
                    if (ui)
                    {
                        ui.UpdateUI();
                    }
                }
            }
        }
        public static int sortType
        {
            get { return data.sortType; }
            set
            {
                if (data.sortType != value)
                {
                    data.sortType = value;
                    var ui = g.ui.GetUI<UILogin>(UIType.Login);
                    if (ui)
                    {
                        Patch_UILogin_UpdateUI.Postfix(ui);
                    }
                    ModMain.SaveData();
                }
            }
        }



    }
}
