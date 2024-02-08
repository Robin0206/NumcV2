using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class InlineNumberConverter : PostLSSLayer
    {
        HashSet<string> numbers = new HashSet<string>();
        List<List<Token>> refas = new List<List<Token>>();
        List<List<Token>> setCalls = new List<List<Token>>();
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> lines = convertToLines(ref input);
            lines = fillNumberArrayAndSubstitute(ref lines);
            //convert the lines to the result
            List<Token> result = new List<Token>();
            foreach (List<Token> line in lines)
            {
                result.AddRange(line);
            }
            return result;
        }

        private List<List<Token>> fillNumberArrayAndSubstitute(ref List<List<Token>> lines)
        {
            //convert the lines and fill the numbers set
            List<List<Token>> resultLines = new List<List<Token>>();
            foreach (List<Token> line in lines)
            {
                
                if (isOperationLine(line))
                {
                    resultLines.Add(convertOperationline(line));
                }
                else
                {
                    resultLines.Add(line);
                }

            }
            //generate the new refas
            foreach(string number in numbers)
            {
                refas.Add(generateRefa(number));
            }
            //add the setcalls
            foreach (string number in numbers)
            {
                setCalls.Add(generateSetCall(number));
            }
            resultLines = addRefasAndSetCalls(resultLines);
            
            return resultLines;
        }

        private List<List<Token>> addRefasAndSetCalls(List<List<Token>> resultLines)
        {
            List<List<Token>> result = new List<List<Token>>();
            result.Add(resultLines[0]);
            result.AddRange(refas);
            result.AddRange(setCalls);
            for(int i = 1; i < resultLines.Count; i++)
            {
                result.Add(resultLines[i]);
            }
            return result;
        }

        private List<Token> generateSetCall(string number)
        {
            List<Token> result = new List<Token>();
            result.Add(new Token("set", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(numberToVarName(number), TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token(number, TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            return result;
        }

        private List<Token> generateRefa(string number)
        {
            List<Token> result = new List<Token>();
            result.Add(new Token("refa", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(numberToVarName(number), TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token(number.Contains(".") ? 2 + " " : 1 + " ", TokenType.NUMBER, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            return result;
        }

        private List<Token> convertOperationline(List<Token> line)
        {
            List<Token> result = new List<Token>();
            foreach(Token token in line)
            {
                if(token.type == TokenType.NUMBER)
                {
                    numbers.Add(token.content);
                    result.Add(new Token(numberToVarName(token.content), TokenType.NAME, 0));
                }
                else
                {
                    result.Add(token);
                }
            }
            return result;

        }

        private bool isOperationLine(List<Token> line)
        {
            int numOperators = 0;
            foreach (Token token in line)
            {
                if (
                    token.type == TokenType.OPERATOR_PLUS ||
                    token.type == TokenType.OPERATOR_MINUS ||
                    token.type == TokenType.OPERATOR_DIVIDE ||
                    token.type == TokenType.OPERATOR_MULTIPLY ||
                    token.type == TokenType.OPERATOR_NOT ||
                    token.type == TokenType.OPERATOR_XOR ||
                    token.type == TokenType.OPERATOR_MODULO ||
                    token.type == TokenType.OPERATOR_SINGLE_EQUALS ||
                    token.type == TokenType.OPERATOR_SINGLE_OR ||
                    token.type == TokenType.OPERATOR_SINGLE_AND ||
                    token.type == TokenType.OPERATOR_DOUBLE_EQUALS ||
                    token.type == TokenType.OPERATOR_DOUBLE_OR ||
                    token.type == TokenType.OPERATOR_DOUBLE_AND ||
                    token.type == TokenType.OPERATOR_LESS ||
                    token.type == TokenType.OPERATOR_MORE
                    )
                {
                    numOperators++;
                }
            }
            return numOperators >= 2;
        }

        public string numberToVarName(string n)
        {
            return "____numBuffer_" + n;
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
