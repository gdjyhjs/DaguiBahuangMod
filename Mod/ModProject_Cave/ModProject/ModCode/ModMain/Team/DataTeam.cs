using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Team
{
    public static class DataTeam
    {
        public static List<string> teamUnits = new List<string>(); // 同行的npc们
        public static List<string> battleUnits = new List<string>(); // 出战的npc们

        public static void InitData()
        {
            if (g.data.obj.ContainsKey("www.yellowshange.com", "CaveTeamPlayerData"))
            {
                string data = g.data.obj.GetString("www.yellowshange.com", "CaveTeamPlayerData");
                teamUnits = JsonConvert.DeserializeObject<List<string>>(data);
            }
            if (g.data.obj.ContainsKey("www.yellowshange.com", "CaveTeamPlayerData2"))
            {
                string data = g.data.obj.GetString("www.yellowshange.com", "CaveTeamPlayerData2");
                battleUnits = JsonConvert.DeserializeObject<List<string>>(data);
            }
            Cave.Log("初始化洞府随行人员 " + battleUnits.Count + "/" + teamUnits.Count);
        }

        public static void SaveData()
        {
            string data = JsonConvert.SerializeObject(teamUnits);
            g.data.obj.SetString("www.yellowshange.com", "CaveTeamPlayerData", data);

            string data2 = JsonConvert.SerializeObject(battleUnits);
            g.data.obj.SetString("www.yellowshange.com", "CaveTeamPlayerData2", data2);
        }
    }
}
