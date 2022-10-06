using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace ITMO.Scripts
{
    public class UnityExceptionsCatcher : MonoBehaviour
    {
        private readonly string _globalLogFilePath;

        private UnityExceptionsCatcher()
        {
            const string filePath = ".\\logs\\";
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            _globalLogFilePath = $"{filePath}Unity-{DateTime.Now:dd_MM_yy_HH_mm_ss}.log";
        }

        private void OnEnable() => Application.logMessageReceived += LogCallback;

        private void LogCallback(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Log) return;
            File.AppendAllText(_globalLogFilePath,
                $"[{DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo)}] [{type}] [message] {condition}\n" +
                $"[stacktrace] {stacktrace}\n");
        }

        private void OnDisable() => Application.logMessageReceived -= LogCallback;
    }
}