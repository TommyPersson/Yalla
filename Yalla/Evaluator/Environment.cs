using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Environment : Dictionary<SymbolNode, AstNode>
    {
    }
}
