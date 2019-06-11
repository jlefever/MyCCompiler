using System.Collections.Generic;

namespace MyCCompiler.Compiler
{
    public interface IExternal { }

    public interface IStatement { }

    public interface IInitDeclarator { }

    public interface IDirectDeclarator { }

    public interface IDeclarationSpecifier { }

    public interface ITypeSpecifier : IDeclarationSpecifier { }

    public interface IExpression { }

    public interface IPrimaryExpression : IExpression { }

    public interface IReturnStatement : IStatement { }

    public interface IIterationStatement : IStatement
    {
        IStatement Body { get; }
    }

    public interface IWhileStatement : IIterationStatement
    {
        IExpression Expression { get; }
    }

    public interface IIfStatement : IStatement
    {
        IExpression Expression { get; }
        IStatement Body { get; }
    }

    public class CompilationUnit
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

        public bool IsMain => FunctionDeclarator.Identifier.Text == "main";

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
        public SymbolTable SymbolTable { get; set; }
        public LinkedList<IStatement> Statements { get; }

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

    public class Pointer
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

    public class DeclarationSpecifiers
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

    public class ParameterList
    {
        public LinkedList<Parameter> Parameters { get; }
        public bool Variadic { get; }

        public ParameterList(LinkedList<Parameter> parameters, bool variadic)
        {
            Parameters = parameters;
            Variadic = variadic;
        }
    }

    public class Parameter
    {
        public DeclarationSpecifiers DeclarationSpecifiers { get; }
        public Declarator Declarator { get; }

        public Parameter(DeclarationSpecifiers declarationSpecifiers, Declarator declarator)
        {
            DeclarationSpecifiers = declarationSpecifiers;
            Declarator = declarator;
        }
    }

    public class IfStatement : IIfStatement
    {
        public IExpression Expression { get; }
        public IStatement Body { get; }

        public IfStatement(IExpression expression, IStatement body)
        {
            Expression = expression;
            Body = body;
        }
    }

    public class IfElseStatement : IIfStatement
    {
        public IExpression Expression { get; }
        public IStatement Body { get; }
        public IStatement Else { get; }

        public IfElseStatement(IExpression expression, IStatement body, IStatement @else)
        {
            Expression = expression;
            Body = body;
            Else = @else;
        }
    }

    public class WhileStatement : IWhileStatement
    {
        public IExpression Expression { get; }
        public IStatement Body { get; }

        public WhileStatement(IExpression expression, IStatement body)
        {
            Expression = expression;
            Body = body;
        }
    }

    public class DoWhileStatement : IWhileStatement
    {
        public IExpression Expression { get; }
        public IStatement Body { get; }

        public DoWhileStatement(IExpression expression, IStatement body)
        {
            Expression = expression;
            Body = body;
        }
    }

    public class ForStatement : IIterationStatement
    {
        public IExpression FirstExpression { get; }
        public IExpression SecondExpression { get; }
        public IExpression ThirdExpression { get; }
        public IStatement Body { get; }

        public ForStatement(IExpression firstExpression, IExpression secondExpression, IExpression thirdExpression, IStatement body)
        {
            FirstExpression = firstExpression;
            SecondExpression = secondExpression;
            ThirdExpression = thirdExpression;
            Body = body;
        }
    }

    public class ReturnStatement : IReturnStatement
    {
        public IExpression Expression { get; }

        public ReturnStatement(IExpression expression)
        {
            Expression = expression;
        }
    }

    public class ReturnVoidStatement : IReturnStatement { }

    public class ExpressionStatement : IStatement
    {
        public IExpression Expression { get; }

        public ExpressionStatement(IExpression expression)
        {
            Expression = expression;
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

    public class StringLiteral : IPrimaryExpression
    {
        public string Text { get; }

        public StringLiteral(string text)
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
        public static Qualifier Const = new Qualifier();
        public static Qualifier Volatile = new Qualifier();
        public static Qualifier Restrict = new Qualifier();
        public static Qualifier Atomic = new Qualifier();

        private Qualifier() { }
    }

    public class Storage : IDeclarationSpecifier
    {
        public static Storage Auto = new Storage();
        public static Storage Register = new Storage();
        public static Storage Static = new Storage();
        public static Storage Extern = new Storage();

        private Storage() { }
    }

    public class TypeKeyword : ITypeSpecifier
    {
        public static TypeKeyword Void = new TypeKeyword();
        public static TypeKeyword Signed = new TypeKeyword();
        public static TypeKeyword Unsigned = new TypeKeyword();
        public static TypeKeyword Short = new TypeKeyword();
        public static TypeKeyword Long = new TypeKeyword();
        public static TypeKeyword Char = new TypeKeyword();
        public static TypeKeyword Int = new TypeKeyword();
        public static TypeKeyword Float = new TypeKeyword();
        public static TypeKeyword Double = new TypeKeyword();
        public static TypeKeyword Bool = new TypeKeyword();
        public static TypeKeyword Complex = new TypeKeyword();

        private TypeKeyword() { }
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
        Comma,
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
