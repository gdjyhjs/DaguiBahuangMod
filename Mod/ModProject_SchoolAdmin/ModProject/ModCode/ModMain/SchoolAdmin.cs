using System;
using System.Text;
using System.Reflection;
using MelonLoader;
using System.Net;
using System.IO;
using GuiBaseUI;

namespace SchoolAdmin
{
    public class SchoolAdmin
    {

        public SchoolAdmin()
        {
            Log("宗门管家加载成功 ");
        }

        public static void Log(string str, bool show = false)
        {
            Print.Log(str, "[宗门管家]");
        }
    }
}