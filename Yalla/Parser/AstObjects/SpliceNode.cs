
namespace Yalla.Parser.AstObjects
{
    public class SpliceNode : AstNode
    {
        public SpliceNode(AstNode value)
        {
            InnerValue = value;
        }

        public AstNode InnerValue { get; private set; }
    }
}
