
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class If : PrimitiveFunctionTestBase
    {
        [Test]
        public void IfShallReturnFirstFormOnlyIfPredicateReturnsTrue()
        {
            const string Program = "(if true 1 2)";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void IfShallReturnSecondFormOnlyIfPredicateReturnsFalse()
        {
            const string Program = "(if false 1 2)";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Value);
        }

        [Test]
        public void IfDoesntNeedAFalseBranch()
        {
            const string Program = "(if true 1)";
            
            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void IfShallReturnNilIfPredicateFailsAndThereIsNoFalseBranch()
        {
            const string Program = "(if false 1)";

            var result = Evaluator.Evaluate(Program) as NilNode;

            Assert.IsNotNull(result);
        }
        
        [Test]
        public void IfTreatsAnyValueOtherThanNilAndFalseAsTrue()
        {
            const string Program = "(if \"asd\" 1)";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }
    }
}
