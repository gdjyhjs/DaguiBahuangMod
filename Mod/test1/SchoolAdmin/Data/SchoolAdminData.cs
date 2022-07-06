using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAdmin
{
    public class SchoolAdminData
    {
        public class PageData
        {
            public List<int> mainTog; // 每个主选项是否勾选
            public List<List<string>> subTog; // 每个子选项的值
            public List<string> schools; // 生效的宗门ID

            public bool IsCheckSchool(MapBuildSchool school)
            {
                if (schools == null)
                    schools = new List<string>();
                return schools.Contains(school.buildData.id);
            }
            public bool IsCheckMainTog(int i)
            {
                if (mainTog == null)
                {
                    mainTog = new List<int>();
                }
                while (i >= mainTog.Count)
                {
                    mainTog.Add(0);
                }
                return mainTog[i] == 1;
            }
            public string GetSubTog(int i, int j)
            {
                if (subTog == null)
                {
                    subTog = new List<List<string>>();
                }
                while (i >= subTog.Count)
                {
                    subTog.Add(new List<string>());
                }
                while (j >= subTog[i].Count)
                {
                    subTog[i].Add("0");
                }
                return subTog[i][j];
            }

            public void ChangeSchool(MapBuildSchool school, bool isAdd)
            {
                if (isAdd)
                {
                    if (!schools.Contains(school.buildData.id))
                        schools.Add(school.buildData.id);
                }
                else
                {
                    if (schools.Contains(school.buildData.id))
                        schools.Remove(school.buildData.id);
                }
            }
            public void ChangeMainTog(int i, int value)
            {
                mainTog[i] = value;
            }
            public void ChangeSubTog(int i, int j, string value)
            {
                subTog[i][j] = value;
            }
        }
        public List<PageData> data;
        public SchoolAdminData()
        {
            data = new List<PageData>();
        }

        public static string key = "SchoolAdminData4";
        public static void SaveData(SchoolAdminData caveData)
        {
            string data = JsonConvert.SerializeObject(caveData);
            g.data.obj.SetString("www_yellowshange_com", key, data);
            CheckJoinSchool.schoolAdminData = caveData;
        }

        public static SchoolAdminData ReadData()
        {
            SchoolAdminData caveData;
            if (g.data.obj.ContainsKey("www_yellowshange_com", key))
            {
                string data = g.data.obj.GetString("www_yellowshange_com", key);
                caveData = JsonConvert.DeserializeObject<SchoolAdminData>(data);
            }
            else
            {
                caveData = null;
            }
            return caveData == null ? new SchoolAdminData() : caveData;
        }
    }
}
