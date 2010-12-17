
namespace Yalla.Parser.AstObjects
{
    public class SpliceNode : AstNode
    {
        public SpliceNode(object value)
        {
            InnerValue = value;
        }

        public object InnerValue { get; private set; }
    }
}
