using System.Collections.Generic;

namespace MyCCompiler.AST
{
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

            return null;
        }
    }

    public class FunctionDefinitionVisitor : CBaseVisitor<FunctionDefinition>
    {
        public override FunctionDefinition VisitFunctionDefinition(CParser.FunctionDefinitionContext context)
        {
            return new FunctionDefinition(context.GetText());
        }
    }

    public class DeclarationVisitor : CBaseVisitor<Declaration>
    {
        public override Declaration VisitDeclaration(CParser.DeclarationContext context)
        {
            var specifier = context.declarationSpecifiers().GetText();
            var lexme = context.initDeclaratorList().GetText();

            return new Declaration(specifier, lexme);
        }
    }
}
