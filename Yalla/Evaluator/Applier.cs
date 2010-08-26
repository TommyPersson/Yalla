using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Applier
    {
        private readonly IDictionary<Type, Func<Applier, FunctionNode, ListNode, Environment, AstNode>> applyFunctions =
            new Dictionary<Type, Func<Applier, FunctionNode, ListNode, Environment, AstNode>>
                {
                    { typeof(AddFunctionNode), (x, y, z, v) => x.Apply((AddFunctionNode)y, z, v) }
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
    }
}
