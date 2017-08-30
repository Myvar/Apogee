using System;

namespace Apogee
{
    public static class Terminal
    {
        /// <summary>
        /// Debug level log
        /// </summary>
        /// <param name="s">message</param>
        public static void Debug(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("DEBUG");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        /// <summary>
        /// Standart level log
        /// </summary>
        /// <param name="s">message</param>
        public static void Log(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("LOG");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}