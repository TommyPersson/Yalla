
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
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= true true)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(= true false)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= \"string\" \"string\")")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= (+ 1 2) 3)")).Value);
        }
    }
}
