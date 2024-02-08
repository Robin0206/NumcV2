using Numc.lexer;
using Numc.SyntacticalSugar;
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
            SyntacticalSugarCompiler compiler = new SyntacticalSugarCompiler();
            string[] compiled = compiler.compileProgram(lines);
            foreach(string i in compiled)
            {
                Console.WriteLine(i);
            }
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
