
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.Language
{
    [TestFixture]
    internal class FlowControl : LanguageTestBase
    {
        [Test]
        public void DoReturnsLastEvaluatedForm()
        {
            const string Program = "(do 1 2 3)";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [Test]
        public void WhenRunsBodyOnlyIfPredicateIsNotFalseOrNil()
        {
            const string Program1 = "(when true 1)";
            const string Program2 = "(when false 1)";

            var result1 = Evaluator.Evaluate(Program1) as IntegerNode;
            var result2 = Evaluator.Evaluate(Program2) as NilNode;

            Assert.IsNotNull(result1);
            Assert.AreEqual(1, result1.Value);

            Assert.IsNotNull(result2);
        }

        [Test]
        public void UnlessRunsBodyOnlyIfPredicateIsFalseOrNil()
        {
            const string Program1 = "(unless false 1)";
            const string Program2 = "(unless true 1)";

            var result1 = Evaluator.Evaluate(Program1) as IntegerNode;
            var result2 = Evaluator.Evaluate(Program2) as NilNode;

            Assert.IsNotNull(result1);
            Assert.AreEqual(1, result1.Value);

            Assert.IsNotNull(result2);
        }
    }
}
