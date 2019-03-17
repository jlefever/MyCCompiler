using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public interface IStatement : INode { }

    public interface IInitDeclarator : INode { }

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
        public LinkedList<IDeclarationSpecifier> DeclarationSpecifiers { get; }
        public Declarator Declarator { get; }
        public CompoundStatement CompoundStatement { get; }

        public FunctionDefinition(LinkedList<IDeclarationSpecifier> declarationSpecifiers, Declarator declarator, CompoundStatement compoundStatement)
        {
            DeclarationSpecifiers = declarationSpecifiers;
            Declarator = declarator;
            CompoundStatement = compoundStatement;
        }
    }

    public class Declaration : IExternal, IStatement
    {
        public LinkedList<IDeclarationSpecifier> DeclarationSpecifiers { get; }
        public LinkedList<IInitDeclarator> InitDeclarators { get; }

        public Declaration(LinkedList<IDeclarationSpecifier> declarationSpecifiers, LinkedList<IInitDeclarator> initDeclarators)
        {
            DeclarationSpecifiers = declarationSpecifiers;
            InitDeclarators = initDeclarators;
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

    public class Declarator : IInitDeclarator
    {
        public IDirectDeclarator DirectDeclarator { get; }

        public Declarator(IDirectDeclarator directDeclarator)
        {
            DirectDeclarator = directDeclarator;
        }
    }

    public class InitializationDeclarator : IInitDeclarator
    {
        public Declarator Declarator { get; }
        // TODO: Initializer

        public InitializationDeclarator(Declarator declarator)
        {
            Declarator = declarator;
        }
    }

    public interface IDirectDeclarator { }

    public class Identifier : IDirectDeclarator
    {
        public string Lexme { get; }

        public Identifier(string lexme)
        {
            Lexme = lexme;
        }
    }

    public class ParensDeclarator : IDirectDeclarator
    {
        public Declarator Declarator { get; }

        public ParensDeclarator(Declarator declarator)
        {
            Declarator = declarator;
        }
    }

    public class FunctionDeclarator : IDirectDeclarator
    {
        private IDirectDeclarator DirectDeclarator { get; }

        public FunctionDeclarator(IDirectDeclarator directDeclarator)
        {
            DirectDeclarator = directDeclarator;
        }
    }

    public interface IPointer : INode
    {
        ISet<Qualifier> Qualifiers { get; }
    }

    public class PointerWithPointer : IPointer
    {
        public ISet<Qualifier> Qualifiers { get; }
        public IPointer Pointer { get; }

        public PointerWithPointer(ISet<Qualifier> qualifiers, IPointer pointer)
        {
            Qualifiers = qualifiers;
            Pointer = pointer;
        }
    }

    public class Pointer : IPointer
    {
        public ISet<Qualifier> Qualifiers { get; }

        public Pointer(ISet<Qualifier> qualifiers)
        {
            Qualifiers = qualifiers;
        }
    }

    public interface IDeclarationSpecifier : INode { }

    public interface ITypeSpecifier : IDeclarationSpecifier { }

    public class TypeSpecifierWithPointer : ITypeSpecifier
    {
        public ITypeSpecifier TypeSpecifier { get; }
        public IPointer Pointer { get; }

        public TypeSpecifierWithPointer(ITypeSpecifier typeSpecifier, IPointer pointer)
        {
            TypeSpecifier = typeSpecifier;
            Pointer = pointer;
        }
    }

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

    public class TypeKeyword : ITypeSpecifier
    {
        public bool Equals(TypeKeyword other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return TypeKeywordKind == other.TypeKeywordKind;
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

            if (obj.GetType() != typeof(TypeKeyword))
            {
                return false;
            }

            return Equals((TypeKeyword)obj);
        }

        public override int GetHashCode()
        {
            return (int)TypeKeywordKind;
        }

        public static bool operator ==(TypeKeyword left, TypeKeyword right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeKeyword left, TypeKeyword right)
        {
            return !Equals(left, right);
        }

        public TypeKeywordKind TypeKeywordKind { get; }

        public TypeKeyword(TypeKeywordKind typeKeywordKind)
        {
            TypeKeywordKind = typeKeywordKind;
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

    public enum TypeKeywordKind
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
