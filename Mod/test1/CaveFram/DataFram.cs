using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveFram
{
    public class DataFramItem
    {
        public int level = 1; // 等级
        public int itemID; // 物品ID
        public float progress; // 成长进度
        public int count = 1; // 结果数量
    }

    public class DataFram
    {
        public Dictionary<int, DataFramItem> data;
        public static string key = "DataFram1";
        public DataFram()
        {
            data = new Dictionary<int, DataFramItem>();
            for (int i = 0; i < 20; i++)
            {
                data.Add(i, new DataFramItem());
            }
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
                string dataStr = g.data.obj.GetString("www_yellowshange_com", key);
                data = JsonConvert.DeserializeObject<DataFram>(dataStr);
            }
            else
            {
                data = null;
            }
            return data == null ? new DataFram() : data;
        }
    }
}
