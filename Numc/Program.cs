using Numc.lexer;
using Numc.SyntacticalSugar.PostLexer.Implementations;
using Numc.SyntacticalSugar.Prelexer.Implementations;
using System;
using System.Collections.Generic;

namespace Numc
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = Reader.Reader.readLinesRaw("../../Testprogram.txt");
            SemicolonAdder sadder = new SemicolonAdder();
            WhitespaceAdder wadder = new WhitespaceAdder();
            Lexer lexer = new Lexer();
            lines = sadder.removeSugar(lines);
            lines = wadder.removeSugar(lines);
            List<Token> tokens = lexer.tokenize(ref lines);
            WithRemover withRemover = new WithRemover();
            tokens = withRemover.removeSugar(tokens);
            
            printTokens(tokens);
            Console.ReadLine();
        }

        static void printTokens(List<Token> tokens) 
        {
            int lastLine = 0;
            int tabLevel = 0;
            foreach(Token t in tokens)
            {
                if (t.content == "}")
                {
                    tabLevel--;
                }
                if (t.line != lastLine)
                {
                    lastLine = t.line;
                    Console.WriteLine();
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
               
                Console.Write(t.content + " ");
                if(t.content == "{")
                {
                    tabLevel++;
                }
            }
        }
    }
}
