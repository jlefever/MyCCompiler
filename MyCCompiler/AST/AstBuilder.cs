using System;
using System.Collections.Generic;

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
            var declarators = Build(context.initDeclaratorList());
            return new Declaration(declarators);
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

        public static INode Build(CParser.PointerContext context)
        {
            throw new NotImplementedException();
        }

        public static INode Build(CParser.TypeQualifierListContext context)
        {
            throw new NotImplementedException();
        }

        public static INode Build(CParser.TypeQualifierContext context)
        {
            throw new NotImplementedException();
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
    }
}
