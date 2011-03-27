using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Parser
{
    [TestFixture]
    public class ParserTests
    {
        private Yalla.Parser.Parser Parser { get; set; }

        [SetUp]
        public void TestSetup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());
        }

        [Test]
        public void ShallParseIntegers()
        {
            var result = Parser.Parse("123");

            var intNode = (int)result.First();

            Assert.AreEqual(123, intNode);
        }

        [Test]
        public void ShallParseDoubles()
        {
            var result = Parser.Parse("3.4");

            var doubleNode = (decimal)result.First();

            Assert.AreEqual(3.4, doubleNode);
        }

        [Test]
        public void ShallParseNegativeNumbers()
        {
            var result = Parser.Parse("-3.4");

            var doubleNode = (decimal)result.First();

            Assert.AreEqual(-3.4, doubleNode);
        }

        [Test]
        public void ShallParseString()
        {
            var result = (string)Parser.Parse("\"Hello World!\"").First();

            Assert.AreEqual("Hello World!", result);
            
            var s = "\"\\\"Hej\\\"\"";
            
            System.Console.Out.WriteLine(s);
            
            var result2 = (string)Parser.Parse(s).First();
            
            Assert.AreEqual("\"Hej\"", result2);
        }

        [Test]
        public void ShallParseSymbols()
        {
            var result = Parser.Parse("sym");

            var symbolNode = result.First() as SymbolNode;

            Assert.IsNotNull(symbolNode);
            Assert.AreEqual("sym", symbolNode.Name);
        }

        [Test]
        public void ShallParseQuotedValues()
        {
            var result = Parser.Parse("'sym");

            var quoteNode = result.First() as QuoteNode;

            Assert.IsNotNull(quoteNode);

            var symbolNode = quoteNode.InnerValue as SymbolNode;

            Assert.IsNotNull(symbolNode);
            Assert.AreEqual("sym", symbolNode.Name);
        }

        [Test]
        public void QuotedCanBeAppliedManyTimes()
        {
            var result = Parser.Parse("''sym");

            var quoteNode = result.First() as QuoteNode;
            var quoteNode2 = quoteNode.InnerValue as QuoteNode;
            var symbolNode = quoteNode2.InnerValue as SymbolNode;

            Assert.AreEqual("sym", symbolNode.Name);
        }

        [Test]
        public void ShallParseEmptyLists()
        {
            var result = Parser.Parse("()");

            var listNode = result.First() as IList<object>;

            Assert.IsNotNull(listNode);
            Assert.AreEqual(0, listNode.Count);
        }

        [Test]
        public void ShallParseLists()
        {
            var result = Parser.Parse("(+ 1 2)");

            var listNode = result.First() as IList<object>;

            Assert.IsNotNull(listNode);
            Assert.AreEqual(3, listNode.Count);
            Assert.AreEqual(new SymbolNode("+"), listNode.ElementAt(0));
            Assert.AreEqual(1, listNode.ElementAt(1));
            Assert.AreEqual(2, listNode.ElementAt(2));
        }

        [Test]
        public void ShallParseBackQuotedValues()
        {
            var result = Parser.Parse("`sym");

            var backQuoteNode = result.First() as BackquoteNode;

            Assert.IsNotNull(backQuoteNode, "not backquotenode :(");

            var symbolNode = backQuoteNode.InnerValue as SymbolNode;

            Assert.IsNotNull(symbolNode, "not symbolnode :(");
            Assert.AreEqual("sym", symbolNode.Name);
        }
    }
}
