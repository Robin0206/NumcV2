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
            ForToWhileConverter forToWhileConverter = new ForToWhileConverter();
            DoToForConverter doToForConverter = new DoToForConverter();
            WhileToIfConverter whileRemover = new WhileToIfConverter();
            IfRemover ifRemover = new IfRemover();
            ReturnConverter returnConverter = new ReturnConverter();
            ExpressionSimplifier expressionSimplifier = new ExpressionSimplifier();
            ArgumentRemover argumentRemover = new ArgumentRemover();
            RefaUpPuller refaUpPuller = new RefaUpPuller();
            ElseToIfConverter elseToIfConverter = new ElseToIfConverter();
            tokens = withRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = doToForConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = forToWhileConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = whileRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = elseToIfConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = ifRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = argumentRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = returnConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = expressionSimplifier.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            tokens = refaUpPuller.removeSugar(tokens);
            /*for(int i = 0; i < tokens.Count; i++)
            {
                tokens[i].print();
                Console.WriteLine("Index: " + i);
            }*/
            
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------");
            Console.ReadLine();
            
        }

        static void printTokens(List<Token> tokens) 
        {
            int tabLevel = 0;
            foreach(Token token in tokens)
            {
                if(token.content == "{")
                {
                    tabLevel++;
                    Console.Write(token.content);
                    Console.WriteLine();
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if(token.content == "}")
                {
                    tabLevel--;
                    Console.WriteLine();
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                    Console.Write(token.content);
                    Console.WriteLine();
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if(token.content == ";")
                {
                    Console.Write(token.content);
                    Console.WriteLine();
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else
                {
                    Console.Write(token.content + " ");
                }
            }
        }
    }
}
