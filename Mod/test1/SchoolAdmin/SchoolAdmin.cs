using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;

namespace SchoolAdmin
{
    public class SchoolAdmin : MelonMod
    {
        public bool isInit = false;
        public static bool isDebug = true;
        public static bool isCheckMd5 = false;
        public static bool isTrue
        {
            get
            {
                Log("是否最新版本="+ isCheckMd5);
                return isCheckMd5;
            }
        } // 是否正版用户
        public static string trueTips
        {
            get
            {
                return GetHttp("http://www.yellowshange.com/caveMod/SchoolAdmin/Md5Tips");
            }
        } // 非正版用户的提示

        public override void OnPreferencesLoaded()
        {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            string md5 =  GetMD5HashFromFile("Mods/SchoolAdmin.dll").Trim().ToLower();
            var newMd5 = GetHttp("http://www.yellowshange.com/caveMod/SchoolAdmin/Md5").Trim().ToLower();
            isCheckMd5 = newMd5 == "0" || md5 == newMd5 || string.IsNullOrWhiteSpace(newMd5);
            Log("宗门管家MOD 欢迎关注我bilibili：八荒大鬼  我的QQ群：50948165 md5:[" + md5 + "] newmd5=[" + newMd5 + "]  new=[" + (md5 == newMd5) + "]", true);
        }

        public static void Log(string str, bool show = false)
        {
            if (show || isDebug)
            {
                MelonLogger.Msg(str);
            }
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
            return (sb.ToString());
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
                return ("");
            }

        }
    }
}