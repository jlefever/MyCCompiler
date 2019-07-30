namespace MyCCompiler.Frontend
{
    public enum TokenKind
    {
        // Single-character tokens
        LeftParen, RightParen, LeftBrace, RightBrace, LeftBracket,
        RightBracket, Comma, Colon, Semicolon, Hook, Tilde,

        // One, two, or three character tokens
        Equal, EqualEqual,
        Bang, BangEqual,
        Greater, GreaterEqual, GreaterGreater, GreaterGreaterEqual,
        Less, LessEqual, LessLess, LessLessEqual,
        Star, StarEqual,
        Slash, SlashEqual,
        Percent, PercentEqual,
        Plus, PlusEqual, PlusPlus,
        Minus, MinusEqual, MinusMinus, Arrow, // AKA "->"
        Amp, AmpEqual, AmpAmp,
        Pipe, PipeEqual, PipePipe,
        Caret, CaretEqual,
        Dot, DotDotDot,
        
        // Literals
        Identifier, String, Integer, Floating, Character,

        // Keywords
        Auto, Register, Static, Extern, Typedef, Void, Char, Short, Int, Long,
        Float, Double, Signed, Unsigned, Struct, Union, Const, Volatile,
        Sizeof, Enum, Case, Default, If, Else, Switch, While, Do, For, Goto,
        Continue, Break, Return,

        Eof
    }
}
