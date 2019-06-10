/*************************
 * 
 * 文件夹操作类
 * 
 * 
 **************************/
using System;
using System.IO;

namespace XGame
{
    public static class XFolderTools
    {
        public static bool Exists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            if (Exists(path))
                return;
            Directory.CreateDirectory(path);
        }

        public static void DeleteDirectory(string path)
        {
            if (!Exists(path))
                return;
            Directory.Delete(path);
        }

        public static void DeleteDirectory(string path, bool recursive)
        {
            if (!Exists(path))
                return;
            Directory.Delete(path, recursive);
        }

        public static void MoveDirectory(string srcDir, string destDir)
        {
            if (!Exists(srcDir))
                return;
            Directory.Move(srcDir, destDir);
        }

        public static bool CheckNullFolder(string path)
        {
            if (!Exists(path))
                return true;
            DirectoryInfo folder = new DirectoryInfo(path);
            FileSystemInfo[] files = folder.GetFileSystemInfos();
            return files.Length <= 0;
        }

        public static string GetFolderName(string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            return folder.Parent.Name;
        }

        // 遍历文件
        public static void TraverseFiles(string dir, Action<string> callBack, bool isTraverse = false, bool isLower = true)
        {
            if (!Exists(dir) || callBack == null)
            {
                return;
            }

            DirectoryInfo folder = new DirectoryInfo(dir);
            FileSystemInfo[] files = folder.GetFileSystemInfos();
            for (int i = 0, length = files.Length; i < length; i++)
            {
                var file = files[i];
                if (file is DirectoryInfo)
                {
                    if (isTraverse)
                    {
                        TraverseFiles(file.FullName, callBack, isTraverse, isLower);
                    }
                }
                else
                {
                    string fullPath = file.FullName.Replace('\\', '/');
                    if (isLower)
                    {
                        fullPath = fullPath.ToLower();
                    }
                    callBack(fullPath);
                }
            }
        }

        // 遍历文件夹
        public static void TraverseFolder(string dir, Action<string> callBack, bool isTraverse = false, bool isLower = true)
        {
            if (!Exists(dir) || callBack == null)
            {
                return;
            }

            DirectoryInfo folder = new DirectoryInfo(dir);
            FileSystemInfo[] files = folder.GetFileSystemInfos();
            for (int i = 0, length = files.Length; i < length; i++)
            {
                var file = files[i];
                if (file is DirectoryInfo)
                {
                    if (isTraverse)
                    {
                        TraverseFolder(file.FullName, callBack, isTraverse, isLower);
                    }
                    string fullPath = file.FullName.Replace('\\', '/');
                    if (isLower)
                    {
                        fullPath = fullPath.ToLower();
                    }
                    callBack(fullPath);
                }
            }
        }
    }
}
