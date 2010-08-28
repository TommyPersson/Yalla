
using System.Collections.Generic;
using System.Linq;

namespace Yalla.Parser.AstObjects
{
    public class ListNode : AstNode
    {
        private readonly IList<AstNode> list;
        
        public ListNode()
        {
            list = new List<AstNode>();
        }

        public ListNode(IList<AstNode> list)
        {
            this.list = list;
        }

        public bool ShallQuoteNextValue { get; set; }
        
        public IList<AstNode> Children()
        {
            return list;
        }
        
        public AstNode First()
        {
            return list.First();
        }

        public ListNode Rest()
        {
            return new ListNode(list.Skip(1).ToList());
        }

        public ListNode Append(ListNode applist)
        {
            return new ListNode(list.Concat(applist.Children()).ToList());
        }

        public ListNode AddChild(AstNode yobject)
        {
            var objectToAdd = yobject;

            if (ShallQuoteNextValue)
            {
                objectToAdd = new QuoteNode(objectToAdd);
                ShallQuoteNextValue = false;
            }

            list.Add(objectToAdd);
            return this;
        }
    }
}