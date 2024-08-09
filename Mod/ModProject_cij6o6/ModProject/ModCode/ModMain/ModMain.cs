using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace MOD_cij6o6
{
    /// <summary>
    /// 此类是模组的主类
    /// </summary>
    public class ModMain
    {
        private TimerCoroutine corUpdate;
		private static HarmonyLib.Harmony harmony;

        /// <summary>
        /// MOD初始化，进入游戏时会调用此函数
        /// </summary>
        public void Init()
        {
			//使用了Harmony补丁功能的，需要手动启用补丁。
			//启动当前程序集的所有补丁
			if (harmony != null)
			{
				harmony.UnpatchSelf();
				harmony = null;
			}
			if (harmony == null)
			{
				harmony = new HarmonyLib.Harmony("MOD_cij6o6");
			}
			harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine("MOD初始化，进入游戏时会调用此函数");
            corUpdate = g.timer.Frame(new Action(OnUpdate), 1, true);
        }

        /// <summary>
        /// MOD销毁，回到主界面，会调用此函数并重新初始化MOD
        /// </summary>
        public void Destroy()
        {
            g.timer.Stop(corUpdate);
        }

        /// <summary>
        /// 每帧调用的函数
        /// </summary>
        private void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Console.WriteLine("按键触发保存图片");
                SaveTopUIPNG();
            }
        }

        private void SaveTopUIPNG()
        {
            var ui = g.ui.GetLayerTopUI(UILayer.UI);
            if (ui == null)
                return;
            var dir = "D:/pngs/" + ui.uiType.uiName;
            Console.WriteLine("保存目录：" + dir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var imgs = ui.GetComponentsInChildren<Image>();
            List<string> cachePngs = new List<string>();

            foreach (var img in imgs)
            {
                if (img == null)
                    continue;
                var sprite = img.sprite;
                if (sprite == null)
                    continue;
                var fileName = img.sprite.texture.name + ".png";
                if (!cachePngs.Contains(fileName))
                {
                    var path = Path.Combine(dir, fileName);
                    cachePngs.Add(path);
                    SaveSprite(sprite, path);
                }
            }
        }

        void SaveSprite(Sprite sprite, string fileName)
        {
            fileName.Replace(":", "_");
            Console.WriteLine("保存 " + sprite + " 到 " + fileName);

            //// 获取Sprite的Texture2D
            //Texture2D texture = sprite.texture;

            //// 创建一个新的Texture2D，用于保存Sprite的区域
            //Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            //Color[] pixels = texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            //croppedTexture.SetPixels(pixels);
            //croppedTexture.Apply();

            //// 将Texture2D编码为PNG格式
            //byte[] pngData = UnityEngine.ImageConversion.EncodeToPNG(croppedTexture);

            //// 保存PNG数据到文件
            //string path = Path.Combine(Application.persistentDataPath, fileName);
            //File.WriteAllBytes(path, pngData);

            // Console.WriteLine("Sprite saved to: " + path);




            // 创建一个RenderTexture，大小与Sprite相同

            RenderTexture rt = new RenderTexture((int)sprite.texture.width, (int)sprite.texture.height, 0);
            rt.Create();

            // 创建一个新的Texture2D，用于保存RenderTexture的内容
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);

            // 将Sprite渲染到RenderTexture
            Graphics.Blit(sprite.texture, rt);

            // 从RenderTexture读取像素数据
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            // 将Texture2D编码为PNG格式
            byte[] pngData = UnityEngine.ImageConversion.EncodeToPNG(tex);

            // 保存PNG数据到文件
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(path, pngData);

            // 清理
            RenderTexture.active = null;
            rt.Release();
            GameObject.Destroy(tex);

            Console.WriteLine("Sprite saved to: " + path);
        }
    }
}
