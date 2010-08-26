using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Evaluator
    {
        private readonly IDictionary<SymbolNode, AstNode> globalEnvironment = new Dictionary<SymbolNode, AstNode>();

        public Evaluator(Parser.Parser parser, IDictionary<SymbolNode, AstNode> environmentExtensions)
        {
            InitializeGlobalEnvironment(environmentExtensions);
        }

        public void InitializeGlobalEnvironment(IDictionary<SymbolNode, AstNode> environmentExtensions)
        {
            //// TODO: Add primitive functions to global env

            if (environmentExtensions != null)
            {
                foreach (var environmentExtension in environmentExtensions)
                {
                    globalEnvironment.Add(environmentExtension);
                }
            }
        }

        public AstNode Evaluate(string input)
        {
            return null;
        }

        public AstNode Evaluate(AstNode node)
        {
            return null;
        }
    }
}
