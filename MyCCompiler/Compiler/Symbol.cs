﻿using System.Collections.Generic;

namespace MyCCompiler.Compiler
{
    public class Symbol
    {
        public string Name { get; }
        public IType Type { get; }
        public int StackOffset { get; set; }

        public Symbol(string name, IType type)
        {
            Name = name;
            Type = type;
            StackOffset = 0;
        }
    }

    public interface IType { }

    public interface IPointable : IType { }

    public class Primitive : IPointable
    {
        public PrimitiveKind PrimitiveKind { get; }
        public ISet<Qualifier> Qualifiers { get; }
        public ISet<Storage> Storages { get; }

        public Primitive(PrimitiveKind primitiveKind, ISet<Qualifier> qualifiers, ISet<Storage> storages)
        {
            PrimitiveKind = primitiveKind;
            Qualifiers = qualifiers;
            Storages = storages;
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
        public ISet<Qualifier> Qualifiers { get; }

        public PointerTo(IPointable target, ISet<Qualifier> qualifiers)
        {
            Target = target;
            Qualifiers = qualifiers;
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
