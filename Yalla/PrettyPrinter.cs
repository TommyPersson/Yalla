
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yalla.Parser.AstObjects;

namespace Yalla
{
    public class PrettyPrinter
    {
        private readonly IDictionary<Type, Action<PrettyPrinter, object>> nodeTypeDispatch =
            new Dictionary<Type, Action<PrettyPrinter, object>>
                {
                    { typeof(decimal), (x, y) => x.PrettyPrintSub((decimal)y) },
                    { typeof(int), (x, y) => x.PrettyPrintSub((int)y) },
                    { typeof(QuoteNode), (x, y) => x.PrettyPrintSub((QuoteNode)y) },
                    { typeof(BackquoteNode), (x, y) => x.PrettyPrintSub((BackquoteNode)y) },
                    { typeof(UnquoteNode), (x, y) => x.PrettyPrintSub((UnquoteNode)y) },
                    { typeof(SpliceNode), (x, y) => x.PrettyPrintSub((SpliceNode)y) },
                    { typeof(string), (x, y) => x.PrettyPrintSub((string)y) },
                    { typeof(SymbolNode), (x, y) => x.PrettyPrintSub((SymbolNode)y) },
                    { typeof(ListNode), (x, y) => x.PrettyPrintSub((ListNode)y) },
                    { typeof(bool), (x, y) => x.PrettyPrintSub((bool)y) },
                    { typeof(object), (x, y) => x.PrettyPrintSub((object)y) },
                    { typeof(NilNode), (x, y) => x.PrettyPrintSub((NilNode)y) },
                    { typeof(FunctionNode), (x, y) => x.PrettyPrintSub((FunctionNode)y) },
                    { typeof(ProcedureNode), (x, y) => x.PrettyPrintSub((ProcedureNode)y) },
                };

        private StringWriter stringWriter; 

        public string PrettyPrint(object node)
        {
            stringWriter = new StringWriter();

            PrettyPrintSub(node);

            return stringWriter.ToString();
        }

        private void PrettyPrintSub(object node)
        {
			if (node != null)
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
        }

        private void PrettyPrintSub(decimal node)
        {
            stringWriter.Write(node);
        }

        private void PrettyPrintSub(int node)
        {
            stringWriter.Write(node);
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

        private void PrettyPrintSub(string node)
        {
            stringWriter.Write("\"" + node + "\"");
        }

        private void PrettyPrintSub(SymbolNode node)
        {
            stringWriter.Write(node.Name);
        }

        private void PrettyPrintSub(bool node)
        {
            stringWriter.Write(node.ToString());
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
