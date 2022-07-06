﻿using Newtonsoft.Json;
using System.IO;

namespace Cave.Config
{


    public static class DataCustom
    {
        public struct V2
        {
            public float x;
            public float y;
        }
        public struct Color
        {
            public int r;
            public int g;
            public int b;
            public int a;
        }
        public struct Outline
        {
            public Color color;
            public V2 distance;
        }
        public struct FontCustom
        {
            public V2 position;
            public int size;
            public int type;
            public int direction;
            public Color color;
            public Outline outline;
        }
        public class CaveNameCustom
        {
            public int id;
            public FontCustom custom = new FontCustom();
        }

        public class Custom
        {
            public CaveNameCustom[] cavename = new CaveNameCustom[0];
        }

        public static Custom data = new Custom();

        static DataCustom()
        {
            Init();
        }

        public static void Init()
        {
            if (!Directory.Exists("Mods/Cave"))
            {
                Cave.Log("无法找到配置。" + "Mods/Cave", 0);
                Cave.Log("请检查Cave文件夹是否正常。" + "Mods/Cave", 0);
                return;
            }

            try
            {
                string data = File.ReadAllText("Mods/Cave/DataCustom.json");
                DataCustom.data = JsonConvert.DeserializeObject<Custom>(data);
            }
            catch (System.Exception)
            {
                Cave.Log("Mods/Cave/DataCustom.json 格式错误，请检查！", 0);
            }
        }

        public static void SaveCustom()
        {
            File.WriteAllText("Mods/Cave/DataCustom.json.bak", JsonConvert.SerializeObject(data));
        }

        public static CaveNameCustom GetCaveNameCustom(int id)
        {
            for (int i = 0; i < data.cavename.Length; i++)
            {
                if (data.cavename[i].id == id)
                {
                    return data.cavename[i];
                }
            }
            return null;
        }
    }
}