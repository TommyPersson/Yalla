using System;
using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Evaluator
    {
        private readonly Environment globalEnvironment =
            new Environment
                {
                    { new SymbolNode("+"), FunctionNode.PrimitiveFunctions["+"] },
                    { new SymbolNode("and"), FunctionNode.PrimitiveFunctions["and"] },
                    { new SymbolNode("or"), FunctionNode.PrimitiveFunctions["or"] },
                    { new SymbolNode("="), FunctionNode.PrimitiveFunctions["="] },
                    { new SymbolNode("lambda"), FunctionNode.PrimitiveFunctions["lambda"] },
                    { new SymbolNode("def"), FunctionNode.PrimitiveFunctions["def"] },
                    { new SymbolNode("true"), AstNode.MakeNode(true) },
                    { new SymbolNode("false"), AstNode.MakeNode(false) },
                };

        private readonly Parser.Parser parser;
        private readonly Applier applier;

        private readonly IDictionary<Type, Func<Evaluator, AstNode, Environment, AstNode>> evaluationFunctions =
            new Dictionary<Type, Func<Evaluator, AstNode, Environment, AstNode>>
                {
                    { typeof(ListNode), (x,y,z) => x.Evaluate((ListNode)y,z) },
                    { typeof(BooleanNode), (x,y,z) => x.Evaluate((BooleanNode)y,z) },
                    { typeof(IntegerNode), (x,y,z) => x.Evaluate((IntegerNode)y,z) },
                    { typeof(DecimalNode), (x,y,z) => x.Evaluate((DecimalNode)y,z) },
                    { typeof(StringNode), (x,y,z) => x.Evaluate((StringNode)y,z) },
                    { typeof(ObjectNode), (x,y,z) => x.Evaluate((ObjectNode)y,z) },
                    { typeof(QuoteNode), (x,y,z) => x.Evaluate((QuoteNode)y,z) },
                    { typeof(SymbolNode), (x,y,z) => x.Evaluate((SymbolNode)y,z) },
                    { typeof(FunctionNode), (x,y,z) => x.Evaluate((FunctionNode)y,z) },
                };

        public Evaluator(Parser.Parser parser, Environment environmentExtensions = null)
        {
            this.parser = parser;
            applier = new Applier(this);

            InitializeGlobalEnvironment(environmentExtensions);
        }

        public void InitializeGlobalEnvironment(Environment environmentExtensions = null)
        {
            if (environmentExtensions != null)
            {
                foreach (var environmentExtension in environmentExtensions)
                {
                    globalEnvironment.Add(environmentExtension.Key, environmentExtension.Value);
                }
            }
        }

        public AstNode Evaluate(string input)
        {
            AstNode lastResult = null;

            foreach (var form in parser.Parse(input))
            {
                lastResult = Evaluate(form, globalEnvironment);
            }

            return lastResult;
        }

        public AstNode Evaluate(AstNode node, Environment environment)
        {
            return evaluationFunctions[node.GetType()].Invoke(this, node, environment);
        }

        public AstNode Evaluate(IEnumerable<AstNode> forms, Environment environment)
        {
            AstNode lastResult = null;

            foreach (var form in forms)
            {
                lastResult = Evaluate(form, environment);
            }

            return lastResult;
        }

        public AstNode Evaluate(ListNode node, Environment environment)
        {
            FunctionNode function = Evaluate(node.First(), environment) as FunctionNode;

            if (function == null)
            {
                throw new ArgumentException("First item in list not a function!");
            }

            return applier.Apply(function, node.Rest(), environment);
        }

        public AstNode Evaluate(BooleanNode node, Environment environment)
        {
            return node;
        }

        public AstNode Evaluate(IntegerNode node, Environment environment)
        {
            return node;
        }

        public AstNode Evaluate(DecimalNode node, Environment environment)
        {
            return node;
        }

        public AstNode Evaluate(StringNode node, Environment environment)
        {
            return node;
        }

        public AstNode Evaluate(ObjectNode node, Environment environment)
        {
            return node;
        }

        public AstNode Evaluate(QuoteNode node, Environment environment)
        {
            return node.InnerValue;
        }

        public AstNode Evaluate(SymbolNode node, Environment environment)
        {
            if (environment.ContainsKey(node))
            {
                return environment[node];               
            }

            if (node.Name.StartsWith("."))
            {
                return new NativeMethodFunctionNode(node.Name.Substring(1));
            }

            throw new ArgumentException("Could not resolve symbol: " + node.Name);
        }

        public AstNode Evaluate(FunctionNode node, Environment environment)
        {
            return node;
        }
    }
}
