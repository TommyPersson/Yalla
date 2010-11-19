
using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class List : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToCreateLists()
        {
            var result = Evaluator.Evaluate("(list 1 2 3)") as ListNode;

            Assert.IsNotNull(result);

            var values = result.Children().Cast<IntegerNode>().Select(x => x.Value);

            Assert.AreEqual(1, values.ElementAt(0));
            Assert.AreEqual(2, values.ElementAt(1));
            Assert.AreEqual(3, values.ElementAt(2));
        }
    }
}
