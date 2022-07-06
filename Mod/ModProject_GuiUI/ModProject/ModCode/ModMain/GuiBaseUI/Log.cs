
using MelonLoader;
using System;
using UnityEngine;

namespace GuiBaseUI
{

    public static class Print
    {
        public static void Log(string str, string title = "[大鬼GUI]")
        {
            LogTitle(title);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void LogWarring(string str, string title = "[大鬼GUI]")
        {
            LogTitle(title);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void LogError(string str, string title = "[大鬼GUI]")
        {
            LogTitle(title);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogTitle(string title = "[大鬼GUI]")
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToString("hh:mm:ss.fff"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(title);
        }
    }
}