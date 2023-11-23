using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Numc.lexer
{
    class Lexer
    {
        static string[] keyWords = {
            "for", 
            "if", 
            "while", 
            "define", 
            "with", 
            "return",
            "from",
            "to",
            "int",
            "real",
            "bool",
            "for",
            "do",
            "times"
        };
        public List<Token> tokenize(ref string[] input)
        {
            List<Token> result = new List<Token>();
            for(int lineNumber = 0; lineNumber < input.Length; lineNumber++)
            {
                result.AddRange(tokenizeLine(input[lineNumber], lineNumber));
            }
            return result;
        }

        private List<Token> tokenizeLine(string input, int lineNumber)
        {
            List<Token> result = new List<Token>();
            foreach (string symbol in input.Split(' '))
            {
                if(!symbol.All(c => char.IsWhiteSpace(c)))
                {
                    result.Add(tokenizeSymbol(symbol, lineNumber));
                }
                
            }
            return result;
        }

        private Token tokenizeSymbol(string symbol, int lineNumber)
        {
            switch (symbol.Replace(" ", ""))
            {
                case "{":
                    return new Token(symbol, TokenType.CURLY_BRACE_LEFT, lineNumber);
                case "}":
                    return new Token(symbol, TokenType.CURLY_BRACE_RIGHT, lineNumber);
                case "(":
                    return new Token(symbol, TokenType.BRACE_LEFT, lineNumber);
                case ")":
                    return new Token(symbol, TokenType.BRACE_RIGHT, lineNumber);

                case "+":
                    return new Token(symbol, TokenType.OPERATOR_PLUS, lineNumber);
                case "-":
                    return new Token(symbol, TokenType.OPERATOR_MINUS, lineNumber);
                case "/":
                    return new Token(symbol, TokenType.OPERATOR_DIVIDE, lineNumber);
                case "*":
                    return new Token(symbol, TokenType.OPERATOR_MULTIPLY, lineNumber);
                case "!":
                    return new Token(symbol, TokenType.OPERATOR_NOT, lineNumber);
                case "^":
                    return new Token(symbol, TokenType.OPERATOR_XOR, lineNumber);
                case "%":
                    return new Token(symbol, TokenType.OPERATOR_MODULO, lineNumber);
                case "=":
                    return new Token(symbol, TokenType.OPERATOR_SINGLE_EQUALS, lineNumber);
                case "|":
                    return new Token(symbol, TokenType.OPERATOR_SINGLE_OR, lineNumber);
                case "&":
                    return new Token(symbol, TokenType.OPERATOR_SINGLE_AND, lineNumber);
                case "<":
                    return new Token(symbol, TokenType.OPERATOR_LESS, lineNumber);
                case ">":
                    return new Token(symbol, TokenType.OPERATOR_MORE, lineNumber);

                case "==":
                    return new Token(symbol, TokenType.OPERATOR_DOUBLE_EQUALS, lineNumber);
                case "||":
                    return new Token(symbol, TokenType.OPERATOR_DOUBLE_OR, lineNumber);
                case "&&":
                    return new Token(symbol, TokenType.OPERATOR_DOUBLE_AND, lineNumber);

                case ";":
                    return new Token(symbol, TokenType.SEMICOLON, lineNumber);
                case ",":
                    return new Token(symbol, TokenType.COMMA, lineNumber);

                default:
                    if (notANumber(symbol))
                    {
                        return new Token(
                            symbol,
                            keyWords.Contains(symbol) ? TokenType.KEYWORD : TokenType.NAME,
                            lineNumber
                        );
                    }
                    else
                    {
                        return new Token(
                            symbol,
                            TokenType.NUMBER,
                            lineNumber
                        );
                    }
                    
            }
        }

        private bool notANumber(string symbol)
        {
            foreach(char i in symbol)
            {
                if(!(Char.IsNumber(i) || i == '.'))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
