
using System.Collections.Generic;
using System.Linq;
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
            var result = (int)Evaluator.Evaluate("((lambda (x y) (+ x y)) 1 2)");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void LambdaBodiesCanContainMultipleForms()
        {
            var result = (int)Evaluator.Evaluate("((lambda ()\n" + 
                                            "  (def z 1)\n" + 
                                            "  (set! z (+ z 2))\n" +
                                            "  z))");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void LabmdaListShouldAllowBodyExpressions()
        {
            const string Program = "(def fn (lambda (& body) body))\n" +
                                     "(fn 1 2 3)\n";

            var result = Evaluator.Evaluate(Program) as IList<object>;

            Assert.IsNotNull(result);

            var items = result;

            Assert.AreEqual(1, ((int)items[0]));
            Assert.AreEqual(2, ((int)items[1]));
            Assert.AreEqual(3, ((int)items[2]));
        }
    }
}
