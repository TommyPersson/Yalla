
namespace Yalla.Parser.AstObjects
{
    public class QuoteNode : AstNode
    {
        public QuoteNode(object value)
        {
            InnerValue = value;
        }

        public object InnerValue { get; private set; }
    }
}
