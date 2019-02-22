namespace MyCCompiler.Analysis
{
    public class SymbolTableListener : CBaseListener
    {
        private SymbolTable _currSymbolTable;
        private string _currLexme;
        private string _currType;

        public SymbolTableListener()
        {
            _currSymbolTable = new SymbolTable(null);
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
            _currSymbolTable.Put(_currLexme, _currType);
            _currLexme = null;
            _currType = null;
        }

        public override void ExitDirectDeclarator(CParser.DirectDeclaratorContext context)
        {
            if (context.Identifier() != null)
            {
                _currLexme = context.Identifier().Symbol.Text;
            }
        }

        public override void ExitTypeSpecifier(CParser.TypeSpecifierContext context)
        {
            _currType = context.GetText();
        }
    }
}
