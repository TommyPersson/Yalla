
using System;
using System.Collections.Generic;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class BackquoteExpander
    {
        private readonly Evaluator evaluator;

        private readonly IDictionary<Type, Func<BackquoteExpander, object, int, Environment, object>> expandFunctions =
            new Dictionary<Type, Func<BackquoteExpander, object, int, Environment, object>>
                {
                    { typeof(BackquoteNode), (x, y, z, t) => x.ExpandBackquote((BackquoteNode)y, z, t) },
                    { typeof(SpliceNode), (x, y, z, t) => x.ExpandSplice((SpliceNode)y, z) },
                    { typeof(UnquoteNode), (x, y, z, t) => x.ExpandUnquote((UnquoteNode)y, z, t) },
                    { typeof(IList<object>), (x, y, z, t) => x.ExpandList((IList<object>)y, z, t) },
                    { typeof(List<object>), (x, y, z, t) => x.ExpandList((IList<object>)y, z, t) },
                    { typeof(QuoteNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(int), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(string), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(decimal), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(object), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(bool), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                    { typeof(SymbolNode), (x, y, z, t) => x.ExpandToQuoted(y, z) },
                };

        public BackquoteExpander(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public object Expand(object node, int backquoteDepth, Environment environment)
        {
            return expandFunctions[node.GetType()].Invoke(this, node, backquoteDepth, environment);
        }

        public object ExpandToQuoted(object node, int backquoteDepth)
        {
            return node;
        }

        public object ExpandList(IList<object> node, int backquoteDepth, Environment environment)
        {
            var result = new List<object>();

            foreach (var child in node)
            {
                var t = child.GetType();

                if (child.GetType() == typeof(SpliceNode))
                {
                    var spliceList = evaluator.Evaluate(Expand(((SpliceNode)child).InnerValue, backquoteDepth - 1, environment), environment) as IList<object>;
                    if (spliceList == null)
                    {
                        throw new ArgumentException("Cannot splice non-list!");
                    }

                    result.AddRange(spliceList);
                }
                else if (child.GetType() == typeof(List<object>) || child.GetType() == typeof(UnquoteNode))
                {
                    result.Add(Expand(child, backquoteDepth, environment));        
                }
                else
                {
                    result.Add(child);
                }
            }

            return result;
        }

        public object ExpandSplice(SpliceNode node, int backquoteDepth)
        {
            throw new ArgumentException("Cannot splice outside of list!");
        }

        public object ExpandBackquote(BackquoteNode node, int backquoteDepth, Environment environment)
        {
            return new QuoteNode(Expand(node.InnerValue, backquoteDepth + 1, environment));
        }

        public object ExpandUnquote(UnquoteNode node, int backquoteDepth, Environment environment)
        {
            if (backquoteDepth < 1)
            {
                throw new ArgumentException("Unquote appeared outside of backquote!");    
            }

            return evaluator.Evaluate(Expand(node.InnerValue, backquoteDepth - 1, environment), environment);
        }
    }
}
