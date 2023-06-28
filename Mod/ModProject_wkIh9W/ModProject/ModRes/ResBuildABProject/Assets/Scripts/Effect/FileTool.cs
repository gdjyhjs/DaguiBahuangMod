using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class FileTool
{
    /// <summary>
    /// ͬ��д���ı�
    /// </summary>
    /// <param name="filePath">�ļ�·��</param>
    /// <param name="dataStr">��������</param>
    public static void WriteText(string filePath, string dataStr)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(dataStr);
        WriteByte(filePath, bytes);
    }

    /// <summary>
    /// ͬ��д������
    /// </summary>
    /// <param name="filePath">�ļ�·��</param>
    /// <param name="bytes">д�������</param>
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
    /// ͬ����ȡ�ı�
    /// </summary>
    /// <param name="filePath">�ļ�·��</param>
    /// <returns>��ȡ������</returns>
    public static string ReadText(string filePath)
    {
        return Encoding.UTF8.GetString(ReadByte(filePath));
    }

    /// <summary>
    /// ͬ����ȡ����
    /// </summary>
    /// <param name="filePath">�ļ�·��</param>
    /// <returns>��ȡ������</returns>
    public static byte[] ReadByte(string filePath)
    {
        FileStream read = new FileStream(filePath, FileMode.Open);
        var bytes = new byte[read.Length];
        read.Read(bytes, 0, bytes.Length);
        read.Close();

        return bytes;
    }

    /// <summary>
    /// �����ļ�
    /// </summary>
    /// <param name="fileFrom">Դ�ļ�</param>
    /// <param name="fileTo">���Ƶ�����Ŀ¼</param>
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
    /// ����һ���ļ���
    /// </summary>
    /// <param name="folder">�ļ���·��</param>
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
    /// �����ļ���
    /// </summary>
    /// <param name="sourceDirPath">Դ�ļ���</param>
    /// <param name="targetDirPath">���Ƶ������ļ���Ŀ¼</param>
    public static void CopyDirectory(string sourceDirPath, string targetDirPath)
    {
        if (!Directory.Exists(sourceDirPath))
        {
            return;
        }

        //���ָ���Ĵ洢·�������ڣ��򴴽��ô洢·��
        if (!Directory.Exists(targetDirPath))
        {
            //����
            Directory.CreateDirectory(targetDirPath);
        }
        //��ȡԴ·���ļ�������
        string[] files = Directory.GetFiles(sourceDirPath);
        //�������ļ��е������ļ�
        foreach (string file in files)
        {
            string pFilePath = targetDirPath + "/" + Path.GetFileName(file);
            File.Copy(file, pFilePath, true);
        }
        string[] dirs = Directory.GetDirectories(sourceDirPath);
        //�ݹ飬�����ļ���
        foreach (string dir in dirs)
        {
            CopyDirectory(dir, targetDirPath + "/" + Path.GetFileName(dir));
        }
    }

    /// <summary>
    /// ɾ���ļ�
    /// </summary>
    /// <param name="files">�ļ��б�</param>
    public static void DeleteFiles(List<FileInfo> files)
    {
        for (int i = 0; i < files.Count; i++)
        {
            files[i].Delete();
        }
    }

    /// <summary>
    /// ɾ�����ļ���
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
    /// ɾ���ļ���
    /// </summary>
    /// <param name="file">�ļ���·��</param>
    /// <returns>�Ƿ�ɹ�</returns>
    public static bool DeleteDir(string file)
    {
        if (!Directory.Exists(file))
        {
            return false;
        }
        //ȥ���ļ��к����ļ���ֻ������
        //ȥ���ļ��е�ֻ������
        System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
        fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

        //ȥ���ļ���ֻ������
        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
        //�ж��ļ����Ƿ񻹴���
        if (Directory.Exists(file))
        {
            foreach (string f in Directory.GetFileSystemEntries(file))
            {
                if (File.Exists(f))
                {
                    //��������ļ�ɾ���ļ�
                    File.Delete(f);
                    Console.WriteLine(f);
                }
                else
                {
                    //ѭ���ݹ�ɾ�����ļ���
                    DeleteDir(f);
                }
            }

            //ɾ�����ļ���
            Directory.Delete(file);

        }
        return true;
    }

    /// <summary>
    /// ��ȡ�ļ��б�
    /// </summary>
    /// <param name="path">�ļ�·��</param>
    /// <param name="searchPattern">�ļ���չ��</param>
    /// <param name="ingorePattern">���Ե���չ��</param>
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
    /// ��ȡָ����������ʣ��ռ��ܴ�С(��λΪB)  
    /// </summary>
    /// <param name="str_HardDiskName">�̷�C/D/E</param>
    /// <returns></returns>
    public static long GetHardDiskFreeSpace(string str_HardDiskName)
    {
        return 10000;
    }

    /// <summary>
    /// ��ȡ�ļ�md5
    /// </summary>
    /// <param name="fileName">�ļ�·��</param>
    /// <returns>MD5ֵ</returns>
    public static string GetMD5HashFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return "";
        }

        FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);   //����ָ��Stream ����Ĺ�ϣֵ  
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
    /// ִ��bat
    /// </summary>
    /// <param name="exeFilename">bat·��</param>
    /// <param name="workDir">����Ŀ¼</param>
    /// <param name="args">����</param>
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
    /// ��ȡ�ļ��б�
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