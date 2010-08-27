
using System;

namespace Yalla
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var evaluator = new Evaluator.Evaluator(new Parser.Parser(new Tokenizer.Tokenizer()));
         
            Console.Out.WriteLine("End you statements with <Enter> <Ctrl-Z> <Enter>. Quit with <Ctrl-C>.");

            while (true)
            {
                Console.Out.WriteLine();
                Console.Out.Write("yalla> ");

                var input = Console.In.ReadToEnd();

                Console.Out.Write("=> ");
                PrettyPrinter.PrettyPrint(evaluator.Evaluate(input));
                Console.Out.WriteLine();         
            }
        }
    }
}
