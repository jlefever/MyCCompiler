using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public enum RegisterKind
    {
        Eax,
        Ebx,
        Ecx,
        Edx,
        Esi,
        Edi,
        Esp,
        Ebp
    }

    public static class RegisterKindExtensions
    {
        public static string GetName(this RegisterKind registerKind)
        {
            return RegisterKindMap[registerKind];
        }

        private static readonly IDictionary<RegisterKind, string> RegisterKindMap = new Dictionary<RegisterKind, string>
        {
            { RegisterKind.Eax, "eax" },
            { RegisterKind.Ebx, "ebx" },
            { RegisterKind.Ecx, "ecx" },
            { RegisterKind.Edx, "edx" },
            { RegisterKind.Esi, "esi" },
            { RegisterKind.Edi, "edi" },
            { RegisterKind.Esp, "esp" },
            { RegisterKind.Ebp, "ebp" }
        };
    }

    public interface IOperand { }

    public interface IWritableOperand : IOperand { }

    public class Memory : IWritableOperand
    {
        public RegisterKind RegisterKind { get; }
        public int Offset { get; }

        public Memory(RegisterKind registerKind, int offset)
        {
            RegisterKind = registerKind;
            Offset = offset;
        }

        public override string ToString()
        {
            if (Offset == 0)
            {
                return $"%({RegisterKind.GetName()})";
            }

            return $"{Offset}(%{RegisterKind.GetName()})";
        }
    }

    public class Register : IWritableOperand
    {
        public RegisterKind RegisterKind { get; }

        public Register(RegisterKind registerKind)
        {
            RegisterKind = registerKind;
        }

        public override string ToString()
        {
            return "%" + RegisterKind.GetName();
        }
    }

    public class IntegerConstant : IOperand
    {
        public int Integer { get; }

        public IntegerConstant(int integer)
        {
            Integer = integer;
        }

        public override string ToString()
        {
            return "$" + Integer;
        }
    }

    public class TextConstant : IOperand
    {
        public string Text { get; }

        public TextConstant(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return "$" + Text;
        }
    }

    public interface ILine { }

    public class Push : ILine
    {
        public IOperand Operand { get; }

        public Push(IOperand operand)
        {
            Operand = operand;
        }

        public override string ToString()
        {
            return $"pushl\t{Operand}";
        }
    }

    public class Mov : ILine
    {
        public IOperand Source { get; }
        public IWritableOperand Destination { get; }

        public Mov(IOperand source, IWritableOperand destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"movl\t{Source}, {Destination}";
        }
    }

    public class Call : ILine
    {
        public string Text { get; }

        public Call(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $"call\t{Text}";
        }
    }

    public class Globl : ILine
    {
        public string Text { get; }

        public Globl(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $".globl {Text}";
        }
    }

    public class Label : ILine
    {
        public string Text { get; }

        public Label(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $"{Text}:";
        }
    }

    public class Leave : ILine
    {
        public override string ToString()
        {
            return "leave";
        }
    }

    public class Ret : ILine
    {
        public override string ToString()
        {
            return "ret";
        }
    }
}
