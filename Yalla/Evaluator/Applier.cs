
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
                    { typeof(NativeMethodFunctionNode), (x, y, z, v) => x.Apply((NativeMethodFunctionNode)y, z, v) },
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

            if (!args.All(x => x.GetType() == typeof(IntegerNode) || x.GetType() == typeof(DoubleNode)))
            {
                throw new ArgumentException("'+' may only take integer or double values!");
            }

            double result = 0;

            foreach (var arg in args)
            {
                if (arg is IntegerNode)
                {
                    result += ((IntegerNode)arg).Value;
                }

                if (arg is DoubleNode)
                {
                    result += ((DoubleNode) arg).Value;
                }
            }

            return ((result % 1) == 0)
                       ? (AstNode)new IntegerNode(Convert.ToInt32(result))
                       : new DoubleNode(result);
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
            if (omethod == null)
            {
                throw new ArgumentException("Method " + method.Name + " not found on type " + otype.Name + "!");
            }

            var result = omethod.Invoke(obj.Object, args.ToArray());

            return AstNode.MakeNode(result);
        }
    }
}
