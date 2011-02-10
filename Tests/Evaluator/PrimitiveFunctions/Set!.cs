
using System;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Set : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToSetDefinedVariables()
        {
            Assert.AreEqual(2, ((int)Evaluator.Evaluate("(def x 1)\n(set! x 2)\nx")));
        }

        [Test]
        public void ShallNotBeAbleToSetUndefinedVariables()
        {
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("(set! x 2)"));
        }

        [Test]
        public void ShallBeAbleToSetVariablesInOutsideScope()
        {
            const string Program = "(def x 2)\n" +
                                     "((lambda () (set! x 3)))\n" +
                                     "x";

            var result = (int)Evaluator.Evaluate(Program);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void ShallBeAbleToSetVariablesDefinedAfterFunctionDefinition()
        {
            const string Program = "(def fn (lambda () (set! y (+ y 1))))\n" +
                                     "(def y (+ 1 1))\n" + 
                                     "(fn)\n" + 
                                     "y";

            var result = (int)Evaluator.Evaluate(Program);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void ShallBeAbleToSetPropertyValues()
        {
            const string Program = "(let ((host \"192.168.1.123\") " +
                                   "      (urib (System.UriBuilder.)))" +
                                   "  (set! .Host urib \"192.168.1.123\")" +
                                   "  (= host (.Host urib)))";

            var result = (bool)Evaluator.Evaluate(Program);

            Assert.IsTrue(result);
        }
    }
}
