using Cave.BuildFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymnasium
{
    public class Gumnasium : BuildBase
    {
        public static bool openGumnasium;
        public Gumnasium()
        {

        }

        public override void Init(string param)
        {
            new Log("打开升级技能");
            openGumnasium = true;
            UIMartialTrain ui = g.ui.OpenUI<UIMartialTrain>(UIType.MartialTrain);
            ui.UpdateRealizeInfoUI();
        }
    }
}