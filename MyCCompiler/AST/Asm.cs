namespace MyCCompiler.AST
{
    public interface IOperand { }

    public interface IWritableOperand : IOperand { }

    public class Register : IWritableOperand
    {
        public static Register Eax = new Register("eax");
        public static Register Ebx = new Register("ebx");
        public static Register Ecx = new Register("ecx");
        public static Register Edx = new Register("edx");
        public static Register Esi = new Register("esi");
        public static Register Edi = new Register("edi");
        public static Register Esp = new Register("esp");
        public static Register Ebp = new Register("ebp");

        public string Name { get; }

        private Register(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "%" + Name;
        }
    }

    public class Memory : IWritableOperand
    {
        public Register Register { get; }
        public int Offset { get; }

        public Memory(Register register, int offset)
        {
            Register = register;
            Offset = offset;
        }

        public override string ToString()
        {
            if (Offset == 0)
            {
                return $"({Register})";
            }

            return $"{Offset}({Register})";
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

    public class And : ILine
    {
        public IOperand Source { get; }
        public IWritableOperand Destination { get; }

        public And(IOperand source, IWritableOperand destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"andl\t{Source}, {Destination}";
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
