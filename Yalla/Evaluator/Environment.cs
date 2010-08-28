using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Environment : Dictionary<SymbolNode, AstNode>
    {
        public Environment()
        {
        }

        public Environment(Environment env) : base(env)
        {
        }

        public Environment Copy()
        {
            return new Environment(this);
        }
    }
}
