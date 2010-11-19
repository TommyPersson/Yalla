
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
            Assert.AreEqual(2, ((IntegerNode)Evaluator.Evaluate("(def x 1)\n(set! x 2)\nx")).Value);
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

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [Test]
        public void ShallBeAbleToSetVariablesDefinedAfterFunctionDefinition()
        {
            const string Program = "(def fn (lambda () (set! y (+ y 1))))\n" +
                                   "(def y (+ 1 1))\n" + 
                                   "(fn)\n" + 
                                   "y";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }
    }
}
