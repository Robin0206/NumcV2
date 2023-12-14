using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class IfRemover : PostLSSLayer
    {
        static int bufferNum = 0;
        List<Token> ifEndingSections;
        public IfRemover()
        {
            ifEndingSections = new List<Token>();
        }
        private List<Token> convertIfBody(List<Token> ifBody)
        {
            List<Token> body = extractBody(ref ifBody);
            List<Token> condition = extractCondition(ref ifBody);
            List<Token> result = new List<Token>();
            string conditionName = "____if_condition_" + bufferNum;
            string labelBodyName = "____if_body_" + bufferNum;
            string labelEndName = "____if_end_" + bufferNum;
            bufferNum++;

            //write refa
            result.Add(new Token("refa", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(conditionName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write condition set
            result.Add(new Token(conditionName, TokenType.NAME, 0));
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, 0));
            result.AddRange(condition);
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write beq
            result.Add(new Token("beq", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(conditionName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("true", TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token(labelBodyName, TokenType.NAME, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write end label
            result.Add(new Token("label", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(labelEndName, TokenType.NAME, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            //write label (ending section)
            ifEndingSections.Add(new Token("label", TokenType.NAME, 0));
            ifEndingSections.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            ifEndingSections.Add(new Token(labelBodyName, TokenType.NAME, 0));
            ifEndingSections.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            ifEndingSections.Add(new Token(";", TokenType.SEMICOLON, 0));
            
            //write body (ending section)
            ifEndingSections.AddRange(body);

            //write beq (ending section)
            ifEndingSections.Add(new Token("beq", TokenType.NAME, 0));
            ifEndingSections.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            ifEndingSections.Add(new Token("true", TokenType.NAME, 0));
            ifEndingSections.Add(new Token(",", TokenType.COMMA, 0));
            ifEndingSections.Add(new Token("true", TokenType.NAME, 0));
            ifEndingSections.Add(new Token(",", TokenType.COMMA, 0));
            ifEndingSections.Add(new Token(labelEndName, TokenType.NAME, 0));
            ifEndingSections.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            ifEndingSections.Add(new Token(";", TokenType.SEMICOLON, 0));

            return result;
        }

        private List<Token> extractCondition(ref List<Token> ifBody)
        {
            List<Token> result = new List<Token>();
            bool writing = false;
            foreach (Token token in ifBody)
            {
                if (token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    break;
                }
                if (writing)
                {
                    result.Add(token);
                }
                if (token.content == "if")
                {
                    writing = true;
                }
            }

            return result;
        }

        private List<Token> extractBody(ref List<Token> ifBody)
        {
            List<Token> result = new List<Token>();
            bool writing = false;
            foreach(Token token in ifBody)
            {
                if (writing)
                {
                    result.Add(token);
                }
                if (token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    writing = true;
                }
                
            }
            return result;
        }

        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            foreach (Token token in input)
            {
                result.Add(token);
            }

            bool foundIf = true;
            int counter = 0;
            while (foundIf)
            {
                int[] ifCoords = findIfCoords(ref result);
                if (ifCoords[0] != -1)
                {
                    //add before if
                    resultBuffer.Clear();
                    List<Token> convertedBody = convertIfBody(
                        getIfBody(ifCoords, ref result)
                    );

                    for (int i = 0; i < ifCoords[0]; i++)
                    {
                        resultBuffer.Add(result[i]);
                    }

                    //add body
                    resultBuffer.AddRange(convertedBody);

                    //add after if
                    for (int i = ifCoords[1]+1; i < result.Count; i++)
                    {
                        result[i].setLine(result[i].line + 2);
                        resultBuffer.Add(result[i]);
                    }
                    //add after if
                    result.Clear();
                    foreach (Token token in resultBuffer)
                    {
                        result.Add(token);
                    }
                }
                else
                {
                    foundIf = false;
                }
                counter++;
            }
            result.RemoveAt(result.Count - 1);
            //add ending section
            //write beq
            result.Add(new Token("beq", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token("true", TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("true", TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("____function_end", TokenType.NAME, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            //writing if bodys
            result.AddRange(ifEndingSections);
            //writing ending label
            result.Add(new Token("label", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token("____function_end", TokenType.NAME, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            //writing ending curly brace 
            result.Add(new Token("}", TokenType.CURLY_BRACE_RIGHT, 0));

            return result;
        }

        

        private List<Token> getIfBody(int[] ifCoords, ref List<Token> input)
        {
            List<Token> result = new List<Token>();
            for (int i = ifCoords[0]; i < ifCoords[1]; i++)
            {
                result.Add(input[i]);
            }
            return result;
        }

        private int[] findIfCoords(ref List<Token> input)
        {
            int[] result = new int[2];

            int braces = 0;
            bool inBody = false;
            bool foundIf = false;
            int currentTabLevel = 0;
            int tabLevelOfDeepestIf = findTabLevelOfDeepestIf(ref input);

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].content == "if" && !foundIf && tabLevelOfDeepestIf == currentTabLevel)
                {
                    result[0] = i;
                    foundIf = true;
                }
                if (foundIf)
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
                else
                {
                    if (input[i].type == TokenType.CURLY_BRACE_LEFT)
                    {
                        currentTabLevel++;
                    }
                    else if (input[i].type == TokenType.CURLY_BRACE_RIGHT)
                    {
                        currentTabLevel--;
                    }
                }
            }
            return new int[] { -1, -1 };
        }

        private int findTabLevelOfDeepestIf(ref List<Token> input)
        {
            int currentTabLevel = 0;
            int deepestTabLevelIf = -1;
            foreach(Token token in input)
            {
                if(token.content == "{")
                {
                    currentTabLevel++;
                }
                if (token.content == "}")
                {
                    currentTabLevel--;
                }
                if(token.content == "if")
                {
                    if(currentTabLevel > deepestTabLevelIf)
                    {
                        deepestTabLevelIf = currentTabLevel;
                    }
                }
            }
            return deepestTabLevelIf;
        }
    }
}
