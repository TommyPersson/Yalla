
using System.Collections.Generic;

namespace Yalla.Parser.AstObjects
{
    public abstract class AstNode
    {
        public static object MakeNode(object o)
        {
            if (o is IList<object>)
            {
                return new ListNode((IList<object>)o);
            }

            if (o == null)
            {
                return new NilNode();
            }

            if (o is AstNode)
            {
                return o as AstNode;
            }

            return o;
        }
    }
}