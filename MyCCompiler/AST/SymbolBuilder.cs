using System;
using System.Collections.Generic;
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
            var keywords = new HashSet<TypeKeywordKind>();
            var pointers = 0;

            foreach (var typeSpecifier in node.TypeSpecifiers)
            {
                if (typeSpecifier is TypeKeyword typeKeyword)
                {
                    keywords.Add(typeKeyword.TypeKeywordKind);
                }
                else
                {
                    var ts = typeSpecifier;
                    while (ts is TypeSpecifierWithPointer tswp)
                    {
                        pointers = pointers + 1;
                        ts = tswp.TypeSpecifier;
                    }

                    keywords.Add(((TypeKeyword)ts).TypeKeywordKind);
                }
            }

            IPointable pointable = new Primitive(PrimitiveKindMap[keywords]);

            for (var i = 0; i < pointers; i++)
            {
                pointable = new PointerTo(pointable);
            }

            return pointable;
        }

        static SymbolBuilder()
        {
            PrimitiveKindMap = new Dictionary<HashSet<TypeKeywordKind>, PrimitiveKind>
                (HashSet<TypeKeywordKind>.CreateSetComparer())
            {
                { TypeKeywordSet(TypeKeywordKind.Char), PrimitiveKind.Char },
                { TypeKeywordSet(Signed, TypeKeywordKind.Char), PrimitiveKind.SignedChar },
                { TypeKeywordSet(Unsigned, TypeKeywordKind.Char), PrimitiveKind.UnsignedChar },
                { TypeKeywordSet(Short), PrimitiveKind.Short },
                { TypeKeywordSet(Short, Int), PrimitiveKind.Short },
                { TypeKeywordSet(Signed, Short), PrimitiveKind.Short },
                { TypeKeywordSet(Signed, Short, Int), PrimitiveKind.Short },
                { TypeKeywordSet(Unsigned, Short), PrimitiveKind.UnsignedShort },
                { TypeKeywordSet(Unsigned, Short, Int), PrimitiveKind.UnsignedShort },
                { TypeKeywordSet(Int), PrimitiveKind.Int },
                { TypeKeywordSet(Signed), PrimitiveKind.Int },
                { TypeKeywordSet(Signed, Int), PrimitiveKind.Int },
                { TypeKeywordSet(Unsigned), PrimitiveKind.UnsignedInt },
                { TypeKeywordSet(Unsigned, Int), PrimitiveKind.UnsignedInt },
                { TypeKeywordSet(Long), PrimitiveKind.Long },
                { TypeKeywordSet(Long, Int), PrimitiveKind.Long },
                { TypeKeywordSet(Signed, Long), PrimitiveKind.Long },
                { TypeKeywordSet(Signed, Long, Int), PrimitiveKind.Long },
                { TypeKeywordSet(Unsigned, Long), PrimitiveKind.UnsignedLong },
                { TypeKeywordSet(Unsigned, Long, Int), PrimitiveKind.UnsignedLong },
                { TypeKeywordSet(Float), PrimitiveKind.Float },
                { TypeKeywordSet(TypeKeywordKind.Double), PrimitiveKind.Double },
                { TypeKeywordSet(Long, TypeKeywordKind.Double), PrimitiveKind.LongDouble }
            };
        }

        private static HashSet<TypeKeywordKind> TypeKeywordSet(params TypeKeywordKind[] keywords)
        {
            return new HashSet<TypeKeywordKind>(keywords);
        }

        private static readonly IDictionary<HashSet<TypeKeywordKind>, PrimitiveKind> PrimitiveKindMap;
    }
}
