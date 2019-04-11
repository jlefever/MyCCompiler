using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class SymbolTable
    {
        public readonly SymbolTable Previous;
        private readonly IDictionary<string, IType> _table;

        public SymbolTable(SymbolTable previous)
        {
            Previous = previous;
            _table = new Dictionary<string, IType>();
        }

        public void Put(string text, IType type)
        {
            _table.Add(text, type);
        }

        public IType Get(string text)
        {
            for (var st = this; st != null; st = st.Previous)
            {
                if (st._table.ContainsKey(text))
                {
                    return st._table[text];
                }
            }

            return null;
        }
    }
}
