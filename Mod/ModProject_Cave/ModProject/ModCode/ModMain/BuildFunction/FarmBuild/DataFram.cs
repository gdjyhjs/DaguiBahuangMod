using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave
{
    public class DataFramItem
    {
        public int level = 1; // 等级
        public int itemID; // 物品ID
        public float progress; // 成长进度
        public int count = 0; // 结果数量
        public int seed = 1; // 随机种子
        public int lingqi = 0; // 灵气
    }
    /// 获取成熟需要的时间
    /// 根据灵田的等级每个月可以产生灵气 【240|1000|3600|11000|30000|50000|60000|70000|80000|90000|100000】
    /// 树成熟需要等同于道具价值10倍的灵气
    /// 成熟后每二倍的灵气可随机获得1-3个灵果
    public class DataFram
    {
        public static int[] framLevelLingqi = new int[] { 1, 240, 1000, 3600, 11000, 30000, 50000, 60000, 70000, 80000, 90000, 100000 };
        public List<DataFramItem> data;
        public string decorate;
        public int open;

        public static string key = "DataFram1";
        public DataFram()
        {
            data = new List<DataFramItem>();
        }

        public static void SaveData(DataFram data)
        {
            string dataStr = JsonConvert.SerializeObject(data);
            g.data.obj.SetString("www_yellowshange_com", key, dataStr);
        }

        public static DataFram ReadData()
        {
            DataFram data;
            if (g.data.obj.ContainsKey("www_yellowshange_com", key))
            {
                try
                {
                    string dataStr = g.data.obj.GetString("www_yellowshange_com", key);
                    data = JsonConvert.DeserializeObject<DataFram>(dataStr);
                }
                catch (Exception e)
                {
                    Cave.LogError(e.Message + "\n" + e.StackTrace);
                    data = null;
                }
            }
            else
            {
                data = null;
            }
            return data == null ? new DataFram() : data;
        }
    }
}
