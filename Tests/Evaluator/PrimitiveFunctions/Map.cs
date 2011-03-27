using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Map : PrimitiveFunctionTestBase
    {
        [Test]
        public void MapShallMapLambdaAcrossList()
        {
            const string Program = "(map (lambda (x) (+ x 1)) '(1 2 3))";

            var result = (IList<object>)Evaluator.Evaluate(Program);

            Assert.AreEqual(2, result.ElementAt(0));
            Assert.AreEqual(3, result.ElementAt(1));
            Assert.AreEqual(4, result.ElementAt(2));
        }
    }
}
