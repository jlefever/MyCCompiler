using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public interface IStatement : INode { }

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
        public string Specifier { get; }
        public string Lexme { get; }

        public Declaration(string specifier, string lexme)
        {
            Specifier = specifier;
            Lexme = lexme;
        }
    }

    public class Statement : IStatement
    {

    }

    public class CompoundStatement : IStatement
    {
        public IList<IStatement> Statements;

        public CompoundStatement(IList<IStatement> statements)
        {
            Statements = statements;
        }
    }
}
