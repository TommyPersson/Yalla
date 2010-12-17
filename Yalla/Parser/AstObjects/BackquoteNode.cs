
namespace Yalla.Parser.AstObjects
{
    public class BackquoteNode : AstNode
    {
        public BackquoteNode(object value)
        {
            InnerValue = value;
        }

        public object InnerValue { get; private set; }
    }
}
