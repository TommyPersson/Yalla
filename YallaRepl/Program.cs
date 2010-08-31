
using System;
using Yalla;
using Yalla.Evaluator;
using Yalla.Parser;
using Yalla.Tokenizer;

namespace YallaRepl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var evaluator = new Evaluator(new Parser(new Tokenizer()));
            var prettyPrinter = new PrettyPrinter();
            
            Console.Out.WriteLine("End you statements with <Enter> <Ctrl-Z> <Enter>. Quit with <Ctrl-C>.");

            while (true)
            {
                Console.Out.WriteLine();
                Console.Out.Write("yalla> ");

                var input = Console.In.ReadToEnd();
                
                Console.Out.Write("=> ");
                try
                {
                    var result = prettyPrinter.PrettyPrint(evaluator.Evaluate(input));
                    Console.Out.Write(result);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Unhandled exception: " + ex.GetType().ToString());
                    Console.Out.WriteLine("Content:");
                    Console.Out.WriteLine(ex.Message);
                }

                Console.Out.WriteLine();         
            }
        }
    }
}
