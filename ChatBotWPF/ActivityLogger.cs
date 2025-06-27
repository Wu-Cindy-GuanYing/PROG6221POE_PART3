using System;
using System.IO;
using System.Diagnostics;

namespace ChatBotWPF
{
    public static class ActivityLogger
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, $"activity_{DateTime.Now:yyyyMMdd}.log");

        static ActivityLogger()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] - {message}{Environment.NewLine}";

            try
            {
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Debug
        }
    }
}