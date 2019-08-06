using System.Text;

namespace MyCCompiler.Frontend
{
    public class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Paren(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Paren("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            return expr.Value == null ? "null" : expr.Value.ToString();
        }

        private string Paren(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append("(").Append(name);

            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }
    }
}
