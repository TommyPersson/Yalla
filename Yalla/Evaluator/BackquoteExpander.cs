using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class BackquoteExpander
    {
        private readonly Evaluator evaluator;

        private readonly IDictionary<Type, Func<BackquoteExpander, AstNode, int, AstNode>> expandFunctions =
            new Dictionary<Type, Func<BackquoteExpander, AstNode, int, AstNode>>
                {
                    { typeof(BackquoteNode), (x, y, z) => x.ExpandBackquote((BackquoteNode)y, z) },
                    { typeof(SpliceNode), (x, y, z) => x.ExpandSplice((SpliceNode)y, z) },
                    { typeof(UnquoteNode), (x, y, z) => x.ExpandUnquote((UnquoteNode)y, z) },
                    { typeof(ListNode), (x, y, z) => x.ExpandListNode((ListNode)y, z) },
                    { typeof(QuoteNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(IntegerNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(StringNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(DecimalNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(ObjectNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(BooleanNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                    { typeof(SymbolNode), (x, y, z) => x.ExpandToQuoted(y, z) },
                };


        public BackquoteExpander(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public AstNode Expand(AstNode node, int backquoteDepth)
        {
            return expandFunctions[node.GetType()].Invoke(this, node, backquoteDepth);
        }

        public AstNode ExpandToQuoted(AstNode node, int backquoteDepth)
        {
            return node;
        }

        public AstNode ExpandListNode(ListNode node, int backquoteDepth)
        {
            var result = new ListNode();

            foreach (var child in node.Children())
            {
                if (child.GetType() == typeof(SpliceNode))
                {
                    var spliceList = Expand(((SpliceNode)child).InnerValue, backquoteDepth - 1) as ListNode;

                    if (spliceList == null)
                    {
                        throw new ArgumentException("Cannot splice non-list!");
                    }

                    foreach (var item in spliceList.Children())
                    {
                        result.AddChild(item);
                    }
                }
                else if (child.GetType() == typeof(ListNode) ||
                    child.GetType() == typeof(UnquoteNode))
                {
                    result.AddChild(Expand(child, backquoteDepth));        
                }
                else
                {
                    result.AddChild(child);
                }
            }

            return result;
        }

        public AstNode ExpandSplice(SpliceNode node, int backquoteDepth)
        {
            throw new ArgumentException("Cannot splice outside of list!");
        }

        public AstNode ExpandBackquote(BackquoteNode node, int backquoteDepth)
        {
            return Expand(node.InnerValue, backquoteDepth + 1);
        }

        public AstNode ExpandUnquote(UnquoteNode node, int backquoteDepth)
        {
            if (backquoteDepth < 1)
            {
                throw new ArgumentException("Unquote appeared outside of backquote!");    
            }

            return evaluator.Evaluate(Expand(node.InnerValue, backquoteDepth - 1));
        }
    }
}
