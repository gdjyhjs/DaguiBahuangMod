using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace Cave
{
    [HarmonyPatch(typeof(PointResourcesMgr), "OnSchoolAction")]
    public class Patch_PointResourcesMgr_OnSchoolAction
    {
        // 过月结束
        [HarmonyPostfix]
        private static void Postfix()
        {
            DataFram data = DataFram.ReadData();
            Grow(data);
            DataFram.SaveData(data);
        }

        public static void Grow(DataFram data)
        {
            foreach (DataFramItem framData in data.data)
            {
                ConfItemPropsItem item = g.conf.itemProps.GetItem(framData.itemID);
                if (item == null || framData.itemID == 0)
                {
                    // 没有种植
                }
                else
                {
                    framData.lingqi += DataFram.framLevelLingqi[framData.level];
                    if (framData.progress < 1)
                    {
                        var need = DataFram.GetItemWorth(item.id) * 10;
                        framData.progress = framData.lingqi * 1f / need;
                        if (framData.progress >= 1)
                        {
                            framData.lingqi -= need;
                        }
                    }

                    if (framData.progress >= 1)
                    {
                        int need = Math.Max(1, DataFram.GetItemWorth(item.id) * 2);
                        while (framData.lingqi >= need)
                        {
                            if (framData.count == 0)
                            {
                                framData.seed = CommonTool.Random(int.MinValue, int.MaxValue);
                            }
                            framData.lingqi -= need;
                            int addCount = CommonTool.Random(1, 3);
                            framData.count += addCount;
                        }
                        if (framData.count > 9999)
                        {
                            framData.count = 9999;
                        }
                    }
                }
            }
        }
    }
}
