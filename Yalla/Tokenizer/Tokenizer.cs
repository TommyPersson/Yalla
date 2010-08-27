
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Yalla.Tokenizer
{
    public class Tokenizer
    {
        private readonly char[] validSymbolChars = new[] { '+', '-', '%', '#', ':', '@', '!', '¤', '$', '*', '_', '.', '=' };

        private string inputBuffer;
        private int inputPosition;
        private char? lookAhead;

        private int currentColumn;
        private int currentRow;
        
        public IEnumerable<Token> Tokenize(string input)
        {
            Initialize(input);
            var tokens = new List<Token>();

            while (HasMoreTokens())
            {
                var token = GetNextToken();
                if (token != null)
                {
                    tokens.Add(token);                    
                }
            }

            tokens.Add(new Token(Token.TokenType.EndOfFile, "<EOF>", currentColumn, currentRow));

            return tokens;
        }

        private void Initialize(string input)
        {
            inputBuffer = input.Replace('\r', '\n');
            inputPosition = 0;
            lookAhead = inputBuffer.ElementAt(inputPosition);
        }

        private bool HasMoreTokens()
        {
            return lookAhead.HasValue;
        }
        
        private Token GetNextToken()
        {
            while (lookAhead != null)
            {
                switch (lookAhead.Value)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        ConsumeWhitespace();
                        break;
                    case '(':
                        Consume();
                        return new Token(Token.TokenType.LParen, "(", currentColumn, currentRow);
                    case ')':
                        Consume();
                        return new Token(Token.TokenType.RParen, ")", currentColumn, currentRow);
                    case '\'':
                        Consume();
                        return new Token(Token.TokenType.Quote, "'", currentColumn, currentRow);
                    case '"':
                        return ParseString();
                    default:
                        if (char.IsLetter(lookAhead.Value) || validSymbolChars.Contains(lookAhead.Value))
                        {
                            return ParseSymbol();
                        }
                        
                        if (char.IsDigit(lookAhead.Value))
                        {
                            return ParseNumber();
                        }

                        throw new SyntaxErrorException("Invalid input at row " + currentRow + ", column " + currentColumn);
                }
            }

            return null;
        }

        private Token ParseString()
        {
            var tokenBuffer = new StringBuilder();

            int beginColumn = 0;
            int beginRow = 0;

            bool begun = false;
            bool finished = false;

            while (lookAhead.HasValue && !finished)
            {
                if (lookAhead == '"')
                {
                    if (!begun)
                    {
                        beginColumn = currentColumn;
                        beginRow = currentRow;
                        begun = true;
                    }
                    else
                    {
                        finished = true;
                    }
                }
                else
                {
                    tokenBuffer.Append(lookAhead);
                }

                Consume();
            }

            if (finished)
            {
                return new Token(Token.TokenType.String, tokenBuffer.ToString(), beginColumn, beginRow);
            }

            throw new SyntaxErrorException("Unterminated string detected at " + beginRow + ", column " + beginColumn);
        }

        private Token ParseNumber()
        {
            var tokenBuffer = new StringBuilder();

            int beginColumn = currentColumn;
            int beginRow = currentRow;

            bool hasDecimalPoint = false;

            while (lookAhead.HasValue && 
                   (char.IsDigit(lookAhead.Value) || lookAhead.Value == '.'))
            {
                if (lookAhead.Value == '.')
                {
                    if (hasDecimalPoint)
                    {
                        throw new SyntaxErrorException("Invalid number format detected at row " + currentRow + ", column " + currentColumn);
                    }

                    hasDecimalPoint = true;
                }

                tokenBuffer.Append(lookAhead.Value);
                Consume();
            }

            return hasDecimalPoint ? new Token(Token.TokenType.Double, tokenBuffer.ToString(), beginColumn, beginRow) 
                                   : new Token(Token.TokenType.Integer, tokenBuffer.ToString(), beginColumn, beginRow);
        }

        private Token ParseSymbol()
        {
            var tokenBuffer = new StringBuilder();

            int beginColumn = currentColumn;
            int beginRow = currentRow;

            while (lookAhead.HasValue &&
                   (char.IsLetter(lookAhead.Value) || char.IsDigit(lookAhead.Value) || validSymbolChars.Contains(lookAhead.Value)))
            {
                tokenBuffer.Append(lookAhead.Value);
                Consume();
            }

            return new Token(Token.TokenType.Symbol, tokenBuffer.ToString(), beginColumn, beginRow);
        }

        private void ConsumeWhitespace()
        {
            while (lookAhead == ' ' ||
                   lookAhead == '\t' ||
                   lookAhead == '\n' ||
                   lookAhead == '\r')
            {
                Consume();
            }
        }

        private void Consume()
        {
            if (lookAhead == '\n')
            {
                currentRow++;
                currentColumn = 0;
            }
            else
            {
                currentColumn++;
            }

            inputPosition++;

            if (inputPosition >= inputBuffer.Length)
            {
                lookAhead = null;
            }
            else
            {
                lookAhead = inputBuffer.ElementAt(inputPosition);
            }
        }
    }
}
