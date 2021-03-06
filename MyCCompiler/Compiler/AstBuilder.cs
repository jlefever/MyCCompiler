﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCCompiler.Compiler
{
    public class AstBuilder
    {
        public CompilationUnit Build(CParser.CompilationUnitContext context)
        {
            return new CompilationUnit(Build(context.translationUnit()));
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<IExternal> Build(CParser.TranslationUnitContext context)
        {
            if (context.translationUnit() == null)
            {
                return CreateLinkedList(Build(context.externalDeclaration()));
            }

            var list = Build(context.translationUnit());
            list.AddLast(Build(context.externalDeclaration()));
            return list;
        }

        public static IExternal Build(CParser.ExternalDeclarationContext context)
        {
            if (context.functionDefinition() != null)
            {
                return Build(context.functionDefinition());
            }

            if (context.declaration() != null)
            {
                return Build(context.declaration());
            }

            // stray ";"
            return null;
        }

        public static FunctionDefinition Build(CParser.FunctionDefinitionContext context)
        {
            if (context.declarationSpecifiers() == null)
            {
                // Not supporting function definitions without declaration specifiers
                throw new NotSupportedException();
            }

            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarator = (FunctionDeclarator)Build(context.declarator()).DirectDeclarator;
            var compoundStatement = Build(context.compoundStatement());
            return new FunctionDefinition(declarationSpecifiers, declarator, compoundStatement);
        }

        public static Declaration Build(CParser.DeclarationContext context)
        {
            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarators = Build(context.initDeclaratorList());
            return new Declaration(declarationSpecifiers, declarators);
        }

        public static DeclarationSpecifiers Build(CParser.DeclarationSpecifiersContext context)
        {
            var typeSpecifiers = new LinkedList<ITypeSpecifier>();
            var storages = new HashSet<Storage>();
            var qualifiers = new HashSet<Qualifier>();

            foreach (var declarationSpecifier in context.declarationSpecifier().Select(Build))
            {
                switch (declarationSpecifier)
                {
                    case ITypeSpecifier typeSpecifier:
                        typeSpecifiers.AddLast(typeSpecifier);
                        continue;
                    case Storage storage:
                        storages.Add(storage);
                        continue;
                    case Qualifier qualifier:
                        qualifiers.Add(qualifier);
                        break;
                }
            }

            return new DeclarationSpecifiers(typeSpecifiers, storages, qualifiers);
        }

        public static IDeclarationSpecifier Build(CParser.DeclarationSpecifierContext context)
        {
            if (context.functionSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.alignmentSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.storageClassSpecifier() != null)
            {
                return Build(context.storageClassSpecifier());
            }

            if (context.typeSpecifier() != null)
            {
                return Build(context.typeSpecifier());
            }

            return Build(context.typeQualifier());
        }

        public static Storage Build(CParser.StorageClassSpecifierContext context)
        {
            // TODO: Handle typedef
            return StorageMap[context.GetText()];
        }

        public static ITypeSpecifier Build(CParser.TypeSpecifierContext context)
        {
            if (context.atomicTypeSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.structOrUnionSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.enumSpecifier() != null)
            {
                throw new NotImplementedException();
            }

            if (context.typedefName() != null)
            {
                throw new NotImplementedException();
            }

            if (context.typeSpecifier() != null)
            {
                var typeSpecifier = Build(context.typeSpecifier());
                var pointer = Build(context.pointer());
                return new TypeSpecifierWithPointer(typeSpecifier, pointer);
            }

            return TypeKeywordMap[context.GetText()];
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<IInitDeclarator> Build(CParser.InitDeclaratorListContext context)
        {
            if (context.initDeclaratorList() == null)
            {
                return CreateLinkedList(Build(context.initDeclarator()));
            }

            var list = Build(context.initDeclaratorList());
            list.AddLast(Build(context.initDeclarator()));
            return list;
        }

        public static IInitDeclarator Build(CParser.InitDeclaratorContext context)
        {
            var declarator = Build(context.declarator());

            if (context.initializer() != null)
            {
                var initializer = Build(context.initializer());
                return new InitializationDeclarator(declarator, initializer);
            }

            return declarator;
        }

        public static IExpression Build(CParser.InitializerContext context)
        {
            if (context.assignmentExpression() != null)
            {
                return Build(context.assignmentExpression());
            }

            // not supporting initializer lists at this moment
            throw new NotSupportedException();
        }

        public static Declarator Build(CParser.DeclaratorContext context)
        {
            if (context.pointer() != null)
            {
                // To my knowledge, this only happens when the
                // direct declarator uses parenthesis.
                // I am not supporting parenthesis at this time.
                throw new NotSupportedException();
            }

            return new Declarator(Build(context.directDeclarator()));
        }

        public static IDirectDeclarator Build(CParser.DirectDeclaratorContext context)
        {
            // identifier, most basic form of declarator
            if (context.Identifier() != null)
            {
                return new Identifier(context.Identifier().Symbol.Text);
            }

            // declarator with parenthesis
            if (context.declarator() != null)
            {
                throw new NotSupportedException();
            }

            var directDeclarator = Build(context.directDeclarator());

            // array
            if (context.GetChild(1).GetText() == "[")
            {
                throw new NotImplementedException();
            }

            // function pointer
            if (context.pointer() != null)
            {
                throw new NotImplementedException();
            }

            // I'm not sure when this happens
            if (context.identifierList() != null)
            {
                throw new NotSupportedException();
            }

            // function
            var identifier = (Identifier)directDeclarator;
            if (context.parameterTypeList() != null)
            {
                var parameterList = Build(context.parameterTypeList());
                return new FunctionDeclarator(identifier, parameterList);
            }

            return new FunctionDeclarator(identifier);
        }

        public static ParameterList Build(CParser.ParameterTypeListContext context)
        {
            var parameters = Build(context.parameterList());
            var variadic = context.ChildCount > 1;
            return new ParameterList(parameters, variadic);
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<Parameter> Build(CParser.ParameterListContext context)
        {
            if (context.parameterList() == null)
            {
                return CreateLinkedList(Build(context.parameterDeclaration()));
            }

            var list = Build(context.parameterList());
            list.AddLast(Build(context.parameterDeclaration()));
            return list;
        }

        public static Parameter Build(CParser.ParameterDeclarationContext context)
        {
            if (context.declarationSpecifiers2() != null)
            {
                // Parameters with abstract declarators
                // are not supported at this time.
                throw new NotImplementedException();
            }

            var declarationSpecifiers = Build(context.declarationSpecifiers());
            var declarator = Build(context.declarator());
            return new Parameter(declarationSpecifiers, declarator);
        }

        public static Pointer Build(CParser.PointerContext context)
        {
            if (context.pointer() != null)
            {
                // I don't know when this happens
                // but if it is to be supported a new pointer type is needed
                throw new NotSupportedException();
            }

            if (context.typeQualifierList() != null)
            {
                var qualifiers = Build(context.typeQualifierList());
                return new Pointer(new HashSet<Qualifier>(qualifiers));
            }

            return new Pointer();
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<Qualifier> Build(CParser.TypeQualifierListContext context)
        {
            if (context.typeQualifierList() == null)
            {
                return CreateLinkedList(Build(context.typeQualifier()));
            }

            var list = Build(context.typeQualifierList());
            list.AddLast(Build(context.typeQualifier()));
            return list;
        }

        public static Qualifier Build(CParser.TypeQualifierContext context)
        {
            return QualifierMap[context.GetText()];
        }

        public static CompoundStatement Build(CParser.CompoundStatementContext context)
        {
            if (context.blockItemList() == null)
            {
                // Empty brackets
                return new CompoundStatement(new LinkedList<IStatement>());
            }

            var statements = Build(context.blockItemList());
            return new CompoundStatement(statements);
        }


        // This is a list grammar. Consider using generics.
        public static LinkedList<IStatement> Build(CParser.BlockItemListContext context)
        {
            if (context.blockItemList() == null)
            {
                return CreateLinkedList(Build(context.blockItem()));
            }

            var list = Build(context.blockItemList());
            list.AddLast(Build(context.blockItem()));
            return list;
        }

        public static IStatement Build(CParser.BlockItemContext context)
        {
            if (context.statement() != null)
            {
                return Build(context.statement());
            }

            return Build(context.declaration());
        }

        public static IStatement Build(CParser.StatementContext context)
        {
            if (context.compoundStatement() != null)
            {
                return Build(context.compoundStatement());
            }

            if (context.expressionStatement() != null)
            {
                return Build(context.expressionStatement());
            }

            if (context.jumpStatement() != null)
            {
                return Build(context.jumpStatement());
            }

            if (context.selectionStatement() != null)
            {
                return Build(context.selectionStatement());
            }

            if (context.iterationStatement() != null)
            {
                return Build(context.iterationStatement());
            }

            // Other statements not supported yet
            throw new NotImplementedException();
        }

        public static IIterationStatement Build(CParser.IterationStatementContext context)
        {
            var body = Build(context.statement());

            if (context.GetChild(0).GetText() == "while")
            {
                var expression = Build(context.expression());
                return new WhileStatement(expression, body);
            }

            if (context.GetChild(0).GetText() == "do")
            {
                var expression = Build(context.expression());
                return new DoWhileStatement(expression, body);
            }

            var first = Build(context.forCondition().expression()[0]);
            var second = Build(context.forCondition().expression()[1]);
            var third = Build(context.forCondition().expression()[2]);
            return new ForStatement(first, second, third, body);
        }

        public static IIfStatement Build(CParser.SelectionStatementContext context)
        {
            if (context.GetChild(0).GetText() == "if")
            {
                var expression = Build(context.expression());
                var body = Build(context.statement()[0]);

                if (context.statement().Length == 2)
                {
                    var @else = Build(context.statement()[1]);
                    return new IfElseStatement(expression, body, @else);
                }

                return new IfStatement(expression, body);
            }

            throw new NotImplementedException();
        }

        public static IReturnStatement Build(CParser.JumpStatementContext context)
        {
            if (context.GetChild(0).GetText() == "return")
            {
                if (context.expression() == null)
                {
                    return new ReturnVoidStatement();
                }

                var expression = Build(context.expression());
                return new ReturnStatement(expression);
            }

            throw new NotSupportedException();
        }

        public static ExpressionStatement Build(CParser.ExpressionStatementContext context)
        {
            // TODO: Handle stray ;
            return new ExpressionStatement(Build(context.expression()));
        }

        public static IExpression Build(CParser.ExpressionContext context)
        {
            if (context.expression() == null)
            {
                return Build(context.assignmentExpression());
            }

            var left = Build(context.expression());
            var right = Build(context.assignmentExpression());
            return new BinaryExpression(BinaryOpKind.Comma, left, right);
        }

        public static IExpression Build(CParser.AssignmentExpressionContext context)
        {
            if (context.conditionalExpression() != null)
            {
                return Build(context.conditionalExpression());
            }

            var identifier = (Identifier)Build(context.unaryExpression());
            var assignmentKind = Build(context.assignmentOperator());
            var expression = Build(context.assignmentExpression());
            return new AssignmentExpression(identifier, assignmentKind, expression);
        }

        public static IExpression Build(CParser.ConditionalExpressionContext context)
        {
            // ternary operator
            if (context.expression() != null)
            {
                throw new NotSupportedException();
            }

            return Build(context.logicalOrExpression());
        }

        public static IExpression Build(CParser.LogicalOrExpressionContext context)
        {
            if (context.logicalOrExpression() == null)
            {
                return Build(context.logicalAndExpression());
            }

            var left = Build(context.logicalOrExpression());
            var right = Build(context.logicalAndExpression());
            return new BinaryExpression(BinaryOpKind.Or, left, right);
        }

        public static IExpression Build(CParser.LogicalAndExpressionContext context)
        {
            if (context.logicalAndExpression() == null)
            {
                return Build(context.inclusiveOrExpression());
            }

            var left = Build(context.logicalAndExpression());
            var right = Build(context.inclusiveOrExpression());
            return new BinaryExpression(BinaryOpKind.And, left, right);
        }

        public static IExpression Build(CParser.InclusiveOrExpressionContext context)
        {
            if (context.inclusiveOrExpression() == null)
            {
                return Build(context.exclusiveOrExpression());
            }

            var left = Build(context.inclusiveOrExpression());
            var right = Build(context.exclusiveOrExpression());
            return new BinaryExpression(BinaryOpKind.BitwiseOr, left, right);
        }

        public static IExpression Build(CParser.ExclusiveOrExpressionContext context)
        {
            if (context.exclusiveOrExpression() == null)
            {
                return Build(context.andExpression());
            }

            var left = Build(context.exclusiveOrExpression());
            var right = Build(context.andExpression());
            return new BinaryExpression(BinaryOpKind.BitwiseXor, left, right);
        }

        public static IExpression Build(CParser.AndExpressionContext context)
        {
            if (context.andExpression() == null)
            {
                return Build(context.equalityExpression());
            }

            var left = Build(context.andExpression());
            var right = Build(context.equalityExpression());
            return new BinaryExpression(BinaryOpKind.BitwiseAnd, left, right);
        }

        public static IExpression Build(CParser.EqualityExpressionContext context)
        {
            if (context.equalityExpression() == null)
            {
                return Build(context.relationalExpression());
            }

            var left = Build(context.equalityExpression());
            var right = Build(context.relationalExpression());
            var op = BinaryOpKindMap[context.GetChild(1).GetText()];
            return new BinaryExpression(op, left, right);
        }

        public static IExpression Build(CParser.RelationalExpressionContext context)
        {
            if (context.relationalExpression() == null)
            {
                return Build(context.shiftExpression());
            }

            var left = Build(context.relationalExpression());
            var right = Build(context.shiftExpression());
            var op = BinaryOpKindMap[context.GetChild(1).GetText()];
            return new BinaryExpression(op, left, right);
        }

        public static IExpression Build(CParser.ShiftExpressionContext context)
        {
            if (context.shiftExpression() == null)
            {
                return Build(context.additiveExpression());
            }

            var left = Build(context.shiftExpression());
            var right = Build(context.additiveExpression());
            var op = BinaryOpKindMap[context.GetChild(1).GetText()];
            return new BinaryExpression(op, left, right);
        }

        public static IExpression Build(CParser.AdditiveExpressionContext context)
        {
            if (context.additiveExpression() == null)
            {
                return Build(context.multiplicativeExpression());
            }

            var left = Build(context.additiveExpression());
            var right = Build(context.multiplicativeExpression());
            var op = BinaryOpKindMap[context.GetChild(1).GetText()];
            return new BinaryExpression(op, left, right);
        }

        public static IExpression Build(CParser.MultiplicativeExpressionContext context)
        {
            if (context.multiplicativeExpression() == null)
            {
                return Build(context.castExpression());
            }

            var left = Build(context.multiplicativeExpression());
            var right = Build(context.castExpression());
            var op = BinaryOpKindMap[context.GetChild(1).GetText()];
            return new BinaryExpression(op, left, right);
        }

        public static IExpression Build(CParser.CastExpressionContext context)
        {
            if (context.unaryExpression() != null)
            {
                return Build(context.unaryExpression());
            }

            // no other cast expresions supported currently
            throw new NotSupportedException();
        }

        public static IExpression Build(CParser.UnaryExpressionContext context)
        {
            if (context.postfixExpression() != null)
            {
                return Build(context.postfixExpression());
            }

            if (context.unaryOperator() != null)
            {
                var op = Build(context.unaryOperator());
                var expression = Build(context.castExpression());
                return new UnaryExpression(op, expression);
            }

            // no other unary expresions supported currently
            throw new NotSupportedException();
        }

        public static IExpression Build(CParser.PostfixExpressionContext context)
        {
            if (context.primaryExpression() != null)
            {
                return Build(context.primaryExpression());
            }

            // function call
            if (context.children.Last().GetText() == ")")
            {
                var identifier = (Identifier)Build(context.postfixExpression());

                if (context.argumentExpressionList() == null)
                {
                    return new FunctionCall(identifier);
                }

                var arguments = Build(context.argumentExpressionList());
                return new FunctionCall(identifier, arguments);
            }

            // no other postfix expresions supported currently
            throw new NotSupportedException();
        }

        // This is a list grammar. Consider using generics.
        public static LinkedList<IExpression> Build(CParser.ArgumentExpressionListContext context)
        {
            if (context.argumentExpressionList() == null)
            {
                return CreateLinkedList(Build(context.assignmentExpression()));
            }

            var list = Build(context.argumentExpressionList());
            list.AddLast(Build(context.assignmentExpression()));
            return list;
        }

        public static IPrimaryExpression Build(CParser.PrimaryExpressionContext context)
        {
            if (context.Identifier() != null)
            {
                return new Identifier(context.Identifier().Symbol.Text);
            }

            if (context.Constant() != null)
            {
                return new Constant(context.Constant().Symbol.Text);
            }

            if (context.StringLiteral() != null)
            {
                return new StringLiteral(context.StringLiteral()[0].Symbol.Text);
            }

            // no other primary expresions supported currently
            throw new NotSupportedException();
        }

        public static AssignmentKind Build(CParser.AssignmentOperatorContext context)
        {
            return AssignmentKindMap[context.GetText()];
        }

        public static UnaryOpKind Build(CParser.UnaryOperatorContext context)
        {
            return UnaryOpKindMap[context.GetText()];
        }

        private static LinkedList<T> CreateLinkedList<T>(T element)
        {
            var list = new LinkedList<T>();
            list.AddLast(element);
            return list;
        }

        private static readonly IDictionary<string, Qualifier> QualifierMap = new Dictionary<string, Qualifier>
        {
            { "const", Qualifier.Const },
            { "volatile", Qualifier.Volatile },
            { "restrict", Qualifier.Restrict },
            { "_Atomic", Qualifier.Atomic }
        };

        private static readonly IDictionary<string, Storage> StorageMap = new Dictionary<string, Storage>
        {
            { "auto", Storage.Auto },
            { "register", Storage.Register },
            { "static", Storage.Static },
            { "extern", Storage.Extern }
        };

        private static readonly IDictionary<string, TypeKeyword> TypeKeywordMap = new Dictionary<string, TypeKeyword>
        {
            { "void", TypeKeyword.Void },
            { "char", TypeKeyword.Char },
            { "short", TypeKeyword.Short },
            { "int", TypeKeyword.Int },
            { "long", TypeKeyword.Long },
            { "float", TypeKeyword.Float },
            { "double", TypeKeyword.Double },
            { "signed", TypeKeyword.Signed },
            { "unsigned", TypeKeyword.Unsigned },
            { "_Bool", TypeKeyword.Bool },
            { "_Complex", TypeKeyword.Complex }
        };

        private static readonly IDictionary<string, AssignmentKind> AssignmentKindMap = new Dictionary<string, AssignmentKind>
        {
            { "=", AssignmentKind.Assign },
            { "*=", AssignmentKind.MulAssign },
            { "/=", AssignmentKind.DivAssign },
            { "%=", AssignmentKind.ModAssign },
            { "+=", AssignmentKind.AddAssign },
            { "-=", AssignmentKind.SubAssign },
            { "<<=", AssignmentKind.LShiftAssign },
            { ">>=", AssignmentKind.RShiftAssign },
            { "&=", AssignmentKind.AndAssign },
            { "^=", AssignmentKind.XorAssign },
            { "|=", AssignmentKind.OrAssign }
        };

        private static readonly IDictionary<string, BinaryOpKind> BinaryOpKindMap = new Dictionary<string, BinaryOpKind>
        {
            { "==", BinaryOpKind.EqualTo },
            { "!=", BinaryOpKind.NotEqualTo },
            { "<", BinaryOpKind.LessThan },
            { ">", BinaryOpKind.GreaterThan },
            { "<=", BinaryOpKind.LessThanOrEqualTo },
            { ">=", BinaryOpKind.GreaterThanOrEqualTo },
            { "<<", BinaryOpKind.LShift },
            { ">>", BinaryOpKind.RShift },
            { "+", BinaryOpKind.Addition },
            { "-", BinaryOpKind.Subtraction },
            { "*", BinaryOpKind.Multiplication },
            { "/", BinaryOpKind.Division },
            { "%", BinaryOpKind.Modulus }
        };

        private static readonly IDictionary<string, UnaryOpKind> UnaryOpKindMap = new Dictionary<string, UnaryOpKind>
        {
            { "&", UnaryOpKind.AddressOf },
            { "*", UnaryOpKind.Dereference },
            { "+", UnaryOpKind.Plus },
            { "-", UnaryOpKind.Minus },
            { "~", UnaryOpKind.BitwiseNot },
            { "!", UnaryOpKind.Not }
        };
    }
}
