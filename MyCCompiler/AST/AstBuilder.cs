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

        // Which branch should be traversed first?
        public static IList<IExternal> Build(CParser.TranslationUnitContext context)
        {
            if (context.translationUnit() != null)
            {
                var list = Build(context.translationUnit());
                list.Add(Build(context.externalDeclaration()));
                return list;
            }

            var external = Build(context.externalDeclaration());
            return new List<IExternal> { external };
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

        public static IList<IDeclarator> Build(CParser.InitDeclaratorListContext context)
        {
            var declarator = Build(context.initDeclarator());

            if (context.initDeclaratorList() == null)
            {
                return new List<IDeclarator> { declarator };
            }

            var list = Build(context.initDeclaratorList());
            list.Add(declarator);
            return list;
        }

        public static IDeclarator Build(CParser.InitDeclaratorContext context)
        {
            var declarator = Build(context.declarator());

            if (context.initializer() != null)
            {
                // TODO: Build the initializer
                return new InitDeclarator(declarator);
            }

            return declarator;
        }

        public static Declarator Build(CParser.DeclaratorContext context)
        {
            return new Declarator(context.GetText());
        }

        public static CompoundStatement Build(CParser.CompoundStatementContext context)
        {
            if (context.blockItemList() == null)
            {
                // Empty brackets
                return new CompoundStatement(new List<IStatement>());
            }

            var statements = Build(context.blockItemList());
            return new CompoundStatement(statements);
        }

        public static IList<IStatement> Build(CParser.BlockItemListContext context)
        {
            var statement = Build(context.blockItem());

            if (context.blockItemList() == null)
            {
                return new List<IStatement> { statement };
            }

            var list = Build(context.blockItemList());
            list.Add(statement);
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
    }
}
