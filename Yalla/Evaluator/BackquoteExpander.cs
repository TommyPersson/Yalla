
using System;
using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class BackquoteExpander
    {
        private readonly Evaluator evaluator;

        private readonly IDictionary<Type, Func<BackquoteExpander, AstNode, int, Environment, AstNode>> expandFunctions =
            new Dictionary<Type, Func<BackquoteExpander, AstNode, int, Environment, AstNode>>
                {
                    { typeof(BackquoteNode), (x, y, z, t) => x.ExpandBackquote((BackquoteNode)y, z, t) },
                    { typeof(SpliceNode), (x, y, z, t) => x.ExpandSplice((SpliceNode)y, z) },
                    { typeof(UnquoteNode), (x, y, z, t) => x.ExpandUnquote((UnquoteNode)y, z, t) },
                    { typeof(ListNode), (x, y, z, t) => x.ExpandListNode((ListNode)y, z, t) },
                    { typeof(QuoteNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(IntegerNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(StringNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(DecimalNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(ObjectNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(BooleanNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(SymbolNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                };

        public BackquoteExpander(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public AstNode Expand(AstNode node, int backquoteDepth, Environment environment)
        {
            return expandFunctions[node.GetType()].Invoke(this, node, backquoteDepth, environment);
        }

        public AstNode ExpandToQuoted(AstNode node, int backquoteDepth)
        {
            return node;
        }

        public AstNode ExpandListNode(ListNode node, int backquoteDepth, Environment environment)
        {
            var result = new ListNode();

            foreach (var child in node.Children())
            {
                if (child.GetType() == typeof(SpliceNode))
                {
                    var spliceList = evaluator.Evaluate(Expand(((SpliceNode)child).InnerValue, backquoteDepth - 1, environment), environment) as ListNode;
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
                    result.AddChild(Expand(child, backquoteDepth, environment));        
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

        public AstNode ExpandBackquote(BackquoteNode node, int backquoteDepth, Environment environment)
        {
            return new QuoteNode(Expand(node.InnerValue, backquoteDepth + 1, environment));
        }

        public AstNode ExpandUnquote(UnquoteNode node, int backquoteDepth, Environment environment)
        {
            if (backquoteDepth < 1)
            {
                throw new ArgumentException("Unquote appeared outside of backquote!");    
            }

            return evaluator.Evaluate(Expand(node.InnerValue, backquoteDepth - 1, environment), environment);
        }
    }
}
