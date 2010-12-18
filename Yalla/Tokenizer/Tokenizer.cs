
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Yalla.Tokenizer
{
    public class Tokenizer
    {
        private readonly char[] invalidSymbolChars = new[] { '(', ')', ' ', '\n', '\t', '\r' };
        private readonly char?[] lookAhead = new char?[2];

        private string inputBuffer;
        private int inputPosition;

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
            lookAhead[0] = inputBuffer.ElementAt(inputPosition);
            lookAhead[1] = inputPosition + 1 < inputBuffer.Length ? inputBuffer.ElementAt(inputPosition + 1) : (char?)null;
        }

        private bool HasMoreTokens()
        {
            return lookAhead[0].HasValue;
        }
        
        private Token GetNextToken()
        {
            while (lookAhead[0].HasValue)
            {
                switch (lookAhead[0].Value)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                        ConsumeWhitespace();
                        break;
                    case ';':
                        ConsumeComment();
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
                    case '`':
                        Consume();
                        return new Token(Token.TokenType.Backquote, "`", currentColumn, currentRow);
                    case '"':
                        return ParseString();
                    default:
                        if ((lookAhead[0].Value == '-' &&
                             lookAhead[1].HasValue && char.IsDigit(lookAhead[1].Value)) ||
                            char.IsDigit(lookAhead[0].Value))
                        {
                            return ParseNumber();
                        }

                        if (lookAhead[0].Value == '~')
                        {
                            if (lookAhead[1].HasValue && lookAhead[1].Value == '@')
                            {
                                Consume();
                                Consume();
                                return new Token(Token.TokenType.Splice, "~@", currentColumn - 1, currentRow);
                            }

                            Consume();
                            return new Token(Token.TokenType.Unquote, "~", currentColumn, currentRow);
                        }

                        if (!invalidSymbolChars.Contains(lookAhead[0].Value))
                        {
                            return ParseSymbol();
                        }

                        throw new SyntaxErrorException("Invalid input (" + lookAhead[0].Value + ") at row " + currentRow + ", column " + currentColumn);
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

            while (lookAhead[0].HasValue && !finished)
            {
                if (lookAhead[0] == '\\')
                {                        
                    Consume();
                    
                    char ch = lookAhead[0].Value;
                    
                    switch(lookAhead[0])    
                    {
                        case 't':
                            ch = '\t';
                            break;
                        case 'r':
                            ch = '\r';
                            break;
                        case 'n':
                            ch = '\n';
                            break;
                        case '\\':
                            break;
                        case '"':
                            break;
                        case 'b':
                            ch = '\b';
                            break;
                        case 'f':
                            ch = '\f';
                            break;
                    }
                    
                    tokenBuffer.Append(ch);
                } 
                else if (lookAhead[0] == '"')
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
                    tokenBuffer.Append(lookAhead[0]);
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

            tokenBuffer.Append(lookAhead[0].Value);
            Consume();
            
            while (lookAhead[0].HasValue && 
                   (char.IsDigit(lookAhead[0].Value) || lookAhead[0].Value == '.'))
            {
                if (lookAhead[0].Value == '.')
                {
                    if (hasDecimalPoint)
                    {
                        throw new SyntaxErrorException("Invalid number format detected at row " + currentRow + ", column " + currentColumn);
                    }

                    hasDecimalPoint = true;
                }

                tokenBuffer.Append(lookAhead[0].Value);
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

            while (lookAhead[0].HasValue && !invalidSymbolChars.Contains(lookAhead[0].Value))
            {
                tokenBuffer.Append(lookAhead[0].Value);
                Consume();
            }

            return new Token(Token.TokenType.Symbol, tokenBuffer.ToString(), beginColumn, beginRow);
        }

        private void ConsumeWhitespace()
        {
            while (lookAhead[0] == ' ' ||
                   lookAhead[0] == '\t' ||
                   lookAhead[0] == '\n')
            {
                Consume();
            }
        }

        private void ConsumeComment()
        {
            while (lookAhead[0] != '\n')
            {
                Consume();
            }
        }

        private void Consume()
        {
            if (lookAhead[0] == '\n')
            {
                currentRow++;
                currentColumn = 0;
            }
            else
            {
                currentColumn++;
            }

            inputPosition++;

            lookAhead[0] = inputPosition < inputBuffer.Length ? inputBuffer.ElementAt(inputPosition) : (char?) null;
            lookAhead[1] = inputPosition + 1 < inputBuffer.Length ? inputBuffer.ElementAt(inputPosition + 1) : (char?) null;
        }
    }
}
