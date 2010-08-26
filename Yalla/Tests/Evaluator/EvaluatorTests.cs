using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Yalla.Tests.Evaluator
{
    [TestFixture]
    internal class EvaluatorTests
    {
        private Yalla.Parser.Parser Parser { get; set; }

        private Yalla.Evaluator.Evaluator Evaluator { get; set; }

        [SetUp]
        public void Setup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());
            Evaluator = new Yalla.Evaluator.Evaluator(Parser, null);
        }

        [Test]
        public void NumberShallEvaluateToNumbers()
        {
            var result = Evaluator.Evaluate("1") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void EvaluatorReturnsResultOfLastForm()
        {
            var result = Evaluator.Evaluate("1 2") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Value);
        }
    }
}
