
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
            var result = Evaluator.Evaluate("((lambda (x y) (+ x y)) 1 2)") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [Test]
        public void LambdaBodiesCanContainMultipleForms()
        {
            var result = Evaluator.Evaluate("((lambda ()\n" + 
                                            "  (def z 1)\n" + 
                                            "  (set! z (+ z 2))\n" +
                                            "  z))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }


        [Test]
        public void LabmdaListShouldAllowBodyExpressions()
        {
            const string Program = "(def fn (lambda (& body) body))\n" +
                                   "(fn 1 2 3)\n";

            var result = Evaluator.Evaluate(Program) as ListNode;

            Assert.IsNotNull(result);

            var items = result.Children();

            Assert.AreEqual(1, ((IntegerNode)items[0]).Value);
            Assert.AreEqual(2, ((IntegerNode)items[1]).Value);
            Assert.AreEqual(3, ((IntegerNode)items[2]).Value);
        }
    }
}
