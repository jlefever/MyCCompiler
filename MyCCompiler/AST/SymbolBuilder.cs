using System;
using System.Collections.Generic;
using System.Linq;
using static MyCCompiler.AST.TypeKeywordKind;

namespace MyCCompiler.AST
{
    public class SymbolBuilder
    {
        public void Visit(CompilationUnit node)
        {
            foreach (var external in node.Externals)
            {
                Visit(external);
            }
        }

        public void Visit(IExternal node)
        {
            switch (node)
            {
                case FunctionDefinition n:
                    Visit(n);
                    break;
                case Declaration n:
                    Visit(n);
                    break;
            }
        }

        public void Visit(FunctionDefinition node)
        {
            Visit(node.DeclarationSpecifiers);
            Visit(node.FunctionDeclarator);
            Visit(node.CompoundStatement);
        }

        public void Visit(FunctionDeclarator functionDeclarator)
        {

        }

        public void Visit(CompoundStatement node)
        {
            foreach (var statement in node.Statements)
            {
                Visit(statement);
            }
        }

        public void Visit(IStatement node)
        {
            switch (node)
            {
                case CompoundStatement n:
                    Visit(n);
                    break;
                case Declaration n:
                    Visit(n);
                    break;
                case ExpressionStatement n:
                    throw new NotImplementedException();
            }
        }

        public void Visit(Declaration node)
        {
            var pointable = Visit(node.DeclarationSpecifiers);
        }

        public IPointable Visit(DeclarationSpecifiers node)
        {
            var keywords = new LinkedList<TypeKeywordKind>();
            var pointers = new LinkedList<ISet<Qualifier>>();

            foreach (var typeSpecifier in node.TypeSpecifiers)
            {
                if (typeSpecifier is TypeKeyword typeKeyword)
                {
                    keywords.AddLast(typeKeyword.TypeKeywordKind);
                }
                else
                {
                    var ts = typeSpecifier;
                    while (ts is TypeSpecifierWithPointer tswp)
                    {
                        pointers.AddLast(tswp.Pointer.Qualifiers);
                        ts = tswp.TypeSpecifier;
                    }

                    keywords.AddLast(((TypeKeyword)ts).TypeKeywordKind);
                }
            }

            var primitiveKind = PrimitiveKindMap[keywords.OrderBy(x => x).ToArray()];
            IPointable pointable = new Primitive(primitiveKind, node.Qualifiers, node.Storages);

            foreach (var set in pointers)
            {
                pointable = new PointerTo(pointable, set);
            }

            return pointable;
        }

        private static readonly IDictionary<TypeKeywordKind[], PrimitiveKind> PrimitiveKindMap = 
            new Dictionary<TypeKeywordKind[], PrimitiveKind>(new EqualityComparer())
            {
                { new [] { TypeKeywordKind.Char }, PrimitiveKind.Char },
                { new [] { Signed, TypeKeywordKind.Char }, PrimitiveKind.SignedChar },
                { new [] { Unsigned, TypeKeywordKind.Char}, PrimitiveKind.UnsignedChar },
                { new [] { Short }, PrimitiveKind.Short },
                { new [] { Short, Int }, PrimitiveKind.Short },
                { new [] { Signed, Short }, PrimitiveKind.Short },
                { new [] { Signed, Short, Int }, PrimitiveKind.Short },
                { new [] { Unsigned, Short }, PrimitiveKind.UnsignedShort },
                { new [] { Unsigned, Short, Int }, PrimitiveKind.UnsignedShort },
                { new [] { Int }, PrimitiveKind.Int },
                { new [] { Signed }, PrimitiveKind.Int },
                { new [] { Signed, Int }, PrimitiveKind.Int },
                { new [] { Unsigned }, PrimitiveKind.UnsignedInt },
                { new [] { Unsigned, Int }, PrimitiveKind.UnsignedInt },
                { new [] { Long }, PrimitiveKind.Long },
                { new [] { Long, Int }, PrimitiveKind.Long },
                { new [] { Signed, Long }, PrimitiveKind.Long },
                { new [] { Signed, Long, Int }, PrimitiveKind.Long },
                { new [] { Unsigned, Long }, PrimitiveKind.UnsignedLong },
                { new [] { Unsigned, Long, Int }, PrimitiveKind.UnsignedLong },
                { new [] { Long, Long }, PrimitiveKind.LongLong },
                { new [] { Long, Long, Int }, PrimitiveKind.LongLong },
                { new [] { Signed, Long, Long }, PrimitiveKind.LongLong },
                { new [] { Signed, Long, Long, Int }, PrimitiveKind.LongLong },
                { new [] { Unsigned, Long, Long }, PrimitiveKind.UnsignedLongLong },
                { new [] { Unsigned, Long, Long, Int }, PrimitiveKind.UnsignedLongLong },
                { new [] { Float }, PrimitiveKind.Float },
                { new [] { TypeKeywordKind.Double }, PrimitiveKind.Double },
                { new [] { Long, TypeKeywordKind.Double }, PrimitiveKind.LongDouble }
            };

        private class EqualityComparer : IEqualityComparer<TypeKeywordKind[]>
        {
            public bool Equals(TypeKeywordKind[] a, TypeKeywordKind[] b)
            {
                if (a == b)
                {
                    return true;
                }

                if (a == null || b == null)
                {
                    return false;
                }

                if (a.Length != b.Length)
                {
                    return false;
                }

                for (var i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(TypeKeywordKind[] a)
            {
                return a.Aggregate(0, (current, item) => current ^ item.GetHashCode());
            }
        }
    }
}
