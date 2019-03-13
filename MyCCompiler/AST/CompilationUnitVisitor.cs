using System;
using System.Collections.Generic;

namespace MyCCompiler.AST
{
    // Consider one subclass of CBaseVisitor of type INode instead?
    // VisitIfNotNull function
    // A Visitor is named {ParseTreeName}Visitor with return type {AstTreeName}
    public class CompilationUnitVisitor : CBaseVisitor<CompilationUnit>
    {
        private readonly IList<IExternal> _externals;

        public CompilationUnitVisitor()
        {
            _externals = new List<IExternal>();
        }

        public override CompilationUnit VisitCompilationUnit(CParser.CompilationUnitContext context)
        {
            Visit(context.translationUnit());
            return new CompilationUnit(_externals);
        }

        public override CompilationUnit VisitTranslationUnit(CParser.TranslationUnitContext context)
        {
            if (context.translationUnit() != null)
            {
                Visit(context.translationUnit());
            }

            var external = context.externalDeclaration().Accept(new ExternalDeclarationVisitor());

            if (external != null)
            {
                _externals.Add(external);
            }

            return null;
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

    public class DeclarationSpecifiersVisitor : CBaseVisitor<INode>
    {
        public override INode VisitDeclarationSpecifier(CParser.DeclarationSpecifierContext context)
        {
            return base.VisitDeclarationSpecifier(context);
        }
    }

    public class InitDeclaratorListVisitor : CBaseVisitor<IList<IDeclarator>>
    {
        // This is one way of traversing "List"s described in the grammar
        // For an alternative way see blockItemList
        // All of these kinds of nodes should probably use the same method of traversal
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
            return new Declarator(context.GetText());
        }
    }

    public class CompoundStatementVisitor : CBaseVisitor<CompoundStatement>
    {
        private readonly IList<IStatement> _statements;

        public CompoundStatementVisitor()
        {
            _statements = new List<IStatement>();
        }

        public override CompoundStatement VisitCompoundStatement(CParser.CompoundStatementContext context)
        {
            if (context.blockItemList() != null)
            {
                Visit(context.blockItemList());
            }

            return new CompoundStatement(_statements);
        }

        public override CompoundStatement VisitBlockItemList(CParser.BlockItemListContext context)
        {
            if (context.blockItemList() != null)
            {
                Visit(context.blockItemList());
            }

            _statements.Add(context.blockItem().Accept(new BlockItemVisitor()));
            return null;
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
