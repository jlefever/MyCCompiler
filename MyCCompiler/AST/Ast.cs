using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public interface IStatement : INode { }

    public interface IInitDeclarator : INode { }

    public interface IDirectDeclarator : INode { }

    public interface IDeclarationSpecifier : INode { }

    public interface ITypeSpecifier : IDeclarationSpecifier { }

    public interface IExpression : INode { }

    public interface IPrimaryExpression : IExpression { }

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
        public DeclarationSpecifiers DeclarationSpecifiers { get; }
        public FunctionDeclarator FunctionDeclarator { get; }
        public CompoundStatement CompoundStatement { get; }

        public FunctionDefinition(DeclarationSpecifiers declarationSpecifiers, FunctionDeclarator functionDeclarator, CompoundStatement compoundStatement)
        {
            DeclarationSpecifiers = declarationSpecifiers;
            FunctionDeclarator = functionDeclarator;
            CompoundStatement = compoundStatement;
        }
    }

    public class Declaration : IExternal, IStatement
    {
        public DeclarationSpecifiers DeclarationSpecifiers { get; }
        public LinkedList<IInitDeclarator> InitDeclarators { get; }

        public Declaration(DeclarationSpecifiers declarationSpecifiers, LinkedList<IInitDeclarator> initDeclarators)
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
        public IExpression Expression { get; }

        public InitializationDeclarator(Declarator declarator, IExpression expression)
        {
            Declarator = declarator;
            Expression = expression;
        }
    }

    public class Identifier : IDirectDeclarator, IPrimaryExpression
    {
        public Symbol Symbol { get; set; }
        public string Text { get; }

        public Identifier(string text)
        {
            Text = text;
        }
    }

    public class FunctionDeclarator : IDirectDeclarator
    {
        public Identifier Identifier { get; }
        public ParameterList ParameterList { get; }

        public FunctionDeclarator(Identifier identifier)
        {
            Identifier = identifier;
            ParameterList = new ParameterList(new LinkedList<Parameter>(), false);
        }

        public FunctionDeclarator(Identifier identifier, ParameterList parameterList)
        {
            Identifier = identifier;
            ParameterList = parameterList;
        }
    }

    public class Pointer : INode
    {
        public ISet<Qualifier> Qualifiers { get; }

        public Pointer()
        {
            Qualifiers = new HashSet<Qualifier>();
        }

        public Pointer(ISet<Qualifier> qualifiers)
        {
            Qualifiers = qualifiers;
        }
    }

    public class DeclarationSpecifiers : INode
    {
        public LinkedList<ITypeSpecifier> TypeSpecifiers { get; }
        public ISet<Storage> Storages { get; }
        public ISet<Qualifier> Qualifiers { get; }

        public DeclarationSpecifiers(LinkedList<ITypeSpecifier> typeSpecifiers, ISet<Storage> storages, ISet<Qualifier> qualifiers)
        {
            TypeSpecifiers = typeSpecifiers;
            Storages = storages;
            Qualifiers = qualifiers;
        }
    }

    public class TypeSpecifierWithPointer : ITypeSpecifier
    {
        public ITypeSpecifier TypeSpecifier { get; }
        public Pointer Pointer { get; }

        public TypeSpecifierWithPointer(ITypeSpecifier typeSpecifier, Pointer pointer)
        {
            TypeSpecifier = typeSpecifier;
            Pointer = pointer;
        }
    }

    public class ParameterList : INode
    {
        public LinkedList<Parameter> Parameters { get; }
        public bool Variadic { get; }

        public ParameterList(LinkedList<Parameter> parameters, bool variadic)
        {
            Parameters = parameters;
            Variadic = variadic;
        }
    }

    public class Parameter : INode
    {
        public DeclarationSpecifiers DeclarationSpecifiers { get; }
        public Declarator Declarator { get; }

        public Parameter(DeclarationSpecifiers declarationSpecifiers, Declarator declarator)
        {
            DeclarationSpecifiers = declarationSpecifiers;
            Declarator = declarator;
        }
    }

    public class ExpressionStatement : IStatement
    {
        public LinkedList<IExpression> Expressions { get; }

        public ExpressionStatement(LinkedList<IExpression> expressions)
        {
            Expressions = expressions;
        }
    }

    public class AssignmentExpression : IExpression
    {
        public Identifier Identifier { get; }
        public AssignmentKind AssignmentKind { get; }
        public IExpression Expression { get; }

        public AssignmentExpression(Identifier identifier, AssignmentKind assignmentKind, IExpression expression)
        {
            Identifier = identifier;
            AssignmentKind = assignmentKind;
            Expression = expression;
        }
    }

    public class Constant : IPrimaryExpression
    {
        public string Text { get; }

        public Constant(string text)
        {
            Text = text;
        }
    }

    public class BinaryExpression : IExpression
    {
        public BinaryOpKind Operator { get; }
        public IExpression Left { get; }
        public IExpression Right { get; }

        public BinaryExpression(BinaryOpKind @operator, IExpression left, IExpression right)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }
    }

    public class UnaryExpression : IExpression
    {
        public UnaryOpKind Operator { get; }
        public IExpression Expression { get; }

        public UnaryExpression(UnaryOpKind @operator, IExpression expression)
        {
            Operator = @operator;
            Expression = expression;
        }
    }

    public class FunctionCall : IExpression
    {
        public Identifier Identifier { get; }
        public LinkedList<IExpression> Arguments { get; }

        public FunctionCall(Identifier identifier)
        {
            Identifier = identifier;
            Arguments = new LinkedList<IExpression>();
        }

        public FunctionCall(Identifier identifier, LinkedList<IExpression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
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
        Signed,
        Unsigned,
        Short,
        Long,
        Char,
        Int,
        Float,
        Double,
        Bool,
        Complex
    }

    public enum AssignmentKind
    {
        Assign,
        MulAssign,
        DivAssign,
        ModAssign,
        AddAssign,
        SubAssign,
        LShiftAssign,
        RShiftAssign,
        AndAssign,
        XorAssign,
        OrAssign
    }

    public enum BinaryOpKind
    {
        Or,
        And,
        BitwiseOr,
        BitwiseXor,
        BitwiseAnd,
        EqualTo,
        NotEqualTo,
        LessThan,
        GreaterThan,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo,
        LShift,
        RShift,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulus
    }

    public enum UnaryOpKind
    {
        AddressOf,
        Dereference,
        Plus,
        Minus,
        BitwiseNot,
        Not
    }
}
