using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ����AB��Դ
/// </summary>
public class CreateAssetBundleEditor
{
    /// <summary>
    /// AB·��
    /// </summary>
    private static string abPath { get { return "Resources"; } }

    /// <summary>
    /// AB����ļ���
    /// </summary>
    private static string outputPath { get { return Application.dataPath + "/AssetBundle"; } }

    [MenuItem("��Ϸ����/����/����AB")]
    static void ABBundle()
    {
        //���֮ǰ���ù���AssetBundleName�������������Ҫ����ԴҲ���
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int j = 0; j < bundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(bundleNames[j], true);
        }

        //����AB����
        Pack(Application.dataPath + "/" + abPath);

        //���д��AB
        BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        //ɾ����ʹ�õ�AB
        DeleteNotUseAB();

        AssetDatabase.Refresh();

        Debug.Log("����AB�ɹ�������");
    }

    /// <summary>
    /// ����·������AB������
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
    /// ����AB����
    /// </summary>
    static void SetABName(string source)
    {
        string abName = source.Replace(new DirectoryInfo(Application.dataPath + "/" + abPath).FullName + "\\", "");
        abName = abName.Substring(0, abName.LastIndexOf(".")).Replace("\\", "/");

        string abAssetPath = "Assets/" + source.Replace(new DirectoryInfo(Application.dataPath).FullName + "\\", "");
        AssetImporter assetImporter = AssetImporter.GetAtPath(abAssetPath);
        assetImporter.assetBundleName = "ab/" + abName + ".ab";

        //����������AB
        string[] deps = AssetDatabase.GetDependencies(abAssetPath);
        for (int i = 0; i < deps.Length; i++)
        {
            //�ű�����Ҫ������
            if (Path.GetExtension(deps[i]) == ".cs")
            {
                continue;
            }

            //��������Ĳ�������
            if ("Assets" + source.Replace("\\", "/").Replace(Application.dataPath, "") == deps[i])
            {
                continue;
            }

            AssetImporter.GetAtPath(deps[i]).assetBundleName = deps[i] + ".ab";
        }
    }

    /// <summary>
    /// ɾ����ʹ�õ�AB
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

        //ɾ�������.manifest
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