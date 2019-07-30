using System;
using System.IO;

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
            throw new NotImplementedException();
        }
    }
}
