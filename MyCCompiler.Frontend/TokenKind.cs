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
        Star, StarEqual,
        Slash, SlashEqual,
        Percent, PercentEqual,
        Caret, CaretEqual,
        Dot, DotDotDot,
        Plus, PlusEqual, PlusPlus,
        Amp, AmpEqual, AmpAmp,
        Pipe, PipeEqual, PipePipe,
        Greater, GreaterEqual, GreaterGreater, GreaterGreaterEqual,
        Less, LessEqual, LessLess, LessLessEqual,
        Minus, MinusEqual, MinusMinus, Arrow, // AKA "->"

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
