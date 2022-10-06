using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ITMO.Scripts
{
    public static class Level
    {
        /// <summary>
        /// Dictionary that contains all levels. <br />
        /// Key - name of the file <br />
        /// Value - path to the file
        /// </summary>
        private static Dictionary<string, string> _allLevels;
        private static readonly string MainDir = $"{Application.dataPath}\\..\\Molecules\\";

        public static LinkedList<string> LevelNamesList;
        public static string CurrentLevelName;
        public static LinkedListNode<string> CurrentLevelNode;

        public static void Initialize()
        {
            if (!Directory.Exists(MainDir)) return;
            var watcher = new FileSystemWatcher(MainDir, "*.*");
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;
            
            try
            {
                GetFiles();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                GetFiles();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static void GetFiles()
        {
            _allLevels = new Dictionary<string, string>();
            if (Directory.Exists(MainDir))
            {
                var dirFiles = Directory.EnumerateFiles(MainDir);
                foreach (var s in dirFiles)
                {
                    if (Path.GetExtension(s).Equals(".xml")) continue;
                    var filename = Path.GetFileNameWithoutExtension(s);
                    if (!File.Exists($"{MainDir}{filename}.xml")) continue;
                    _allLevels.Add(filename, $"{MainDir}{filename}");
                }
            }

            LevelNamesList = new LinkedList<string>(_allLevels.Keys);
        }

        public static string GetLevelPath(string lvl) => _allLevels[lvl];
    }
}