using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class VariableAssignmentConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> inputLines = convertToLines(ref input);
            List<List<Token>> resultLines = new List<List<Token>>();
            foreach (List<Token> line in inputLines)
            {
                if (isVariableAssignment(line)){
                    resultLines.Add(convertVariableAssignment(line));
                }
                else
                {
                    resultLines.Add(line);
                }
            }
            List<Token> result = new List<Token>();
            foreach(List<Token> line in resultLines)
            {
                result.AddRange(line);
            }
            return result;
        }

        private List<Token> convertVariableAssignment(List<Token> line)
        {
            Token opA = line[0];
            Token opB = line[2];
            List<Token> result = new List<Token>();
            if(opB.type == TokenType.NAME) {
                result.Add(new Token("setV", TokenType.NAME, 0));
            }
            else
            {
                result.Add(new Token("set", TokenType.NAME, 0));
            }
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(opA);
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(opB);
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            return result;
        }

        private bool isVariableAssignment(List<Token> line)
        {
            bool containsSingleEquals = false;
            bool containsBrace = false;
            int numberOfOperands = 0;
            foreach (Token token in line)
            {
                if (token.type == TokenType.OPERATOR_SINGLE_EQUALS)
                {
                    containsSingleEquals = true;
                }
                if (token.type == TokenType.BRACE_LEFT)
                {
                    containsBrace = true;
                }
                if (token.type == TokenType.NAME || token.type == TokenType.NUMBER)
                {
                    numberOfOperands++;
                }
            }
            return containsSingleEquals && !containsBrace && numberOfOperands == 2;
        }

        private List<List<Token>> convertToLines(ref List<Token> input)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<Token> currentLine = new List<Token>();
            foreach (Token token in input)
            {
                currentLine.Add(token);
                if (token.content == ";" || token.content == "{" || token.content == "}")
                {
                    result.Add(new List<Token>());
                    foreach (Token lineToken in currentLine)
                    {
                        result[result.Count - 1].Add(lineToken);
                    }
                    currentLine.Clear();
                }
            }
            return result;
        }
    }
}
