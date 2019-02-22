using System.Collections.Generic;

namespace MyCCompiler.Analysis
{
    public class SymbolTable
    {
        public readonly SymbolTable Previous;
        private readonly IDictionary<string, string> _table;

        public SymbolTable(SymbolTable previous)
        {
            Previous = previous;
            _table = new Dictionary<string, string>();
        }

        public void Put(string lexme, string type)
        {
            _table.Add(lexme, type);
        }

        public string Get(string lexme)
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
