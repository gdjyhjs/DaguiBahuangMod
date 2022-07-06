using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave
{
    public class BuildConfig
    {
        public int id;
        public int width;
        public int height;
    }
    public class BgConfig
    {
        public string name;
        public int width;
        public int height;
        public float x;
        public float y;
        public string anchor;
    }

    public class CaveConfig
    {
        public Dictionary<int, BuildConfig> buildConfig = new Dictionary<int, BuildConfig>();
        public List<BgConfig> bgConfig = new List<BgConfig>();

        public CaveConfig()
        {
            Init();
        }

        public void Init()
        {
            string dir = Cave.GetModDir(); ;
            if (!Directory.Exists(dir))
            {
                Cave.Log("无法找到配置。" + dir, 0);
                return;
            }
            DirectoryInfo direction = new DirectoryInfo(dir);
            FileInfo[] files = direction.GetFiles("config_*.txt", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i];
                try
                {
                    string[] text = File.ReadAllText(item.FullName).Split('\n');
                    for (int j = 0; j < text.Length; j++)
                    {
                        string t = text[j];
                        if (string.IsNullOrWhiteSpace(t) || t.IndexOf('#') == 0)
                            continue;
                        string[] data = t.Split(' ');
                        if (data.Length < 2)
                            continue;

                        switch (data[0])
                        {
                            case "build":
                                {
                                    int id = int.Parse(data[1]);
                                    int width = int.Parse(data[2]);
                                    int height = int.Parse(data[3]);
                                    if (!buildConfig.ContainsKey(id))
                                    {
                                        buildConfig.Add(id, new BuildConfig() { id = id, width = width, height = height });
                                    }
                                    break;
                                }
                            case "bg":
                                {
                                    string name = data[1];
                                    int width = int.Parse(data[2]);
                                    int height = int.Parse(data[3]);
                                    float x = float.Parse(data[4]);
                                    float y = float.Parse(data[5]);
                                    string anchor = data[6];
                                    bgConfig.Add(new BgConfig() { name = name, width = width, height = height, x = x, y = y, anchor = anchor });
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Cave.Log("配置文件错误。" + item.FullName + "\n" + e.Message, 0);
                }
            }
        }
    }
}
