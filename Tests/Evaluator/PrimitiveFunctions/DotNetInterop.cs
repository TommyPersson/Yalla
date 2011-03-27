using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    public class DotNetInterop : PrimitiveFunctionTestBase
    {
        [Test]
        public void NativeMethodsAreSymbolsThatBeginWithADot()
        {
            var result = Evaluator.Evaluate("(.ToUpper \"Hello World!\")") as string;

            Assert.IsNotNull(result);
            Assert.AreEqual("HELLO WORLD!", result);
        }

        [Test]
        public void NativeMethodsShouldBeAbleToHaveArguments()
        {
            var result = Evaluator.Evaluate("(.Substring \"Hello World!\" 6)") as string;

            Assert.IsNotNull(result);
            Assert.AreEqual("World!", result);
        }

        [Test]
        public void ShallBeAbleToConstructNativeObjects()
        {
            var result = Evaluator.Evaluate("(System.String. (.ToCharArray \"Hello World!\"))") as string;

            Assert.IsNotNull(result);
            Assert.AreEqual("Hello World!", result);
        }

        [Test]
        public void ShallBeAbleToCallStaticMethods()
        {
            var result = (bool)Evaluator.Evaluate("(System.String/IsNullOrEmpty \"\")");

            Assert.IsTrue(result);
        }
    }
}
