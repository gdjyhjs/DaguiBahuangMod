using Chinese;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD_wkIh9W
{
    public class FindTool
    {
        public Dictionary<string, string> cache_py = new Dictionary<string, string>();
        public Dictionary<string, string> cache_pinyin = new Dictionary<string, string>();
        public string[] findStr = new string[0];

        public void SetFindStr(string findName)
        {
            if (string.IsNullOrWhiteSpace(findName))
            {
                findStr = new string[0];
            }
            else
            {
                findStr = findName.Split(' ');
            }
        }
        public bool CheckFind(string itemName)
        {
            if (findStr.Length == 0)
            {
                return true;
            }

            foreach (var item in findStr)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                if (itemName.Contains(item))
                {
                    return true;
                }

            }
            string pinyinStr;
            try
            {
                pinyinStr = Pinyin.GetString(itemName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine($"itemName=[{itemName}]");
                return false;
            }
            string pinyin = null, py = null;
            if (cache_pinyin.ContainsKey(itemName))
            {
                pinyin = cache_pinyin[itemName];
            }
            else if (UnityEngine.PlayerPrefs.HasKey(itemName + "_pinyin"))
            {
                pinyin = UnityEngine.PlayerPrefs.GetString(itemName + "_pinyin");
            }
            if (cache_py.ContainsKey(itemName))
            {
                py = cache_py[itemName];
            }
            else if (UnityEngine.PlayerPrefs.HasKey(itemName + "_py"))
            {
                py = UnityEngine.PlayerPrefs.GetString(itemName + "_py");
            }

            try
            {
                if (pinyin == null || py == null)
                {
                    if (ModMain.chinaInit != 0)
                    {
                        return false;
                    }

                    string[] list = pinyinStr.Split(' ');

                    List<string> pinyinList = new List<string>();
                    List<string> pyList = new List<string>();

                    foreach (var item in list)
                    {
                        if (string.IsNullOrWhiteSpace(item) || item.Length < 2)
                        {
                            continue;
                        }
                        pinyinList.Add(item.Substring(0, item.Length - 1));
                        pyList.Add(item.Substring(0, 1));
                    }

                    pinyin = string.Join("", pinyinList);
                    py = string.Join("", pyList);
                    UnityEngine.PlayerPrefs.SetString(itemName + "_pinyin", pinyin);
                    UnityEngine.PlayerPrefs.SetString(itemName + "_py", py);
                    cache_pinyin.Add(itemName, pinyin);
                    cache_py.Add(itemName, py);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("拼音搜索错误：["+ itemName+"]\n"+e.ToString());
            }

            foreach (var item in findStr)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                if (pinyin.Contains(item))
                {
                    return true;
                }
                if (py.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
