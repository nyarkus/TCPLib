using NLog;
using NLog.Config;
using NLog.Targets;

namespace TCPLib.Server
{
    public class Console
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private static bool _inited = false;
        public static LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public static string LogPath { get; private set; } = @"Logs";
        public static bool SaveLogs { get; set; } = true;

        internal static void Initialize(int deleteLogsAfterDays)
        {
            if (_inited) return;

            var time = DateTime.Now;
            Directory.CreateDirectory(LogPath);

            foreach (var file in Directory.GetFiles(LogPath))
            {
                if (new FileInfo(file).CreationTime.AddDays(deleteLogsAfterDays) < DateTime.Now)
                {
                    File.Delete(file);
                }
            }

            LogPath = Path.Combine(LogPath, $"{time:MM.dd HH.mm.ss}.log");

            SetupLogger();

            _logger.Info("Console initialized");
            _inited = true;
        }
        public static void SetupLogger()
        {
            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = LogPath,
                Layout = "${longdate} ${level:uppercase=true} ${message}",
                ArchiveEvery = FileArchivePeriod.Day
            };
            var coloredConsoleTarget = new ColoredConsoleTarget("coloredConsole")
            {
                Layout = "${date:format=HH\\:MM\\:ss} ${level:uppercase=true:color=level} ${message}",    
            };
            coloredConsoleTarget.WordHighlightingRules.Add(new()
            {
                Condition = "level == LogLevel.Debug",
                Regex = @"\b(DEBUG|INFO|WARN|ERROR|FATAL)\b?",
                CompileRegex = true,
                ForegroundColor = ConsoleOutputColor.Black,
                BackgroundColor = ConsoleOutputColor.Yellow,
            });
            coloredConsoleTarget.WordHighlightingRules.Add(new()
            {
                Condition = "level == LogLevel.Info",
                Regex = @"\b(DEBUG|INFO|WARN|ERROR|FATAL)\b?",
                CompileRegex = true,
                ForegroundColor = ConsoleOutputColor.Green,
            });
            coloredConsoleTarget.WordHighlightingRules.Add(new()
            {
                Condition = "level == LogLevel.Warn",
                Regex = @"\b(DEBUG|INFO|WARN|ERROR|FATAL)\b?",
                CompileRegex = true,
                ForegroundColor = ConsoleOutputColor.Yellow,
            });
            coloredConsoleTarget.WordHighlightingRules.Add(new()
            {
                Condition = "level == LogLevel.Error",
                Regex = @"\b(DEBUG|INFO|WARN|ERROR|FATAL)\b?",
                CompileRegex = true,
                ForegroundColor = ConsoleOutputColor.Red,
            });
            coloredConsoleTarget.WordHighlightingRules.Add(new()
            {
                Condition = "level == LogLevel.Fatal",
                Regex = @"\b(DEBUG|INFO|WARN|ERROR|FATAL)\b?",
                CompileRegex = true,
                BackgroundColor = ConsoleOutputColor.Red,
                ForegroundColor = ConsoleOutputColor.Black,
            });
            if(SaveLogs)
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
            config.AddRule(LogLevel, NLog.LogLevel.Fatal, coloredConsoleTarget);

            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }

        private static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        public static void Debug(object log)
        {
            _logger.Debug(log);
        }
        public static void Trace(object log)
        {
            _logger.Trace(log);
        }
        public static void Info(object log)
        {
            _logger.Info(log);
        }
        public static void Warning(object log)
        {
            _logger.Warn(log);
        }
        public static void Error(object log)
        {
            _logger.Error(log);
        }
        public static void Fatal(object log)
        {
            _logger.Fatal(log);
        }
    }
}
