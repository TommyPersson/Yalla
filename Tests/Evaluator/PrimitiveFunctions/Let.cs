
using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Let : PrimitiveFunctionTestBase
    {
        [Test]
        public void LetShallBindVariablesInBindingListToTheLocalEnvironment()
        {
            var result = Evaluator.Evaluate("(let ((a 1) " + 
                                            "      (b 2)) " +
                                            "  (+ a b))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [Test]
        public void LetShallEvaluateBindingsInOrder()
        {
            var result = Evaluator.Evaluate("(let ((a 1) " +
                                            "      (b (+ a 1))) " +
                                            "  (+ a b))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }
    }
}
