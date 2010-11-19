
using System;
using System.Collections.Generic;
using System.Linq;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Applier
    {
        private readonly IDictionary<Type, Func<Applier, FunctionNode, ListNode, Environment, AstNode>> applyFunctions =
            new Dictionary<Type, Func<Applier, FunctionNode, ListNode, Environment, AstNode>>
                {
                    { typeof(AddFunctionNode), (x, y, z, v) => x.Apply((AddFunctionNode)y, z, v) },
                    { typeof(AndFunctionNode), (x, y, z, v) => x.Apply((AndFunctionNode)y, z, v) },
                    { typeof(OrFunctionNode), (x, y, z, v) => x.Apply((OrFunctionNode)y, z, v) },
                    { typeof(EqualFunctionNode), (x, y, z, v) => x.Apply((EqualFunctionNode)y, z, v) },
                    { typeof(ListFunctionNode), (x, y, z, v) => x.Apply((ListFunctionNode)y, z, v) },
                    { typeof(ConsFunctionNode), (x, y, z, v) => x.Apply((ConsFunctionNode)y, z, v) },
                    { typeof(NativeMethodFunctionNode), (x, y, z, v) => x.Apply((NativeMethodFunctionNode)y, z, v) },
                    { typeof(LambdaFunctionNode), (x, y, z, v) => x.Apply((LambdaFunctionNode)y, z, v) },
                    { typeof(ProcedureNode), (x, y, z, v) => x.Apply((ProcedureNode)y, z, v) },
                    { typeof(DefineFunctionNode), (x, y, z, v) => x.Apply((DefineFunctionNode)y, z, v) },
                    { typeof(DefmacroFunctionNode), (x, y, z, v) => x.Apply((DefmacroFunctionNode)y, z, v) },
                    { typeof(SetFunctionNode), (x, y, z, v) => x.Apply((SetFunctionNode)y, z, v) },
                };

        private readonly Evaluator evaluator;

        public Applier(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public AstNode Apply(FunctionNode function, ListNode arguments, Environment environment)
        {
            return applyFunctions[function.GetType()].Invoke(this, function, arguments, environment);
        }

        public AstNode Apply(AddFunctionNode function, ListNode arguments, Environment environment)
        {
            var args = arguments.Children().Select(x => evaluator.Evaluate(x, environment));

            if (!args.All(x => x.GetType() == typeof(IntegerNode) || x.GetType() == typeof(DecimalNode)))
            {
                throw new ArgumentException("'+' may only take integer or double values!");
            }

            decimal result = 0;

            foreach (var arg in args)
            {
                if (arg is IntegerNode)
                {
                    result += ((IntegerNode)arg).Value;
                }

                if (arg is DecimalNode)
                {
                    result += ((DecimalNode) arg).Value;
                }
            }

            return ((result % 1) == 0)
                       ? (AstNode)new IntegerNode(Convert.ToInt32(result))
                       : new DecimalNode(result);
        }

        public AstNode Apply(AndFunctionNode function, ListNode arguments, Environment environment)
        {
            foreach (var argument in arguments.Children())
            {
                var result = evaluator.Evaluate(argument, environment) as BooleanNode;

                if (result == null)
                {
                    throw new ArgumentException("Non-boolean value: " + result);
                }

                if (!result.Value)
                {
                    return AstNode.MakeNode(false);
                }
            }

            return AstNode.MakeNode(true);
        }

        public AstNode Apply(OrFunctionNode function, ListNode arguments, Environment environment)
        {
            foreach (var argument in arguments.Children())
            {
                var result = evaluator.Evaluate(argument, environment) as BooleanNode;

                if (result == null)
                {
                    throw new ArgumentException("Non-boolean value: " + result);
                }

                if (result.Value)
                {
                    return AstNode.MakeNode(true);
                }
            }

            if (arguments.Children().Count != 0)
            {
                return AstNode.MakeNode(false);
            }

            return AstNode.MakeNode(true);
        }

        public AstNode Apply(EqualFunctionNode function, ListNode arguments, Environment environment)
        {
            var args = arguments.Children().Select(x => evaluator.Evaluate(x, environment));

            var firstValue = args.First();
            var remainingValues = args.Skip(1);

            bool result = false;

            foreach (var value in remainingValues)
            {
                result = firstValue.Equals(value);
            }

            return AstNode.MakeNode(result);
        }

        public AstNode Apply(ListFunctionNode function, ListNode arguments, Environment environment)
        {
            var args = arguments.Children().Select(x => evaluator.Evaluate(x, environment)).ToList();

            return AstNode.MakeNode(args);
        }

        public AstNode Apply(ConsFunctionNode function, ListNode arguments, Environment environment)
        {
            var args = arguments.Children().Select(x => evaluator.Evaluate(x, environment)).ToList();

            if (args.Count != 2)
            {
                throw new ArgumentException("Cons expects 2 arguments, got " + args.Count);
            }

            var list = args.ElementAt(1) as ListNode;
            if (list == null)
            {
                throw new ArgumentException("Second argument to cons must be a list.");
            }

            return ((ListNode)AstNode.MakeNode(new List<AstNode> { args.ElementAt(0) })).Append(list);
        }

        public AstNode Apply(NativeMethodFunctionNode method, ListNode arguments, Environment environment)
        {
            if (arguments.Children().Count == 0)
            {
                throw new ArgumentException("No argument passed to native method call!");
            }

            var obj = evaluator.Evaluate(arguments.Children().First(), environment) as ObjectNode;
            if (obj == null)
            {
                throw new ArgumentException("Argument to native method call not an object!");
            }

            var argNodes = arguments.Children().Skip(1).Select(x => evaluator.Evaluate(x, environment) as ObjectNode);
            if (argNodes.Any(x => x == null))
            {
                throw new ArgumentException("Arguments to " + method.Name + " must be objects.");
            }

            var args = argNodes.Select(x => x.Object);
            var argTypes = args.Select(x => x.GetType());
            var otype = obj.Object.GetType();

            var omethod = otype.GetMethod(method.Name, argTypes.ToArray());
            if (omethod != null)
            {
                return AstNode.MakeNode(omethod.Invoke(obj.Object, args.ToArray()));
            }

            var oproperty = otype.GetProperty(method.Name);
            if (oproperty != null)
            {
                return AstNode.MakeNode(oproperty.GetValue(obj.Object, null));
            }

            var ofield = otype.GetField(method.Name);
            if (ofield != null)
            {
                return AstNode.MakeNode(ofield.GetValue(obj));
            }

            throw new ArgumentException(method.Name + " is not a valid method, property or field on " + otype);
        }

        public AstNode Apply(LambdaFunctionNode function, ListNode arguments, Environment environment)
        {
            var parameterList = arguments.First() as ListNode;
            var body = arguments.Rest();

            if (parameterList == null)
            {
                throw new ArgumentException("Missing parameter list in lambda!");
            }
            
            if (!parameterList.Children().All(x => x is SymbolNode))
            {
                throw new ArgumentException("Parameters must be symbols!");
            }

            return new ProcedureNode(parameterList.Children().Cast<SymbolNode>(), body.Children(), environment.CreateChildEnvironment());
        }

        public AstNode Apply(ProcedureNode procedure, ListNode arguments, Environment environment)
        {
            if (arguments.Children().Count != procedure.Parameters.Count() && !procedure.Parameters.Select(x => x.Name).Contains("&"))
            {
                throw new ArgumentException("Wrong number of arguments given to procedure! Expected: " + procedure.Parameters.Count());
            }

            var localEnv = procedure.Environment.CreateChildEnvironment();
            
            for (int i = 0; i < procedure.Parameters.Count(); ++i)
            {
                AstNode evaluatedArgument;
                bool finished = false;
                var parameterSymbol = procedure.Parameters.ElementAt(i);

                if (parameterSymbol.Name == "&")
                {
                    parameterSymbol = procedure.Parameters.ElementAt(i + 1);

                    evaluatedArgument =
                        procedure.IsMacro ? new ListNode(arguments.Children().Skip(i).ToList())
                        : new ListNode(arguments.Children().Skip(i).Select(arg => evaluator.Evaluate(arg, environment)).ToList());

                    localEnv.DefineSymbol(parameterSymbol, evaluatedArgument);

                    finished = true;
                }
                else
                {
                    evaluatedArgument =
                        procedure.IsMacro
                            ? arguments.Children().ElementAt(i)
                            : evaluator.Evaluate(arguments.Children().ElementAt(i), environment);
                }

                localEnv.DefineSymbol(parameterSymbol, evaluatedArgument);

                if (finished)
                {
                    break;
                }
            }

            var res = evaluator.Evaluate(procedure.Body, localEnv);

            if (procedure.IsMacro)
            {
                return evaluator.Evaluate(res, environment);
            }
            else
            {
                return res;
            }
        }

        public AstNode Apply(DefineFunctionNode function, ListNode arguments, Environment environment)
        {
            if (arguments.Children().Count != 2)
            {
                throw new ArgumentException("Wrong number of arguments given to def! Expected: " + 2);
            }

            var symbol = arguments.Children().ElementAt(0) as SymbolNode;

            if (symbol == null)
            {
                throw new ArgumentException("First argument to def not a symbol!");
            }

            var valueForm = arguments.Children().ElementAt(1);

            var result = evaluator.Evaluate(valueForm, environment);

            environment.DefineSymbol(symbol, result);

            return symbol;
        }

        public AstNode Apply(DefmacroFunctionNode function, ListNode arguments, Environment environment)
        {
            if (arguments.Children().Count != 3)
            {
                throw new ArgumentException("Wrong number of arguments given to defmacro! Expected: " + 3);
            }

            var symbol = arguments.Children().ElementAt(0) as SymbolNode;
            if (symbol == null)
            {
                throw new ArgumentException("First argument to defmacro not a symbol!");
            }

            var parameterList = arguments.Children().ElementAt(1) as ListNode;
            if (parameterList == null)
            {
                throw new ArgumentException("Second argument to defmacro not a parameterlist!");
            }

            var body = arguments.Children().ElementAt(2);

            var macro = new ProcedureNode(parameterList.Children().Cast<SymbolNode>(), new[] { body }, environment.CreateChildEnvironment(), true);
            
            environment.DefineSymbol(symbol, macro);

            return symbol;
        }

        public AstNode Apply(SetFunctionNode function, ListNode arguments, Environment environment)
        {
            if (arguments.Children().Count != 2)
            {
                throw new ArgumentException("Wrong number of arguments given to set!! Expected: " + 2);
            }

            var symbol = arguments.Children().ElementAt(0) as SymbolNode;
            if (symbol == null)
            {
                throw new ArgumentException("First argument to set! not a symbol!");
            }

            var value = evaluator.Evaluate(arguments.Children().ElementAt(1), environment);

            if (!environment.SetSymbolValue(symbol, value))
            {
                throw new ArgumentException("Symbol '" + symbol.Name + "' not defined!");
            }

            return value;
        }
    }
}
