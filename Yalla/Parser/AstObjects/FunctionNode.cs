using System.Collections.Generic;
using Yalla.Evaluator;
using Environment = Yalla.Evaluator.Environment;

namespace Yalla.Parser.AstObjects
{
    public abstract class FunctionNode : AstNode
    {
        public static readonly IDictionary<string, FunctionNode> PrimitiveFunctions =
            new Dictionary<string, FunctionNode>
                {
                    { "+", new PrimitiveFunction("+") },
                    { "=", new PrimitiveFunction("=") },
                    { "<", new PrimitiveFunction("<") },
                    { "and", new PrimitiveFunction("and") },
                    { "or", new PrimitiveFunction("or") },
                    { "cons", new PrimitiveFunction("cons") },
                    { "lambda", new PrimitiveFunction("lambda") },
                    { "def", new PrimitiveFunction("def") },
                    { "defmacro", new PrimitiveFunction("defmacro") },
                    { "set!", new PrimitiveFunction("set!") },
                    { "if", new PrimitiveFunction("if") },
                    { "let", new PrimitiveFunction("let") },
                    { "map", new PrimitiveFunction("map") },
                    { "make-func", new PrimitiveFunction("make-func") },
                };
        

        public FunctionNode()
        {
        }
        
        public FunctionNode(string symbol)
        {
            Symbol = symbol;
        }
        
        public string Symbol { get; protected set; }
    }

    public class PrimitiveFunction : FunctionNode
    {
        public PrimitiveFunction(string symbol) : base(symbol)
        {
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

    public class NativeConstructorFunctionNode : FunctionNode
    {
        public NativeConstructorFunctionNode(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; private set; }
    }

    public class NativeStaticMethodFunctionNode : FunctionNode
    {
        public NativeStaticMethodFunctionNode(string typeName, string methodName)
        {
            TypeName = typeName;
            MethodName = methodName;
        }

        public string TypeName { get; private set; }

        public string MethodName { get; private set; }
    }


    public class ProcedureNode : FunctionNode
    {
        public ProcedureNode(
            IEnumerable<SymbolNode> parameters, 
            IEnumerable<object> body, 
            Environment environment, 
            bool isMacro = false)
        {
            Parameters = parameters;
            Body = body;
            Environment = environment;
            IsMacro = isMacro;
        }

        public IEnumerable<SymbolNode> Parameters { get; private set; }

        public IEnumerable<object> Body { get; private set; }

        public Environment Environment { get; private set; }

        public bool IsMacro { get; private set; }
    }
}
