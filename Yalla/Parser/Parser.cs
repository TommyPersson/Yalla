
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

        public Parser(Tokenizer.Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public IList<AstNode> Parse(string input)
        {
            return Parse(tokenizer.Tokenize(input));
        }

        public IList<AstNode> Parse(IEnumerable<Token> tokens)
        {
            Stack<ListNode> lists = new Stack<ListNode>();

            var result = new ListNode();
            lists.Push(result);

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case Token.TokenType.LParen:
                        lists.Push(new ListNode());
                        break;

                    case Token.TokenType.RParen:
                        if (lists.Count == 1)
                        {
                            throw new Exception("Extra parenthesis detected at " + token.Row + ":" + token.Column);
                        }

                        var popped = lists.Pop();
                        lists.Peek().AddChild(popped);
                        break;

                    case Token.TokenType.Quote:
                        lists.Peek().ShallQuoteNextValue = true;
                        break;

                    case Token.TokenType.EndOfFile:
                        if (lists.Count > 1)
                        {
                            throw new Exception("Unexpected EOF at " + token.Row + ":" + token.Column + ". Unbalanced parenthesis.");
                        }

                        break;

                    default:
                        lists.Peek().AddChild(Parse(token));
                        break;
                }
            }

            return result.Children();
        }

        public AstNode Parse(Token token)
        {
            switch (token.Type)
            {
                case Token.TokenType.Integer:
                    return new IntegerNode(int.Parse(token.Value));
                case Token.TokenType.Double:
                    return new DoubleNode(float.Parse(token.Value, CultureInfo.InvariantCulture));
                case Token.TokenType.String:
                    return new StringNode(token.Value);
                case Token.TokenType.Symbol:
                    return new SymbolNode(token.Value);
            }

            return null;
        }
    }
}
