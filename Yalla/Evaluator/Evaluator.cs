using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Evaluator
    {
        private readonly Environment globalEnvironment =
            new Environment(new Dictionary<SymbolNode, AstNode>
                                {
                                    {new SymbolNode("+"), FunctionNode.PrimitiveFunctions["+"]},
                                    {new SymbolNode("and"), FunctionNode.PrimitiveFunctions["and"]},
                                    {new SymbolNode("or"), FunctionNode.PrimitiveFunctions["or"]},
                                    {new SymbolNode("="), FunctionNode.PrimitiveFunctions["="]},
                                    {new SymbolNode("list"), FunctionNode.PrimitiveFunctions["list"]},
                                    {new SymbolNode("cons"), FunctionNode.PrimitiveFunctions["cons"]},
                                    {new SymbolNode("lambda"), FunctionNode.PrimitiveFunctions["lambda"]},
                                    {new SymbolNode("def"), FunctionNode.PrimitiveFunctions["def"]},
                                    {new SymbolNode("defmacro"), FunctionNode.PrimitiveFunctions["defmacro"]},
                                    {new SymbolNode("set!"), FunctionNode.PrimitiveFunctions["set!"]},
                                    {new SymbolNode("if"), FunctionNode.PrimitiveFunctions["if"]},
                                    {new SymbolNode("nil"), new NilNode()},
                                    {new SymbolNode("true"), AstNode.MakeNode(true)},
                                    {new SymbolNode("false"), AstNode.MakeNode(false)},
                                });

        private readonly Parser.Parser parser;
        private readonly Applier applier;
        private readonly BackquoteExpander backqouteExpander;

        private readonly IDictionary<Type, Func<Evaluator, AstNode, Environment, AstNode>> evaluationFunctions =
            new Dictionary<Type, Func<Evaluator, AstNode, Environment, AstNode>>
                {
                    { typeof(ListNode), (x,y,z) => x.Evaluate((ListNode)y,z) },
                    { typeof(BooleanNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                    { typeof(IntegerNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                    { typeof(DecimalNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                    { typeof(StringNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                    { typeof(ObjectNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                    { typeof(QuoteNode), (x,y,z) => x.Evaluate((QuoteNode)y,z) },
                    { typeof(BackquoteNode), (x,y,z) => x.Evaluate((BackquoteNode)y,z) },
                    { typeof(SymbolNode), (x,y,z) => x.Evaluate((SymbolNode)y,z) },
                    { typeof(FunctionNode), (x,y,z) => x.EvaluateToSelf(y,z) },
                };

        public Evaluator(Parser.Parser parser, Environment environmentExtensions = null)
        {
            this.parser = parser;
            applier = new Applier(this);
            backqouteExpander = new BackquoteExpander(this);

            ReadCoreLanguageCode();

            InitializeGlobalEnvironment(environmentExtensions);
        }

        private void ReadCoreLanguageCode()
        {
            var textStream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Yalla.Evaluator.Language.Core.yl"));

            var text = textStream.ReadToEnd();

            Evaluate(text);
        }

        public void InitializeGlobalEnvironment(Environment environmentExtensions = null)
        {
            if (environmentExtensions != null)
            {
                foreach (var environmentExtension in environmentExtensions)
                {
                    globalEnvironment.DefineSymbol(environmentExtension.Key, environmentExtension.Value);
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

        public AstNode Evaluate(AstNode node)
        {
            return Evaluate(node, globalEnvironment);
        }

        public AstNode Evaluate(AstNode node, Environment environment)
        {
            try
            {
                return evaluationFunctions[node.GetType()].Invoke(this, node, environment);
            }
            catch (KeyNotFoundException)
            {
                throw new SyntaxErrorException("Cannot evaluate " + node.GetType() + " in this context.");
            }
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
        
        public AstNode Evaluate(QuoteNode node, Environment environment)
        {
            return node.InnerValue;
        }

        public AstNode Evaluate(BackquoteNode node, Environment environment)
        {
            return backqouteExpander.Expand(node.InnerValue, 1, environment);
        }

        public AstNode Evaluate(SymbolNode node, Environment environment)
        {
            if (environment.CanLookUpSymbol(node))
            {
                return environment.LookUpSymbol(node);               
            }

            if (node.Name.StartsWith("."))
            {
                return new NativeMethodFunctionNode(node.Name.Substring(1));
            }

            throw new ArgumentException("Could not resolve symbol: " + node.Name);
        }

        public AstNode EvaluateToSelf(AstNode node, Environment environment)
        {
            return node;
        }
    }
}
