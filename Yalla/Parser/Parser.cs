
using System;
using System.Collections.Generic;
using System.Globalization;
using Yalla.Parser.AstObjects;
using Yalla.Tokenizer;

namespace Yalla.Parser
{
    public class Parser
    {
        private readonly Tokenizer.Tokenizer tokenizer;

        private Stack<Tuple<IList<object>, Stack<QuoteType>>> lists = new Stack<Tuple<IList<object>, Stack<QuoteType>>>();

        public Parser(Tokenizer.Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        private enum QuoteType
        {
            Quote,
            Backquote,
            Unquote,
            Splice
        }
		
		public object ParseForm(string input)
		{
			return Parse(input)[0];
		}
		
		public IList<object> Parse(string input)
        {
            return Parse(tokenizer.Tokenize(input));
        }

        public IList<object> Parse(IEnumerable<Token> tokens)
        {
            lists = new Stack<Tuple<IList<object>, Stack<QuoteType>>>();
            var result = new Tuple<IList<object>, Stack<QuoteType>>(new List<object>(), new Stack<QuoteType>());
            lists.Push(result);

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case Token.TokenType.LParen:
                        lists.Push(new Tuple<IList<object>, Stack<QuoteType>>(new List<object>(), new Stack<QuoteType>()));
                        break;

                    case Token.TokenType.RParen:
                        if (lists.Count == 1)
                        {
                            throw new Exception("Extra parenthesis detected at " + token.Row + ":" + token.Column);
                        }

                        var popped = lists.Pop();
                        AddNode(popped.Item1);
                        break;

                    case Token.TokenType.Quote:
                        lists.Peek().Item2.Push(QuoteType.Quote);
                        break;

                    case Token.TokenType.Backquote:
                        lists.Peek().Item2.Push(QuoteType.Backquote);
                        break;

                    case Token.TokenType.Unquote:
                        lists.Peek().Item2.Push(QuoteType.Unquote);
                        break;

                    case Token.TokenType.Splice:
                        lists.Peek().Item2.Push(QuoteType.Splice);
                        break;
                        
                    case Token.TokenType.EndOfFile:
                        if (lists.Count > 1)
                        {
                            throw new Exception("Unexpected EOF at " + token.Row + ":" + token.Column + ". Unbalanced parenthesis.");
                        }

                        break;

                    default:
                        AddNode(Parse(token));
                        break;
                }
            }

            return result.Item1;
        }

        public object Parse(Token token)
        {
            switch (token.Type)
            {
				case Token.TokenType.Integer:
                    return int.Parse(token.Value);
                case Token.TokenType.Double:
                    return decimal.Parse(token.Value, CultureInfo.InvariantCulture);
                case Token.TokenType.String:
                    return token.Value;
                case Token.TokenType.Symbol:
                    return new SymbolNode(token.Value);
            }

            return null;
        }

        public void AddNode(object node)
        {
            object value = node;

            while (lists.Peek().Item2.Count > 0)
            {
                switch (lists.Peek().Item2.Pop())
                {
                    case QuoteType.Quote:
                        value = new QuoteNode(value);
                        break;

                    case QuoteType.Backquote:
                        value = new BackquoteNode(value);
                        break;

                    case QuoteType.Unquote:
                        value = new UnquoteNode(value);
                        break;

                    case QuoteType.Splice:
                        value = new SpliceNode(value);
                        break;
                }
            }

            lists.Peek().Item1.Add(value);
        }
    }
}
