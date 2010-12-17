
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Plus : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToAddValues()
        {
            var result = (int)Evaluator.Evaluate("(+ 1 2 (+ 3 4))");

            Assert.AreEqual(10, result);
        }
    }
}
