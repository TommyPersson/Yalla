
namespace Yalla.Parser.AstObjects
{
    public class QuoteNode : AstNode
    {
        public QuoteNode(AstNode value)
        {
            InnerValue = value;
        }

        public AstNode InnerValue { get; private set; }
    }
}
