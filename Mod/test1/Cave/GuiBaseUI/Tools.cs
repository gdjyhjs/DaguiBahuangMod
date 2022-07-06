using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public static void AddScale(GameObject go, float maxScale = 1.2f)
        {
            float scale = 1;
            UIEventListener uiEve = go.AddComponent<UIEventListener>();
            Action enterAction = () =>
            {
                scale = maxScale;
            };
            Action exitAction = () =>
            {
                scale = 1;
            };
            uiEve.onMouseEnterCall = enterAction;
            uiEve.onMouseExitCall = exitAction;
            uiEve.onPressStartCall = exitAction;

            TimerCoroutine timer = null;
            Action action = () =>
            {
                if (go == null)
                {
                    g.timer.Stop(timer);
                    return;
                }
                float size = go.transform.localScale.x;
                size = Mathf.Lerp(size, scale, Time.deltaTime * 30);
                go.transform.localScale = Vector3.one * size;
            };
            timer = g.timer.Frame(action, 1, true);
        }

        public static string GetHttp(string url)
        {

            try
            {
                //创建Get请求

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                request.ContentType = "text/html;charset=UTF-8";

                //接受返回来的数据

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

                string retString = streamReader.ReadToEnd();

                streamReader.Close();

                stream.Close();

                response.Close();

                return (retString);

            }

            catch (Exception e)
            {
                return  ("");
            }

        }

        public static T GetComponentOrAdd<T>(GameObject g) where T : Component
        {
            T t = g.GetComponent<T>();
            if (t == null)
            {
                t = g.AddComponent<T>();
            }
            return t;
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            System.IO.FileStream file = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return (fileName + " 计算MD5值：" + sb.ToString());
        }
    }
}