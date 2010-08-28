
namespace Yalla.Parser.AstObjects
{
    public class BackquoteNode : AstNode
    {
        public BackquoteNode(AstNode value)
        {
            InnerValue = value;
        }

        public AstNode InnerValue { get; private set; }
    }
}
