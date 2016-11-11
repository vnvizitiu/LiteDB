﻿using System;
using System.IO;
using LiteDB.Platform;
using Windows.Storage;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;

namespace LiteDB.Tests
{
    public class TestBase
    {
        public TestBase()
        {
            LitePlatform.Initialize(new LitePlatformWindowsStore());
        }
    }

    public class TestPlatform
    {
        public static string GetFullPath(string filename)
        {
            return filename;
        }

        public static long GetFileSize(string filename)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = AsyncHelpers.RunSync(folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists));
            var properties = AsyncHelpers.RunSync(file.GetBasicPropertiesAsync());

            return (long)properties.Size;
        }

        public static void FileWriteAllText(string filename, string content)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = AsyncHelpers.RunSync(folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists));
            var bytes = Encoding.UTF8.GetBytes(content);

            AsyncHelpers.RunSync(async () => await FileIO.WriteBytesAsync(file, bytes));
        }

        public static void DeleteFile(string filename)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = AsyncHelpers.RunSync(folder.GetFileAsync(filename));

#pragma warning disable 4014
            var t = file.DeleteAsync();
#pragma warning restore 4014
        }

        public static string FileReadAllText(string filename)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = AsyncHelpers.RunSync(folder.GetFileAsync(filename));
            var buffer = AsyncHelpers.RunSync(FileIO.ReadBufferAsync(file));
            var arr = buffer.ToArray();
            var res = Encoding.UTF8.GetString(arr, 0, arr.Length);

            return res;
        }
    }
}

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClassAttribute : TestPlatform.UnitTestFramework.TestClassAttribute { }
    public class TestMethodAttribute : TestPlatform.UnitTestFramework.TestMethodAttribute { }
    public class Assert : TestPlatform.UnitTestFramework.Assert { }
}
