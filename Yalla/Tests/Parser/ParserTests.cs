
using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Yalla.Tests.Parser
{
    [TestFixture]
    internal class ParserTests
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

            var intNode = result.First() as IntegerNode;

            Assert.IsNotNull(intNode);
            Assert.AreEqual(123, intNode.Value);
        }

        [Test]
        public void ShallParseDoubles()
        {
            var result = Parser.Parse("3.4");

            var doubleNode = result.First() as DoubleNode;

            Assert.IsNotNull(doubleNode);
            Assert.AreEqual(3.4, doubleNode.Value);
        }

        [Test]
        public void ShallParseString()
        {
            var result = Parser.Parse("\"Hello World!\"");

            var stringNode = result.First() as StringNode;

            Assert.IsNotNull(stringNode);
            Assert.AreEqual("Hello World!", stringNode.Value);
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
        public void ShallParseEmptyLists()
        {
            var result = Parser.Parse("()");

            var listNode = result.First() as ListNode;

            Assert.IsNotNull(listNode);
            Assert.AreEqual(0, listNode.Children().Count);
        }

        [Test]
        public void ShallParseLists()
        {
            var result = Parser.Parse("(+ 1 2)");

            var listNode = result.First() as ListNode;

            Assert.IsNotNull(listNode);
            Assert.AreEqual(3, listNode.Children().Count);
            Assert.AreEqual(new SymbolNode("+"), listNode.Children().ElementAt(0));
            Assert.AreEqual(new IntegerNode(1), listNode.Children().ElementAt(1));
            Assert.AreEqual(new IntegerNode(2), listNode.Children().ElementAt(2));
        }
    }
}
