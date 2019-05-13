using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public class SymbolTable
    {
        public SymbolTable Parent { get; }
        private readonly LinkedList<SymbolTable> _children;
        private readonly IDictionary<string, Symbol> _table;

        /// <summary>
        /// Create a root SymbolTable
        /// </summary>
        public SymbolTable()
        {
            _children = new LinkedList<SymbolTable>();
            _table = new Dictionary<string, Symbol>();
        }

        /// <summary>
        /// Create a child SymbolTable
        /// </summary>
        /// <param name="parent">The parent SymbolTable</param>
        public SymbolTable(SymbolTable parent)
        {
            _children = new LinkedList<SymbolTable>();
            _table = new Dictionary<string, Symbol>();
            Parent = parent;
            Parent._children.AddLast(this);
        }

        /// <summary>
        /// Adds a symbol to the SymbolTable
        /// </summary>
        /// <param name="name">The name of the symbol</param>
        /// <param name="type">Tye type of the symbol</param>
        public void Put(string name, IType type)
        {
            _table.Add(name, new Symbol(name, type));
        }

        /// <summary>
        /// Looks through this SymbolTable and all parents for a
        /// Symbol with the given name
        /// </summary>
        /// <param name="name">The name of a symbol</param>
        /// <returns>Symbol with the given name</returns>
        public Symbol Get(string name)
        {
            for (var st = this; st != null; st = st.Parent)
            {
                if (st._table.ContainsKey(name))
                {
                    return st._table[name];
                }
            }

            return null;
        }

        /// <summary>
        /// Counts the number of symbols in this subtree of the SymbolTable
        /// </summary>
        /// <returns>The total symbols in this subtree</returns>
        public int Count()
        {
            var count = _table.Count;

            foreach (var child in _children)
            {
                count = count + child.Count();
            }

            return count;
        }
    }
}
