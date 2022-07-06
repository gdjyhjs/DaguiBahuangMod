using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace GuiBaseUI
{
    public static class ImageManager
    {
        public class GifData
        {
            public string[] list;
            public float[] fram;
        }

        public static bool BuildGif(string path, int buildID)
        {
            string dir = "Mods/Cave/" + buildID;
            if (string.IsNullOrWhiteSpace(path))
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
            try
            {
                Image imgGif = Image.FromFile(path);
                // 从此图像创建一个新的FrameDimension对象
                FrameDimension ImgFrmDim = new FrameDimension(imgGif.FrameDimensionsList[0]);
                // 确定图像中的帧数请注意，所有图像至少包含1帧，但动画GIF将包含1帧以上。
                int n = imgGif.GetFrameCount(ImgFrmDim);

                Directory.CreateDirectory(dir);
                GifData gifData = new GifData();
                gifData.list = new string[n];
                gifData.fram = new float[n];

                // 将每一帧保存为jpeg格式
                for (int i = 0; i < n; i++)
                {
                    imgGif.SelectActiveFrame(ImgFrmDim, i);
                    string name = "Frame" + i + ".png";
                    imgGif.Save(dir + "/" + name, ImageFormat.Png);
                    gifData.list[i] = name;

                    for (int j = 0; j < imgGif.PropertyIdList.Length; j++)//遍历帧属性
                    {
                        if ((int)imgGif.PropertyIdList.GetValue(j) == 0x5100)//如果是延迟时间
                        {
                            PropertyItem pItem = (PropertyItem)imgGif.PropertyItems.GetValue(j);//获取延迟时间属性
                            byte[] delayByte = new byte[4];//延迟时间，以1/100秒为单位
                            delayByte[0] = pItem.Value[i * 4];
                            delayByte[1] = pItem.Value[1 + i * 4];
                            delayByte[2] = pItem.Value[2 + i * 4];
                            delayByte[3] = pItem.Value[3 + i * 4];
                            int delay = BitConverter.ToInt32(delayByte, 0) * 10; //乘以10，获取到毫秒
                            gifData.fram[i] = delay * 0.001f;
                            break;
                        }
                    }
                }
                string config = JsonConvert.SerializeObject(gifData);
                File.WriteAllText(dir + "/config.json", config);
                return true;
            }
            catch (Exception e)
            {
                Print.LogError(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
        // 加载gif
        public static TimerCoroutine LoadGif(UnityEngine.UI.Image image, string dir)
        {
            image.sprite = null;
            TimerCoroutine cor = null;
            if (!Directory.Exists(dir))
            {
                return cor;
            }
            Transform root = image.transform;
            try
            {
                if (!dir.EndsWith("/") || !dir.EndsWith("\\"))
                {
                    dir += "/";
                }
                string config = File.ReadAllText(dir + "config.json");
                GifData gifData = JsonConvert.DeserializeObject<GifData>(config);
                if (gifData.list.Length < 1)
                    return cor;
                DataStruct<Sprite, int, int>[] sprites = new DataStruct<Sprite, int, int>[gifData.list.Length];
                for (int i = 0; i < gifData.list.Length; i++)
                {
                    sprites[i] = LoadSprite(dir + gifData.list[i]);
                }
                image.sprite = sprites[0].t1;
                image.SetNativeSize();
                int idx = 0, max = gifData.list.Length;
                float delay = gifData.fram[idx];
                if (sprites.Length > 1)
                {
                    Action action = () =>
                    {
                        if (image == null)
                        {
                            g.timer.Stop(cor);
                            return;
                        }
                        delay -= Time.deltaTime;
                        if (delay < 0)
                        {
                            idx = (idx + 1) % max;
                            image.sprite = sprites[idx].t1;
                            if (gifData.fram[idx] > 0)
                            {
                                delay = gifData.fram[idx];
                            }
                            else
                            {
                                delay = gifData.fram[0];
                            }
                        }
                    };
                    cor = g.timer.Frame(action, 1, true);
                }
                return cor;
            }
            catch (Exception e)
            {
                Print.LogError(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 以IO方式加载图片
        /// </summary>
        public static DataStruct<Sprite, int, int> LoadSprite(string path)
        {
            Image img = Image.FromFile(path);
            byte[] bytes = File.ReadAllBytes(path);
            var tmpTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            UnityEngine.ImageConversion.LoadImage(tmpTex, bytes);
            //创建Sprite
            Sprite sprite = Sprite.CreateSprite(tmpTex, new Rect(0.0f, 0.0f, tmpTex.width, tmpTex.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect, new Vector4(0, 0, 0, 0), false);
            DataStruct<Sprite, int, int> result = new DataStruct<Sprite, int, int>(sprite, tmpTex.width, tmpTex.height);
            return result;
        }

        /// <summary>
        ///   border:
        ///     The border sizes of the sprite (X=left, Y=bottom, Z=right, W=top).
        /// 以IO方式加载图片
        /// </summary>
        public static DataStruct<Sprite, int, int> LoadSprite(string path, Vector4 border)
        {
            Image img = Image.FromFile(path);
            byte[] bytes = File.ReadAllBytes(path);
            var tmpTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            UnityEngine.ImageConversion.LoadImage(tmpTex, bytes);
            //创建Sprite
            Sprite sprite = Sprite.CreateSprite(tmpTex, new Rect(0.0f, 0.0f, tmpTex.width, tmpTex.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect, border, false);
            DataStruct<Sprite, int, int> result = new DataStruct<Sprite, int, int>(sprite, tmpTex.width, tmpTex.height);
            return result;
        }

        public static TimerCoroutine LoadGuiUISprite(UnityEngine.UI.Image image, string name, bool autoSize = false)
        {
            string path = "Mods/GuiUI/Image/" + name;
            if (!File.Exists(path))
            {
                return null;
            }
            string ex = Path.GetExtension(path);
            if (ex == ".gif")
            {
                Image gifImage = Image.FromFile(path);
                if (gifImage == null)
                    return null;
                // 从此图像创建一个新的FrameDimension对象
                FrameDimension ImgFrmDim = new FrameDimension(gifImage.FrameDimensionsList[0]);
                // 确定图像中的帧数请注意，所有图像至少包含1帧，但动画GIF将包含1帧以上。
                int frameCount = gifImage.GetFrameCount(ImgFrmDim);
                Sprite[] sprites = new Sprite[frameCount];
                float[] frams = new float[frameCount];

                // 将每一帧保存为jpeg格式
                for (int i = 0; i < frameCount; i++)
                {
                    gifImage.SelectActiveFrame(ImgFrmDim, i);
                    Bitmap bigMap = new Bitmap(gifImage.Width, gifImage.Height);
                    System.Drawing.Graphics.FromImage(bigMap).DrawImage(gifImage, Point.Empty);
                    Texture2D frameTexture = new Texture2D(bigMap.Width, bigMap.Height, TextureFormat.ARGB32, false);
                    for (int x = 0; x < gifImage.Width; x++)
                    {
                        for (int y = 0; y < gifImage.Height; y++)
                        {
                            System.Drawing.Color sourceColor = bigMap.GetPixel(x, y);
                            frameTexture.SetPixel(x, bigMap.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                        }
                        frameTexture.Apply();
                        Sprite sprite = Sprite.CreateSprite(frameTexture, new Rect(0.0f, 0.0f, frameTexture.width, frameTexture.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect, new Vector4(0, 0, 0, 0), false);
                        sprites[i] = sprite;
                    }

                    gifImage.Save(name, ImageFormat.Png);

                    for (int j = 0; j < gifImage.PropertyIdList.Length; j++)//遍历帧属性
                    {
                        if ((int)gifImage.PropertyIdList.GetValue(j) == 0x5100)//如果是延迟时间
                        {
                            PropertyItem pItem = (PropertyItem)gifImage.PropertyItems.GetValue(j);//获取延迟时间属性
                            byte[] delayByte = new byte[4];//延迟时间，以1/100秒为单位
                            delayByte[0] = pItem.Value[i * 4];
                            delayByte[1] = pItem.Value[1 + i * 4];
                            delayByte[2] = pItem.Value[2 + i * 4];
                            delayByte[3] = pItem.Value[3 + i * 4];
                            int delay = BitConverter.ToInt32(delayByte, 0) * 10; //乘以10，获取到毫秒
                            frams[i] = delay * 0.001f;
                            break;
                        }
                    }
                }

                TimerCoroutine cor = null;
                Transform root = image.transform;
                try
                {
                    image.sprite = sprites[0];
                    if (autoSize)
                    {
                        image.SetNativeSize();
                    }
                    int idx = 0, max = sprites.Length;
                    float delay = frams[idx];
                    if (sprites.Length > 1)
                    {
                        Action action = () =>
                        {
                            if (image == null)
                            {
                                g.timer.Stop(cor);
                                return;
                            }
                            delay -= Time.deltaTime;
                            if (delay < 0)
                            {
                                idx = (idx + 1) % max;
                                image.sprite = sprites[idx];
                                if (frams[idx] > 0)
                                {
                                    delay = frams[idx];
                                }
                                else
                                {
                                    delay = frams[0];
                                }
                            }
                        };
                        cor = g.timer.Frame(action, 1, true);
                    }
                    return cor;
                }
                catch (Exception e)
                {
                    Print.LogError(e.Message + "\n" + e.StackTrace);
                    return null;
                }
            }
            else
            {
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tmpTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                UnityEngine.ImageConversion.LoadImage(tmpTex, bytes);
                //创建Sprite
                Sprite sprite = Sprite.CreateSprite(tmpTex, new Rect(0.0f, 0.0f, tmpTex.width, tmpTex.height), new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect, new Vector4(0, 0, 0, 0), false);
                image.sprite = sprite;
                if (autoSize)
                {
                    image.SetNativeSize();
                }
            }
            return null;
        }
    }
}
