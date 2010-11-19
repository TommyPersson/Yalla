
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Def : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToDefineVariables()
        {
            var result = Evaluator.Evaluate("(def x 1)\n(+ x 1)") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Value);
        }

    }
}
