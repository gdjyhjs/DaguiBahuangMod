using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOD_wkIh9W
{
    public class ConfDaguiToolItem
    {
        public ConfDaguiToolItem(string q1, string q2, string q3, string q4, string q5, string q6, string q7,
            string q8, string q9, string q10, string q11, string q12)
        {
            func = q1;
            p1 = q2;
            p2 = q3;
            p3 = q4;
            p4 = q5;
            type = q6;
            funccn = q7;
            p1cn = q8;
            p2cn = q9;
            p3cn = q10;
            p4cn = q11;
            typecn = q12;
        }
        public string func;
        public string p1;
        public string p2;
        public string p3;
        public string p4;
        public string type;
        public string funccn;
        public string p1cn;
        public string p2cn;
        public string p3cn;
        public string p4cn;
        public string typecn;

        public string para1 { get { return p1cn; } }
        public string para2 { get { return p2cn; } }
        public string para3 { get { return p3cn; } }
        public string para4 { get { return p4cn; } }
        public string typeName { get { return typecn; } }
        public string titleName { get { return funccn; } }
    }

    public class ConfDaguiToolBase
    {
        public readonly Dictionary<string, ConfDaguiToolItem> allItems = new Dictionary<string, ConfDaguiToolItem>();
        public readonly Dictionary<string, List<ConfDaguiToolItem>> items = new Dictionary<string, List<ConfDaguiToolItem>>();
        public ConfDaguiToolBase(string text)
        {
            var list1 = text.Split('\n');
            foreach (var line in list1)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var list2 = line.Split('\t');
                if (list2.Length < 13)
                    continue;
                if (list2[12].Trim() != ("y"))
                {
                    continue;
                }
                ConfDaguiToolItem item = new ConfDaguiToolItem(list2[0], list2[1], list2[2],
                    list2[3], list2[4], list2[5], list2[6], list2[7], list2[8], list2[9], list2[10], list2[11]);

                if (!items.ContainsKey(item.type))
                {
                    items.Add(item.type, new List<ConfDaguiToolItem>());
                }
                items[item.type].Add(item);
                allItems.Add(item.func, item);
            }
            Console.WriteLine("已填充命令类别：" + items.Count);
            foreach (var item in items)
            {
                Console.WriteLine(item.Key + " 数量： " + item.Value.Count);
            }
        }
    }
}
