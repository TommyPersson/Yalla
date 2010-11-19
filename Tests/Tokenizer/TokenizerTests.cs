
using System.Data;
using System.Linq;
using NUnit.Framework;
using Yalla.Tokenizer;

namespace Tests.Tokenizer
{
    [TestFixture]
    internal class TokenizerTests
    {
        [Test]
        public void CanTokenizeParentheses()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize("(())");

            Assert.AreEqual(5, tokens.Count());
            Assert.AreEqual(Token.TokenType.LParen, tokens.ElementAt(0).Type);
            Assert.AreEqual(Token.TokenType.LParen, tokens.ElementAt(1).Type);
            Assert.AreEqual(Token.TokenType.RParen, tokens.ElementAt(2).Type);
            Assert.AreEqual(Token.TokenType.RParen, tokens.ElementAt(3).Type);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens.ElementAt(4).Type);
        }

        [Test]
        public void CanTokenizeQuotes()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize("'()");

            Assert.AreEqual(4, tokens.Count());
            Assert.AreEqual(Token.TokenType.Quote, tokens.ElementAt(0).Type);
            Assert.AreEqual(Token.TokenType.LParen, tokens.ElementAt(1).Type);
            Assert.AreEqual(Token.TokenType.RParen, tokens.ElementAt(2).Type);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens.ElementAt(3).Type);
        }

        [Test]
        public void CanTokenizeStrings()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize("\"Hello, World!\"");

            Assert.AreEqual(2, tokens.Count());
            Assert.AreEqual(Token.TokenType.String, tokens.ElementAt(0).Type);
            Assert.AreEqual("Hello, World!", tokens.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens.ElementAt(1).Type);
        }

        [Test]
        public void ShallFrownAtUnterminatedStrings()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();

            Assert.Throws(typeof(SyntaxErrorException), () => tokenizer.Tokenize("\"Hello, World!"));
        }

        [Test]
        public void CanTokenizeSymbols()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize(".abc");
            var tokens2 = tokenizer.Tokenize("abc123");

            Assert.AreEqual(2, tokens.Count());
            Assert.AreEqual(Token.TokenType.Symbol, tokens.ElementAt(0).Type);
            Assert.AreEqual(".abc", tokens.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens.ElementAt(1).Type);


            Assert.AreEqual(2, tokens2.Count());
            Assert.AreEqual(Token.TokenType.Symbol, tokens2.ElementAt(0).Type);
            Assert.AreEqual("abc123", tokens2.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens2.ElementAt(1).Type);
        }

        [Test]
        public void CanTokenizeNumbers()
        {
            var tokenizer = new Yalla.Tokenizer.Tokenizer();
            var tokens = tokenizer.Tokenize("123");
            var tokens2 = tokenizer.Tokenize("123.456");
            var tokens3 = tokenizer.Tokenize("-123");

            Assert.AreEqual(2, tokens.Count());
            Assert.AreEqual(Token.TokenType.Integer, tokens.ElementAt(0).Type);
            Assert.AreEqual("123", tokens.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens.ElementAt(1).Type);

            Assert.AreEqual(2, tokens2.Count());
            Assert.AreEqual(Token.TokenType.Double, tokens2.ElementAt(0).Type);
            Assert.AreEqual("123.456", tokens2.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens2.ElementAt(1).Type);

            Assert.AreEqual(2, tokens3.Count());
            Assert.AreEqual(Token.TokenType.Integer, tokens3.ElementAt(0).Type);
            Assert.AreEqual("-123", tokens3.ElementAt(0).Value);
            Assert.AreEqual(Token.TokenType.EndOfFile, tokens3.ElementAt(1).Type);
        }
    }
}
