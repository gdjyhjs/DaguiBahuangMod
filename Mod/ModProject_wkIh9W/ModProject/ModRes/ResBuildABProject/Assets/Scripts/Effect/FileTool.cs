using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class FileTool
{
    /// <summary>
    /// 同步写入文本
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="dataStr">数据内容</param>
    public static void WriteText(string filePath, string dataStr)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(dataStr);
        WriteByte(filePath, bytes);
    }

    /// <summary>
    /// 同步写入数据
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="bytes">写入的数据</param>
    public static void WriteByte(string filePath, byte[] bytes)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        FileStream writer = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        writer.Write(bytes, 0, bytes.Length);
        writer.Close();
    }

    /// <summary>
    /// 同步读取文本
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>读取的内容</returns>
    public static string ReadText(string filePath)
    {
        return Encoding.UTF8.GetString(ReadByte(filePath));
    }

    /// <summary>
    /// 同步读取数据
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>读取的数据</returns>
    public static byte[] ReadByte(string filePath)
    {
        FileStream read = new FileStream(filePath, FileMode.Open);
        var bytes = new byte[read.Length];
        read.Read(bytes, 0, bytes.Length);
        read.Close();

        return bytes;
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    /// <param name="fileFrom">源文件</param>
    /// <param name="fileTo">复制到的新目录</param>
    public static void CopyFile(String fileFrom, String fileTo)
    {
        String[] folders = fileTo.Replace("\\", "/").Split('/');

        String dir = folders[0];
        for (int i = 1; i < folders.Length - 1; i++)
        {
            dir += "/" + folders[i];

            MakeFolder(dir);
        }

        File.Copy(fileFrom, fileTo, true);
    }

    /// <summary>
    /// 创建一个文件夹
    /// </summary>
    /// <param name="folder">文件夹路径</param>
    public static void MakeFolder(String folder)
    {

        if (Directory.Exists(folder))
        {
            return;
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(folder);
        if (directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }
    }

    /// <summary>
    /// 复制文件夹
    /// </summary>
    /// <param name="sourceDirPath">源文件夹</param>
    /// <param name="targetDirPath">复制到的新文件夹目录</param>
    public static void CopyDirectory(string sourceDirPath, string targetDirPath)
    {
        if (!Directory.Exists(sourceDirPath))
        {
            return;
        }

        //如果指定的存储路径不存在，则创建该存储路径
        if (!Directory.Exists(targetDirPath))
        {
            //创建
            Directory.CreateDirectory(targetDirPath);
        }
        //获取源路径文件的名称
        string[] files = Directory.GetFiles(sourceDirPath);
        //遍历子文件夹的所有文件
        foreach (string file in files)
        {
            string pFilePath = targetDirPath + "/" + Path.GetFileName(file);
            File.Copy(file, pFilePath, true);
        }
        string[] dirs = Directory.GetDirectories(sourceDirPath);
        //递归，遍历文件夹
        foreach (string dir in dirs)
        {
            CopyDirectory(dir, targetDirPath + "/" + Path.GetFileName(dir));
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="files">文件列表</param>
    public static void DeleteFiles(List<FileInfo> files)
    {
        for (int i = 0; i < files.Count; i++)
        {
            files[i].Delete();
        }
    }

    /// <summary>
    /// 删除空文件夹
    /// </summary>
    public static void DeleteEmptyDir(string parentFolder)
    {
        if (!Directory.Exists(parentFolder))
        {
            return;
        }

        var dir = new DirectoryInfo(parentFolder);
        var subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);

        foreach (var subdir in subdirs)
        {
            if (!Directory.Exists(subdir.FullName)) continue;

            var subFiles = subdir.GetFileSystemInfos("*.*", SearchOption.AllDirectories);

            var findFile = false;
            foreach (var sub in subFiles)
            {
                findFile = (sub.Attributes & FileAttributes.Directory) == 0;

                if (findFile) break;
            }

            if (!findFile)
            {
                subdir.Delete(true);
                if (File.Exists(subdir.FullName + ".meta"))
                {
                    File.Delete(subdir.FullName + ".meta");
                }
            }
        }
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="file">文件夹路径</param>
    /// <returns>是否成功</returns>
    public static bool DeleteDir(string file)
    {
        if (!Directory.Exists(file))
        {
            return false;
        }
        //去除文件夹和子文件的只读属性
        //去除文件夹的只读属性
        System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
        fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

        //去除文件的只读属性
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
        //判断文件夹是否还存在
        if (Directory.Exists(file))
        {
            foreach (string f in Directory.GetFileSystemEntries(file))
            {
                if (File.Exists(f))
                {
                    //如果有子文件删除文件
                    File.Delete(f);
                    Console.WriteLine(f);
                }
                else
                {
                    //循环递归删除子文件夹
                    DeleteDir(f);
                }
            }

            //删除空文件夹
            Directory.Delete(file);

        }
        return true;
    }

    /// <summary>
    /// 获取文件列表
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="searchPattern">文件扩展名</param>
    /// <param name="ingorePattern">忽略的扩展名</param>
    /// <returns></returns>
    public static List<FileInfo> GetFiles(string path, string searchPattern = "*.*", List<string> ingorePattern = null)
    {

        List<FileInfo> files = new List<FileInfo>();
        GetFiles(path, searchPattern, files);

        if (ingorePattern != null)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (ingorePattern.Contains(Path.GetExtension(files[i].FullName).Replace(".", "")))
                {
                    files.RemoveAt(i);
                    i--;
                }
            }
        }

        return files;
    }

    /// <summary>
    /// 获取指定驱动器的剩余空间总大小(单位为B)  
    /// </summary>
    /// <param name="str_HardDiskName">盘符C/D/E</param>
    /// <returns></returns>
    public static long GetHardDiskFreeSpace(string str_HardDiskName)
    {
        return 10000;
    }

    /// <summary>
    /// 获取文件md5
    /// </summary>
    /// <param name="fileName">文件路径</param>
    /// <returns>MD5值</returns>
    public static string GetMD5HashFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return "";
        }

        FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);   //计算指定Stream 对象的哈希值  
        file.Close();
        file.Dispose();

        StringBuilder Ac = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            Ac.Append(retVal[i].ToString("x2"));
        }
        return Ac.ToString();
    }

    /// <summary>
    /// 执行bat
    /// </summary>
    /// <param name="exeFilename">bat路径</param>
    /// <param name="workDir">工作目录</param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    public static bool ExecuteProgram(string exeFilename, string args)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
        info.FileName = exeFilename;
        info.WorkingDirectory = Path.GetDirectoryName(exeFilename);
        info.UseShellExecute = true;
        info.Arguments = args;
        info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        System.Diagnostics.Process task = null;
        bool rt = true;
        try
        {
            task = System.Diagnostics.Process.Start(info);
            if (task != null)
            {
                task.WaitForExit();
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ExecuteProgram:" + e.ToString());
            return false;
        }
        finally
        {
            if (task != null && task.HasExited)
            {
                rt = (task.ExitCode == 0);
            }
        }

        return rt;
    }

    /// <summary>
    /// 获取文件列表
    /// </summary>
    private static void GetFiles(string path, string searchPattern, List<FileInfo> files)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        DirectoryInfo dirInfo = new DirectoryInfo(path);

        files.AddRange(dirInfo.GetFiles(searchPattern));
        foreach (System.IO.DirectoryInfo subdir in dirInfo.GetDirectories())
        {
            GetFiles(subdir.FullName, searchPattern, files);
        }
    }
}