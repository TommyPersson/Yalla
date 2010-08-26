using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yalla.Parser.AstObjects
{
    public abstract class FunctionNode : AstNode
    {
        public static readonly IDictionary<string, FunctionNode> PrimitiveFunctions =
            new Dictionary<string, FunctionNode>
                {
                    { "+", new AddFunctionNode() },
                };

        public string Symbol { get; protected set; }

        public static FunctionNode MakeFunctionNode(string name)
        {
            if (PrimitiveFunctions.ContainsKey(name))
            {
                return PrimitiveFunctions[name];
            }

            return null;
        }
    }

    public class AddFunctionNode : FunctionNode
    {
        public AddFunctionNode()
        {
            Symbol = "+";
        }
    }

    public class AndFunctionNode : FunctionNode
    {
        public AndFunctionNode()
        {
            Symbol = "and";
        }
    }

    public class OrFunctionNode : FunctionNode
    {
        public OrFunctionNode()
        {
            Symbol = "or";
        }
    }

    public class NotFunctionNode : FunctionNode
    {
        public NotFunctionNode()
        {
            Symbol = "not";
        }
    }

    public class EqualFunctionNode : FunctionNode
    {
        public EqualFunctionNode()
        {
            Symbol = "=";
        }
    }
}
