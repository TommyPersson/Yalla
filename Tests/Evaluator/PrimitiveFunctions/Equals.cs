
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Equals : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToCompareValues()
        {
            Assert.IsTrue(((bool)Evaluator.Evaluate("(= true true)")));
            Assert.IsFalse(((bool)Evaluator.Evaluate("(= true false)")));
            Assert.IsTrue(((bool)Evaluator.Evaluate("(= \"string\" \"string\")")));
            Assert.IsTrue(((bool)Evaluator.Evaluate("(= (+ 1 2) 3)")));
        }
    }
}
