using System;
using System.Collections.Generic;

using static MyCCompiler.Frontend.TokenKind;

namespace MyCCompiler.Frontend
{
    public class Scanner
    {
        private readonly string _source;
        private readonly ICollection<Token> _tokens = new LinkedList<Token>();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source)
        {
            _source = source;
        }

        public ICollection<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(Eof, string.Empty, null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(LeftParen); break;
                case ')': AddToken(RightParen); break;
                case '{': AddToken(LeftBrace); break;
                case '}': AddToken(RightBrace); break;
                case '[': AddToken(LeftBracket); break;
                case ']': AddToken(RightBracket); break;
                case ',': AddToken(Comma); break;
                case ':': AddToken(Colon); break;
                case ';': AddToken(Semicolon); break;
                case '?': AddToken(Hook); break;
                case '~': AddToken(Tilde); break;
                case '=': AddToken(Match('=') ? EqualEqual : Equal); break;
                case '!': AddToken(Match('=') ? BangEqual : Bang); break;
                case '*': AddToken(Match('=') ? StarEqual : Star); break;
                case '/': AddToken(Match('=') ? SlashEqual : Slash); break;
                case '%': AddToken(Match('=') ? PercentEqual : Percent); break;
                case '^': AddToken(Match('=') ? CaretEqual : Caret); break;
                case '.': AddToken(Match('.', '.') ? DotDotDot : Dot); break;
                case '+':
                    if (Match('=')) AddToken(PlusEqual);
                    else if (Match('+')) AddToken(PlusPlus);
                    else AddToken(Plus);
                    break;
                case '&':
                    if (Match('=')) AddToken(AmpEqual);
                    else if (Match('&')) AddToken(AmpAmp);
                    else AddToken(Amp);
                    break;
                case '|':
                    if (Match('=')) AddToken(PipeEqual);
                    else if (Match('|')) AddToken(PipePipe);
                    else AddToken(Pipe);
                    break;
                case '>':
                    if (Match('=')) AddToken(GreaterEqual);
                    else if (Match('>', '=')) AddToken(GreaterGreaterEqual);
                    else if (Match('>')) AddToken(GreaterGreater);
                    else AddToken(Greater);
                    break;
                case '<':
                    if (Match('=')) AddToken(LessEqual);
                    else if (Match('<', '=')) AddToken(LessLessEqual);
                    else if (Match('<')) AddToken(LessLess);
                    else AddToken(Less);
                    break;
                case '-':
                    if (Match('=')) AddToken(MinusEqual);
                    else if (Match('-')) AddToken(MinusMinus);
                    else if (Match('>')) AddToken(Arrow);
                    else AddToken(Minus);
                    break;

                case '\n': _line++; break;
                case '"': ScanString(); break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                default:
                    if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        Program.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            var text = Extract();

            if (Keywords.TryGetValue(text, out var kind))
            {
                AddToken(kind);
                return;
            }

            AddToken(Identifier, text);
        }

        // TODO: Account for all the different types of numbers
        // https://docs.microsoft.com/en-us/cpp/c-language/c-constants
        private void ScanNumber()
        {
            while (IsDigit(Peek())) Advance();

            var value = Convert.ToInt32(Extract());
            AddToken(Integer, value);
        }

        private void ScanString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                    Program.Error(_line, "Newline in string.");
                }

                Advance();
            }

            // Handle an unterminated string.
            if (IsAtEnd())
            {
                Program.Error(_line, "Unterminated string.");
                return;
            }

            // Advance the closing ".
            Advance();

            // Extract while trimming the surrounding quotes.
            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenKind.String, value);
        }

        private bool Match(char expected)
        {
            if (Peek() != expected)
            {
                return false;
            }

            _current++;
            return true;
        }

        private bool Match(char first, char second)
        {
            if (Peek() != first || PeekNext() != second)
            {
                return false;
            }

            _current += 2;
            return true;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : _source[_current];
        }

        private char PeekNext()
        {
            return IsAtEnd(1) ? '\0' : _source[_current + 1];
        }

        private char PeekNextNext()
        {
            return IsAtEnd(2) ? '\0' : _source[_current + 2];
        }

        private static bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsAtEnd(int offset = 0)
        {
            return _current + offset >= _source.Length;
        }

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private void AddToken(TokenKind kind, object literal = null)
        {
            _tokens.Add(new Token(kind, Extract(), literal, _line));
        }

        private string Extract()
        {
            return _source.Substring(_start, _current - _start);
        }

        private static readonly IDictionary<string, TokenKind> Keywords = new Dictionary<string, TokenKind>
        {
            { "auto", Auto },
            { "register", Register },
            { "static", Static },
            { "extern", Extern },
            { "typedef", Typedef },
            { "void", TokenKind.Void },
            { "char", TokenKind.Char },
            { "short", Short },
            { "int", Int },
            { "long", Long },
            { "float", Float },
            { "double", TokenKind.Double },
            { "signed", Signed },
            { "unsigned", Unsigned },
            { "struct", Struct },
            { "union", Union },
            { "const", Const },
            { "volatile", Volatile },
            { "sizeof", Sizeof },
            { "enum", TokenKind.Enum },
            { "case", Case },
            { "default", Default },
            { "if", If },
            { "else", Else },
            { "switch", Switch },
            { "while", While },
            { "do", Do },
            { "for", For },
            { "goto", Goto },
            { "continue", Continue },
            { "break", Break },
            { "return", Return }
        };
    }
}
