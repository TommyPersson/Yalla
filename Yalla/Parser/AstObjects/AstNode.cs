
namespace Yalla.Parser.AstObjects
{
    public abstract class AstNode
    {
        public static AstNode MakeNode(object o)
        {
            if (o is int)
            {
                return new IntegerNode((int)o);
            }

            if (o is double)
            {
                return new DoubleNode((double)o);
            }

            if (o is bool)
            {
                return BooleanNode.MakeBoolean((bool)o);
            }

            if (o is string)
            {
                return new StringNode((string)o);
            }

            return new ObjectNode(o);
        }
    }
}