using NUnit.Framework;

namespace Tests.Evaluator.Language
{
    [TestFixture]
    public class Io : LanguageTestBase
    {
        [Test]
        public void PrintlnShallWriteToStdOut()
        {
            const string Program = "(println \"Hello, World!\")";

            Evaluator.Evaluate(Program);

            Assert.AreEqual("Hello, World!" + System.Environment.NewLine, StdOut.ToString());
        }
    }
}
