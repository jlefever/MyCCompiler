using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Write preamble
            // Save the old base pointer value
            list.AddLast(new Push(new Register(RegisterKind.Ebp)));
            list.AddLast(new Mov(new Register(RegisterKind.Esp), new Register(RegisterKind.Ebp)));
            list.AddLast(new Call("___main"));

            // Write body

            // Write epilogue
            list.AddLast(new Leave());
            list.AddLast(new Ret());
        }

        private static RegisterKind[] _callerSaved = { RegisterKind.Eax, RegisterKind.Ecx, RegisterKind.Edx };
        private static RegisterKind[] _calleeSaved = { RegisterKind.Ebx, RegisterKind.Edi, RegisterKind.Esi };


    }
}
