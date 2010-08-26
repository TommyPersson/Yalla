using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yalla.Parser.AstObjects
{
    public class FunctionNode : AstNode
    {
        private static readonly IDictionary<string, PrimitiveFunctionNode> PrimitiveFunctions =
            new Dictionary<string, PrimitiveFunctionNode>
                {
                    { "+", new AddFunctionNode() },
                };

        public static FunctionNode MakeFunctionNode(string name)
        {
            if (PrimitiveFunctions.ContainsKey(name))
            {
                return PrimitiveFunctions[name];
            }

            return null;
        }
    }

    public abstract class PrimitiveFunctionNode : FunctionNode
    {
        public string Symbol { get; protected set; }
    }

    public class AddFunctionNode : PrimitiveFunctionNode
    {
        public AddFunctionNode()
        {
            Symbol = "+";
        }
    }

    public class AndFunctionNode : PrimitiveFunctionNode
    {
        public AndFunctionNode()
        {
            Symbol = "and";
        }
    }

    public class OrFunctionNode : PrimitiveFunctionNode
    {
        public OrFunctionNode()
        {
            Symbol = "or";
        }
    }

    public class NotFunctionNode : PrimitiveFunctionNode
    {
        public NotFunctionNode()
        {
            Symbol = "not";
        }
    }

    public class EqualFunctionNode : PrimitiveFunctionNode
    {
        public EqualFunctionNode()
        {
            Symbol = "=";
        }
    }
}
