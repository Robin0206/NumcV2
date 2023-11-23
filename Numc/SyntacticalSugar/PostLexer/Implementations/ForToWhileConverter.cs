using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class ForToWhileConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            foreach(Token token in input)
            {
                result.Add(token);
            }
            int counter = 0;
            bool convertedForLoop = true;
            while (convertedForLoop)
            {
                int[] forCoords = findForCoords(ref result);
                if(forCoords[0] != -1)
                {
                    //add before for
                    resultBuffer.Clear();
                    List<Token> whileBody = convertForBody(
                        getForBody(forCoords, ref result)
                    );

                    for(int i = 0; i < forCoords[0]; i++)
                    {
                        resultBuffer.Add(result[i]);
                    }

                    //add while
                    resultBuffer.AddRange(whileBody);

                    //add after while
                    for(int i = forCoords[1]+1; i < result.Count; i++)
                    {
                        result[i].setLine(result[i].line + 2);
                        resultBuffer.Add(result[i]);
                    }
                    result.Clear();
                    foreach(Token token in resultBuffer)
                    {
                        result.Add(token);
                    }
                }
                else
                {
                    convertedForLoop = false;
                }
                counter++;
            }
            return result;
            
        }

        private List<Token> convertForBody(List<Token> currentForBody)
        {
            Token varName = currentForBody[1];
            Token startValue = currentForBody[3];
            Token endValue = currentForBody[5];

            List<Token> result = new List<Token>();

            //setting the var
            result.Add(varName);
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, varName.line));
            result.Add(startValue);
            result.Add(new Token(";", TokenType.SEMICOLON, varName.line));
            //while start
            result.Add(new Token("while", TokenType.KEYWORD, currentForBody[1].line + 1));
            result.Add(new Token(varName.content, TokenType.NAME, currentForBody[1].line + 1));
            result.Add(new Token("<", TokenType.OPERATOR_LESS, currentForBody[1].line + 1));
            result.Add(new Token(endValue.content, endValue.type, currentForBody[1].line + 1));
            result.Add(new Token("{", TokenType.CURLY_BRACE_LEFT, currentForBody[1].line + 1));
            //while body
            bool write = false;
            //currentForBody.RemoveAt(currentForBody.Count - 1);
            foreach(Token token in currentForBody)
            {
                if (!write)
                {
                    if(token.type == TokenType.CURLY_BRACE_LEFT)
                    {
                        write = true;
                    }
                }
                else
                {
                    result.Add(token);
                    result[result.Count - 1].setLine(result[result.Count - 1].line+1);
                }
            }
            //increment
            result.Add(new Token(varName.content, TokenType.NAME, result[result.Count - 1].line + 2));
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, result[result.Count - 1].line));
            result.Add(new Token(varName.content, TokenType.NAME, result[result.Count - 1].line));
            result.Add(new Token("+", TokenType.OPERATOR_PLUS, result[result.Count - 1].line));
            result.Add(new Token("1", TokenType.NUMBER, result[result.Count - 1].line));
            result.Add(new Token(";", TokenType.SEMICOLON, result[result.Count - 1].line));
            result.Add(new Token("}", TokenType.CURLY_BRACE_RIGHT, result[result.Count - 1].line+1));

            return result;
        }

        private List<Token> getForBody(int[] forCoords, ref List<Token> input)
        {
            List<Token> result = new List<Token>();
            for(int i = forCoords[0]; i < forCoords[1]; i++)
            {
                result.Add(input[i]);
            }
            return result;
        }

        private int[] findForCoords(ref List<Token> input)
        {
            int[] result = new int[2];

            int braces = 0;
            bool inBody = false;
            bool foundFor = false;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].content == "for" && !foundFor)
                {
                    result[0] = i;
                    foundFor = true;
                }
                if (foundFor)
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
