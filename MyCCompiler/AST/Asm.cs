﻿namespace MyCCompiler.AST
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

    public class Call : ILine
    {
        public string Text { get; }

        public Call(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return $"\tcall\t{Text}";
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

    public class Ascii : ILine
    {
        public string Text { get; }

        public Ascii(string text)
        {
            // TODO: Do this somewhere else
            Text = text.Substring(1, text.Length - 2);
        }

        public override string ToString()
        {
            return $".ascii \"{Text}\\0\"";
        }
    }

    // Don't use this
    public class DirectText : ILine
    {
        public string Line { get; }

        public DirectText(string line)
        {
            Line = line;
        }

        public override string ToString()
        {
            return Line;
        }
    }
}