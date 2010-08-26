
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yalla
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tokenizer = new Tokenizer.Tokenizer();

            var parser = new Parser.Parser(tokenizer);
            var result = parser.Parse("(123.123 '(\"Hello World!\" 1 2 3 asdf (.method object)))");

            PrettyPrinter.PrettyPrint(result);

            Console.ReadKey();
        }
    }
}
