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
            if (context.declarationSpecifiers() == null)
            {
                // Not supporting function definitions without declaration specifiers
                throw new NotSupportedException();
            }

            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarator = Build(context.declarator());
            var compoundStatement = Build(context.compoundStatement());
            return new FunctionDefinition(declarationSpecifiers, declarator, compoundStatement);
        }

        public static Declaration Build(CParser.DeclarationContext context)
        {
            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarators = Build(context.initDeclaratorList());
            return new Declaration(declarationSpecifiers, declarators);
        }

        public static DeclarationSpecifiers Build(CParser.DeclarationSpecifiersContext context)
        {
            var typeSpecifiers = new HashSet<ITypeSpecifier>();
            var storages = new HashSet<Storage>();
            var qualifiers = new HashSet<Qualifier>();

            foreach (var declarationSpecifier in context.declarationSpecifier().Select(Build))
            {
                switch (declarationSpecifier)
                {
                    case ITypeSpecifier typeSpecifier:
                        typeSpecifiers.Add(typeSpecifier);
                        continue;
                    case Storage storage:
                        storages.Add(storage);
                        continue;
                    case Qualifier qualifier:
                        qualifiers.Add(qualifier);
                        break;
                }
            }

            return new DeclarationSpecifiers(typeSpecifiers, storages, qualifiers);
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
        public static LinkedList<IInitDeclarator> Build(CParser.InitDeclaratorListContext context)
        {
            if (context.initDeclaratorList() == null)
            {
                return CreateLinkedList(Build(context.initDeclarator()));
            }

            var list = Build(context.initDeclaratorList());
            list.AddLast(Build(context.initDeclarator()));
            return list;
        }

        public static IInitDeclarator Build(CParser.InitDeclaratorContext context)
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
                // To my knowledge, this only happens when the
                // direct declarator uses parenthesis.
                // I am not supporting parenthesis at this time.
                throw new NotImplementedException();
            }

            return new Declarator(Build(context.directDeclarator()));
        }

        public static IDirectDeclarator Build(CParser.DirectDeclaratorContext context)
        {
            // identifier, most basic form of declarator
            if (context.Identifier() != null)
            {
                return new Identifier(context.Identifier().Symbol.Text);
            }

            // declarator with parenthesis
            if (context.declarator() != null)
            {
                throw new NotSupportedException();
            }

            var directDeclarator = Build(context.directDeclarator());

            // array
            if (context.GetChild(1).GetText() == "[")
            {
                throw new NotImplementedException();
            }

            // function pointer
            if (context.pointer() != null)
            {
                throw new NotImplementedException();
            }

            // I'm not sure when this happens
            if (context.identifierList() != null)
            {
                throw new NotSupportedException();
            }

            // function
            if (context.parameterTypeList() != null)
            {
                var parameterList = Build(context.parameterTypeList());
                return new FunctionDeclarator(directDeclarator, parameterList);
            }

            return new FunctionDeclarator(directDeclarator);
        }

        public static ParameterList Build(CParser.ParameterTypeListContext context)
        {
            var parameters = Build(context.parameterList());
            var variadic = context.ChildCount > 1;
            return new ParameterList(parameters, variadic);
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<Parameter> Build(CParser.ParameterListContext context)
        {
            if (context.parameterList() == null)
            {
                return CreateLinkedList(Build(context.parameterDeclaration()));
            }

            var list = Build(context.parameterList());
            list.AddLast(Build(context.parameterDeclaration()));
            return list;
        }

        public static Parameter Build(CParser.ParameterDeclarationContext context)
        {
            if (context.declarationSpecifiers2() != null)
            {
                // Parameters with abstract declarators
                // are not supported at this time.
                throw new NotImplementedException();
            }

            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarator = Build(context.declarator());
            return new Parameter(declarationSpecifiers, declarator);
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
