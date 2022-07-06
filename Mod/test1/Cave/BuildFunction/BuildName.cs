using Cave.Config;
using GuiBaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.BuildFunction
{
    class BuildName : GuiBaseUI.ClassBase
    {
        public BuildName()
        {
        }

        public override void Init(string param)
        {
            CaveBuildData build = null;
            DataCustom.CaveNameCustom nameCustom = null;
            string togConnect = null;
            bool togState = false;
            string fixName = "洞府";
            if (CaveBuildOpen.openItem != null)
            {
                build = CaveBuildOpen.openItem;
                fixName = build.GetName();
                togState = string.IsNullOrWhiteSpace(build.param);
                togConnect = "同步修改洞府名称";
            }

            UIFastFunction.FastInputText($"书写<color=#004FCA>{fixName}</color>内容", (ss, tt) =>
            {
                if (g.conf.textBlock.IsBlock(ss, false) != 0)
                {
                    UITipItem.AddTip(GameTool.LS("tip_mingganci"));
                    return;
                }
                if (tt)
                {
                    build.param = "";
                    MainCave.data.name = ss;
                }
                else
                {
                    build.param = ss;
                }
                DataCave.SaveData(MainCave.data);
                MainCave.data.InitCave();
                DramaFunction.UpdateMapAllUI();
                MainCave.cave.CreateBuilds();

            }, togState, togConnect);
        }
    }
}
