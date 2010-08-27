using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Environment : Dictionary<SymbolNode, AstNode>
    {
    }
}
