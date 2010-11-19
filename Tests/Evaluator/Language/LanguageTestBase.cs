
using System.IO;
using NUnit.Framework;
using Yalla.Evaluator;

namespace Tests.Evaluator.Language
{
    internal class LanguageTestBase
    {
        protected Yalla.Parser.Parser Parser { get; set; }

        protected Yalla.Evaluator.Evaluator Evaluator { get; set; }

        protected StringWriter StdOut { get; set; }

        protected StringReader StdIn { get; set; }

        [SetUp]
        protected void Setup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());

            StdOut = new StringWriter();

            Evaluator = new Yalla.Evaluator.Evaluator(Parser, new Environment(), StdOut, StdIn);
        }
    }
}
