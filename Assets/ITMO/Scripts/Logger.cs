using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ITMO.Scripts
{
    public class Logger
    {
        private readonly string fileName;
        private readonly List<string> list;

        public Logger(params string[] postfixes)
        {
            list = new List<string>();
            const string filePath = ".\\logs\\";
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            var sb = new StringBuilder(filePath + DateTime.Now.ToString("dd_MM_yy_HH_mm_ss"));
            foreach (var s in postfixes) sb.Append("_").Append(s);
            fileName = sb + ".csv";
        }

        public void AddInfo(string infoToLog) => list.Add(infoToLog);

        public void WriteInfo()
        {
            File.AppendAllLines(fileName, list);
            list.Clear();
        }

        
    }
}