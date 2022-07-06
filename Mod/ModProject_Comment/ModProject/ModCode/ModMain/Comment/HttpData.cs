using System;
using Il2CppSystem.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;

namespace Comment
{
    public class HttpData 
    {
        public void GetHttp(UIBase ui, string url, Action<string> call)
        {
            CommentMain.Log("访问：" + url);
            UnityWebRequest getData = UnityWebRequest.Get(url);
            {
                getData.SendWebRequest();
                TimerCoroutine cor = null;
                Action action = () =>
                {
                    if (getData.result != UnityWebRequest.Result.InProgress)
                    {
                        //CommentMain.Log("获取评论结果：" + getData.result);
                        g.timer.Stop(cor);
                        if (getData.result == UnityWebRequest.Result.Success)
                        {
                            string result = "";
                            try
                            {
                                string text = getData.downloadHandler.text;
                                if (text.StartsWith("error"))
                                {
                                    result = text;
                                }
                                else if (text == "ok")
                                {
                                    result = text;
                                }
                                else if (text.StartsWith("{"))
                                {
                                    result = text;
                                }
                                else
                                {
                                    var inputBytes = getData.downloadHandler.data;
                                    using (MemoryStream mem = new MemoryStream())
                                    {
                                        mem.Write(inputBytes, 0, inputBytes.Length);
                                        mem.Position = 0;
                                        using (GZipStream gzip = new GZipStream(mem, CompressionMode.Decompress))
                                        {
                                            using (StreamReader reader = new StreamReader(gzip))
                                            {
                                                result = reader.ReadToEnd();
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                CommentMain.Log("error: http1:" + e.Message + "\n" + e.StackTrace);
                                //CommentMain.Log(result);
                                result = "error: http1";
                            }
                            call.Invoke(result);
                        }
                        else
                        {
                            CommentMain.Log("error:http2:" + getData.result);
                            call.Invoke("error: http2");
                        }
                    }
                    else
                    {
                        //CommentMain.Log("获取评论中");
                    }
                };
                cor = g.timer.Frame(action, 1, true);
                ui.AddCor(cor);
            }
        }
    }
}