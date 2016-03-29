using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDShared.Util
{
    public static class Logger
    {

        public enum LogLevel
        {
            Normal,
            Info,
            Warning,
            Error
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
            }
        }

        private static void ResetConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Log(string tag, string message, LogLevel logLevel = LogLevel.Normal)
        {
            SetConsoleColor(logLevel);

            Console.WriteLine(string.Format("[{0}] {1}", tag, message));

            ResetConsoleColor();
        }

        public static void Log(string tag, string message, LogLevel logLevel = LogLevel.Normal, params object[] args)
        {
            SetConsoleColor(logLevel);

            Console.WriteLine(string.Format("[{0}] {1}", tag, string.Format(message, args)));

            ResetConsoleColor();
        }

    }
}
