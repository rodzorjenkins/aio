using System;

namespace BDShared.Util
{
    public static class Logger
    {

        public enum LogLevel
        {
            Normal,
            Info,
            Warning,
            Error,
            Script,
            Config
        }

        private static void SetConsoleColor(LogLevel logLevel)
        {
            switch(logLevel)
            {
                case LogLevel.Normal:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Script:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogLevel.Config:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
        }

        private static void ResetConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Log(string tag, string message, LogLevel logLevel = LogLevel.Normal)
        {
            SetConsoleColor(logLevel);

            Console.Write(string.Format("[{0}] ", tag));

            ResetConsoleColor();

            Console.WriteLine(message);
        }

        public static void Log(string tag, string message, LogLevel logLevel = LogLevel.Normal, params object[] args)
        {
            SetConsoleColor(logLevel);

            Console.Write(string.Format("[{0}] ", tag));
            
            ResetConsoleColor();

            Console.WriteLine(string.Format(message, args));
        }

    }
}
