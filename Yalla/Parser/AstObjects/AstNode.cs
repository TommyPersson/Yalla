
using System.Collections.Generic;

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

            if (o is decimal)
            {
                return new DecimalNode((decimal)o);
            }

            if (o is bool)
            {
                return new BooleanNode((bool)o);
            }

            if (o is string)
            {
                return new StringNode((string)o);
            }

            if (o is IList<AstNode>)
            {
                return new ListNode((IList<AstNode>)o);
            }

            if (o == null)
            {
                return new NilNode();
            }

            return new ObjectNode(o);
        }
    }
}