
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Lambda : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallReturnAProcedureObject()
        {
            var result = Evaluator.Evaluate("(lambda (x) x)") as ProcedureNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Parameters.Count());
            Assert.IsTrue(result.Body.First() is SymbolNode);
        }

        [Test]
        public void CanRunLambdaProcedure()
        {
            var result = Evaluator.Evaluate("((lambda (x y) (+ x y)) 1 2)") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }
    }
}
