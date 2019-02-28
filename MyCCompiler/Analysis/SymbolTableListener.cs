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

        public override void ExitDeclarationSpecifiers(CParser.DeclarationSpecifiersContext context)
        {
            _currSymbol.Type = new Variable(EnumUtil.PrimitiveKindMap[context.GetText()]);
        }
    }
}
