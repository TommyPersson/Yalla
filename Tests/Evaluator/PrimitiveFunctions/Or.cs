
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Or : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToOrBooleans()
        {
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(or true false)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(or false false)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(or)")).Value);
        }
    }
}
