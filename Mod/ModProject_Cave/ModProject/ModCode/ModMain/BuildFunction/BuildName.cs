using Cave.Config;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.BuildFunction
{
    class BuildName : GuiBaseUI.ClassBase
    {
        CaveBuildData build = null;
        string togConnect = null;
        bool togState = false;
        string fixName = "洞府";

        public override void Init(string param)
        {
            if (CaveBuildOpen.openItem != null)
            {
                build = CaveBuildOpen.openItem;
                fixName = build.GetName();
                togState = string.IsNullOrWhiteSpace(build.param);
                togConnect = GameTool.LS("Cave_TongbuXiugaiDongfuMingzi");
            }

            InitUI();
        }

        private void InitUI()
        {
            GuiBaseUI.UIFastFunction.FastInputText(string.Format(GameTool.LS("Cave_ShuxieNeirong"), $"<color=#004FCA>{fixName}</color>"), (connect, toggleValue) =>
            {
                if (g.conf.textBlock.IsBlock(connect, false) != 0)
                {
                    UITipItem.AddTip(GameTool.LS("tip_mingganci"));
                    return;
                }
                if (toggleValue)
                {
                    build.param = "";
                    MainCave.data.name = connect;
                }
                else
                {
                    build.param = connect;
                }
                DataCave.SaveData(MainCave.data);
                MainCave.data.InitCave();
                DramaFunction.UpdateMapAllUI();
                MainCave.cave.CreateBuilds();

            }, togState, togConnect);
        }
    }


}
