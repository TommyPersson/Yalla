using NUnit.Framework;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class Try : PrimitiveFunctionTestBase
    {
        [Test]
        public void ShallBeAbleToCatchThrownExceptions()
        {
            // InvalidTimeZoneException should be special enough not to be thrown by parser or evaluator
            const string Program = "(try " +
                                   "  (throw (System.InvalidTimeZoneException. \"Something Broke!\")) " + 
                                   "  (catch (System.InvalidTimeZoneException e) " + 
                                   "    (.Message e)))";

            Assert.AreEqual("Something Broke!", Evaluator.Evaluate(Program));
        }
    }
}
