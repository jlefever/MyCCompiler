using Antlr4.Runtime;
using MyCCompiler.Compiler;
using System.Collections.Generic;
using System.IO;

namespace MyCCompiler
{
    public class Program
    {
        private static void Main(string[] args)
        {
            CParser parser;

            using (var fileStream = new StreamReader(args[0]))
            {
                var inputStream = new AntlrInputStream(fileStream);
                var lexer = new CLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                parser = new CParser(tokenStream);
            }

            var astBuilder = new AstBuilder();
            var ast = astBuilder.Build(parser.compilationUnit());

            var symbolBuilder = new SymbolBuilder();
            symbolBuilder.Visit(ast);

            var asmBuilder = new AsmBuilder();
            var asm = asmBuilder.Visit(ast);

            if (args.Length == 1)
            {
                var index = args[0].LastIndexOf('.');
                var filename = args[0].Substring(0, index) + ".s";
                WriteAsm(asm, filename);
                return;
            }

            WriteAsm(asm, args[1]);
        }

        private static void WriteAsm(IEnumerable<ILine> lines, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
