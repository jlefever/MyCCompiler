using System.IO;

namespace MyCCompiler.DevTools
{
    public class Program
    {
        public static void Main()
        {
            const string directory = "../../../../MyCCompiler.Frontend/";
            const string @namespace = "MyCCompiler.Frontend";

            const string exprName = "Expr";

            var expr = AstGenerator.DefineAst(new[]
            {
                "Binary   : Expr Left, Expr Right, Token Op",
                //"Unary    : Token Op, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                //"Identifier : Token Name"
            }, exprName, @namespace);

            WriteFile(expr, directory, exprName);
        }

        private static void WriteFile(string text, string directory, string name)
        {
            var path = Path.Combine(directory, name + ".cs");
            using var writer = new StreamWriter(path);
            writer.WriteLine(text);
        }
    }
}
