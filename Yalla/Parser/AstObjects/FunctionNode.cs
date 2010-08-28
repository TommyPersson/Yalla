
using System.Collections.Generic;
using Yalla.Evaluator;

namespace Yalla.Parser.AstObjects
{
    public abstract class FunctionNode : AstNode
    {
        public static readonly IDictionary<string, FunctionNode> PrimitiveFunctions =
            new Dictionary<string, FunctionNode>
                {
                    { "+", new AddFunctionNode() },
                    { "=", new EqualFunctionNode() },
                    { "and", new AndFunctionNode() },
                    { "or", new OrFunctionNode() },
                    { "lambda", new LambdaFunctionNode() },
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

    public class NativeMethodFunctionNode : FunctionNode
    {
        public NativeMethodFunctionNode(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
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

    public class LambdaFunctionNode : FunctionNode
    {
        public LambdaFunctionNode()
        {
            Symbol = "lambda";
        }
    }

    public class ProcedureNode : FunctionNode
    {
        public ProcedureNode(IEnumerable<SymbolNode> parameters, IEnumerable<AstNode> body, Environment environment)
        {
            Parameters = parameters;
            Body = body;
            Environment = environment;
        }

        public IEnumerable<SymbolNode> Parameters { get; private set; }

        public IEnumerable<AstNode> Body { get; private set; }

        public Environment Environment { get; private set; }
    }
}
