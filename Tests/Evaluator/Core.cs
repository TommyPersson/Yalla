
using System;
using NUnit.Framework;
using Yalla.Parser.AstObjects;
using Environment = Yalla.Evaluator.Environment;

namespace Tests.Evaluator
{
    [TestFixture]
    internal class Core
    {
        private Yalla.Evaluator.Evaluator Evaluator { get; set; }

        [SetUp]
        public void Setup()
        {
            Evaluator = new Yalla.Evaluator.Evaluator(new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer()), new Environment(), null, null);
        }

        [Test]
        public void NumberShallEvaluateToNumbers()
        {
            var result = (int)Evaluator.Evaluate("1");
            var result2 = (decimal)Evaluator.Evaluate("1.1");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1.1, result2);
        }

        [Test]
        public void EvaluatorReturnsResultOfLastForm()
        {
            var result = (int)Evaluator.Evaluate("1 2\n3\n\n");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void QuotedNodesShouldEvaluateToTheirContents()
        {
            var result = Evaluator.Evaluate("'sym") as SymbolNode;

            Assert.IsNotNull(result);
            Assert.AreEqual("sym", result.Name);
        }

        [Test]
        public void ShallThrowExceptionIfSymbolDoesntExist()
        {
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("somesym"));
        }

        [Test]
        public void StringShouldEvaluateToStrings()
        {
            var result = Evaluator.Evaluate("\"Hello World!\"") as string;

            Assert.IsNotNull(result);
            Assert.AreEqual("Hello World!", result);
        }

        [Test]
        public void ListsShouldEvaluateAsFunctionCalls()
        {
            var result = (int)Evaluator.Evaluate("(+ 1 2 (+ 3 4))");

            Assert.AreEqual(10, result);
        }

        [Test]
        public void ShallThrowExceptionIfFirstItemInListIsNotAFunction()
        {
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("(1 2 3)"));
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("('1 2 3)"));
        }

        [Test]
        public void ShallThrowExceptionIfFunctionDoesntExist()
        {
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("(secret+ 1 2)"));
        }
    }
}
