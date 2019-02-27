using System.Collections.Generic;

namespace MyCCompiler.Analysis
{
    public enum Primitive
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

    public class PrimitiveUtil
    {
        public static IDictionary<string, Primitive> Map = new Dictionary<string, Primitive>
        {
            { "void", Primitive.Void },
            { "char", Primitive.Char },
            { "signedchar", Primitive.SignedChar },
            { "unsignedchar", Primitive.UnsignedChar },
            { "short", Primitive.Short },
            { "shortint", Primitive.Short },
            { "signedshort", Primitive.Short },
            { "signedshortint", Primitive.Short },
            { "unsignedshort", Primitive.UnsignedShort },
            { "unsignedshortint", Primitive.UnsignedShort },
            { "int", Primitive.Int },
            { "signed", Primitive.Int },
            { "signedint", Primitive.Int },
            { "unsigned", Primitive.UnsignedInt },
            { "unsignedint", Primitive.UnsignedInt },
            { "long", Primitive.Long },
            { "longint", Primitive.Long },
            { "signedlong", Primitive.Long },
            { "signedlongint", Primitive.Long },
            { "unsignedlong", Primitive.UnsignedLong },
            { "unsignedlongint", Primitive.UnsignedLong },
            { "longlong", Primitive.LongLong },
            { "longlongint", Primitive.LongLong },
            { "signedlonglong", Primitive.LongLong },
            { "signedlonglongint", Primitive.LongLong },
            { "unsignedlonglong", Primitive.UnsignedLongLong },
            { "unsignedlonglongint", Primitive.UnsignedLongLong },
            { "float", Primitive.Float },
            { "double", Primitive.Double },
            { "longdouble", Primitive.LongDouble }
        };
    }
}
