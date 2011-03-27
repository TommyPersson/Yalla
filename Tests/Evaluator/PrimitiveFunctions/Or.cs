using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Or : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToOrBooleans()
        {
            Assert.IsTrue(((bool)Evaluator.Evaluate("(or true false)")));
            Assert.IsFalse(((bool)Evaluator.Evaluate("(or false false)")));
            Assert.IsTrue(((bool)Evaluator.Evaluate("(or)")));
        }
    }
}
