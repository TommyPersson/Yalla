
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
                    { typeof(DoubleNode), x => PrettyPrint((DoubleNode)x) },
                    { typeof(IntegerNode), x => PrettyPrint((IntegerNode)x) },
                    { typeof(QuoteNode), x => PrettyPrint((QuoteNode)x) },
                    { typeof(StringNode), x => PrettyPrint((StringNode)x) },
                    { typeof(SymbolNode), x => PrettyPrint((SymbolNode)x) },
                    { typeof(ListNode), x => PrettyPrint((ListNode)x) },
                };

        public static void PrettyPrint(AstNode node)
        {
            NodeTypeDispatch[node.GetType()].Invoke(node);
        }

        public static void PrettyPrint(DoubleNode node)
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

        public static void PrettyPrint(StringNode node)
        {
            Console.Write(node.Value);
        }

        public static void PrettyPrint(SymbolNode node)
        {
            Console.Write(node.Name);
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
