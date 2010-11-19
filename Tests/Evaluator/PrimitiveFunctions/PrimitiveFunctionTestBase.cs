
using NUnit.Framework;

namespace Tests.Evaluator.PrimitiveFunctions
{
    internal class PrimitiveFunctionTestBase
    {
        protected Yalla.Parser.Parser Parser { get; set; }

        protected Yalla.Evaluator.Evaluator Evaluator { get; set; }

        [SetUp]
        protected void Setup()
        {
            Parser = new Yalla.Parser.Parser(new Yalla.Tokenizer.Tokenizer());
            Evaluator = new Yalla.Evaluator.Evaluator(Parser);
        }
    }
}
