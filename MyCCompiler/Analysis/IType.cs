using System.Collections.Generic;

namespace MyCCompiler.Analysis
{
    public interface IType { }

    public class Pointer : IType
    {
        public IType Type;
    }

    public class Variable : IType
    {
        public PrimitiveKind PrimitiveKind { get; }

        public Variable(PrimitiveKind primitiveKind)
        {
            PrimitiveKind = primitiveKind;
        }        

        public override string ToString()
        {
            return PrimitiveKind.ToString();
        }
    }

    public enum PrimitiveKind
    {
        Void,
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

    public enum QualifierKind
    {
        Const,
        Volatile,
        Restrict,
        Atomic
    }

    public enum StorageKind
    {
        Auto,
        Register,
        Static,
        Extern
    }

    public static class EnumUtil
    {
        public static IDictionary<string, PrimitiveKind> PrimitiveKindMap = new Dictionary<string, PrimitiveKind>
        {
            { "void", PrimitiveKind.Void },
            { "char", PrimitiveKind.Char },
            { "signedchar", PrimitiveKind.SignedChar },
            { "unsignedchar", PrimitiveKind.UnsignedChar },
            { "short", PrimitiveKind.Short },
            { "shortint", PrimitiveKind.Short },
            { "signedshort", PrimitiveKind.Short },
            { "signedshortint", PrimitiveKind.Short },
            { "unsignedshort", PrimitiveKind.UnsignedShort },
            { "unsignedshortint", PrimitiveKind.UnsignedShort },
            { "int", PrimitiveKind.Int },
            { "signed", PrimitiveKind.Int },
            { "signedint", PrimitiveKind.Int },
            { "unsigned", PrimitiveKind.UnsignedInt },
            { "unsignedint", PrimitiveKind.UnsignedInt },
            { "long", PrimitiveKind.Long },
            { "longint", PrimitiveKind.Long },
            { "signedlong", PrimitiveKind.Long },
            { "signedlongint", PrimitiveKind.Long },
            { "unsignedlong", PrimitiveKind.UnsignedLong },
            { "unsignedlongint", PrimitiveKind.UnsignedLong },
            { "longlong", PrimitiveKind.LongLong },
            { "longlongint", PrimitiveKind.LongLong },
            { "signedlonglong", PrimitiveKind.LongLong },
            { "signedlonglongint", PrimitiveKind.LongLong },
            { "unsignedlonglong", PrimitiveKind.UnsignedLongLong },
            { "unsignedlonglongint", PrimitiveKind.UnsignedLongLong },
            { "float", PrimitiveKind.Float },
            { "double", PrimitiveKind.Double },
            { "longdouble", PrimitiveKind.LongDouble }
        };

        public static IDictionary<string, QualifierKind> QualifierKindMap = new Dictionary<string, QualifierKind>
        {
            { "const", QualifierKind.Const },
            { "volatile", QualifierKind.Volatile },
            { "restrict", QualifierKind.Restrict },
            { "_Atomic", QualifierKind.Atomic }
        };

        public static IDictionary<string, StorageKind> StorageKindMap = new Dictionary<string, StorageKind>
        {
            { "auto", StorageKind.Auto },
            { "register", StorageKind.Register },
            { "static", StorageKind.Static },
            { "extern", StorageKind.Extern }
        };
    }
}
