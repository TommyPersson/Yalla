
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Evaluator
    {
        private readonly Environment globalEnvironment =
            new Environment(new Dictionary<SymbolNode, object>
                                {
                                    { new SymbolNode("+"), FunctionNode.PrimitiveFunctions["+"] },
                                    { new SymbolNode("and"), FunctionNode.PrimitiveFunctions["and"] },
                                    { new SymbolNode("or"), FunctionNode.PrimitiveFunctions["or"] },
                                    { new SymbolNode("="), FunctionNode.PrimitiveFunctions["="] },
                                    { new SymbolNode("cons"), FunctionNode.PrimitiveFunctions["cons"] },
                                    { new SymbolNode("lambda"), FunctionNode.PrimitiveFunctions["lambda"] },
                                    { new SymbolNode("def"), FunctionNode.PrimitiveFunctions["def"] },
                                    { new SymbolNode("defmacro"), FunctionNode.PrimitiveFunctions["defmacro"] },
                                    { new SymbolNode("set!"), FunctionNode.PrimitiveFunctions["set!"] },
                                    { new SymbolNode("if"), FunctionNode.PrimitiveFunctions["if"] },
                                    { new SymbolNode("let"), FunctionNode.PrimitiveFunctions["let"] },
                                    { new SymbolNode("map"), FunctionNode.PrimitiveFunctions["map"] },
                                    { new SymbolNode("nil"), new NilNode() },
                                    { new SymbolNode("true"), AstNode.MakeNode(true) },
                                    { new SymbolNode("false"), AstNode.MakeNode(false) }
                                });

        private readonly Parser.Parser parser;
        private readonly Applier applier;
        private readonly BackquoteExpander backqouteExpander;

        private readonly IDictionary<Type, Func<Evaluator, object, Environment, object>> evaluationFunctions =
            new Dictionary<Type, Func<Evaluator, object, Environment, object>>
                {
                    { typeof(ListNode), (x, y, z) => x.Evaluate((ListNode)y, z) },
                    { typeof(bool), (x, y, z) => x.EvaluateToSelf(y, z) },
                    { typeof(int), (x, y, z) => x.EvaluateToSelf(y, z) },
                    { typeof(decimal), (x, y, z) => x.EvaluateToSelf(y, z) },
                    { typeof(string), (x, y, z) => x.EvaluateToSelf(y, z) },
                    { typeof(object), (x, y, z) => x.EvaluateToSelf(y, z) },
                    { typeof(QuoteNode), (x, y, z) => x.Evaluate((QuoteNode)y, z) },
                    { typeof(BackquoteNode), (x, y, z) => x.Evaluate((BackquoteNode)y, z) },
                    { typeof(SymbolNode), (x, y, z) => x.Evaluate((SymbolNode)y, z) },
                    { typeof(FunctionNode), (x, y, z) => x.EvaluateToSelf(y, z) },
                };

        public Evaluator(Parser.Parser parser, Environment environmentExtensions, TextWriter stdOut, TextReader stdIn)
        {
            this.parser = parser;
            applier = new Applier(this);
            backqouteExpander = new BackquoteExpander(this);
            
            InitializeGlobalEnvironment(environmentExtensions, stdOut, stdIn);

            ReadCoreLanguageCode();
        }

		public Environment GlobalEnvironment { get { return globalEnvironment; } }
		
        public object Evaluate(string input)
        {
            object lastResult = null;

            foreach (var form in parser.Parse(input))
            {
                lastResult = Evaluate(form, globalEnvironment);
            }

            return lastResult;
        }

        public object Evaluate(object node)
        {
            return Evaluate(node, globalEnvironment);
        }

        public object Evaluate(object node, Environment environment)
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

        public object Evaluate(IEnumerable<object> forms, Environment environment)
        {
            object lastResult = null;

            foreach (var form in forms)
            {
                lastResult = Evaluate(form, environment);
            }

            return lastResult;
        }

        public object Evaluate(ListNode node, Environment environment)
        {
            FunctionNode function = Evaluate(node.First(), environment) as FunctionNode;

            if (function == null)
            {
                throw new ArgumentException("First item in list not a function!");
            }

            return applier.Apply(function, node.Rest(), environment);
        }
        
        public object Evaluate(QuoteNode node, Environment environment)
        {
            return node.InnerValue;
        }

        public object Evaluate(BackquoteNode node, Environment environment)
        {
            return backqouteExpander.Expand(node.InnerValue, 1, environment);
        }

        public object Evaluate(SymbolNode node, Environment environment)
        {
            if (node.Name.StartsWith("."))
            {
                return new NativeMethodFunctionNode(node.Name.Substring(1));
            }

            if (node.Name.EndsWith("."))
            {
                return new NativeConstructorFunctionNode(node.Name.TrimEnd('.'));
            }

            var split = node.Name.Split('/');

            if (split.Length > 1)
            {
                if (split.Length != 2)
                {
                    throw new ArgumentException("A symbol may not contain more than one '/'!");
                }

                return new NativeStaticMethodFunctionNode(split[0], split[1]);
            }
            

            if (environment.CanLookUpSymbol(node))
            {
                return environment.LookUpSymbol(node);               
            }

            throw new ArgumentException("Could not resolve symbol: " + node.Name);
        }

        public object EvaluateToSelf(object node, Environment environment)
        {
            return node;
        }

        private void ReadCoreLanguageCode()
        {
            var textStream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Yalla.Evaluator.Language.Core.yl"));

            var text = textStream.ReadToEnd();

            Evaluate(text);
        }

        private void InitializeGlobalEnvironment(Environment environmentExtensions, TextWriter stdOut, TextReader stdIn)
        {
            globalEnvironment.DefineSymbol(new SymbolNode("*evaluator*"), AstNode.MakeNode(this));
            globalEnvironment.DefineSymbol(new SymbolNode("*parser*"), AstNode.MakeNode(parser));
            globalEnvironment.DefineSymbol(new SymbolNode("*applier*"), AstNode.MakeNode(applier));
            globalEnvironment.DefineSymbol(new SymbolNode("*stdout*"), AstNode.MakeNode(stdOut));
            globalEnvironment.DefineSymbol(new SymbolNode("*stdin*"), AstNode.MakeNode(stdIn));

            if (environmentExtensions != null)
            {
                foreach (var environmentExtension in environmentExtensions)
                {
                    globalEnvironment.DefineSymbol(environmentExtension.Key, environmentExtension.Value);
                }
            }
        }
    }
}
