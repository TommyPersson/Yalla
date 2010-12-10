
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.Language
{
    [TestFixture]
    internal class Lists : LanguageTestBase
    {
        [Test]
        public void ShallBeAbleToCreateLists()
        {
            const string Program = "(list 1 2 3)";

            var result = Evaluator.Evaluate(Program) as ListNode;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Children().Contains(AstNode.MakeNode(1)));
            Assert.IsTrue(result.Children().Contains(AstNode.MakeNode(2)));
            Assert.IsTrue(result.Children().Contains(AstNode.MakeNode(3)));
        }

        [Test]
        public void FirstShallGetTheFirstItem()
        {
            const string Program = "(first (list 1 2 3))";

            var result = Evaluator.Evaluate(Program) as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void RestShallGetTheRestOfTheItem()
        {
            const string Program = "(rest (list 1 2 3))";

            var result = Evaluator.Evaluate(Program) as ListNode;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Children().Contains(AstNode.MakeNode(1)));
            Assert.IsTrue(result.Children().Contains(AstNode.MakeNode(2)));
            Assert.IsTrue(result.Children().Contains(AstNode.MakeNode(3)));
        }

        [Test]
        public void EmptyShallReturnTrueIfListIsEmptyFalseOtherwise()
        {
            const string Program1 = "(empty? (list 1 2 3))";
            const string Program2 = "(empty? (list))";

            var result1 = Evaluator.Evaluate(Program1) as BooleanNode;
            var result2 = Evaluator.Evaluate(Program2) as BooleanNode;

            Assert.IsNotNull(result1);
            Assert.AreEqual(false, result1.Value);

            Assert.IsNotNull(result2);
            Assert.AreEqual(true, result2.Value);
        }
    }
}
