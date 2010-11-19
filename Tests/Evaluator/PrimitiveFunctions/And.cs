
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class And : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToAndValues()
        {
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(and true true)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(and true false true)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(and)")).Value);
        }
    }
}
