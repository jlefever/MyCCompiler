using System.Resources;

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
        public static Register Ax = new Register("ax");
        public static Register Bx = new Register("bx");
        public static Register Cx = new Register("cx");
        public static Register Dx = new Register("dx");
        public static Register Ah = new Register("ah");
        public static Register Bh = new Register("bh");
        public static Register Ch = new Register("ch");
        public static Register Dh = new Register("dh");
        public static Register Al = new Register("al");
        public static Register Bl = new Register("bl");
        public static Register Cl = new Register("cl");
        public static Register Dl = new Register("dl");

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

    public class LabeledData : IOperand
    {
        public string Text { get; }

        public LabeledData(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return "$" + Text;
        }
    }

    public class LabeledCode : IOperand
    {
        public string Text { get; }

        public LabeledCode(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public interface ILine { }

    public abstract class UnaryInstruction : ILine
    {
        public string Instruction { get; }
        public IOperand Operand { get; }

        protected UnaryInstruction(string instruction, IOperand operand)
        {
            Instruction = instruction;
            Operand = operand;
        }

        public override string ToString()
        {
            return $"\t{Instruction}\t{Operand}";
        }
    }

    public class Push : UnaryInstruction
    {
        public Push(IOperand destination) : base("push", destination) { }
    }

    public class Pop : UnaryInstruction
    {
        public Pop(IOperand destination) : base("pop", destination) { }
    }

    public class Idiv : UnaryInstruction
    {
        public Idiv(IOperand destination) : base("idiv", destination) { }
    }

    public class Sete : UnaryInstruction
    {
        public Sete(IOperand destination) : base("sete", destination) { }
    }

    public class Setne : UnaryInstruction
    {
        public Setne(IOperand destination) : base("setne", destination) { }
    }

    public class Setg : UnaryInstruction
    {
        public Setg(IOperand destination) : base("setg", destination) { }
    }

    public class Setge : UnaryInstruction
    {
        public Setge(IOperand destination) : base("setge", destination) { }
    }

    public class Setl : UnaryInstruction
    {
        public Setl(IOperand destination) : base("setl", destination) { }
    }

    public class Setle : UnaryInstruction
    {
        public Setle(IOperand destination) : base("setle", destination) { }
    }

    public class Jmp : UnaryInstruction
    {
        public Jmp(LabeledCode destination) : base("jmp", destination) { }
    }

    public class Je : UnaryInstruction
    {
        public Je(LabeledCode destination) : base("je", destination) { }
    }

    public class Jne : UnaryInstruction
    {
        public Jne(LabeledCode destination) : base("jne", destination) { }
    }

    public class Jz : UnaryInstruction
    {
        public Jz(LabeledCode destination) : base("jz", destination) { }
    }

    public class Call : UnaryInstruction
    {
        public Call(LabeledCode destination) : base("call", destination) { }
    }

    public class Neg : UnaryInstruction
    {
        public Neg(IWritableOperand destination) : base("negl", destination) { }
    }

    public class Not : UnaryInstruction
    {
        public Not(IWritableOperand destination) : base("notl", destination) { }
    }

    public abstract class BinaryInstruction : ILine
    {
        public string Instruction { get; }
        public IOperand Source { get; }
        public IWritableOperand Destination { get; }

        protected BinaryInstruction(string instruction, IOperand source, IWritableOperand destination)
        {
            Instruction = instruction;
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"\t{Instruction}\t{Source}, {Destination}";
        }
    }

    public class Mov : BinaryInstruction
    {
        public Mov(IOperand source, IWritableOperand destination)
            : base("movl", source, destination) { }
    }

    public class And : BinaryInstruction
    {
        public And(IOperand source, IWritableOperand destination)
            : base("andl", source, destination) { }
    }

    public class Or : BinaryInstruction
    {
        public Or(IOperand source, IWritableOperand destination)
            : base("orl", source, destination) { }
    }

    public class Xor : BinaryInstruction
    {
        public Xor(IOperand source, IWritableOperand destination)
            : base("xorl", source, destination) { }
    }

    public class Sub : BinaryInstruction
    {
        public Sub(IOperand source, IWritableOperand destination)
            : base("subl", source, destination) { }
    }

    public class Add : BinaryInstruction
    {
        public Add(IOperand source, IWritableOperand destination)
            : base("addl", source, destination) { }
    }

    public class Imul : BinaryInstruction
    {
        public Imul(IOperand source, IWritableOperand destination)
            : base("imull", source, destination) { }
    }

    public class Cmp : BinaryInstruction
    {
        public Cmp(IOperand source, IWritableOperand destination)
            : base("cmpl", source, destination) { }
    }

    public class Shl : BinaryInstruction
    {
        public Shl(IOperand source, IWritableOperand destination)
            : base("shl", source, destination) { }
    }

    public class Shr : BinaryInstruction
    {
        public Shr(IOperand source, IWritableOperand destination)
            : base("shr", source, destination) { }
    }

    public class Lea : BinaryInstruction
    {
        public Lea(IOperand source, IWritableOperand destination)
            : base("leal", source, destination) { }
    }

    public class Leave : ILine
    {
        public override string ToString()
        {
            return "\tleave";
        }
    }

    public class Ret : ILine
    {
        public override string ToString()
        {
            return "\tret";
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

    public class GloblDirective : ILine
    {
        public string Text { get; }

        public GloblDirective(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $".globl {Text}";
        }
    }

    public class TextDirective : ILine
    {
        public override string ToString()
        {
            return ".text";
        }
    }

    public class RDataDirective : ILine
    {
        public override string ToString()
        {
            return ".section .rdata";
        }
    }

    public class AsciiDirective : ILine
    {
        public string Text { get; }

        public AsciiDirective(string text)
        {
            // TODO: Do this somewhere else
            Text = text.Substring(1, text.Length - 2);
        }

        public override string ToString()
        {
            return $".ascii \"{Text}\\0\"";
        }
    }
}
