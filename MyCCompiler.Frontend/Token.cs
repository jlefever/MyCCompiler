namespace MyCCompiler.Frontend
{
    public class Token
    {
        public TokenKind Kind { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        public Token(TokenKind kind, string lexeme, object literal, int line)
        {
            Kind = kind;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return Kind + " " + Lexeme + " " + Literal;
        }
    }
}
