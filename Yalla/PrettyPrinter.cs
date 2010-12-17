
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yalla.Parser.AstObjects;

namespace Yalla
{
    public class PrettyPrinter
    {
        private readonly IDictionary<Type, Action<PrettyPrinter, AstNode>> nodeTypeDispatch =
            new Dictionary<Type, Action<PrettyPrinter, AstNode>>
                {
                    { typeof(DecimalNode), (x, y) => x.PrettyPrintSub((DecimalNode)y) },
                    { typeof(IntegerNode), (x, y) => x.PrettyPrintSub((IntegerNode)y) },
                    { typeof(QuoteNode), (x, y) => x.PrettyPrintSub((QuoteNode)y) },
                    { typeof(BackquoteNode), (x, y) => x.PrettyPrintSub((BackquoteNode)y) },
                    { typeof(UnquoteNode), (x, y) => x.PrettyPrintSub((UnquoteNode)y) },
                    { typeof(SpliceNode), (x, y) => x.PrettyPrintSub((SpliceNode)y) },
                    { typeof(StringNode), (x, y) => x.PrettyPrintSub((StringNode)y) },
                    { typeof(SymbolNode), (x, y) => x.PrettyPrintSub((SymbolNode)y) },
                    { typeof(ListNode), (x, y) => x.PrettyPrintSub((ListNode)y) },
                    { typeof(BooleanNode), (x, y) => x.PrettyPrintSub((BooleanNode)y) },
                    { typeof(ObjectNode), (x, y) => x.PrettyPrintSub((ObjectNode)y) },
                    { typeof(NilNode), (x, y) => x.PrettyPrintSub((NilNode)y) },
                    { typeof(FunctionNode), (x, y) => x.PrettyPrintSub((FunctionNode)y) },
                    { typeof(ProcedureNode), (x, y) => x.PrettyPrintSub((ProcedureNode)y) },
                };

        private StringWriter stringWriter; 

        public string PrettyPrint(AstNode node)
        {
            stringWriter = new StringWriter();

            PrettyPrintSub(node);

            return stringWriter.ToString();
        }

        private void PrettyPrintSub(AstNode node)
        {
			if (nodeTypeDispatch.ContainsKey(node.GetType()))
			{
            	nodeTypeDispatch[node.GetType()].Invoke(this, node);
			}
			else
			{
				stringWriter.Write("<" + node.GetType() + ": " + node + ">");
			}
			
        }

        private void PrettyPrintSub(DecimalNode node)
        {
            stringWriter.Write(node.Value);
        }

        private void PrettyPrintSub(IntegerNode node)
        {
            stringWriter.Write(node.Value);
        }

        private void PrettyPrintSub(QuoteNode node)
        {
            stringWriter.Write("'");
            PrettyPrint(node.InnerValue);
        }

        private void PrettyPrintSub(BackquoteNode node)
        {
            stringWriter.Write("`");
            PrettyPrint(node.InnerValue);
        }

        private void PrettyPrintSub(UnquoteNode node)
        {
            stringWriter.Write("~");
            PrettyPrint(node.InnerValue);
        }

        private void PrettyPrintSub(SpliceNode node)
        {
            stringWriter.Write("~@");
            PrettyPrint(node.InnerValue);
        }

        private void PrettyPrintSub(StringNode node)
        {
            stringWriter.Write("\"" + node.Value + "\"");
        }

        private void PrettyPrintSub(SymbolNode node)
        {
            stringWriter.Write(node.Name);
        }

        private void PrettyPrintSub(BooleanNode node)
        {
            stringWriter.Write(node.Value.ToString());
        }

        private void PrettyPrintSub(ObjectNode node)
        {
            stringWriter.Write(node.Object.ToString());
        }

        private void PrettyPrintSub(NilNode node)
        {
            stringWriter.Write("nil");
        }

        private void PrettyPrintSub(FunctionNode node)
        {
            stringWriter.Write("<primitive-function: " + node.Symbol + ">");
        }

        private void PrettyPrintSub(ProcedureNode node)
        {
            stringWriter.Write("<procedure: " + node.Symbol + ">");
        }

        private void PrettyPrintSub(ListNode node)
        {
            stringWriter.Write("(");

            for (int i = 0; i < node.Children().Count; i++)
            {
                var printnode = node.Children().ElementAt(i);
                PrettyPrintSub(printnode);
                
                if (i < node.Children().Count - 1)
                {
                    stringWriter.Write(" ");
                }
            }

            stringWriter.Write(")");
        }
    }
}
