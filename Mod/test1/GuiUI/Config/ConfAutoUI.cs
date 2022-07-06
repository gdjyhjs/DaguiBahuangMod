using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static UIType;

namespace GuiBaseUI
{
    public static class ConfAutoUI
    {
        public static Dictionary<string, List<AutoData>> autoDatas = new Dictionary<string, List<AutoData>>();
        static ConfAutoUI()
        {
            Init();
        }
        public static string dir = "Mods/GuiUI";
        public static void Init()
        {
            if (!Directory.Exists(dir))
            {
                new Log("无法找到配置。" + dir);
                return;
            }
            new Log("初始化配置");
            autoDatas.Clear();
            DirectoryInfo direction = new DirectoryInfo(dir);
            List<string> addFile = new List<string>();
            ReadConf(addFile, direction, true); // 读取json
            GuiUI.Class1.CheckConfig(); // 验证配置文件
            ReadConf(addFile, direction, false); // 读取ui

            foreach (var item in autoDatas)
            {
                item.Value.Sort((a, b) =>
                {
                    if (a.order > b.order)
                    {
                        return 1;
                    } else if (a.order < b.order) {
                        return -1;
                    } else {
                        return 0;
                    }
                });
            }
        }

        private static void ReadConf(List<string> addFile, DirectoryInfo direction, bool readConf)
        {
            FileInfo[] files = direction.GetFiles("*AutoUI." + (readConf ? "json" : "ui"), SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo item = files[i];
                string fileKey = Path.GetFileNameWithoutExtension(item.FullName);
                if (addFile.Contains(fileKey))
                {
                    addFile.Remove(fileKey);
                }
                try
                {
                    addFile.Add(fileKey);
                    string connect;
                    if (readConf)
                    {
                        connect = File.ReadAllText(item.FullName);
                    }
                    else
                    {
                        connect = File.ReadAllText(item.FullName);
                        connect = Tools.GetStringByString(connect);
                        //File.WriteAllText(item.FullName + ".test", connect);
                    }
                    AutoData data = JsonConvert.DeserializeObject<AutoData>(connect);
                    if (!autoDatas.ContainsKey(data.uiType))
                    {
                        autoDatas.Add(data.uiType, new List<AutoData>());
                    }
                    data.path = item.FullName;
                    if (readConf)
                    {
                        if (data.debug == 0)
                        {
                            new Log("压缩UI：" + data.path);
                            string path = data.path;
                            path = path.Replace(".json", ".ui");
                            string str = JsonConvert.SerializeObject(data);
                            string gzip = Tools.GZipCompressString(str);
                            File.WriteAllText(path, gzip);
                        }
                        else
                        {
                            new Log("加入UI："+ data.path);
                            autoDatas[data.uiType].Add(data);
                        }
                    }
                    else
                    {
                        new Log("加入UI：" + data.path);
                        autoDatas[data.uiType].Add(data);
                    }
                }
                catch (Exception e)
                {
                    new Log("配置文件错误。" + item.FullName + "\n" + e.Message);
                }
            }
        }

        public static List<AutoData> GetAutoUI(UITypeBase uiType)
        {
            string uiName = uiType.uiName;
            if (autoDatas.ContainsKey(uiName))
            {
                new Log(uiName+" 获取UI配置 " + autoDatas[uiName].Count);
                return autoDatas[uiName];
            }
            else
            {
                return new List<AutoData>();
            }
        }
    }
}
