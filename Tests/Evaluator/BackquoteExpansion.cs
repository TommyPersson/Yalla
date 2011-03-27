using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Yalla.Evaluator;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator
{
    [TestFixture]
    public class BackquoteExpansion
    {
        private Yalla.Evaluator.Evaluator Evaluator { get; set; }

        [SetUp]
        public void Setup()
        {
            Evaluator = new Yalla.Evaluator.Evaluator(new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer()), new Environment(), null, null);
        }

        [Test]
        public void SingleValuesBehaveAsQuoted()
        {
            Assert.AreEqual("sym", ((SymbolNode)Evaluator.Evaluate("`sym")).Name);
        }

        [Test]
        public void UnqoutedValuesAreEvaluated()
        {
            Assert.AreEqual(1, ((int)Evaluator.Evaluate("(def a 1)\n`~a")));
        }

        [Test]
        public void ShallSpliceListsIntoLists()
        {
            var result = (IList<object>)Evaluator.Evaluate("`(a b ~@'(c d))");

            Assert.AreEqual("a", ((SymbolNode)result.ElementAt(0)).Name);
            Assert.AreEqual("b", ((SymbolNode)result.ElementAt(1)).Name);
            Assert.AreEqual("c", ((SymbolNode)result.ElementAt(2)).Name);
            Assert.AreEqual("d", ((SymbolNode)result.ElementAt(3)).Name);
        }

        [Test]
        public void ShallHandleNestedBackQuotes()
        {
            var result = (IList<object>)Evaluator.Evaluate("`(a b ~@`(c d ~(+ 1 2)))");

            Assert.AreEqual("a", ((SymbolNode)result.ElementAt(0)).Name);
            Assert.AreEqual("b", ((SymbolNode)result.ElementAt(1)).Name);
            Assert.AreEqual("c", ((SymbolNode)result.ElementAt(2)).Name);
            Assert.AreEqual("d", ((SymbolNode)result.ElementAt(3)).Name);
            Assert.AreEqual(3, ((int)result.ElementAt(4)));
        }
    }
}
