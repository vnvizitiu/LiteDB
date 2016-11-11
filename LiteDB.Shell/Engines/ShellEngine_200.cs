﻿extern alias v200;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using v200::LiteDB;

namespace LiteDB.Shell
{
    class ShellEngine_200 : IShellEngine
    {
        private LiteDatabase _db;

        public Version Version { get { return typeof(LiteDatabase).Assembly.GetName().Version; } }

        public bool Detect(string filename)
        {
            using (var s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var header = new byte[4096];
                s.Read(header, 0, 4096);
                return header[52] >= 5; // FILE_VERSION
            }
        }

        public void Open(string connectionString)
        {
            _db = new LiteDatabase(connectionString);
            _db.Log.Logging += ShellProgram.LogMessage;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Debug(bool enabled)
        {
            _db.Log.Level = enabled ? Logger.FULL : Logger.NONE;
        }

        public void Run(string command, Display display)
        {
            var result = _db.Run(command);

            this.WriteResult(result, display);
        }

        public void Dump(TextWriter writer)
        {
            var mapper = new BsonMapper().UseCamelCase();

            foreach (var name in _db.GetCollectionNames())
            {
                var col = _db.GetCollection(name);
                var indexes = col.GetIndexes().Where(x => x.Field != "_id");

                writer.WriteLine("-- Collection '{0}'", name);

                foreach (var index in indexes)
                {
                    writer.WriteLine("db.{0}.ensureIndex {1} {2}",
                        name,
                        index.Field,
                        JsonSerializer.Serialize(mapper.ToDocument<IndexOptions>(index.Options)));
                }

                foreach (var doc in col.Find(Query.All()))
                {
                    writer.WriteLine("db.{0}.insert {1}", name, JsonSerializer.Serialize(doc, false, true));
                }
            }
        }

        #region Display Bson Result

        private void WriteResult(BsonValue result, Display display)
        {
            var index = 0;

            if (result.IsNull) return;

            if (result.IsDocument)
            {
                display.WriteLine(ConsoleColor.DarkCyan, JsonSerializer.Serialize(result, display.Pretty, false));
            }
            else if (result.IsArray)
            {
                foreach (var doc in result.AsArray)
                {
                    display.Write(ConsoleColor.Cyan, string.Format("[{0}]:{1}", ++index, display.Pretty ? Environment.NewLine : " "));
                    display.WriteLine(ConsoleColor.DarkCyan, JsonSerializer.Serialize(doc, display.Pretty, false));
                }

                if (index == 0)
                {
                    display.WriteLine(ConsoleColor.DarkCyan, "no documents");
                }
            }
            else if (result.IsString)
            {
                display.WriteLine(ConsoleColor.DarkCyan, result.AsString);
            }
            else
            {
                display.WriteLine(ConsoleColor.DarkCyan, JsonSerializer.Serialize(result, display.Pretty, false));
            }
        }

        #endregion
    }
}