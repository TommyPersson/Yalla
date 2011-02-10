
using System.Collections.Generic;
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

            var result = Evaluator.Evaluate(Program) as IList<object>;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(AstNode.MakeNode(1)));
            Assert.IsTrue(result.Contains(AstNode.MakeNode(2)));
            Assert.IsTrue(result.Contains(AstNode.MakeNode(3)));
        }

        [Test]
        public void FirstShallGetTheFirstItem()
        {
            const string Program = "(first (list 1 2 3))";

            var result = (int)Evaluator.Evaluate(Program);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void RestShallGetTheRestOfTheItem()
        {
            const string Program = "(rest (list 1 2 3))";

            var result = Evaluator.Evaluate(Program) as IList<object>;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Contains(AstNode.MakeNode(1)));
            Assert.IsTrue(result.Contains(AstNode.MakeNode(2)));
            Assert.IsTrue(result.Contains(AstNode.MakeNode(3)));
        }

        [Test]
        public void EmptyShallReturnTrueIfListIsEmptyFalseOtherwise()
        {
            const string Program1 = "(empty? (list 1 2 3))";
            const string Program2 = "(empty? (list))";

            var result1 = (bool)Evaluator.Evaluate(Program1);
            var result2 = (bool)Evaluator.Evaluate(Program2);

            Assert.AreEqual(false, result1);
            Assert.AreEqual(true, result2);
        }
        
        [Test]
        public void AssociationLists()
        {
            const string Program1 = "(assoc 's1 '(s1 r1 s2 r2))";
            const string Program2 = "(assoc 's2 '(s1 r1 s2 r2))";
            const string Program3 = "(assoc 's3 '(s1 r1 s2 r2))";
            
            var result1 = (SymbolNode)Evaluator.Evaluate(Program1);
            var result2 = (SymbolNode)Evaluator.Evaluate(Program2);
            var result3 = (NilNode)Evaluator.Evaluate(Program3);

            Assert.AreEqual(new SymbolNode("r1"), result1);
            Assert.AreEqual(new SymbolNode("r2"), result2);
            Assert.IsNotNull(result3);
        }
    }
}
