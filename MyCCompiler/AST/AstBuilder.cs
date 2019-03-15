using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCCompiler.AST
{
    public class AstBuilder
    {
        public CompilationUnit Build(CParser.CompilationUnitContext context)
        {
            return new CompilationUnit(Build(context.translationUnit()));
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<IExternal> Build(CParser.TranslationUnitContext context)
        {
            if (context.translationUnit() == null)
            {
                return CreateLinkedList(Build(context.externalDeclaration()));
            }

            var list = Build(context.translationUnit());
            list.AddLast(Build(context.externalDeclaration()));
            return list;
        }

        public static IExternal Build(CParser.ExternalDeclarationContext context)
        {
            if (context.functionDefinition() != null)
            {
                return Build(context.functionDefinition());
            }

            if (context.declaration() != null)
            {
                return Build(context.declaration());
            }

            // stray ";"
            return null;
        }

        public static FunctionDefinition Build(CParser.FunctionDefinitionContext context)
        {
            var cs = Build(context.compoundStatement());
            return new FunctionDefinition(context.GetText(), cs);
        }

        public static Declaration Build(CParser.DeclarationContext context)
        {
            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarators = Build(context.initDeclaratorList());
            return new Declaration(declarationSpecifiers, declarators);
        }

        public static LinkedList<IDeclarationSpecifier> Build(CParser.DeclarationSpecifiersContext context)
        {
            return new LinkedList<IDeclarationSpecifier>(context.declarationSpecifier().Select(Build));
        }

        public static IDeclarationSpecifier Build(CParser.DeclarationSpecifierContext context)
        {
            if (context.functionSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.alignmentSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.storageClassSpecifier() != null)
            {
                return Build(context.storageClassSpecifier());
            }

            if (context.typeSpecifier() != null)
            {
                return Build(context.typeSpecifier());
            }

            return Build(context.typeQualifier());
        }

        public static Storage Build(CParser.StorageClassSpecifierContext context)
        {
            // TODO: Handle typedef
            return new Storage(StorageKindMap[context.GetText()]);
        }

        public static ITypeSpecifier Build(CParser.TypeSpecifierContext context)
        {
            if (context.atomicTypeSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.structOrUnionSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.enumSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.typedefName() != null)
            {
                throw new NotImplementedException();
            }

            if (context.typeSpecifier() != null)
            {
                var typeSpecifier = Build(context.typeSpecifier());
                var pointer = Build(context.pointer());
                return new TypeSpecifierWithPointer(typeSpecifier, pointer);
            }

            return new TypeKeyword(TypeKeywordKindMap[context.GetText()]);
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<IDeclarator> Build(CParser.InitDeclaratorListContext context)
        {
            if (context.initDeclaratorList() == null)
            {
                return CreateLinkedList(Build(context.initDeclarator()));
            }

            var list = Build(context.initDeclaratorList());
            list.AddLast(Build(context.initDeclarator()));
            return list;
        }

        public static IDeclarator Build(CParser.InitDeclaratorContext context)
        {
            var declarator = Build(context.declarator());

            if (context.initializer() != null)
            {
                // TODO: Build the initializer
                return new InitializationDeclarator(declarator);
            }

            return declarator;
        }

        public static Declarator Build(CParser.DeclaratorContext context)
        {
            if (context.pointer() != null)
            {
                // I don't know when this happens
                // but it should call the build pointer method
                throw new NotImplementedException();
            }

            return new Declarator(Build(context.directDeclarator()));
        }

        public static Identifier Build(CParser.DirectDeclaratorContext context)
        {
            if (context.Identifier() == null)
            {
                // Ignore for now
                return null;
            }

            return new Identifier(context.Identifier().Symbol.Text);
        }

        public static IPointer Build(CParser.PointerContext context)
        {
            var qualifiers = Enumerable.Empty<Qualifier>();

            if (context.typeQualifierList() != null)
            {
                qualifiers = Build(context.typeQualifierList());
            }

            if (context.pointer() != null)
            {
                // I don't know when this happens
                // but it should return the PointerWithPointer type OR
                // return a LinkedList of "terminal" pointers
                throw new NotImplementedException();
            }

            return new Pointer(new HashSet<Qualifier>(qualifiers));
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<Qualifier> Build(CParser.TypeQualifierListContext context)
        {
            if (context.typeQualifierList() == null)
            {
                return CreateLinkedList(Build(context.typeQualifier()));
            }

            var list = Build(context.typeQualifierList());
            list.AddLast(Build(context.typeQualifier()));
            return list;
        }

        public static Qualifier Build(CParser.TypeQualifierContext context)
        {
            return new Qualifier(QualifierKindMap[context.GetText()]);
        }

        public static CompoundStatement Build(CParser.CompoundStatementContext context)
        {
            if (context.blockItemList() == null)
            {
                // Empty brackets
                return new CompoundStatement(new LinkedList<IStatement>());
            }

            var statements = Build(context.blockItemList());
            return new CompoundStatement(statements);
        }


        // This is a list grammar. Consider using generics.
        public static LinkedList<IStatement> Build(CParser.BlockItemListContext context)
        {
            if (context.blockItemList() == null)
            {
                return CreateLinkedList(Build(context.blockItem()));
            }

            var list = Build(context.blockItemList());
            list.AddLast(Build(context.blockItem()));
            return list;
        }

        public static IStatement Build(CParser.BlockItemContext context)
        {
            if (context.statement() != null)
            {
                return Build(context.statement());
            }

            return Build(context.declaration());
        }

        public static IStatement Build(CParser.StatementContext context)
        {
            if (context.compoundStatement() != null)
            {
                return Build(context.compoundStatement());
            }

            // Other statements not supported yet
            throw new NotImplementedException();
        }

        private static LinkedList<T> CreateLinkedList<T>(T element)
        {
            var list = new LinkedList<T>();
            list.AddLast(element);
            return list;
        }

        private static readonly IDictionary<string, QualifierKind> QualifierKindMap = new Dictionary<string, QualifierKind>
        {
            { "const", QualifierKind.Const },
            { "volatile", QualifierKind.Volatile },
            { "restrict", QualifierKind.Restrict },
            { "_Atomic", QualifierKind.Atomic }
        };

        private static readonly IDictionary<string, StorageKind> StorageKindMap = new Dictionary<string, StorageKind>
        {
            { "auto", StorageKind.Auto },
            { "register", StorageKind.Register },
            { "static", StorageKind.Static },
            { "extern", StorageKind.Extern }
        };

        private static readonly IDictionary<string, TypeKeywordKind> TypeKeywordKindMap = new Dictionary<string, TypeKeywordKind>
        {
            { "void", TypeKeywordKind.Void },
            { "char", TypeKeywordKind.Char },
            { "short", TypeKeywordKind.Short },
            { "int", TypeKeywordKind.Int },
            { "long", TypeKeywordKind.Long },
            { "float", TypeKeywordKind.Float },
            { "double", TypeKeywordKind.Double },
            { "signed", TypeKeywordKind.Signed },
            { "unsigned", TypeKeywordKind.Unsigned },
            { "_Bool", TypeKeywordKind.Bool },
            { "_Complex", TypeKeywordKind.Complex },
        };
    }
}
