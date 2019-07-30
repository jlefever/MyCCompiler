using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyCCompiler.Frontend
{
    public class Program
    {
        private static bool _hadError;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Environment.Exit(-1);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            Run(File.ReadAllText(path));

            if (_hadError)
            {
                Environment.Exit(-1);
            }
        }

        private static void RunPrompt()
        {
            Console.WriteLine("The C Programming Language");
            while (true)
            {
                Console.Write(">> ");
                Run(Console.ReadLine());
                _hadError = false;
            }
        }

        private static void Run(string text)
        {
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();
            Console.WriteLine(string.Join("", tokens));
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }
    }
}
