using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using System.IO;

namespace Cave
{

    public static class CaveStateData
    {
        public static Color Blud = new Color(0, 0.3f, 0.8f);
        public static string blud = "004FCA";
    }

    public class CaveBuildDataBase
    {
        public int id; // 配置ID
        public int type; // 类型
        public int level; // 等级
        public int price; // 等级
        public float x; // 位置
        public float y; // 位置
        public bool put; // 是否放置
        public string name; // 名字
        public string atlas; // 图集
        public string image; // 图片
        public int maxLevel; // 最大等级
        public int upgradeMoney; // 最大等级
        public string des = "";

        public string GetName()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                if (type == 1)
                {
                    Cave.Log($"id={id} type={type} name{GameTool.LS(g.conf.townBuilding.GetItem(id).name)}");
                    return GameTool.LS(g.conf.townBuilding.GetItem(id).name);
                }
                else if (type == 2)
                {
                    Cave.Log($"id={id} type={type} name{GameTool.LS(g.conf.schoolBuilding.GetItem(id).name)}");
                    return GameTool.LS(g.conf.schoolBuilding.GetItem(id).name);
                }
            }
            Cave.Log($"id={id} type={type} name{name}");
            return name;
        }

        public Sprite GetSprite()
        {
            Cave.Log(id + " 存在图片= " + File.Exists("Mods/Cave/" + id + ".png"));
            if (File.Exists("Mods/Cave/" + id + ".png"))
            {
                var config = GetConfig();
                if (config != null)
                {
                    return CaveTool.LoadSprite("Mods/Cave/" + id + ".png", config.width, config.height);
                }
                else
                {
                    return CaveTool.LoadSprite("Mods/Cave/" + id + ".png");
                }
            }

            if (!string.IsNullOrWhiteSpace(atlas))
            {
                var sprite = SpriteTool.GetSprite(atlas, image);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (!string.IsNullOrWhiteSpace(image))
            {
                var sprite = SpriteTool.GetSpriteBigTex(image);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (type == 1)
            {
                var sprite = SpriteTool.GetSprite("TownCommon", "Build" + id);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (type == 2)
            {
                var sprite = SpriteTool.GetSprite("SchoolCommon", "BuildUp" + id);
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildDown" + id);
                }
                int lv = level;
                if (lv < 1)
                    lv = 1;
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildUp" + id + "_" + lv);
                }
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildDown" + id + "_" + lv);
                }
                if (sprite != null)
                {
                    return sprite;
                }
            }
            return SpriteTool.GetSprite("TownCommon", "Build2007");
        }

        public BuildConfig GetConfig()
        {
            if (Cave.config.buildConfig.ContainsKey(id))
            {
                return Cave.config.buildConfig[id];
            }
            else
            {
                return null;
            }
        }
    }

    public class CaveNpcData
    {
        public string unitID;
        public int state; // 0:未入住  1:邀请中  2:已入住
        public int sung; // 幸福指数
        public string lastPoint; // 上个月所在的坐标
    }


    public class CaveData
    {
        /// <summary>
        /// 0没洞府   1正在选址    2选好地址
        /// </summary>
        public int state = 0;
        /// <summary>
        /// 选择的背景
        /// </summary>
        public int bg = 0;
        // 定居位置
        public int x = 0;
        public int y = 0;
        public int open = 0; // 打开洞府的次数
        public int into = 0; // 点击邀请NPC入住的次数
        public string name = "大鬼的洞府";
        public int version = 0;
        public CaveBuildDataBase[] builds; // 所有建筑数据
        public Dictionary<string, CaveNpcData> npcDatas = new Dictionary<string, CaveNpcData>(); // npc 键入住的NPC信息
        public List<string> caveLog = new List<string>(); // 洞府记事

        public CaveData()
        {
            UpLevelVersion();
        }

        public void UpLevelVersion()
        {
            // 版本补丁
            int minVersion = 3;
            if (version < minVersion)
            {
                version = minVersion;
                List<CaveBuildDataBase> builds = new List<CaveBuildDataBase>();
                builds.Add(new CaveBuildDataBase() { id = 2008, type = 1, level = 0, price = 200000, maxLevel = 4 }); // 工坊
                builds.Add(new CaveBuildDataBase() { id = 2009, type = 1, level = 0, price = 200000, maxLevel = 4 }); // 酒馆
                builds.Add(new CaveBuildDataBase() { id = 2011, type = 1, level = 0, price = 200000, maxLevel = 1 }); // 建木

                builds.Add(new CaveBuildDataBase() { id = 1007, type = 2, level = 0, price = 80000, maxLevel = 1 }); // 传送阵
                builds.Add(new CaveBuildDataBase() { id = 1006, type = 2, level = 0, price = 50000, maxLevel = 10 }); // 灵阁
                builds.Add(new CaveBuildDataBase() { id = 1005, type = 2, level = 0, price = 150000, maxLevel = 1 }); // 疗伤院

                builds.Add(new CaveBuildDataBase() { id = 3001, type = 3, level = 0, price = 100000, name = "灵田", atlas = "CommonTaoistHeartIcom", image = "wanxiang_3" }); // 灵田
                builds.Add(new CaveBuildDataBase() { id = 3002, type = 3, level = 0, price = 100000, name = "阵法", atlas = "CommonTaoistHeartIcom", image = "wuming_3" }); // 阵法
                builds.Add(new CaveBuildDataBase() { id = 3003, type = 3, level = 0, price = 100000, name = "正房", atlas = "SchoolCommon", image = "BuildUp1001_4" }); // 正房
                builds.Add(new CaveBuildDataBase() { id = 4004, type = 3, level = 0, price = 100000, name = "客房", atlas = "SchoolCommon", image = "BuildUp1006_2" }); // 客房

                this.builds = builds.ToArray();
            }
            if (version < 4)
            {
                foreach (var item in builds)
                {
                    if (item.id == 1006)
                    {
                        item.maxLevel = 4;
                        if (item.level > 4)
                            item.level = 4;
                    }
                }
                version = 4;
            }
            if (version < 5)
            {
                bool ok = false;
                foreach (var item in builds)
                {
                    if (item.id == 4004)
                    {
                        item.maxLevel = 100;
                        item.upgradeMoney = item.price;
                        item.des = "一间用来招待客人的居所，每一级可多招纳一名修士。";
                        ok = true;
                    }
                }
                if (!ok)
                {
                    List<CaveBuildDataBase> builds = new List<CaveBuildDataBase>(this.builds);
                    CaveBuildDataBase item = new CaveBuildDataBase() { id = 4004, type = 3, level = 0, price = 100000, name = "客房", atlas = "SchoolCommon", image = "BuildUp1006_2" };
                    item.maxLevel = 100;
                    item.upgradeMoney = item.price;
                    item.des = "一间用来招待客人的居所，每一级可多招纳一名修士。";
                    builds.Add(item); // 客房
                    this.builds = builds.ToArray();
                }
                version = 5;
            }
        }

        public static string key = "CaveData4";

        public static void SaveData(CaveData caveData)
        {
            string data = JsonConvert.SerializeObject(caveData);
            g.data.obj.SetString("www_yellowshange_com", key, data);
        }

        public static CaveData ReadData()
        {
            CaveData caveData;
            if (g.data.obj.ContainsKey("www_yellowshange_com", key))
            {
                string data = g.data.obj.GetString("www_yellowshange_com", key);
                caveData = JsonConvert.DeserializeObject<CaveData>(data);
                caveData.InitCave();
                caveData.UpLevelVersion();
            }
            else
            {
                caveData = null;
            }
            return caveData == null ? new CaveData() : caveData;
        }

        public void InitCave()
        {
            MapEventBase mapEvent = g.world.mapEvent.GetGridEvent(new UnityEngine.Vector2Int(x, y));
            if (mapEvent != null)
            {
                Action<DramaData> action = (data) =>
                {
                    // 触发事件
                    Cave.mainCave = new MainCave();
                };
                mapEvent.onOpenDramaCall = action;
                ConfWorldFortuitousEventBaseItem eventItem = g.conf.worldFortuitousEventBase.GetItem(mapEvent.eventData.id);
                eventItem.dramaID = "0";
                eventItem.showName = 1;

                ConfLocalTextItem textConf = g.conf.localText.allText[eventItem.name];
                textConf.ch = name;
            }
        }

        // 0:未入住  1:邀请中  2:已入住
        public int GetNpcIntoState(string unitID)
        {
            if (npcDatas.ContainsKey(unitID))
                return npcDatas[unitID].state;
            return 0;
        }

        // 0:未入住  1:邀请中  2:已入住
        public void SetNpcIntoState(string unitID, int state)
        {
            Cave.Log("设置 "+ unitID+" "+ state);
            if (state != 0)
            {
                if (npcDatas.ContainsKey(unitID))
                {
                    npcDatas[unitID].state = state;
                }
                else
                {
                    npcDatas.Add(unitID, new CaveNpcData() { unitID = unitID, state = state, sung = 0, lastPoint = "" });
                }
            }
            else if (npcDatas.ContainsKey(unitID))
            {
                Cave.Log("开始删除居民 " + unitID + " " + state);
                npcDatas.Remove(unitID);
                Cave.Log("成功删除居民 " + unitID + " " + state);
            }
        }

        // 获取建筑等级
        public int GetBuildLevel(int id)
        {
            foreach (var item in builds)
            {
                if (item.id == id)
                {
                    return item.level;
                }
            }
            return 0;
        }

        public Vector2Int GetPoint()
        {
            return new Vector2Int(x, y);
        }

        public void AddLog(string str)
        {
            int month = g.world.run.roundMonth;
            string logStr = $"<color=red>" + ((month / 12) + 1) + GameTool.LS("common_nian") + ((month % 12) + 1) + GameTool.LS("common_yue") + GameTool.LS("maohao") + "</color>" + str;
            caveLog.Add(logStr);
            if (caveLog.Count > 200)
                caveLog.RemoveAt(0);
        }
    }

    public class CaveTool
    {

        /// <summary>
        /// 以IO方式进行加载
        /// </summary>
        public static Sprite LoadSprite(string path, int width = 320, int height = 320)
        {
            //创建文件读取流
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            //创建Texture

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            UnityEngine.ImageConversion.LoadImage(texture, bytes);

            //创建Sprite
            Sprite sprite = Sprite.CreateSprite(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 500.0f, 0, SpriteMeshType.FullRect, new Vector4(0,0,0,0), false);



            return sprite;
        }
    }
}
