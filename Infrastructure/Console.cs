namespace BoxMaker_Server
{
    public class Console
    {
        private const int LevelWidth = 5;
        private const int SourceWidth = 18;
        private static readonly object SyncRoot = new object();

        public static void WriteLine(string text)
        {
            LogEntry entry = Parse(text ?? string.Empty);

            lock (SyncRoot)
            {
                WritePrefix(entry);
                System.Console.WriteLine(entry.Message);

                if (!string.IsNullOrWhiteSpace(entry.Detail))
                {
                    WriteDetail(entry.Detail);
                }

                System.Console.ResetColor();
            }
        }

        private static LogEntry Parse(string text)
        {
            List<string> tags = new List<string>();
            int index = 0;

            while (index < text.Length && text[index] == '[')
            {
                int close = text.IndexOf(']', index);
                if (close <= index)
                {
                    break;
                }

                string tag = text.Substring(index + 1, close - index - 1).Trim();
                if (tag.Length == 0)
                {
                    break;
                }

                tags.Add(tag);
                index = close + 1;
            }

            string body = text.Substring(index).Trim();
            string source = "SERVER";
            string context = string.Empty;

            foreach (string tag in tags)
            {
                if (TryGetLevelFromTag(tag, out _))
                {
                    continue;
                }

                SplitSource(tag, out source, out context);
                break;
            }

            SplitMessage(body, out string message, out string detail);

            if (!string.IsNullOrWhiteSpace(context))
            {
                message = $"{message} ({context})";
            }

            LogLevel level = ResolveLevel(tags, body);
            return new LogEntry(level, NormalizeSource(source), message, detail);
        }

        private static LogLevel ResolveLevel(List<string> tags, string body)
        {
            foreach (string tag in tags)
            {
                if (TryGetLevelFromTag(tag, out LogLevel level))
                {
                    return level;
                }
            }

            if (body.Contains("失败", StringComparison.OrdinalIgnoreCase) ||
                body.Contains("错误", StringComparison.OrdinalIgnoreCase) ||
                body.Contains("exception", StringComparison.OrdinalIgnoreCase))
            {
                return LogLevel.Error;
            }

            if (body.Contains("warn", StringComparison.OrdinalIgnoreCase))
            {
                return LogLevel.Warn;
            }

            return LogLevel.Info;
        }

        private static bool TryGetLevelFromTag(string tag, out LogLevel level)
        {
            switch (tag.ToLowerInvariant())
            {
                case "red":
                case "error":
                case "err":
                    level = LogLevel.Error;
                    return true;
                case "yellow":
                case "warn":
                case "warning":
                    level = LogLevel.Warn;
                    return true;
                case "green":
                case "info":
                case "white":
                    level = LogLevel.Info;
                    return true;
                case "debug":
                case "gray":
                case "grey":
                    level = LogLevel.Debug;
                    return true;
                default:
                    level = LogLevel.Info;
                    return false;
            }
        }

        private static void SplitSource(string value, out string source, out string context)
        {
            int separator = value.IndexOf(" - ", StringComparison.Ordinal);
            if (separator < 0)
            {
                source = value;
                context = string.Empty;
                return;
            }

            source = value.Substring(0, separator);
            context = value.Substring(separator + 3);
        }

        private static void SplitMessage(string body, out string message, out string detail)
        {
            int separator = body.IndexOf(" ：", StringComparison.Ordinal);
            if (separator < 0)
            {
                separator = body.IndexOf(": ", StringComparison.Ordinal);
            }

            if (separator < 0)
            {
                message = string.IsNullOrWhiteSpace(body) ? "(empty log message)" : body;
                detail = string.Empty;
                return;
            }

            message = body.Substring(0, separator).Trim();
            detail = body.Substring(separator + 2).Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "(empty log message)";
            }
        }

        private static string NormalizeSource(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "SERVER";
            }

            string normalized = source.Trim();
            return normalized switch
            {
                "客户端" => "CLIENT",
                "服务器" => "SERVER",
                "服务端" => "SERVER",
                _ => normalized.ToUpperInvariant(),
            };
        }

        private static void WritePrefix(LogEntry entry)
        {
            System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
            System.Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            System.Console.Write(" ");

            System.Console.ForegroundColor = GetLevelColor(entry.Level);
            System.Console.Write(GetLevelName(entry.Level).PadRight(LevelWidth));
            System.Console.Write(" ");

            System.Console.ForegroundColor = System.ConsoleColor.DarkCyan;
            System.Console.Write(TrimToWidth(entry.Source, SourceWidth).PadRight(SourceWidth));

            System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
            System.Console.Write(" | ");

            System.Console.ForegroundColor = System.ConsoleColor.White;
        }

        private static void WriteDetail(string detail)
        {
            string indent = new string(' ', "HH:mm:ss.fff ".Length + LevelWidth + 1 + SourceWidth + 3);
            string[] lines = detail.Replace("\r\n", "\n").Split('\n');

            foreach (string line in lines)
            {
                System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
                System.Console.Write(indent);
                System.Console.Write("> ");
                System.Console.ForegroundColor = System.ConsoleColor.Gray;
                System.Console.WriteLine(line);
            }
        }

        private static string TrimToWidth(string value, int width)
        {
            if (value.Length <= width)
            {
                return value;
            }

            return value.Substring(0, width - 1) + ".";
        }

        private static string GetLevelName(LogLevel level)
        {
            return level switch
            {
                LogLevel.Error => "ERROR",
                LogLevel.Warn => "WARN",
                LogLevel.Debug => "DEBUG",
                _ => "INFO",
            };
        }

        private static System.ConsoleColor GetLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Error => System.ConsoleColor.Red,
                LogLevel.Warn => System.ConsoleColor.Yellow,
                LogLevel.Debug => System.ConsoleColor.DarkGray,
                _ => System.ConsoleColor.Green,
            };
        }

        private enum LogLevel
        {
            Info,
            Warn,
            Error,
            Debug,
        }

        private sealed class LogEntry
        {
            public LogEntry(LogLevel level, string source, string message, string detail)
            {
                Level = level;
                Source = source;
                Message = message;
                Detail = detail;
            }

            public LogLevel Level { get; }

            public string Source { get; }

            public string Message { get; }

            public string Detail { get; }
        }
    }
}
