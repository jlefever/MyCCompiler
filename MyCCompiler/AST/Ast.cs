using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public interface IStatement : INode { }

    public interface IDeclarator : INode { }

    public class CompilationUnit : INode
    {
        public LinkedList<IExternal> Externals { get; }

        public CompilationUnit(LinkedList<IExternal> externals)
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
        public LinkedList<IDeclarator> Declarators { get; }

        public Declaration(LinkedList<IDeclarator> declarators)
        {
            Declarators = declarators;
        }
    }

    public class CompoundStatement : IStatement
    {
        public LinkedList<IStatement> Statements;

        public CompoundStatement(LinkedList<IStatement> statements)
        {
            Statements = statements;
        }
    }

    public class Declarator : IDeclarator
    {
        public Identifier Identifier { get; }

        public Declarator(Identifier identifier)
        {
            Identifier = identifier;
        }
    }

    public class InitializationDeclarator : IDeclarator
    {
        public Declarator Declarator { get; }
        // TODO: Initializer

        public InitializationDeclarator(Declarator declarator)
        {
            Declarator = declarator;
        }
    }

    public class Identifier
    {
        public string Lexme { get; }

        public Identifier(string lexme)
        {
            Lexme = lexme;
        }
    }

    public interface IPointer : INode
    {
        bool IsAtomic { get; }
        bool IsConst { get; }
        bool IsRestricted { get; }
        bool IsVolitile { get; }
    }

    public class PointerToPointer : IPointer
    {
        public IPointer Pointer { get; }
        public bool IsAtomic { get; }
        public bool IsConst { get; }
        public bool IsRestricted { get; }
        public bool IsVolitile { get; }

        public PointerToPointer(IPointer pointer, bool isAtomic, bool isConst, bool isRestricted, bool isVolitile)
        {
            Pointer = pointer;
            IsAtomic = isAtomic;
            IsConst = isConst;
            IsRestricted = isRestricted;
            IsVolitile = isVolitile;
        }
    }

    public class TerminalPointer : IPointer
    {
        public bool IsAtomic { get; }
        public bool IsConst { get; }
        public bool IsRestricted { get; }
        public bool IsVolitile { get; }

        public TerminalPointer(bool isAtomic, bool isConst, bool isRestricted, bool isVolitile)
        {
            IsAtomic = isAtomic;
            IsConst = isConst;
            IsRestricted = isRestricted;
            IsVolitile = isVolitile;
        }
    }
}
