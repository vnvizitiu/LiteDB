﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB.Shell
{
    public class LiteShell
    {
        public List<ILiteCommand> Commands { get; set; }

        public LiteDatabase Database { get; set; }

        public LiteShell(LiteDatabase db)
        {
            if (db == null) throw new ArgumentNullException("db");

            this.Database = db;
            this.Commands = new List<ILiteCommand>();

            var type = typeof(ILiteCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass);

            foreach (var t in types)
            {
                this.Commands.Add((ILiteCommand)Activator.CreateInstance(t));
            }
        }

        public BsonValue Run(string command)
        {
            if (string.IsNullOrEmpty(command)) return BsonValue.Null;

            var s = new StringScanner(command);

            foreach (var cmd in this.Commands)
            {
                if (cmd.IsCommand(s))
                {
                    if (this.Database == null)
                    {
                        throw new LiteException("No database. Use `open <filename>` to open/create database"); 
                    }

                    return cmd.Execute(this.Database, s);
                }
            }

            throw new LiteException("Command ´" + command + "´ is not a valid command");
        }
    }
}
