using Numc.lexer;
using Numc.SyntacticalSugar.PostLexer.Implementations.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class OperationConverter : PostLSSLayer
    {
        OperationLineConverter operationLineConverter = new OperationLineConverter();
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> inputLines = convertToLines(ref input);
            List<List<Token>> resultLines = new List<List<Token>>();
            foreach (List<Token> line in inputLines)
            {
                if (isOperationLine(line))
                {
                    resultLines.Add(operationLineConverter.convert(line));
                }
                else
                {
                    resultLines.Add(line);
                }
            }
            List<Token> result = new List<Token>();
            foreach (List<Token> line in resultLines)
            {
                result.AddRange(line);
            }
            return result;
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
        private bool containsToken(string tokenContent, List<Token> line)
        {
            foreach (Token token in line)
            {
                if (token.content == tokenContent)
                {
                    return true;
                }
            }
            return false;
        }

        private bool containsOperator(List<Token> line)
        {
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
                    return true;
                }
            }
            return false;
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
    }
}

