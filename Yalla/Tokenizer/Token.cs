
namespace Yalla.Tokenizer
{
    public class Token
    {
        public Token(TokenType type, string value, int column, int row)
        {
            Type = type;
            Value = value;
            Column = column;
            Row = row;
        }

        public enum TokenType
        {
            LParen,
            RParen,
            Symbol,
            String,
            Integer,
            Double,
            EndOfFile,
            Quote
        }

        public TokenType Type { get; set; }

        public string Value { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }
    }
}
