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
    public class CmdKey
    {
        public bool shift = false;//@
        public bool ctrl = false;//#
        public bool alt = false;//%
        public string key = "";

        public override string ToString()
        {
            if (string.IsNullOrEmpty(key))
            {
                return "";
            }
            return (shift ? "Shift+" : "") + (ctrl ? "Ctrl+" : "") + (alt ? "Alt+" : "") + key;
        }

        public string GetState()
        {
            return (shift ? "Shift+" : "") + (ctrl ? "Ctrl+" : "") + (alt ? "Alt+" : "") + key;
        }

        public bool IsKeyDown()
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            if (shift && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                return false;
            }
            if (ctrl && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                return false;
            }
            if (alt && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            {
                return false;
            }
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), key);
            return Input.GetKeyDown(keyCode);
        }
    }
}
