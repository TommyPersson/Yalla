
using NUnit.Framework;
using Yalla.Parser.AstObjects;

namespace Yalla.Tests.Evaluator
{
    [TestFixture]
    internal partial class EvaluatorTests
    {
        [Test]
        public void Plus()
        {
            var result = Evaluator.Evaluate("(+ 1 2 (+ 3 4))") as IntegerNode;

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Value);
        }

        [Test]
        public void And()
        {
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(and true true)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(and true false true)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(and)")).Value);
        }

        [Test]
        public void Or()
        {
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(or true false)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(or false false)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(or)")).Value);
        }

        [Test]
        public void Equals()
        {
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= true true)")).Value);
            Assert.IsFalse(((BooleanNode)Evaluator.Evaluate("(= true false)")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= \"string\" \"string\")")).Value);
            Assert.IsTrue(((BooleanNode)Evaluator.Evaluate("(= (+ 1 2) 3)")).Value);
        }

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
    }
}