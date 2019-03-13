using System;
using System.Collections.Generic;

namespace MyCCompiler.AST
{
    // Consider one subclass of CBaseVisitor of type INode instead?
    // VisitIfNotNull function
    // A Visitor is named {ParseTreeName}Visitor with return type {AstTreeName}
    public class CompilationUnitVisitor : CBaseVisitor<CompilationUnit>
    {
        public override CompilationUnit VisitCompilationUnit(CParser.CompilationUnitContext context)
        {
            var externals = context.translationUnit().Accept(new TranslationUnitVisitor());
            return new CompilationUnit(externals);
        }
    }

    public class TranslationUnitVisitor : CBaseVisitor<IList<IExternal>>
    {
        // Which branch should be traversed first?
        public override IList<IExternal> VisitTranslationUnit(CParser.TranslationUnitContext context)
        {
            if (context.translationUnit() != null)
            {
                var list = context.translationUnit().Accept(new TranslationUnitVisitor());
                list.Add(context.externalDeclaration().Accept(new ExternalDeclarationVisitor()));
                return list;
            }

            var external = context.externalDeclaration().Accept(new ExternalDeclarationVisitor());
            return new List<IExternal> { external };
        }
    }

    public class ExternalDeclarationVisitor : CBaseVisitor<IExternal>
    {
        public override IExternal VisitExternalDeclaration(CParser.ExternalDeclarationContext context)
        {
            if (context.functionDefinition() != null)
            {
                return context.functionDefinition().Accept(new FunctionDefinitionVisitor());
            }

            if (context.declaration() != null)
            {
                return context.declaration().Accept(new DeclarationVisitor());
            }

            // stray ";"
            return null;
        }
    }

    public class FunctionDefinitionVisitor : CBaseVisitor<FunctionDefinition>
    {
        public override FunctionDefinition VisitFunctionDefinition(CParser.FunctionDefinitionContext context)
        {
            var cs = context.compoundStatement().Accept(new CompoundStatementVisitor());
            return new FunctionDefinition(context.GetText(), cs);
        }
    }

    public class DeclarationVisitor : CBaseVisitor<Declaration>
    {
        public override Declaration VisitDeclaration(CParser.DeclarationContext context)
        {
            var declarators = context.initDeclaratorList().Accept(new InitDeclaratorListVisitor());
            return new Declaration(declarators);
        }
    }

    public class InitDeclaratorListVisitor : CBaseVisitor<IList<IDeclarator>>
    {
        public override IList<IDeclarator> VisitInitDeclaratorList(CParser.InitDeclaratorListContext context)
        {
            var declarator = context.initDeclarator().Accept(new InitDeclaratorVisitor());

            if (context.initDeclaratorList() == null)
            {
                return new List<IDeclarator> { declarator };
            }

            var list = context.initDeclaratorList().Accept(new InitDeclaratorListVisitor());
            list.Add(declarator);
            return list;
        }
    }

    public class InitDeclaratorVisitor : CBaseVisitor<IDeclarator>
    {
        public override IDeclarator VisitInitDeclarator(CParser.InitDeclaratorContext context)
        {
            var declarator = context.declarator().Accept(new DeclaratorVisitor());

            if (context.initializer() != null)
            {
                // TODO: Visit the initializer
                return new InitDeclarator(declarator);
            }

            return declarator;
        }
    }

    public class DeclaratorVisitor : CBaseVisitor<Declarator>
    {
        public override Declarator VisitDeclarator(CParser.DeclaratorContext context)
        {
            //if (context.gccDeclaratorExtension() != null)
            //{
            //    throw new NotSupportedException();
            //}

            return new Declarator(context.GetText());
        }
    }

    public class CompoundStatementVisitor : CBaseVisitor<CompoundStatement>
    {
        public override CompoundStatement VisitCompoundStatement(CParser.CompoundStatementContext context)
        {
            if (context.blockItemList() == null)
            {
                // Empty brackets
                return new CompoundStatement(new List<IStatement>());
            }

            var statements = context.blockItemList().Accept(new BlockItemListVisitor());
            return new CompoundStatement(statements);
        }
    }

    public class BlockItemListVisitor : CBaseVisitor<IList<IStatement>>
    {
        public override IList<IStatement> VisitBlockItemList(CParser.BlockItemListContext context)
        {
            var statement = context.blockItem().Accept(new BlockItemVisitor());

            if (context.blockItemList() == null)
            {
                return new List<IStatement> { statement };
            }

            var list = context.blockItemList().Accept(new BlockItemListVisitor());
            list.Add(statement);
            return list;
        }
    }

    public class BlockItemVisitor : CBaseVisitor<IStatement>
    {
        public override IStatement VisitBlockItem(CParser.BlockItemContext context)
        {
            if (context.statement() != null)
            {
                return context.statement().Accept(new StatementVisitor());
            }

            return context.declaration().Accept(new DeclarationVisitor());
        }
    }

    public class StatementVisitor : CBaseVisitor<IStatement>
    {
        public override IStatement VisitStatement(CParser.StatementContext context)
        {
            if (context.compoundStatement() != null)
            {
                return context.compoundStatement().Accept(new CompoundStatementVisitor());
            }

            // Other statements not supported yet
            throw new NotImplementedException();
        }
    }
}
