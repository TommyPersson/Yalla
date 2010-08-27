
using System;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Yalla.Tests.Evaluator
{
    [TestFixture]
    internal partial class EvaluatorTests
    {
        private Yalla.Parser.Parser Parser { get; set; }

        private Yalla.Evaluator.Evaluator Evaluator { get; set; }

        [SetUp]
        public void Setup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());
            Evaluator = new Yalla.Evaluator.Evaluator(Parser);
        }

        [Test]
        public void NumberShallEvaluateToNumbers()
        {
            var result = Evaluator.Evaluate("1") as IntegerNode;
            var result2 = Evaluator.Evaluate("1.1") as DoubleNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
            Assert.IsNotNull(result2);
            Assert.AreEqual(1.1, result2.Value);
        }

        [Test]
        public void EvaluatorReturnsResultOfLastForm()
        {
            var result = Evaluator.Evaluate("1 2") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Value);
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
            var result = Evaluator.Evaluate("\"Hello World!\"") as StringNode;

            Assert.IsNotNull(result);
            Assert.AreEqual("Hello World!", result.Value);
        }

        [Test]
        public void ListsShouldEvaluateAsFunctionCalls()
        {
            var result = Evaluator.Evaluate("(+ 1 2 (+ 3 4))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Value);
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
