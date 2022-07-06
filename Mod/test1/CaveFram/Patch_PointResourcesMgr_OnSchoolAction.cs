using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace CaveFram
{
    [HarmonyPatch(typeof(PointResourcesMgr), "OnSchoolAction")]
    public class Patch_PointResourcesMgr_OnSchoolAction
    {
        // 过月结束
        [HarmonyPostfix]
        private static void Postfix()
        {
            DataFram data = DataFram.ReadData();
            foreach (DataFramItem framData in data.data.Values)
            {
                ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
                if (item == null || framData.itemID == 0)
                {
                    // 没有种植
                }
                else
                {
                    int needMonth = BuildFarm.GetNeedTime(framData.level, item.level);
                    framData.progress += 1f / needMonth;
                    if (framData.progress > 1)
                    {
                        framData.progress -= 1;
                        framData.count++;
                    }
                }
            }
            DataFram.SaveData(data);
        }
    }
}
