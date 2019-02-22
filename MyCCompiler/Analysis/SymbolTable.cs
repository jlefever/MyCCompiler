using System.Collections.Generic;

namespace MyCCompiler.Analysis
{
    public class SymbolTable
    {
        public readonly SymbolTable Previous;
        private readonly IDictionary<string, Symbol> _table;

        public SymbolTable(SymbolTable previous)
        {
            Previous = previous;
            _table = new Dictionary<string, Symbol>();
        }

        public void Put(Symbol symbol)
        {
            _table.Add(symbol.Lexme, symbol);
        }

        public Symbol Get(string lexme)
        {
            for (var st = this; st != null; st = st.Previous)
            {
                if (st._table.ContainsKey(lexme))
                {
                    return st._table[lexme];
                }
            }

            return null;
        }
    }
}
