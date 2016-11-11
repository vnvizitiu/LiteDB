﻿using System;
using System.IO;
using LiteDB.Platform;

namespace LiteDB.Tests
{
    public class TestBase
    {
        public TestBase()
        {
            LitePlatform.Initialize(new LitePlatformFullDotNet());
        }
    }

    public class TestPlatform
    {
        public static string GetFullPath(string filename)
        {
            return Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../TestResults/") + filename;
        }

        public static long GetFileSize(string filename)
        {
            return new FileInfo(filename).Length;
        }

        public static void FileWriteAllText(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public static void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        public static string FileReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}
