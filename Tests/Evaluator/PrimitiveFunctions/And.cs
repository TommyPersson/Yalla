
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
            Assert.IsTrue(((bool)Evaluator.Evaluate("(and true true)")));
            Assert.IsFalse(((bool)Evaluator.Evaluate("(and true false true)")));
            Assert.IsTrue(((bool)Evaluator.Evaluate("(and)")));
        }
    }
}
