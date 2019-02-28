using System;

namespace MyCCompiler.Analysis
{
    public class SymbolTableListener : CBaseListener
    {
        private SymbolTable _currSymbolTable;
        private Symbol _currSymbol;

        public SymbolTableListener()
        {
            _currSymbolTable = new SymbolTable(null);
            _currSymbol = new Symbol();
        }

        public override void EnterCompoundStatement(CParser.CompoundStatementContext context)
        {
            _currSymbolTable = new SymbolTable(_currSymbolTable);
        }

        public override void ExitCompoundStatement(CParser.CompoundStatementContext context)
        {
            _currSymbolTable = _currSymbolTable.Previous;
        }

        public override void ExitDeclaration(CParser.DeclarationContext context)
        {
            _currSymbolTable.Put(_currSymbol);
            _currSymbol = new Symbol();
        }

        public override void ExitDirectDeclarator(CParser.DirectDeclaratorContext context)
        {
            if (context.Identifier() != null)
            {
                _currSymbol.Lexme = context.Identifier().Symbol.Text;
            }
        }

        // TODO: This should actually traverse the tree. It was written this was to test the Pointer type.
        public override void ExitDeclarationSpecifiers(CParser.DeclarationSpecifiersContext context)
        {
            var text = context.GetText();
            var index = text.IndexOf("*", StringComparison.Ordinal);

            if (index == -1)
            {
                _currSymbol.Type = new Variable(EnumUtil.PrimitiveKindMap[text]);
                return;
            }

            var primitive = text.Substring(0, index);
            var pointerCount = text.Length - index;
            IType prev = new Variable(EnumUtil.PrimitiveKindMap[primitive]);

            for (var i = 0; i < pointerCount; i++)
            {
                prev = new Pointer(prev);
            }

            _currSymbol.Type = prev;
        }
    }
}
