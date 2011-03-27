using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Def : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToDefineVariables()
        {
            var result = (int)Evaluator.Evaluate("(def x 1)\n(+ x 1)");

            Assert.AreEqual(2, result);
        }
    }
}
