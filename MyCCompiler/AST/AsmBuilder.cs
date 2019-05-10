using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class AsmBuilder
    {
        public void Visit(CompilationUnit node)
        {
            foreach (var external in node.Externals)
            {
                Visit(external);
            }
        }

        public void Visit(IExternal node)
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

        public void Visit(FunctionDefinition node)
        {
            var list = new LinkedList<ILine>();

            // Create globl directive and label for function
            var funcName = "_" + node.FunctionDeclarator.Identifier.Text;
            list.AddLast(new Globl(funcName));
            list.AddLast(new Label(funcName));

            // Preamble
            // Save the old base pointer value
            list.AddLast(new Push(Register.Ebp));

            // Create stack frame
            list.AddLast(new Mov(Register.Esp, Register.Ebp));

            // Aligns the stack frame to a 16 byte boundary 
            list.AddLast(new And(new IntegerConstant(-16), Register.Esp));

            // Reserve space on the stack for local variables?
            // Or just do it on the fly whenever a variable is declared?

            // Setup GCC (might be optional)
            list.AddLast(new Call("___main"));

            // Write body

            // Epilogue
            list.AddLast(new Leave());
            list.AddLast(new Ret());
        }

        private static Register[] _callerSaved = { Register.Eax, Register.Ecx, Register.Edx };
        private static Register[] _calleeSaved = { Register.Ebx, Register.Edi, Register.Esi };
    }
}
