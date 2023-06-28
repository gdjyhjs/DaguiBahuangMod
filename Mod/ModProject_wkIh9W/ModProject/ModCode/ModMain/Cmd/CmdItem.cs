using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace MOD_wkIh9W
{
    public class CmdItem
    {
        public List<string> cmds = new List<string>();
        public string name = "";
        public CmdKey key = new CmdKey();

        public override string ToString()
        {
            List<string> list = new List<string>(cmds.ToArray());
            list.Insert(0, name + "|" + (key.shift ? "@" : "") + (key.ctrl ? "#" : "") + (key.alt ? "%" : "") + key);
            return string.Join("\n", list);
        }

        public void Run()
        {
            foreach (var item in cmds)
            {
                ModMain.cmdRun.Cmd(item.Split(' '));
            }
        }

        public static CmdItem FromString(string str)
        {
            CmdItem cmd = new CmdItem();
            List<string> list = new List<string>(str.Split('\n'));
            if (list.Count > 0)
            {
                list.RemoveAll((v) => string.IsNullOrWhiteSpace(v));
                for (int i = 1; i < list.Count; i++)
                {
                    cmd.cmds.Add(list[i]);
                }
                var line = list[0];
                string[] data = line.Split('|');
                cmd.name = data[0];
                if (data.Length > 1)
                {
                    string key = data[1];
                    if (key.StartsWith("@"))
                    {
                        cmd.key.shift = true;
                        key = key.Substring(1, key.Length - 1);
                    }
                    if (key.StartsWith("#"))
                    {
                        cmd.key.ctrl = true;
                        key = key.Substring(1, key.Length - 1);
                    }
                    if (key.StartsWith("%"))
                    {
                        cmd.key.alt = true;
                        key = key.Substring(1, key.Length - 1);
                    }
                    cmd.name = key;
                }
            }

            return cmd;
        }
    }

    
}
