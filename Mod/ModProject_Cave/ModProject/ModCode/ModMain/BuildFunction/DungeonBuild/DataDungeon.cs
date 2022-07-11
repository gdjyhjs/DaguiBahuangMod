using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cave.BuildFunction;

namespace Cave
{
    public class DataDungeon
    {
        /// <summary>
        /// 关押的单位
        /// </summary>
        public List<string> units = new List<string>();
        public Dictionary<string, float[]> unitPosi = new Dictionary<string, float[]>(); // NPC所在地图位置 [x, y, 方向]
        public Dictionary<string, int> unitConquer = new Dictionary<string, int>(); // NPC征服度
        public string decorate;
        public int open;
        public int vip;

        public static string key = "DataDungeonItem1";
        public DataDungeon()
        {

        }

        // 增加征服度
        public void AddConquer(string unitid, float value)
        {
            int v = 0;
            if (unitConquer.ContainsKey(unitid))
            {
                v = unitConquer[unitid];
            }
            unitConquer[unitid] = Mathf.Clamp(v + Mathf.CeilToInt(value), 0, 100);
        }

        // 增加征服度
        public int GetConquer(string unitid)
        {
            if (unitConquer.ContainsKey(unitid))
            {
                return unitConquer[unitid];
            }
            return 0;
        }

        public static void SaveData(DataDungeon data)
        {
            string dataStr = JsonConvert.SerializeObject(data);
            g.data.obj.SetString("www_yellowshange_com", key, dataStr);
        }

        public static DataDungeon ReadData()
        {
            DataDungeon data;
            if (g.data.obj.ContainsKey("www_yellowshange_com", key))
            {
                try
                {
                    string dataStr = g.data.obj.GetString("www_yellowshange_com", key);
                    data = JsonConvert.DeserializeObject<DataDungeon>(dataStr);
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

            // 测试 TODO
            data = data == null ? new DataDungeon() : data;

            List<string> delUnits = new List<string>();
            foreach (var unitId in data.units)
            {
                var unit = g.world.unit.GetUnit(unitId, true);
                if (unit == null)
                {
                    delUnits.Add(unitId);
                }
                else if(unit.GetLuck(DungeonBuild.prisonerLuckId) == null)
                {
                    delUnits.Add(unitId);
                }
            }
            // 删除不存在的NPC
            foreach (var unitId in delUnits)
            {
                data.units.Remove(unitId);
                data.unitPosi.Remove(unitId);
                data.unitConquer.Remove(unitId);
            }

            Cave.Log("关押数量：" + data.units.Count);
            return data;
        }
    }
}
