using System;
using NUnit.Framework;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Throw : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToThrowExceptions()
        {
            // InvalidTimeZoneException should be special enough not to be thrown by parser or evaluator
            Assert.Throws(typeof(InvalidTimeZoneException), () => Evaluator.Evaluate("(throw (System.InvalidTimeZoneException. \"Something Broke!\"))"));
        }
    }
}
