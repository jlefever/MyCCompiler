using System.Collections.Generic;
using System.Linq;
using static MyCCompiler.AST.TypeKeywordKind;

namespace MyCCompiler.AST
{
    public class SymbolBuilder
    {
        private SymbolTable _currSymbolTable;

        public SymbolBuilder()
        {
            _currSymbolTable = new SymbolTable(null);
        }

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
            EnterScope();
            var declarationType = BuildDeclarationType(node.DeclarationSpecifiers);
            var functionType = BuildFunctionType(node.FunctionDeclarator, declarationType);
            Visit(node.CompoundStatement);
            ExitScope();
            SetIdentifierType(node.FunctionDeclarator.Identifier, functionType);
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
                case IIfStatement n:
                    Visit(n);
                    break;
                case IReturnStatement n:
                    Visit(n);
                    break;
                case CompoundStatement n:
                    EnterScope();
                    Visit(n);
                    ExitScope();
                    break;
                case Declaration n:
                    Visit(n);
                    break;
                case ExpressionStatement n:
                    Visit(n);
                    break;
            }
        }

        public void Visit(IIfStatement node)
        {
            Visit(node.Expression);
            Visit(node.Body);

            if (node is IfElseStatement n)
            {
                Visit(n.Else);
            }
        }

        public void Visit(IReturnStatement node)
        {
            if (node is ReturnStatement n)
            {
                Visit(n.Expression);
            }
        }

        public void Visit(Declaration node)
        {
            var type = BuildDeclarationType(node.DeclarationSpecifiers);

            foreach (var initDeclarator in node.InitDeclarators)
            {
                Visit(initDeclarator, type);
            }
        }

        public void Visit(IInitDeclarator node, IPointable type)
        {
            switch (node)
            {
                case Declarator n:
                    Visit(n, type);
                    break;
                case InitializationDeclarator n:
                    Visit(n, type);
                    break;
            }
        }

        public void Visit(Declarator node, IPointable type)
        {
            Visit(node.DirectDeclarator, type);
        }

        public void Visit(InitializationDeclarator node, IPointable type)
        {
            Visit(node.Declarator, type);
            Visit(node.Expression);
        }

        public void Visit(IDirectDeclarator node, IPointable type)
        {
            switch (node)
            {
                case Identifier n:
                    SetIdentifierType(n, type);
                    Visit(n);
                    break;
                case FunctionDeclarator n:
                    BuildFunctionType(n, type);
                    break;
            }
        }

        public void Visit(Identifier node)
        {
            node.Symbol = _currSymbolTable.Get(node.Text);
        }

        public Function BuildFunctionType(FunctionDeclarator node, IPointable type)
        {
            var parameterTypes = new LinkedList<IPointable>();

            foreach (var parameter in node.ParameterList.Parameters)
            {
                var declarationType = BuildDeclarationType(parameter.DeclarationSpecifiers);
                parameterTypes.AddLast(declarationType);
                Visit(parameter.Declarator, declarationType);
            }

            return new Function(type, parameterTypes.ToArray(), node.ParameterList.Variadic);
        }

        public IPointable BuildDeclarationType(DeclarationSpecifiers node)
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

        public void Visit(ExpressionStatement node)
        {
            Visit(node.Expression);
        }

        public void Visit(IExpression node)
        {
            switch (node)
            {
                case IPrimaryExpression n:
                    Visit(n);
                    break;
                case AssignmentExpression n:
                    Visit(n);
                    break;
                case BinaryExpression n:
                    Visit(n);
                    break;
                case UnaryExpression n:
                    Visit(n);
                    break;
                case FunctionCall n:
                    Visit(n);
                    break;
            }
        }

        public void Visit(AssignmentExpression node)
        {
            Visit(node.Identifier);
            Visit(node.Expression);
        }

        public void Visit(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        public void Visit(UnaryExpression node)
        {
            Visit(node.Expression);
        }

        public void Visit(IPrimaryExpression node)
        {
            if (node is Identifier n)
            {
                Visit(n);
            }
        }

        public void Visit(FunctionCall node)
        {
            Visit(node.Identifier);

            foreach (var argument in node.Arguments)
            {
                Visit(argument);
            }
        }

        public void SetIdentifierType(Identifier identifier, IType type)
        {
            _currSymbolTable.Put(new Symbol(identifier.Text, type));
        }

        private void EnterScope()
        {
            _currSymbolTable = new SymbolTable(_currSymbolTable);
        }

        private void ExitScope()
        {
            _currSymbolTable = _currSymbolTable.Previous;
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
