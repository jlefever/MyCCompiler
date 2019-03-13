using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public interface IStatement : INode { }

    public interface IDeclarator : INode { }

    public class CompilationUnit : INode
    {
        public IList<IExternal> Externals { get; }

        public CompilationUnit(IList<IExternal> externals)
        {
            Externals = externals;
        }
    }

    public class FunctionDefinition : IExternal
    {
        public string Text { get; }
        public CompoundStatement CompoundStatement { get; }

        public FunctionDefinition(string text, CompoundStatement compoundStatement)
        {
            Text = text;
            CompoundStatement = compoundStatement;
        }
    }

    public class Declaration : IExternal, IStatement
    {
        public IList<IDeclarator> Declarators { get; }

        public Declaration(IList<IDeclarator> declarators)
        {
            Declarators = declarators;
        }
    }

    public class CompoundStatement : IStatement
    {
        public IList<IStatement> Statements;

        public CompoundStatement(IList<IStatement> statements)
        {
            Statements = statements;
        }
    }

    public class Declarator : IDeclarator
    {
        public string Text { get; }

        public Declarator(string text)
        {
            Text = text;
        }
    }

    public class InitDeclarator : IDeclarator
    {
        public Declarator Declarator { get; }
        // TODO: Initializer

        public InitDeclarator(Declarator declarator)
        {
            Declarator = declarator;
        }
    }
}
