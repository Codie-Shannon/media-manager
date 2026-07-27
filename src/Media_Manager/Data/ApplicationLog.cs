using System;
using System.IO;
using System.Text;

namespace Media_Manager.Data
{
    public static class ApplicationLog
    {
        private const long MaximumLogBytes = 2 * 1024 * 1024;
        private static readonly object Sync = new object();
        private static string logPath;

        public static string LogPath
        {
            get
            {
                EnsureInitialized();
                return logPath;
            }
        }

        public static void Initialize(string dataDirectory)
        {
            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                return;
            }

            try
            {
                string directory = Path.Combine(
                    Path.GetFullPath(dataDirectory),
                    "Logs");
                Directory.CreateDirectory(directory);
                logPath = Path.Combine(directory, "MediaManager.log");
                Info("Application logging initialized.");
            }
            catch
            {
                logPath = null;
            }
        }

        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        public static void Warning(string message)
        {
            Write("WARN", message, null);
        }

        public static void Error(string message, Exception exception = null)
        {
            Write("ERROR", message, exception);
        }

        private static void Write(
            string level,
            string message,
            Exception exception)
        {
            try
            {
                EnsureInitialized();
                if (string.IsNullOrWhiteSpace(logPath))
                {
                    return;
                }

                lock (Sync)
                {
                    RotateIfRequired();
                    string safeMessage = (message ?? string.Empty)
                        .Replace("\r", " ")
                        .Replace("\n", " ");
                    StringBuilder line = new StringBuilder();
                    line.Append(DateTime.UtcNow.ToString("O"));
                    line.Append(" [");
                    line.Append(level);
                    line.Append("] ");
                    line.Append(safeMessage);
                    if (exception != null)
                    {
                        line.Append(" | ");
                        line.Append(exception.GetType().Name);
                        line.Append(": ");
                        line.Append(
                            (exception.Message ?? string.Empty)
                                .Replace("\r", " ")
                                .Replace("\n", " "));
                    }

                    File.AppendAllText(
                        logPath,
                        line + Environment.NewLine,
                        Encoding.UTF8);
                }
            }
            catch
            {
                // Logging must never become a second application failure.
            }
        }

        private static void EnsureInitialized()
        {
            if (!string.IsNullOrWhiteSpace(logPath))
            {
                return;
            }

            string fallback = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "Media_Manager");
            Initialize(fallback);
        }

        private static void RotateIfRequired()
        {
            if (!File.Exists(logPath)
                || new FileInfo(logPath).Length < MaximumLogBytes)
            {
                return;
            }

            string previous = logPath + ".1";
            if (File.Exists(previous))
            {
                File.Delete(previous);
            }

            File.Move(logPath, previous);
        }
    }
}
