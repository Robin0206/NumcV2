using Numc.lexer;
using System.Collections.Generic;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class WhileToIfConverter : PostLSSLayer
    {
        static int bufferNum = 0;
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            foreach (Token token in input)
            {
                result.Add(token);
            }

            bool foundWhile = true;
            int counter = 0;
            while (foundWhile)
            {
                int[] doCoords = findWhileCoords(ref result);
                if (doCoords[0] != -1)
                {
                    //add before while
                    resultBuffer.Clear();
                    List<Token> forBody = convertWhileBody(
                        getWhileBody(doCoords, ref result)
                    );

                    for (int i = 0; i < doCoords[0]; i++)
                    {
                        resultBuffer.Add(result[i]);
                    }

                    //add if
                    resultBuffer.AddRange(forBody);

                    //add after while
                    for (int i = doCoords[1]+1; i < result.Count; i++)
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
                    foundWhile = false;
                }
                counter++;
            }

            return result;
        }

        private List<Token> convertWhileBody(List<Token> whileBody)
        {
            List<Token> result = new List<Token>();
            List<Token> condition = new List<Token>();
            List<Token> body = new List<Token>();
            bool writing = false;
            string conditionBufferName = "____condition_buffer_" + bufferNum;
            string labelName = "____while_start_" + bufferNum;
            bufferNum++;

            //fill Condition
            foreach(Token token in whileBody)
            {
                if(token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    break;
                }
                if (writing)
                {
                    condition.Add(token);
                }
                if(token.content == "while")
                {
                    writing = true;
                }
            }

            //get body
            writing = false;
            foreach(Token token in whileBody)
            {
                if (writing)
                {
                    body.Add(token);
                }
                if (token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    writing = true;
                }
            }
            //body.RemoveAt(body.Count - 1);

            //write refa
            result.Add(new Token("refa", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(conditionBufferName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write Label call
            result.Add(new Token("label", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(labelName, TokenType.NAME, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write conditionBuffer set
            result.Add(new Token(conditionBufferName, TokenType.NAME, 0));
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, 0));
            foreach(Token token in condition)
            {
                result.Add(token);
            }
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write if
            result.Add(new Token("if", TokenType.KEYWORD, 0));
            result.Add(new Token(conditionBufferName, TokenType.NAME, 0));

            //write curly brace
            result.Add(new Token("{", TokenType.CURLY_BRACE_LEFT, 0));

            //write body
            foreach (Token token in body)
            {
                result.Add(token);
            }

            //write conditionBuffer set
            result.Add(new Token(conditionBufferName, TokenType.NAME, 0));
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, 0));
            foreach (Token token in condition)
            {
                result.Add(token);
            }
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write beq
            result.Add(new Token("beq", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(conditionBufferName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("true", TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token(labelName, TokenType.NAME, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write curly brace
            result.Add(new Token("}", TokenType.CURLY_BRACE_RIGHT, 0));

            return result;
        }

        private List<Token> getWhileBody(int[] whileCoords, ref List<Token> input)
        {
            List<Token> result = new List<Token>();
            for (int i = whileCoords[0]; i < whileCoords[1]; i++)
            {
                result.Add(input[i]);
            }
            return result;
        }

        private int[] findWhileCoords(ref List<Token> input)
        {
            int[] result = new int[2];

            int braces = 0;
            bool inBody = false;
            bool foundWhile = false;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].content == "while" && !foundWhile)
                {
                    result[0] = i;
                    foundWhile = true;
                }
                if (foundWhile)
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
