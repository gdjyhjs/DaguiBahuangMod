using System;
using System.Reflection;
using System.Collections.Generic;
using GuiBaseUI;
using System.IO;
using Newtonsoft.Json;

namespace GuiUI
{
    public class GuiUI
    {
        public static bool verified;

        public GuiUI()
        {
            string log = "欢迎使用八荒大鬼MOD 请关注我bilibili：八荒大鬼  我的QQ群：50948165 我的主页：www.yellowshange.com";
            Print.Log(log);
            Print.LogWarring(log);
            Print.LogError(log);

            string[] dirList = new string[]{
                "Mods/GuiUI/Image",
                "Mods/GuiUI/Log",
            };
            foreach (var dir in dirList)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        // 验证配置文件
        public static void CheckConfig()
        {
            CheckData();
        }

        static bool makeFileList = true;
        private static void MakeFileList()
        {
            if (!makeFileList)
                return;
            string dir = "Mods/GuiUI";
            if (!Directory.Exists(dir))
            {
                Print.LogError("无法找到配置。" + dir);
                return;
            }
            DirectoryInfo direction = new DirectoryInfo(dir);
            FileInfo[] allui = direction.GetFiles("*AutoUI.ui", SearchOption.AllDirectories);
            string gamepath = Path.GetFullPath("./");
            List<UIData> list = new List<UIData>();
            for (int i = 0; i < allui.Length; i++)
            {
                var item = allui[i];
                string fullName = item.FullName;
                string path = fullName.Replace(gamepath, "").Replace('\\', '/');
                string md5 = Tools.GetMD5HashFromFile(path);
                UIData data = new UIData() { path = path, md5 = md5 };
                list.Add(data);
            }
            string listData = JsonConvert.SerializeObject(list);
            File.WriteAllText("Mods/GuiUI/Log/listData.json", listData);
            File.WriteAllText("Mods/GuiUI/Log/listData.ui", Tools.GZipCompressString(listData));
        }

        // 检查数据
        private static void CheckData()
        {
            /*
            string errPath = "Mods/GuiUI/无法联网提示AutoUI.ui";
            string errTipsUI = "H4sIAAAAAAAAC9VWW2/TMBT+L37u2vSS0lWlEgOBkJhAwJ4ImtwkzYzSOEoc2KgqdUKFDTaNXQBtGgLEELeJgYYEjAJ/Bqfd0/4CJ5eG0W6IbvBApObY+Y7r4+989nEVKWrJ0VBeiCFqKaqF8oPhE0MOuThhqiiPzlCNGCiGDKqo5zAbg0/nKWXwhVnYsMvUqtgof6mKiAKQW3/Vmn4JIJYZueqNF6BjYks1GHR4Y53P327fmHGXN7zv1CaMUAPlq2gc5QfSghCHYCagmcxCsxZDNrmuhnAu10GTghCiMtY7cLIDeva6b8GDVLAGHoaj6zFUchjzZgt6TB1nnXaFKk7wT2awxiHMmK4mhqlhs8QV4lxTyZgDIctUp5bnB6+UKMaQFtpSaLFvYWZaLtsqC4MT4rlwXXGxJ+5cGHcujDsHHhaekLENw4NlWlTXo7ippunhmmqxkHi+8am98dB9cjMid+8MRAnq4j4TUZ/O9TCfjphP7UH8wG+ZryLMdAwSQcdppUI9KYUIUhzgFJe0vmhlviyTuxhK1v4ks0GvHybd+7fc1fX9tdzDeBelP9Xaw2c6wsR++dxfyVVg0TBU2QtudLTgU3rUUpViwZv6aFootr7Mtz6vuvca2yurO82VQsIDioWE7xqifG6jfeMLv7UVuH3furnTnHEfPHY377lP6+6jZ+5q3b3/tjU9NXz2xE5zEiiCRmvpHW+8bs8u8LXn2+tN3qzzuXkYyN/M8MYL3phtv/8Ezh6ZDqP+kaNghr2gy9RgF3xuUmInvcKvihB8PQi+GoTdW8xhOjHUYOm/c4cDA9wVYjNsyN0Hhsd3DWCsE82o+MkVundg/yqanuVf63xxiS++DN7fP97hUwutrW8HV9TAYLYTc2YwntpPVCnxX2mqVBztfgqJUvGAaYWs+Jnybe/5+T8l9zA5FaOcZvc/KA6ZUz+PSjivH2HZMeSwS03VGLH00THGTDufSNgmltV4iejE+8VlWkkks5mMOHgk6w3ERIfBwdiAlj3FEpA01Hq9zBub7uaL3efDfyyY5IEE07X2PaXSqw0xqruZ3utQNhlpI5fu7zb0sybvqsY61WjcNA5SjYWuDfUXqvHlWOcmdjIvSZ5GJMlmKq6EBpumLUmyf6WQJKC1/fZDwLIkDVMFsFMOGTktSUHpak8uQXlz5+621raOgfJGTsev2BBi7QfMgCXkBQsAAA==";
            try
            {
                verified = true;
                string fileListStr = Tools.GetHttp("http://www.yellowshange.com/guiUIMod/GetList.php");
                List<UIData> fileList = JsonConvert.DeserializeObject<List<UIData>>(fileListStr);
                foreach (var item in fileList)
                {
                    bool isui = Path.GetExtension(item.path) == ".ui";
                    string newmd5 = item.md5;
                    string oldmd5 = Tools.GetMD5HashFromFile(item.path);
                    if (oldmd5 != newmd5)
                    {
                        if (isui)
                        {
                            string url = "http://www.yellowshange.com/guiUIMod/fileList/" + Path.GetFileName(item.path);
                            string guidemo = Tools.GetHttp(url);
                            if (guidemo == "0" || string.IsNullOrWhiteSpace(guidemo))
                            {
                                if (verified)
                                {
                                    File.WriteAllText(errPath, errTipsUI);
                                    verified = false;
                                }
                            }
                            else
                            {
                                try
                                {
                                    File.WriteAllText(item.path, guidemo);
                                }
                                catch (Exception e1)
                                {
                                    Print.LogError("BB文件验证失败。");
                                    File.WriteAllText(errPath, errTipsUI);
                                    Print.LogError(e1.Message + "\n" + e1.StackTrace);
                                }
                            }
                        }
                        else
                        {
                            string url = "http://www.yellowshange.com/guiUIMod/imgList/" + Path.GetFileName(item.path);
                            try
                            {
                                Tools.SavePhotoFromUrl(item.path, url);
                            }
                            catch (Exception)
                            {
                                if (verified)
                                {
                                    Print.LogError("DD文件验证失败。");
                                    File.WriteAllText(errPath, errTipsUI);
                                    verified = false;
                                }
                            }
                        }
                    }
                    if (verified)
                    {
                        if (File.Exists(errPath))
                        {
                            File.Delete(errPath);
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                Print.LogError("CC文件验证失败。");
                File.WriteAllText(errPath, errTipsUI);
                Print.LogError(e2.Message + "\n" + e2.StackTrace);
            }
            */
        }

        class UIData
        {
            public string path;
            public string md5;
        }
    }
}
