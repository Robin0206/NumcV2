using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations.HelperClasses
{
    class OperationLineConverter
    {
        internal List<Token> convert(List<Token> line)
        {
            List<Token> arguments = getArguments(ref line);
            Token functionName = getFunctionName(ref line);
            return generateFunctionCall(functionName, arguments);
        }

        private List<Token> generateFunctionCall(Token functionName, List<Token> arguments)
        {
            List<Token> functionCall = new List<Token>();
            functionCall.Add(functionName);
            functionCall.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            functionCall.Add(arguments[0]);
            functionCall.Add(new Token(",", TokenType.COMMA, 0));
            functionCall.Add(arguments[1]);
            functionCall.Add(new Token(",", TokenType.COMMA, 0));
            functionCall.Add(arguments[2]);
            functionCall.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            functionCall.Add(new Token(";", TokenType.SEMICOLON, 0));
            return functionCall;
        }

        private Token getFunctionName(ref List<Token> line)
        {
            switch (line[3].type)
            {
                 case TokenType.OPERATOR_PLUS:
                    return new Token("add", TokenType.NAME, 0);
                 case TokenType.OPERATOR_MINUS:
                    return new Token("sub", TokenType.NAME, 0);
                 case TokenType.OPERATOR_DIVIDE:
                    return new Token("div", TokenType.NAME, 0);
                 case TokenType.OPERATOR_MULTIPLY:
                    return new Token("mul", TokenType.NAME, 0);
                 case TokenType.OPERATOR_XOR:
                    return new Token("xor", TokenType.NAME, 0);
                 case TokenType.OPERATOR_MODULO:
                    return new Token("%", TokenType.NAME, 0);
                 case TokenType.OPERATOR_SINGLE_OR:
                    return new Token("binOr", TokenType.NAME, 0);
                 case TokenType.OPERATOR_SINGLE_AND:
                    return new Token("binAnd", TokenType.NAME, 0);
                 case TokenType.OPERATOR_DOUBLE_EQUALS:
                    return new Token("logEq", TokenType.NAME, 0);
                 case TokenType.OPERATOR_DOUBLE_OR:
                    return new Token("logOr", TokenType.NAME, 0);
                 case TokenType.OPERATOR_DOUBLE_AND:
                    return new Token("logAnd", TokenType.NAME, 0);
                 case TokenType.OPERATOR_LESS:
                    return new Token("lessThan", TokenType.NAME, 0);
                case TokenType.OPERATOR_MORE:
                    return new Token("greaterThan", TokenType.NAME, 0);
                default:
                    throw new Exception("Invalid operator");
            }
        }

        private List<Token> getArguments(ref List<Token> line)
        {
            List<Token> result = new List<Token>();
            result.Add(line[0]);
            result.Add(line[2]);
            result.Add(line[4]);
            return result;
        }
    }
}
