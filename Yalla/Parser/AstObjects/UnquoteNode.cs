
namespace Yalla.Parser.AstObjects
{
    public class UnquoteNode : AstNode
    {
        public UnquoteNode(object value)
        {
            InnerValue = value;
        }

        public object InnerValue { get; private set; }
    }
}
