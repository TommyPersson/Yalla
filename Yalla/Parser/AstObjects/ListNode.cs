
using System.Collections.Generic;
using System.Linq;

namespace Yalla.Parser.AstObjects
{
    public class ListNode : AstNode
    {
        private readonly IList<object> list;
        
        public ListNode()
        {
            list = new List<object>();
        }

        public ListNode(IList<object> list)
        {
            this.list = list;
        }

        public IList<object> Children()
        {
            return list;
        }
        
        public object First()
        {
            return list.First();
        }

        public ListNode Rest()
        {
            return new ListNode(list.Skip(1).ToList());
        }

        public bool IsEmpty()
        {
            return list.Count == 0;
        }

        public ListNode Append(ListNode applist)
        {
            return new ListNode(list.Concat(applist.Children()).ToList());
        }

        public ListNode AddChild(object obj)
        {
            list.Add(obj);
            return this;
        }
    }
}