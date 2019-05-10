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

            // Reserve space on the stack for local variables?
            // Or just do it on the fly whenever a variable is declared?

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
                    Visit(n);
                    break;
            }
        }

        private void Visit(ExpressionStatement node)
        {
            if (!(node.Expression is FunctionCall))
            {
                throw new NotImplementedException();
            }

            var functionCall = node.Expression as FunctionCall;
            var offset = functionCall.Arguments.Count * 4;

            foreach (var expression in Reverse(functionCall.Arguments))
            {
                offset = offset - 4;

                if (expression is Constant)
                {
                    var integer = Convert.ToInt32(((Constant)expression).Text);
                    Add(new Mov(new IntegerConstant(integer), new Memory(Register.Esp, offset)));
                    continue;
                }

                if (expression is StringLiteral)
                {
                    var text = ((StringLiteral) expression).Text;

                    // TODO: Fix this hacky garbage
                    Add(new DirectText(".section .rdata,\"dr\""));
                    Add(new Label("TheMagicString"));
                    Add(new Ascii(text));
                    Add(new TextDirective());
                    Add(new Mov(new TextConstant("TheMagicString"), new Memory(Register.Esp, offset)));
                    continue;
                }

                throw new NotImplementedException();
            }

            Add(new Call("_" + functionCall.Identifier.Text));
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
            var directDeclarator = node.DirectDeclarator;

            if (!(directDeclarator is Identifier))
            {
                throw new NotImplementedException();
            }

            var identifier = directDeclarator as Identifier;

            // TODO: Check type
            // TODO: Calculate and save offset
            Add(new Sub(new IntegerConstant(4), Register.Esp));
        }

        private void Visit(InitializationDeclarator node)
        {
            Visit(node.Declarator);

            if (!(node.Expression is Constant))
            {
                // TODO: Evaluate expression trees
                throw new NotImplementedException();
            }

            var constant = node.Expression as Constant;
            var integer = Convert.ToInt32(constant.Text);

            // TODO: Use offset
            Add(new Mov(new IntegerConstant(integer), new Memory(Register.Esp, 0)));
        }

        private void Add(ILine line)
        {
            _lines.AddLast(line);
        }

        private IEnumerable<T> Reverse<T>(LinkedList<T> list)
        {
            var element = list.Last;
            while (element != null)
            {
                yield return element.Value;
                element = element.Previous;
            }
        }

        private static Register[] _callerSaved = { Register.Eax, Register.Ecx, Register.Edx };
        private static Register[] _calleeSaved = { Register.Ebx, Register.Edi, Register.Esi };
    }
}
