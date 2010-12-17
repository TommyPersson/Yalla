
using System.Collections;
using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Environment : IEnumerable<KeyValuePair<SymbolNode, object>>
    {
        private readonly Dictionary<SymbolNode, object> table = new Dictionary<SymbolNode, object>();

        public Environment()
        {
            table = new Dictionary<SymbolNode, object>();
        }

        public Environment(Dictionary<SymbolNode, object> table)
        {
            this.table = table;
        }

        public Environment(Environment env)
        {
            Parent = env.Parent;
            table = new Dictionary<SymbolNode, object>(env.table);
        }
        
        public Environment Parent { get; set; }

        public bool DefineSymbol(SymbolNode symbol, object value)
        {
            bool shadowing = CanLookUpSymbol(symbol);

            table[symbol] = value;

            return shadowing;
        }

        public bool SetSymbolValue(SymbolNode symbol, object value)
        {
            if (table.ContainsKey(symbol))
            {
                table[symbol] = value;
                return true;
            }

            if (Parent != null)
            {
                return Parent.SetSymbolValue(symbol, value);
            }

            return false;
        }

        public object LookUpSymbol(SymbolNode symbolNode)
        {
            if (table.ContainsKey(symbolNode))
            {
                return table[symbolNode];
            }
            
            if (Parent != null)
            {
                return Parent.LookUpSymbol(symbolNode);
            }

            return null;
        }

        public bool CanLookUpSymbol(SymbolNode symbolNode)
        {
            if (table.ContainsKey(symbolNode))
            {
                return true;
            }

            if (Parent != null)
            {
                return Parent.CanLookUpSymbol(symbolNode);
            }

            return false;
        }

        public Environment CreateChildEnvironment()
        {
            var child = new Environment { Parent = this };

            return child;
        }

        public IEnumerator<KeyValuePair<SymbolNode, object>> GetEnumerator()
        {
            return table.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
