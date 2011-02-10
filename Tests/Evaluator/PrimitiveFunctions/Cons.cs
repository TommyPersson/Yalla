
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class Cons : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToConsAnObjectAndLists()
        {
            Assert.AreEqual(2, ((int)((IList<object>)Evaluator.Evaluate("(cons 1 (list 2))")).ElementAt(1)));
        }

        [Test]
        public void SecondArgumentMustBeAList()
        {
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("(cons 1 2)"));
            Assert.Throws(typeof(ArgumentException), () => Evaluator.Evaluate("(cons 1)"));
        }
    }
}
