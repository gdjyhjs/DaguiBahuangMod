using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuiBaseUI
{
    public static class Tools
    {
        public static T FindObjectOfType<T>() where T : UnityEngine.Object
        {
            UnhollowerBaseLib.Il2CppArrayBase<T> list = UnityEngine.Object.FindObjectsOfType<T>();
            if (list.Length < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        public static List<T> FindObjectsOfType<T>() where T : UnityEngine.Object
        {
            UnhollowerBaseLib.Il2CppArrayBase<T> list = UnityEngine.Object.FindObjectsOfType<T>();
            List<T> result = new List<T>(list);
            return result;
        }
    }
}