
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class DefMacro : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToDefineMacros()
        {
            var result = Evaluator.Evaluate("(defmacro defun (sym params body) " +
                                            "  `(def ~sym (lambda ~params ~body))) " +
                                            "(defun my+ (x y) (+ x y)) " +
                                            "(my+ 1 2)") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }
    }
}
