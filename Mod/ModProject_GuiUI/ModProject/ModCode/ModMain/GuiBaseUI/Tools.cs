using MelonLoader;
using System;
using System.Collections.Generic;
using System.Data;
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

        public static void AddScale(GameObject go, float maxScale = 1.2f, Action enter = null, Action exit = null, float baseScale = 1, GameObject triggerObj = null)
        {
            if (triggerObj == null)
            {
                triggerObj = go;
            }
            float scale = 1;
            UIEventListener uiEve = triggerObj.AddComponent<UIEventListener>();
            Action enterAction = () =>
            {
                enter?.Invoke();
                scale = maxScale;
            };
            Action exitAction = () =>
            {
                exit?.Invoke();
                scale = 1;
            };
            uiEve.onMouseEnterCall = enterAction;
            uiEve.onMouseExitCall = exitAction;
            uiEve.onPressStartCall = exitAction;

            TimerCoroutine timer = null;

            float size = 1;
            Action action = () =>
            {
                if (go == null)
                {
                    g.timer.Stop(timer);
                    return;
                }
                size = Mathf.Lerp(size, scale, Time.deltaTime * 30);
                go.transform.localScale = Vector3.one * size * baseScale;
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
                request.Timeout = 1000;
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
                MelonLogger.Msg(e.Message + "\n" + e.StackTrace);
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
            try
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
                return sb.ToString();
            }
            catch (Exception e)
            {
                Print.LogError("md5 err:"+e.Message+" "+e.StackTrace);
            }
            return "00000000000000000000000000000000";
        }

        #region  压缩和解压字符串
        /// <summary>
        /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
        /// </summary>
        /// <param name="rawString">需要压缩的字符串</param>
        /// <returns>压缩后的Base64编码的字符串</returns>
        public static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }
        }

        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static byte[] Compress(byte[] rawData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 解压Sring 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string GetStringByString(string Value)
        {
            //DataSet ds = new DataSet();
            string CC = GZipDecompressString(Value);
            //System.IO.StringReader Sr = new System.IO.StringReader(CC);
            //ds.ReadXml(Sr);
            return CC;
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static DataSet GetDatasetByString(string Value)
        {
            DataSet ds = new DataSet();
            string CC = GZipDecompressString(Value);
            System.IO.StringReader Sr = new System.IO.StringReader(CC);
            ds.ReadXml(Sr);
            return ds;
        }


        /// <summary>
        /// 将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
            }
        }

        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] zippedData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();

        }
        #endregion


        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址</param>
        /// <param name="Url">图片网址</param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FileName, string Url)
        {
            Print.Log("下载图片   url="+ Url+ "   FileName="+ FileName);
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Timeout = 1000;
                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    Value = SaveBinaryFile(response, FileName);

                }

            }
            catch (Exception err)
            {
                Print.LogError("imgErr:"+err.Message + "\n" + err.StackTrace);
            }
            return Value;
        }
        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}