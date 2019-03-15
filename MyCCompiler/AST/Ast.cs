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
        public LinkedList<IDeclarationSpecifier> DeclarationSpecifiers { get; }
        public LinkedList<IDeclarator> Declarators { get; }

        public Declaration(LinkedList<IDeclarationSpecifier> declarationSpecifiers, LinkedList<IDeclarator> declarators)
        {
            DeclarationSpecifiers = declarationSpecifiers;
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
        ISet<Qualifier> Qualifiers { get; }
    }

    public class PointerToPointer : IPointer
    {
        public ISet<Qualifier> Qualifiers { get; }
        public IPointer Pointer { get; }

        public PointerToPointer(ISet<Qualifier> qualifiers, IPointer pointer)
        {
            Qualifiers = qualifiers;
            Pointer = pointer;
        }
    }

    public class TerminalPointer : IPointer
    {
        public ISet<Qualifier> Qualifiers { get; }

        public TerminalPointer(ISet<Qualifier> qualifiers)
        {
            Qualifiers = qualifiers;
        }
    }

    public interface IDeclarationSpecifier : INode { }

    public class Qualifier : IDeclarationSpecifier
    {
        public bool Equals(Qualifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return QualifierKind == other.QualifierKind;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Qualifier))
            {
                return false;
            }

            return Equals((Qualifier)obj);
        }

        public override int GetHashCode()
        {
            return (int)QualifierKind;
        }

        public static bool operator ==(Qualifier left, Qualifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Qualifier left, Qualifier right)
        {
            return !Equals(left, right);
        }

        public QualifierKind QualifierKind { get; }

        public Qualifier(QualifierKind qualifierKind)
        {
            QualifierKind = qualifierKind;
        }
    }

    public class Storage : IDeclarationSpecifier
    {
        public bool Equals(Storage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return StorageKind == other.StorageKind;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Storage))
            {
                return false;
            }

            return Equals((Storage)obj);
        }

        public override int GetHashCode()
        {
            return (int)StorageKind;
        }

        public static bool operator ==(Storage left, Storage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Storage left, Storage right)
        {
            return !Equals(left, right);
        }

        public StorageKind StorageKind { get; }

        public Storage(StorageKind storageKind)
        {
            StorageKind = storageKind;
        }
    }

    public class TypeSpecifier : IDeclarationSpecifier
    {
        public bool Equals(TypeSpecifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return TypeSpecifierKind == other.TypeSpecifierKind;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(TypeSpecifier))
            {
                return false;
            }

            return Equals((TypeSpecifier)obj);
        }

        public override int GetHashCode()
        {
            return (int)TypeSpecifierKind;
        }

        public static bool operator ==(TypeSpecifier left, TypeSpecifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeSpecifier left, TypeSpecifier right)
        {
            return !Equals(left, right);
        }

        public TypeSpecifierKind TypeSpecifierKind { get; }

        public TypeSpecifier(TypeSpecifierKind typeSpecifierKind)
        {
            TypeSpecifierKind = typeSpecifierKind;
        }
    }

    public enum QualifierKind
    {
        Const,
        Volatile,
        Restrict,
        Atomic
    }

    public enum StorageKind
    {
        Auto,
        Register,
        Static,
        Extern
    }

    public enum TypeSpecifierKind
    {
        Void,
        Char,
        Short,
        Int,
        Long,
        Float,
        Double,
        Signed,
        Unsigned,
        Bool,
        Complex
    }
}
