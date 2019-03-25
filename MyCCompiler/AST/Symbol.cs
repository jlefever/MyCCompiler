namespace MyCCompiler.AST
{
    public interface IType { }

    public interface IPointable : IType { }

    public class Symbol
    {
        public string Identifier { get; }
        public IType Type { get; }

        public Symbol(string identifier, IType type)
        {
            Identifier = identifier;
            Type = type;
        }
    }

    public class Primitive : IPointable
    {
        public PrimitiveKind PrimitiveKind { get; }

        public Primitive(PrimitiveKind primitiveKind)
        {
            PrimitiveKind = primitiveKind;
        }
    }

    public class Function : IType
    {
        public IPointable ReturnType { get; }
        public IPointable[] Parameters { get; }
        public bool Variadic { get; }
        public bool HasReturnValue { get; }

        public Function(IPointable[] parameters, bool variadic)
        {
            Parameters = parameters;
            Variadic = variadic;
            HasReturnValue = false;
        }

        public Function(IPointable returnType, IPointable[] parameters, bool variadic)
        {
            ReturnType = returnType;
            Parameters = parameters;
            Variadic = variadic;
            HasReturnValue = true;
        }
    }

    public class PointerTo : IPointable
    {
        public IPointable Target { get; }

        public PointerTo(IPointable target)
        {
            Target = target;
        }
    }

    public class PointerToVoid : IPointable { }

    public enum PrimitiveKind
    {
        Char,
        SignedChar,
        UnsignedChar,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Long,
        UnsignedLong,
        LongLong,
        UnsignedLongLong,
        Float,
        Double,
        LongDouble
    }
}
