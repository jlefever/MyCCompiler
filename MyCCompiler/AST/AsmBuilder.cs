using System;
using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class AsmBuilder
    {
        private readonly LinkedList<ILine> _lines;
        private int _currFrameOffset;
        private int _asciiCount;

        public AsmBuilder()
        {
            _lines = new LinkedList<ILine>();
            _currFrameOffset = 0;
            _asciiCount = 0;
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
            // First, we want to assign offsets to our parameters so we can find them
            // when evaluating expresions. We start at 8 so as to skip over the saved
            // EBP on the stack and the return address placed by the call instruction.
            var offset = 8;
            var parameters = node.FunctionDeclarator.ParameterList.Parameters;

            foreach (var parameter in parameters)
            {
                if (!(parameter.Declarator.DirectDeclarator is Identifier))
                {
                    throw new NotSupportedException();
                }

                var identifier = (Identifier)parameter.Declarator.DirectDeclarator;
                identifier.Symbol.StackOffset = offset;
                offset = offset + 4;
            }

            // Create globl directive and label for function
            var funcName = "_" + node.FunctionDeclarator.Identifier.Text;
            Add(new GloblDirective(funcName));
            Add(new Label(funcName));

            // Save the old base pointer value
            Add(new Push(Register.Ebp));

            // Create stack frame
            Add(new Mov(Register.Esp, Register.Ebp));

            // Aligns the stack frame to a 16 byte boundary
            Add(new And(new IntegerConstant(-16), Register.Esp));

            // Reserve space on the stack for local variables
            var stackFrameSize = node.CompoundStatement.SymbolTable.Count * 4;
            Add(new Sub(new IntegerConstant(stackFrameSize), Register.Esp));

            // Setup GCC (optional)
            // list.AddLast(new Call("___main"));

            // Write body
            foreach (var statement in node.CompoundStatement.Statements)
            {
                Visit(statement);
            }

            // Leave will move the value in EBP into ESP, deallocating any local variables
            // Leave also pops the old base pointer value off the stack
            Add(new Leave());

            // Remove return address from stack (placed by the call instruction)
            Add(new Ret());
        }

        private void Visit(IStatement node)
        {
            switch (node)
            {
                case IIfStatement n:
                    throw new NotImplementedException();
                case IReturnStatement n:
                    Visit(n);
                    break;
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

        private void Visit(IReturnStatement node)
        {
            if (node is ReturnVoidStatement)
            {
                return;
            }

            Visit(((ReturnStatement)node).Expression);
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
                    Visit(n);
                    break;
            }
        }

        private void Visit(Identifier node)
        {
            var offset = node.Symbol.StackOffset;
            Add(new Mov(new Memory(Register.Ebp, offset), ResultRegister));
        }

        private void Visit(Constant node)
        {
            var integer = new IntegerConstant(Convert.ToInt32(node.Text));
            Add(new Mov(integer, ResultRegister));
        }

        private void Visit(StringLiteral node)
        {
            var label = $"SL{_asciiCount}";
            _asciiCount++;

            Add(new DirectText(".section .rdata"));
            Add(new Label(label));
            Add(new Ascii(node.Text));
            Add(new TextDirective());
            Add(new Mov(new TextConstant(label), ResultRegister));
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
            Add(new Mov(ResultRegister, new Memory(Register.Ebp, offset)));
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
            // We treat a function call like any other expression. The only obligation any
            // expression has is to place its result in ResultRegister. An expression can
            // arbitrarly change any other register (other than ESP or EBP). Because of this,
            // there is no use in saving the convential "caller-saved" registers on the stack.

            // If we  were to adapt a smarter process for evaluating expressions, we might
            // have to save the "caller-saved" registers.

            // Push arguments onto stack in reverse order
            foreach (var expression in Reverse(node.Arguments))
            {
                Visit(expression);
                Add(new Push(ResultRegister));
            }

            // Call function
            Add(new Call("_" + node.Identifier.Text));

            // Remove arguments from stack
            Add(new Add(new IntegerConstant(node.Arguments.Count * 4), Register.Esp));

            // Here is where we would restore contents of caller-saved registers
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
                // This should never be anything but an identifier
                throw new NotSupportedException();
            }

            var identifier = (Identifier)node.DirectDeclarator;
            _currFrameOffset = _currFrameOffset - 4;
            identifier.Symbol.StackOffset = _currFrameOffset;
        }

        // This shares code with AssignmentExpression
        private void Visit(InitializationDeclarator node)
        {
            // Calculate and store the offset
            Visit(node.Declarator);

            // Eval expression tree
            Visit(node.Expression);

            // This should never be anything but an identifier (for now)
            // What about arrays or pointers?
            if (!(node.Declarator.DirectDeclarator is Identifier))
            {
                throw new NotSupportedException();
            }

            // Special case??
            if (node.Expression is Constant)
            {
                // var integer = Convert.ToInt32(((Constant)node.Expression).Text);
            }

            var offset = ((Identifier)node.Declarator.DirectDeclarator).Symbol.StackOffset;
            Add(new Mov(ResultRegister, new Memory(Register.Ebp, offset)));
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
        private static readonly Register[] CallerSaved = { Register.Eax, Register.Ecx, Register.Edx };
        private static readonly Register[] CalleeSaved = { Register.Ebx, Register.Edi, Register.Esi };
    }
}
