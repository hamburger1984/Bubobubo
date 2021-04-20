using System;

namespace Shared
{
    public class Log
    {
        public static void Debug(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }
    }
}