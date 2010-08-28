
namespace Yalla.Parser.AstObjects
{
    public class UnquoteNode : AstNode
    {
        public UnquoteNode(AstNode value)
        {
            InnerValue = value;
        }

        public AstNode InnerValue { get; private set; }
    }
}
