using System.Collections.Generic;
using System.Linq;

namespace MyCCompiler.Compiler
{
    public class SymbolBuilder
    {
        private SymbolTable _currSymbolTable;

        public SymbolBuilder()
        {
            _currSymbolTable = new SymbolTable();
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
            node.SymbolTable = _currSymbolTable;

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
                case IIterationStatement n:
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

        public void Visit(IIterationStatement node)
        {
            Visit(node.Body);

            switch (node)
            {
                case IWhileStatement n:
                    Visit(n.Expression);
                    break;
                case ForStatement n:
                    Visit(n.FirstExpression);
                    Visit(n.SecondExpression);
                    Visit(n.ThirdExpression);
                    break;
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
                    EnterScope();
                    var functionType = BuildFunctionType(n, type);
                    ExitScope();
                    SetIdentifierType(n.Identifier, functionType);
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
            var keywords = new LinkedList<TypeKeyword>();
            var pointers = new LinkedList<ISet<Qualifier>>();

            foreach (var typeSpecifier in node.TypeSpecifiers)
            {
                if (typeSpecifier is TypeKeyword typeKeyword)
                {
                    keywords.AddLast(typeKeyword);
                }
                else
                {
                    var ts = typeSpecifier;
                    while (ts is TypeSpecifierWithPointer tswp)
                    {
                        pointers.AddLast(tswp.Pointer.Qualifiers);
                        ts = tswp.TypeSpecifier;
                    }

                    keywords.AddLast(ts as TypeKeyword);
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
            _currSymbolTable.Put(identifier.Text, type);
        }

        private void EnterScope()
        {
            _currSymbolTable = new SymbolTable(_currSymbolTable);
        }

        private void ExitScope()
        {
            _currSymbolTable = _currSymbolTable.Parent;
        }

        private static readonly IDictionary<TypeKeyword[], PrimitiveKind> PrimitiveKindMap =
            new Dictionary<TypeKeyword[], PrimitiveKind>(new EqualityComparer())
            {
                { new [] { TypeKeyword.Char }, PrimitiveKind.Char },
                { new [] { TypeKeyword.Signed, TypeKeyword.Char }, PrimitiveKind.SignedChar },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Char}, PrimitiveKind.UnsignedChar },
                { new [] { TypeKeyword.Short }, PrimitiveKind.Short },
                { new [] { TypeKeyword.Short, TypeKeyword.Int }, PrimitiveKind.Short },
                { new [] { TypeKeyword.Signed, TypeKeyword.Short }, PrimitiveKind.Short },
                { new [] { TypeKeyword.Signed, TypeKeyword.Short, TypeKeyword.Int }, PrimitiveKind.Short },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Short }, PrimitiveKind.UnsignedShort },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Short, TypeKeyword.Int }, PrimitiveKind.UnsignedShort },
                { new [] { TypeKeyword.Int }, PrimitiveKind.Int },
                { new [] { TypeKeyword.Signed }, PrimitiveKind.Int },
                { new [] { TypeKeyword.Signed, TypeKeyword.Int }, PrimitiveKind.Int },
                { new [] { TypeKeyword.Unsigned }, PrimitiveKind.UnsignedInt },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Int }, PrimitiveKind.UnsignedInt },
                { new [] { TypeKeyword.Long }, PrimitiveKind.Long },
                { new [] { TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.Long },
                { new [] { TypeKeyword.Signed, TypeKeyword.Long }, PrimitiveKind.Long },
                { new [] { TypeKeyword.Signed, TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.Long },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Long }, PrimitiveKind.UnsignedLong },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.UnsignedLong },
                { new [] { TypeKeyword.Long, TypeKeyword.Long }, PrimitiveKind.LongLong },
                { new [] { TypeKeyword.Long, TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.LongLong },
                { new [] { TypeKeyword.Signed, TypeKeyword.Long, TypeKeyword.Long }, PrimitiveKind.LongLong },
                { new [] { TypeKeyword.Signed, TypeKeyword.Long, TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.LongLong },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Long, TypeKeyword.Long }, PrimitiveKind.UnsignedLongLong },
                { new [] { TypeKeyword.Unsigned, TypeKeyword.Long, TypeKeyword.Long, TypeKeyword.Int }, PrimitiveKind.UnsignedLongLong },
                { new [] { TypeKeyword.Float }, PrimitiveKind.Float },
                { new [] { TypeKeyword.Double }, PrimitiveKind.Double },
                { new [] { TypeKeyword.Long, TypeKeyword.Double }, PrimitiveKind.LongDouble }
            };

        private class EqualityComparer : IEqualityComparer<TypeKeyword[]>
        {
            public bool Equals(TypeKeyword[] a, TypeKeyword[] b)
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

            public int GetHashCode(TypeKeyword[] a)
            {
                return a.Aggregate(0, (current, item) => current ^ item.GetHashCode());
            }
        }
    }
}
