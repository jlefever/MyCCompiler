using System;
using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class AsmBuilder
    {
        private readonly LinkedList<ILine> _lines;
        private int _currFrameOffset;

        public AsmBuilder()
        {
            _lines = new LinkedList<ILine>();
            _currFrameOffset = 0;
        }

        public LinkedList<ILine> Visit(CompilationUnit node)
        {
            // Not sure if this should go here
            Add(new TextDirective());

            foreach (var external in node.Externals)
            {
                Visit(external);
            }

            return _lines;
        }

        private void Visit(IExternal node)
        {
            switch (node)
            {
                case FunctionDefinition n:
                    Visit(n);
                    break;
                case Declaration n:
                    break;
            }
        }

        private void Visit(FunctionDefinition node)
        {
            // Create globl directive and label for function
            var funcName = "_" + node.FunctionDeclarator.Identifier.Text;
            Add(new GloblDirective(funcName));
            Add(new Label(funcName));

            // Preamble
            // Save the old base pointer value
            Add(new Push(Register.Ebp));

            // Create stack frame
            Add(new Mov(Register.Esp, Register.Ebp));

            // Aligns the stack frame to a 16 byte boundary 
            Add(new And(new IntegerConstant(-16), Register.Esp));

            // Reserve space on the stack for local variables
            var stackFrameSize = node.CompoundStatement.SymbolTable.Count * 4;
            _currFrameOffset = stackFrameSize;
            Add(new Sub(new IntegerConstant(stackFrameSize), Register.Esp));

            // Setup GCC (optional)
            // list.AddLast(new Call("___main"));

            // Write body
            foreach (var statement in node.CompoundStatement.Statements)
            {
                Visit(statement);
            }

            // Epilogue
            Add(new Leave());
            Add(new Ret());
        }

        private void Visit(IStatement node)
        {
            switch (node)
            {
                case IIfStatement n:
                    throw new NotImplementedException();
                case IReturnStatement n:
                    throw new NotImplementedException();
                case CompoundStatement n:
                    throw new NotImplementedException();
                case Declaration n:
                    Visit(n);
                    break;
                case ExpressionStatement n:
                    Visit(n.Expression);
                    break;
            }
        }

        private void Visit(IExpression node)
        {
            switch (node)
            {
                case IPrimaryExpression n:
                    Visit(n);
                    break;
                case AssignmentExpression n:
                    Visit(n);
                    break;
                case BinaryExpression n:
                    Visit(n);
                    break;
                case UnaryExpression n:
                    Visit(n);
                    break;
                case FunctionCall n:
                    Visit(n);
                    break;
            }
        }

        private void Visit(IPrimaryExpression node)
        {
            switch (node)
            {
                case Identifier n:
                    Visit(n);
                    break;
                case Constant n:
                    Visit(n);
                    break;
                case StringLiteral n:
                    // TODO: Add support for string literals
                    throw new NotImplementedException();
            }
        }

        private void Visit(Identifier node)
        {
            var offset = node.Symbol.StackOffset;
            Add(new Mov(new Memory(Register.Esp, offset), ResultRegister));
        }

        private void Visit(Constant node)
        {
            var integer = new IntegerConstant(Convert.ToInt32(node.Text));
            Add(new Mov(integer, ResultRegister));
        }

        private void Visit(AssignmentExpression node)
        {
            if (node.AssignmentKind != AssignmentKind.Assign)
            {
                // TODO: Different kind of assignments
                throw new NotImplementedException();
            }

            // Evaluate the RHS expression first
            // Result will be in ResultRegister
            Visit(node.Expression);

            var offset = node.Identifier.Symbol.StackOffset;
            Add(new Mov(ResultRegister, new Memory(Register.Esp, offset)));
        }

        private void Visit(BinaryExpression node)
        {
            // Evalate the left tree and put result in ResultRegister
            Visit(node.Left);

            // Push the result of the left tree onto the stack
            Add(new Push(ResultRegister));

            // Evalate the right tree and put result in ResultRegister
            Visit(node.Right);

            // Pop the left result into a secondary register
            Add(new Pop(AltRegister));

            switch (node.Operator)
            {
                case BinaryOpKind.Comma:
                    break;
                case BinaryOpKind.Or:
                    break;
                case BinaryOpKind.And:
                    break;
                case BinaryOpKind.BitwiseOr:
                    break;
                case BinaryOpKind.BitwiseXor:
                    break;
                case BinaryOpKind.BitwiseAnd:
                    break;
                case BinaryOpKind.EqualTo:
                    break;
                case BinaryOpKind.NotEqualTo:
                    break;
                case BinaryOpKind.LessThan:
                    break;
                case BinaryOpKind.GreaterThan:
                    break;
                case BinaryOpKind.LessThanOrEqualTo:
                    break;
                case BinaryOpKind.GreaterThanOrEqualTo:
                    break;
                case BinaryOpKind.LShift:
                    break;
                case BinaryOpKind.RShift:
                    break;
                case BinaryOpKind.Addition:
                    Add(new Add(AltRegister, ResultRegister));
                    break;
                case BinaryOpKind.Subtraction:
                    Add(new Sub(AltRegister, ResultRegister));
                    break;
                case BinaryOpKind.Multiplication:
                    Add(new Imul(AltRegister, ResultRegister));
                    break;
                case BinaryOpKind.Division:
                    break;
                case BinaryOpKind.Modulus:
                    break;
            }
        }

        private void Visit(UnaryExpression node)
        {
            // Evalate and put result in ResultRegister
            Visit(node.Expression);

            switch (node.Operator)
            {
                case UnaryOpKind.AddressOf:
                    break;
            }
        }

        private void Visit(FunctionCall node)
        {
            var offset = node.Arguments.Count * 4;

            foreach (var expression in Reverse(node.Arguments))
            {
                offset = offset - 4;

                if (expression is StringLiteral literal)
                {
                    var text = literal.Text;

                    // TODO: Fix this hacky garbage
                    Add(new DirectText(".section .rdata,\"dr\""));
                    Add(new Label("TheMagicString"));
                    Add(new Ascii(text));
                    Add(new TextDirective());
                    Add(new Mov(new TextConstant("TheMagicString"), new Memory(Register.Esp, offset)));
                    continue;
                }

                Visit(expression);
                Add(new Mov(ResultRegister, new Memory(Register.Esp, offset)));
            }

            Add(new Call("_" + node.Identifier.Text));
        }

        private void Visit(Declaration node)
        {
            foreach (var initDeclarators in node.InitDeclarators)
            {
                switch (initDeclarators)
                {
                    case Declarator n:
                        Visit(n);
                        break;
                    case InitializationDeclarator n:
                        Visit(n);
                        break;
                }
            }
        }

        private void Visit(Declarator node)
        {
            if (!(node.DirectDeclarator is Identifier))
            {
                // This should never be a FunctionDeclarator
                throw new NotSupportedException();
            }

            var identifier = (Identifier) node.DirectDeclarator;
            _currFrameOffset = _currFrameOffset - 4;
            identifier.Symbol.StackOffset = _currFrameOffset;
        }

        private void Visit(InitializationDeclarator node)
        {
            Visit(node.Declarator);

            if (!(node.Expression is Constant))
            {
                // TODO: Evaluate expression trees
                throw new NotImplementedException();
            }

            var integer = Convert.ToInt32(((Constant)node.Expression).Text);
            Add(new Mov(new IntegerConstant(integer), new Memory(Register.Esp, _currFrameOffset)));
        }

        private void Add(ILine line)
        {
            _lines.AddLast(line);
        }

        private static IEnumerable<T> Reverse<T>(LinkedList<T> list)
        {
            var element = list.Last;
            while (element != null)
            {
                yield return element.Value;
                element = element.Previous;
            }
        }

        private static readonly Register ResultRegister = Register.Eax;
        private static readonly Register AltRegister = Register.Edx;
        private static Register[] _callerSaved = { Register.Eax, Register.Ecx, Register.Edx };
        private static Register[] _calleeSaved = { Register.Ebx, Register.Edi, Register.Esi };
    }
}
