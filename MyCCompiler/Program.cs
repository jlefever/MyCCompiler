using Antlr4.Runtime;
using MyCCompiler.Analysis;
using System.IO;
using MyCCompiler.AST;

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

            var compilationUnit = parser.compilationUnit().Accept(new CompilationUnitVisitor());
        }
    }
}
