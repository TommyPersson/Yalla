
using System;
using System.Collections.Generic;
using System.Linq;
using Yalla.Parser.AstObjects;

namespace Yalla
{
    public class PrettyPrinter
    {
        private static readonly IDictionary<Type, Action<AstNode>> NodeTypeDispatch =
            new Dictionary<Type, Action<AstNode>>
                {
                    { typeof(DecimalNode), x => PrettyPrint((DecimalNode)x) },
                    { typeof(IntegerNode), x => PrettyPrint((IntegerNode)x) },
                    { typeof(QuoteNode), x => PrettyPrint((QuoteNode)x) },
                    { typeof(BackquoteNode), x => PrettyPrint((BackquoteNode)x) },
                    { typeof(UnquoteNode), x => PrettyPrint((UnquoteNode)x) },
                    { typeof(SpliceNode), x => PrettyPrint((SpliceNode)x) },
                    { typeof(StringNode), x => PrettyPrint((StringNode)x) },
                    { typeof(SymbolNode), x => PrettyPrint((SymbolNode)x) },
                    { typeof(ListNode), x => PrettyPrint((ListNode)x) },
                    { typeof(BooleanNode), x => PrettyPrint((BooleanNode)x) },
                    { typeof(ObjectNode), x => PrettyPrint((ObjectNode)x) },
                };

        public static void PrettyPrint(AstNode node)
        {
            NodeTypeDispatch[node.GetType()].Invoke(node);
        }

        public static void PrettyPrint(DecimalNode node)
        {
            Console.Write(node.Value);
        }

        public static void PrettyPrint(IntegerNode node)
        {
            Console.Write(node.Value);
        }

        public static void PrettyPrint(QuoteNode node)
        {
            Console.Write("'");
            PrettyPrint(node.InnerValue);
        }

        public static void PrettyPrint(BackquoteNode node)
        {
            Console.Write("`");
            PrettyPrint(node.InnerValue);
        }

        public static void PrettyPrint(UnquoteNode node)
        {
            Console.Write("~");
            PrettyPrint(node.InnerValue);
        }

        public static void PrettyPrint(SpliceNode node)
        {
            Console.Write("~@");
            PrettyPrint(node.InnerValue);
        }

        public static void PrettyPrint(StringNode node)
        {
            Console.Write("\"" + node.Value + "\"");
        }

        public static void PrettyPrint(SymbolNode node)
        {
            Console.Write(node.Name);
        }

        public static void PrettyPrint(BooleanNode node)
        {
            Console.Write(node.Value.ToString());
        }

        public static void PrettyPrint(ObjectNode node)
        {
            Console.Write(node.Object.ToString());
        }

        public static void PrettyPrint(ListNode node)
        {
            Console.Out.Write("(");

            for (int i = 0; i < node.Children().Count; i++)
            {
                var printnode = node.Children().ElementAt(i);
                PrettyPrint(printnode);
                
                if (i < node.Children().Count - 1)
                {
                    Console.Write(" ");
                }
            }

            Console.Out.Write(")");
        }
    }
}
