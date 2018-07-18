using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawlerWpfClient
{
    public sealed class DefaultLogger : ILogger
    {
        private readonly static DefaultLogger _defaultLogger = new DefaultLogger();
        private readonly static object _lockConsole = new object();
        private readonly static object _lockFile = new object();
        private string _fileLogPath = "LogDatas.log";
        private DefaultLogger()
        {

        }
        public static DefaultLogger GetInstance()
        {
            return _defaultLogger;
        }
        public static DefaultLogger Instance { get => _defaultLogger; }
        public event Action<object, LogLevel, string> LoggerEvent;

        public void Debug(string even)
        {
            Log(LogLevel.Debug, even);
        }

        public void Info(string even)
        {
            Log(LogLevel.Info, even);
        }

        public void Warning(string even)
        {
            Log(LogLevel.Warning, even);
        }

        public void Error(string even)
        {
            Log(LogLevel.FatalError, even);
        }

        public void FatalError(string even)
        {
            Log(LogLevel.Error, even);
        }
        private void Log(LogLevel historyLogLevel, string even)
        {
            //ConsoleLog(historyLogLevel, even);
            FileLog(historyLogLevel, even);
            OnHistoryLog(historyLogLevel, even);
        }
        private void OnHistoryLog(LogLevel historyLogLevel, string even)
        {
            LoggerEvent?.Invoke(this, historyLogLevel, even);
        }
        private void ConsoleLog(LogLevel historyLogLevel, string even)
        {
            lock (_lockConsole)
            {
                switch (historyLogLevel)
                {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.FatalError:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(even);
            }

        }
        private void FileLog(LogLevel level, string even)
        {
            try
            {
                lock (_lockFile)
                {
                    using (var sw = System.IO.File.AppendText(_fileLogPath))
                    {
                        sw.WriteLine($"[{level}]{even}");
                        sw.Flush();
                        sw.Close();
                    }
                }

            }
            catch
            {
                //Todo
            }

        }
    }
    public interface ILogger
    {
        void Debug(string even);
        void Info(string even);
        void Warning(string even);
        void FatalError(string even);
        void Error(string even);
        event Action<object, LogLevel, string> LoggerEvent;
    }
    public enum LogLevel
    {
        Debug = 0,
        Info,
        Warning,
        Error,
        FatalError
    }
}
