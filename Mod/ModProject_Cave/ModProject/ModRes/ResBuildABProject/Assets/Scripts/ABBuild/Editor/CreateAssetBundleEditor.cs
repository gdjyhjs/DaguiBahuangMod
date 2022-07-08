using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 创建AB资源
/// </summary>
public class CreateAssetBundleEditor
{
    /// <summary>
    /// AB路径
    /// </summary>
    private static string abPath { get { return "Resources"; } }

    /// <summary>
    /// AB输出文件夹
    /// </summary>
    private static string outputPath { get { return Application.dataPath + "/AssetBundle"; } }

    [MenuItem("游戏工具/更新/更新AB")]
    static void ABBundle()
    {
        //清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int j = 0; j < bundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(bundleNames[j], true);
        }

        //设置AB名称
        Pack(Application.dataPath + "/" + abPath);

        //进行打包AB
        BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        //删除不使用的AB
        DeleteNotUseAB();

        AssetDatabase.Refresh();

        Debug.Log("更新AB成功！！！");
    }

    /// <summary>
    /// 根据路径设置AB按名称
    /// </summary>
    private static void Pack(string abPath)
    {
        DirectoryInfo folder = new DirectoryInfo(abPath);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                Pack(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    SetABName(files[i].FullName);
                }
            }
        }
    }

    /// <summary>
    /// 设置AB名称
    /// </summary>
    static void SetABName(string source)
    {
        string abName = source.Replace(new DirectoryInfo(Application.dataPath + "/" + abPath).FullName + "\\", "");
        abName = abName.Substring(0, abName.LastIndexOf(".")).Replace("\\", "/");

        string abAssetPath = "Assets/" + source.Replace(new DirectoryInfo(Application.dataPath).FullName + "\\", "");
        AssetImporter assetImporter = AssetImporter.GetAtPath(abAssetPath);
        assetImporter.assetBundleName = "ab/" + abName + ".ab";

        //设置依赖的AB
        string[] deps = AssetDatabase.GetDependencies(abAssetPath);
        for (int i = 0; i < deps.Length; i++)
        {
            //脚本不需要打依赖
            if (Path.GetExtension(deps[i]) == ".cs")
            {
                continue;
            }

            //依赖自身的不做处理
            if ("Assets" + source.Replace("\\", "/").Replace(Application.dataPath, "") == deps[i])
            {
                continue;
            }

            AssetImporter.GetAtPath(deps[i]).assetBundleName = deps[i] + ".ab";
        }
    }

    /// <summary>
    /// 删除不使用的AB
    /// </summary>
    private static void DeleteNotUseAB()
    {
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        Dictionary<string, string> bundleNamesDict = new Dictionary<string, string>();
        for (int i = 0; i < bundleNames.Length; i++)
        {
            bundleNamesDict[bundleNames[i]] = "";
        }

        string outputPath2 = new DirectoryInfo(outputPath).FullName;
        List<FileInfo> files = FileTool.GetFiles(outputPath2);
        foreach (var item in files)
        {
            string extension = Path.GetExtension(item.FullName);
            if (extension == ".manifest")
            {
                continue;
            }

            string abName = item.FullName.Replace(outputPath2 + "\\", "").Replace("\\", "/");
            if (abName == "AssetBundle")
            {
                continue;
            }

            if (!bundleNamesDict.ContainsKey(abName))
            {
                File.Delete(outputPath2 + "/" + abName);
                File.Delete(outputPath2 + "/" + abName + ".manifest");
            }
        }
        FileTool.DeleteEmptyDir(outputPath2);

        //删除多余的.manifest
        List<FileInfo> fileInfos = FileTool.GetFiles(outputPath2, "*.manifest");
        for (int i = 0; i < fileInfos.Count; i++)
        {
            if (Path.GetFileName(fileInfos[i].FullName) == "AssetBundle.manifest")
            {
                continue;
            }
            File.Delete(fileInfos[i].FullName);
        }
    }
}