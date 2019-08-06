using System;
using System.Linq;

using static MyCCompiler.Frontend.TokenKind;

namespace MyCCompiler.Frontend
{
    public class Parser
    {
        private class ParseError : Exception { }

        private readonly Token[] _tokens;
        private int _current;

        public Parser(Token[] tokens)
        {
            _tokens = tokens;
        }

        public Expr Expression()
        {
            try
            {
                return BinaryHelper(AssignmentExpression, Comma);
            }
            catch (ParseError)
            {
                return null;
            }
        }

        private Expr AssignmentExpression()
        {
            return ConditionalExpression();
        }

        private Expr ConditionalExpression()
        {
            return LogicalOrExpression();
        }

        private Expr LogicalOrExpression()
        {
            return BinaryHelper(LogicalAndExpression, PipePipe);
        }

        private Expr LogicalAndExpression()
        {
            return BinaryHelper(InclusiveOrExpression, AmpAmp);
        }

        private Expr InclusiveOrExpression()
        {
            return BinaryHelper(ExclusiveOrExpression, Pipe);
        }

        private Expr ExclusiveOrExpression()
        {
            return BinaryHelper(AndExpression, Caret);
        }

        private Expr AndExpression()
        {
            return BinaryHelper(EqualityExpression, Amp);
        }

        private Expr EqualityExpression()
        {
            return BinaryHelper(RelationalExpression, EqualEqual, BangEqual);
        }

        private Expr RelationalExpression()
        {
            return BinaryHelper(ShiftExpression, Less, Greater, LessEqual, GreaterEqual);
        }

        private Expr ShiftExpression()
        {
            return BinaryHelper(AdditiveExpression, LessLess, GreaterGreater);
        }

        private Expr AdditiveExpression()
        {
            return BinaryHelper(MultiplicativeExpression, Plus, Minus);
        }

        private Expr MultiplicativeExpression()
        {
            return BinaryHelper(CastExpression, Star, Slash, Percent);
        }

        private Expr CastExpression()
        {
            return UnaryExpression();
        }

        private Expr UnaryExpression()
        {
            return PostfixExpression();
        }

        private Expr PostfixExpression()
        {
            return PrimaryExpression();
        }

        private Expr PrimaryExpression()
        {
            if (Match(Integer))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(LeftParen))
            {
                var expr = Expression();
                Consume(RightParen, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenKind kind, string message)
        {
            if (Check(kind)) return Advance();
            throw Error(Peek(), message);
        }

        private static ParseError Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParseError();
        }

        private Expr BinaryHelper(Func<Expr> next, params TokenKind[] kinds)
        {
            var expr = next();

            while (Match(kinds))
            {
                var op = Previous();
                var right = next();
                expr = new Binary(expr, right, op);
            }

            return expr;
        }

        private bool Match(params TokenKind[] kinds)
        {
            if (!kinds.Any(Check)) return false;
            Advance();
            return true;
        }

        private bool Check(TokenKind kind)
        {
            if (IsAtEnd()) return false;
            return Peek().Kind == kind;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Kind == Eof;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
