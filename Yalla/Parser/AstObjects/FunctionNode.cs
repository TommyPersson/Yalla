
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
                    { "cons", new ConsFunctionNode() },
                    { "lambda", new LambdaFunctionNode() },
                    { "def", new DefineFunctionNode() },
                    { "defmacro", new DefmacroFunctionNode() },
                    { "set!", new SetFunctionNode() },
                    { "if", new IfFunctionNode() },
                    { "let", new LetFunctionNode() },
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

    public class ConsFunctionNode : FunctionNode
    {
        public ConsFunctionNode()
        {
            Symbol = "cons";
        }
    }

    public class DefineFunctionNode : FunctionNode
    {
        public DefineFunctionNode()
        {
            Symbol = "def";
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
        public ProcedureNode(IEnumerable<SymbolNode> parameters, IEnumerable<object> body, Environment environment, bool isMacro = false)
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
    
    public class DefmacroFunctionNode : FunctionNode
    {
        public DefmacroFunctionNode()
        {
            Symbol = "defmacro";
        }
    }

    public class SetFunctionNode : FunctionNode
    {
        public SetFunctionNode()
        {
            Symbol = "set!";
        }
    }
    
    public class IfFunctionNode : FunctionNode
    {
        public IfFunctionNode()
        {
            Symbol = "if";
        }
    }

    public class LetFunctionNode : FunctionNode
    {
        public LetFunctionNode()
        {
            Symbol = "let";
        }
    }
}
