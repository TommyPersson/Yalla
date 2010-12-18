
using System;
using System.Linq;
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Map : PrimitiveFunctionTestBase
    {
        [Test]
        public void MapShallMapLambdaAcrossList()
        {
            const string Program = "(map (lambda (x) (+ x 1)) '(1 2 3))";

            var result = (ListNode)Evaluator.Evaluate(Program);

            Assert.AreEqual(2, result.Children().ElementAt(0));
            Assert.AreEqual(3, result.Children().ElementAt(1));
            Assert.AreEqual(4, result.Children().ElementAt(2));
        }
    }
}
