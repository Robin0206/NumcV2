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
            InlineOperationRemover inlineOperationRemover = new InlineOperationRemover();
            FunctionCallRemover functionCallConverter = new FunctionCallRemover();
            Console.WriteLine("---------------------------------------------------remove function calls");
            Console.WriteLine();
            tokens = functionCallConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------remove with");
            Console.WriteLine();
            tokens = withRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------convert do to for");
            Console.WriteLine();
            tokens = doToForConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------convert for to while");
            Console.WriteLine();
            tokens = forToWhileConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------convert while to if");
            Console.WriteLine();
            tokens = whileRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------convert else to if");
            Console.WriteLine();
            tokens = elseToIfConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------convert if to branches");
            Console.WriteLine();
            tokens = ifRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------remove arguments");
            Console.WriteLine();
            tokens = argumentRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------add brackets to return");
            Console.WriteLine();
            tokens = returnConverter.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------remove inline operations");
            Console.WriteLine();
            tokens = inlineOperationRemover.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------simplify expressions");
            Console.WriteLine();
            tokens = expressionSimplifier.removeSugar(tokens);
            printTokens(tokens);
            Console.WriteLine("---------------------------------------------------pull refas up");
            Console.WriteLine();
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
            int line = 0;
            int tabLevel = 0;
            Console.Write(line + " ");
            line++;
            foreach (Token token in tokens)
            {
                if(token.content == "{")
                {
                    tabLevel++;
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if(token.content == "}")
                {
                    tabLevel--;
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if(token.content == ";")
                {
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
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
