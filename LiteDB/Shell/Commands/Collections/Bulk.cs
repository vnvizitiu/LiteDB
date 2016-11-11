﻿using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB.Shell.Commands
{
    internal class CollectionBulk : BaseCollection, IShellCommand
    {
        public bool IsCommand(StringScanner s)
        {
            return this.IsCollectionCommand(s, "bulk");
        }

        public BsonValue Execute(DbEngine engine, StringScanner s)
        {
            var col = this.ReadCollection(engine, s);
            var filename = s.Scan(@".*");
            var fileHandler = LitePlatform.Platform.FileHandler;

            using (var sr = new StreamReader(fileHandler.OpenFileAsStream(filename, true), Encoding.UTF8))
            {
                var docs = JsonSerializer.DeserializeArray(sr);

                return engine.Insert(col, docs.Select(x => x.AsDocument));
            }
        }
    }
}
