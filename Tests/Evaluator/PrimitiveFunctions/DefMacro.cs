
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
            var result = (int)Evaluator.Evaluate("(defmacro defun (sym params body) " +
	                                              "  `(def ~sym (lambda ~params ~body))) " +
	
	                                              "(defun my+ (x y) (+ x y)) " +
	
	                                              "(my+ 1 2)");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void MacrosShallSupportBodies()
        {
            var result = (int)Evaluator.Evaluate("(defmacro defun (sym params & body) " +
	                                              "  `(def ~sym (lambda ~params ~@body))) " +
	
	                                              "(defun my+ (x y)" +
	                                              "  (def z 1)" +
	                                              "  (set! z (+ x y z)) " +
	                                              "  z) " +

                                                  "(my+ 1 2)");

            Assert.AreEqual(4, result);
        }
    }
}
