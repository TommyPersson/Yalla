
using System.IO;
using NUnit.Framework;
using Yalla.Evaluator;

namespace Tests.Evaluator.PrimitiveFunctions
{
    internal class PrimitiveFunctionTestBase
    {
        protected Yalla.Parser.Parser Parser { get; set; }

        protected Yalla.Evaluator.Evaluator Evaluator { get; set; }

        protected TextWriter StdOut { get; set; }

        protected TextReader StdIn { get; set; }

        [SetUp]
        protected void Setup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());

            StdOut = new StringWriter();

            Evaluator = new Yalla.Evaluator.Evaluator(Parser, new Environment(), StdOut, StdIn);
        }
    }
}
