using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class DoToForConverter : PostLSSLayer
    {
        static int refaCalls = 0;
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            foreach (Token token in input)
            {
                result.Add(token);
            }

            bool foundDo = true;
            int counter = 0;
            while (foundDo)
            {
                int[] doCoords = findDoCoords(ref result);
                if (doCoords[0] != -1)
                {
                    //add before do
                    resultBuffer.Clear();
                    List<Token> forBody = convertDoBody(
                        getDoBody(doCoords, ref result)
                    );

                    for (int i = 0; i < doCoords[0]; i++)
                    {
                        resultBuffer.Add(result[i]);
                    }

                    //add for
                    resultBuffer.AddRange(forBody);

                    //add after do
                    for (int i = doCoords[1]; i < result.Count; i++)
                    {
                        result[i].setLine(result[i].line + 2);
                        resultBuffer.Add(result[i]);
                    }
                    result.Clear();
                    foreach (Token token in resultBuffer)
                    {
                        result.Add(token);
                    }
                }
                else
                {
                    foundDo = false;
                }
                counter++;
            }

            return result;
        }

        private List<Token> getDoBody(int[] doCoords, ref List<Token> input)
        {
            List<Token> result = new List<Token>();
            for (int i = doCoords[0]; i < doCoords[1]; i++)
            {
                result.Add(input[i]);
            }
            return result;
        }

        private List<Token> convertDoBody(List<Token> currentDoBody)
        {
            //int == 1
            List<Token> result = new List<Token>();
            string doBufferName = "____doBuffer_" + refaCalls;
            result.Add(new Token("refa", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(doBufferName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("1", TokenType.NUMBER, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            result.Add(new Token("for", TokenType.KEYWORD, 0));
            result.Add(new Token(doBufferName, TokenType.NAME, 0));
            result.Add(new Token("from", TokenType.KEYWORD, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token("to", TokenType.KEYWORD, 0));
            result.Add(new Token(currentDoBody[1].content, TokenType.NUMBER, 0));
            bool writing = false;
            foreach(Token t in currentDoBody)
            {
                if(t.content == "{")
                {
                    writing = true;
                }
                if (writing)
                {
                    result.Add(t);
                }
            }
            refaCalls++;
            return result;
        }

        private int[] findDoCoords(ref List<Token> input)
        {
            int[] result = new int[2];

            int braces = 0;
            bool inBody = false;
            bool foundDo = false;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].content == "do" && !foundDo)
                {
                    result[0] = i;
                    foundDo = true;
                }
                if (foundDo)
                {
                    if (!inBody)
                    {
                        if (input[i].type == TokenType.CURLY_BRACE_LEFT)
                        {
                            inBody = true;
                            braces++;
                        }
                    }
                    else
                    {
                        if (input[i].type == TokenType.CURLY_BRACE_LEFT)
                        {
                            braces++;
                        }
                        else if (input[i].type == TokenType.CURLY_BRACE_RIGHT)
                        {
                            braces--;
                        }

                        if (braces == 0)
                        {
                            result[1] = i;
                            return result;
                        }
                    }
                }
            }
            return new int[] { -1, -1 };
        }
    }
}
