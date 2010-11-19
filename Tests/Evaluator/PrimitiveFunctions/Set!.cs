
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
    }
}
