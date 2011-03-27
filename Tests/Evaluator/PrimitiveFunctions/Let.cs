using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Let : PrimitiveFunctionTestBase
    {
        [Test]
        public void LetShallBindVariablesInBindingListToTheLocalEnvironment()
        {
            var result = (int)Evaluator.Evaluate("(let ((a 1) " + 
	                                              "      (b 2)) " +
	                                              "  (+ a b))");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void LetShallEvaluateBindingsInOrder()
        {
            var result = (int)Evaluator.Evaluate("(let ((a 1) " +
	                                              "      (b (+ a 1))) " +
	                                              "  (+ a b))");

            Assert.AreEqual(3, result);
        }
    }
}
