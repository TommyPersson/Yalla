
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Plus : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleTo()
        {
            var result = Evaluator.Evaluate("(+ 1 2 (+ 3 4))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Value);
        }
    }
}
