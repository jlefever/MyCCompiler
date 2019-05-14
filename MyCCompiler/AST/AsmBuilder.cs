using System;
using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class AsmBuilder
    {
        private readonly LinkedList<ILine> _lines;
        private int _currFrameOffset;
        private int _stringLiteralCount;
        private int _labelCount;

        public AsmBuilder()
        {
            _lines = new LinkedList<ILine>();
            _currFrameOffset = 0;
            _stringLiteralCount = 0;
            _labelCount = 0;
        }

        public LinkedList<ILine> Visit(CompilationUnit node)
        {
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
                    // Not supporting global variables or function signitures at the moment.
                    // This would not be unreasonable to add later.
                    throw new NotSupportedException();
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
            if (node.IsMain)
            {
                Add(new And(new IntegerConstant(-16), Register.Esp));
            }

            // Reserve space on the stack for local variables
            var stackFrameSize = node.CompoundStatement.SymbolTable.Count() * 4;
            Add(new Sub(new IntegerConstant(stackFrameSize), Register.Esp));

            // Reset frame offset
            _currFrameOffset = 0;

            // Setup GCC (optional)
            if (node.IsMain)
            {
                Add(new Call(new LabeledCode("___main")));
            }

            // Visit body
            Visit(node.CompoundStatement);

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
                    Visit(n);
                    break;
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

        private void Visit(CompoundStatement node)
        {
            foreach (var statement in node.Statements)
            {
                Visit(statement);
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
            var label = $"SL{_stringLiteralCount++}";

            Add(new RDataDirective());
            Add(new Label(label));
            Add(new AsciiDirective(node.Text));
            Add(new TextDirective());
            Add(new Mov(new LabeledData(label), ResultRegister));
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
            // Some binary expressions require special handling
            switch (node.Operator)
            {
                case BinaryOpKind.Division:
                case BinaryOpKind.Modulus:
                    VisitDivisionOrModulus(node);
                    return;
                case BinaryOpKind.LShift:
                case BinaryOpKind.RShift:
                    VisitLShiftOrRShift(node);
                    return;
                case BinaryOpKind.Or:
                case BinaryOpKind.And:
                    VisitLogicalAndOr(node);
                    return;
            }

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
                    // Do nothing. ResultRegister already contains the result.
                    return;
                case BinaryOpKind.BitwiseOr:
                    Add(new Or(AltRegister, ResultRegister));
                    return;
                case BinaryOpKind.BitwiseXor:
                    Add(new Xor(AltRegister, ResultRegister));
                    return;
                case BinaryOpKind.BitwiseAnd:
                    Add(new And(AltRegister, ResultRegister));
                    return;
                case BinaryOpKind.Addition:
                    Add(new Add(AltRegister, ResultRegister));
                    return;
                case BinaryOpKind.Subtraction:
                    Add(new Sub(AltRegister, ResultRegister));
                    return;
                case BinaryOpKind.Multiplication:
                    Add(new Imul(AltRegister, ResultRegister));
                    return;
            }

            // Only binary comparators are remaining
            // Compare left tree with right tree
            Add(new Cmp(AltRegister, ResultRegister));

            // Zero out EAX, so AL can be used safely
            Add(new Mov(new IntegerConstant(0), ResultRegister));

            switch (node.Operator)
            {
                case BinaryOpKind.EqualTo:
                    Add(new Sete(LowerResultRegister));
                    return;
                case BinaryOpKind.NotEqualTo:
                    Add(new Sete(LowerResultRegister));
                    return;
                case BinaryOpKind.LessThan:
                    Add(new Setl(LowerResultRegister));
                    return;
                case BinaryOpKind.GreaterThan:
                    Add(new Setg(LowerResultRegister));
                    return;
                case BinaryOpKind.LessThanOrEqualTo:
                    Add(new Setle(LowerResultRegister));
                    return;
                case BinaryOpKind.GreaterThanOrEqualTo:
                    Add(new Setge(LowerResultRegister));
                    return;
            }
        }

        private void VisitLShiftOrRShift(BinaryExpression node)
        {
            // Evalate the left tree and put result in ResultRegister
            Visit(node.Left);

            // Push the result of the left tree onto the stack
            Add(new Push(ResultRegister));

            // Evalate the right tree and put result in ResultRegister
            Visit(node.Right);

            // Move right result, ResultRegister, to AltRegister
            Add(new Mov(ResultRegister, AltRegister));

            // Pop the left result into ResultRegister
            Add(new Pop(ResultRegister));

            switch (node.Operator)
            {
                case BinaryOpKind.LShift:
                    Add(new Shl(LowerAltRegister, ResultRegister));
                    break;
                case BinaryOpKind.RShift:
                    Add(new Shr(LowerAltRegister, ResultRegister));
                    break;
            }
        }

        private void VisitDivisionOrModulus(BinaryExpression node)
        {
            // Evalate the left tree and put result in ResultRegister
            Visit(node.Left);

            // Push the result of the left tree onto the stack
            Add(new Push(ResultRegister));

            // Evalate the right tree and put result in ResultRegister
            Visit(node.Right);

            // Move right result, ResultRegister, to AltRegister
            Add(new Mov(ResultRegister, AltRegister));

            // Pop the left result into ResultRegister
            Add(new Pop(ResultRegister));

            // Zero out EDX
            Add(new Mov(new IntegerConstant(0), Register.Edx));

            // Do EDX:EAX / ECX and put the quotient in EAX and remainder in EDX
            Add(new Idiv(Register.Ecx));

            // If the operator is modulus, move the remainder into the result register
            if (node.Operator == BinaryOpKind.Modulus)
            {
                Add(new Mov(Register.Edx, ResultRegister));
            }
        }

        private void VisitLogicalAndOr(BinaryExpression node)
        {
            var rightLabel = $"L{_labelCount++}";
            var finalLabel = $"L{_labelCount++}";

            // Evalate the left tree and put result in ResultRegister
            Visit(node.Left);

            // Check if the left tree is true
            Add(new Cmp(new IntegerConstant(0), ResultRegister));

            if (node.Operator == BinaryOpKind.Or)
            {
                // The left tree was 0, so we have to evaluate the right tree
                Add(new Je(new LabeledCode(rightLabel)));

                // We didn't jump, so the left tree is true and we can return true
                Add(new Mov(new IntegerConstant(1), ResultRegister));
                Add(new Jmp(new LabeledCode(finalLabel)));
            }
            else
            {
                // The left tree was 0, so we have to evaluate the right tree
                Add(new Jne(new LabeledCode(rightLabel)));

                // We didn't jump, so the left tree is false and we can return false
                Add(new Jmp(new LabeledCode(finalLabel)));
            }

            // Generate code for evaluating the right tree
            Add(new Label(rightLabel));

            // Evalate the right tree and put result in ResultRegister
            Visit(node.Right);

            // Check if the right tree is true
            Add(new Cmp(new IntegerConstant(0), ResultRegister));

            // Zero out EAX
            Add(new Mov(new IntegerConstant(0), ResultRegister));

            // Set the low byte of EAX to 1 if the right tree is not 0
            Add(new Setne(LowerResultRegister));

            // Add the final label
            Add(new Label(finalLabel));
        }

        private void Visit(UnaryExpression node)
        {
            // Evalate and put result in ResultRegister
            Visit(node.Expression);

            switch (node.Operator)
            {
                case UnaryOpKind.AddressOf:
                    break;
                case UnaryOpKind.Dereference:
                    break;
                case UnaryOpKind.Plus:
                    // No need for integer promotion as everything is already integers
                    break;
                case UnaryOpKind.Minus:
                    Add(new Neg(ResultRegister));
                    break;
                case UnaryOpKind.BitwiseNot:
                    Add(new Not(ResultRegister));
                    break;
                case UnaryOpKind.Not:
                    Add(new Cmp(new IntegerConstant(0), ResultRegister));
                    Add(new Mov(new IntegerConstant(0), ResultRegister));
                    Add(new Sete(LowerResultRegister));
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
            Add(new Call(new LabeledCode("_" + node.Identifier.Text)));

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
        private static readonly Register LowerResultRegister = Register.Al;
        private static readonly Register AltRegister = Register.Ecx;
        private static readonly Register LowerAltRegister = Register.Cl;
        private static readonly Register[] CallerSaved = { Register.Eax, Register.Ecx, Register.Edx };
        private static readonly Register[] CalleeSaved = { Register.Ebx, Register.Edi, Register.Esi };
    }
}
