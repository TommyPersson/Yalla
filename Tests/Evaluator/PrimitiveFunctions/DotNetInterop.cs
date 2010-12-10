
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Tests.Evaluator.PrimitiveFunctions
{
    [TestFixture]
    internal class DotNetInterop : PrimitiveFunctionTestBase
    {
        [Test]
        public void NativeMethodsAreSymbolsThatBeginWithADot()
        {
            var result = Evaluator.Evaluate("(.ToUpper \"Hello World!\")") as StringNode;

            Assert.IsNotNull(result);
            Assert.AreEqual("HELLO WORLD!", result.Value);
        }

        [Test]
        public void NativeMethodsShouldBeAbleToHaveArguments()
        {
            var result = Evaluator.Evaluate("(.Substring \"Hello World!\" 6)") as StringNode;

            Assert.IsNotNull(result);
            Assert.AreEqual("World!", result.Value);
        }

        [Test]
        public void ShallBeAbleToConstructNativeObjects()
        {
            var result = Evaluator.Evaluate("(System.String. (.ToCharArray \"Hello World!\"))") as StringNode;

            Assert.IsNotNull(result);
            Assert.AreEqual("Hello World!", result.Value);
        }
    }
}
