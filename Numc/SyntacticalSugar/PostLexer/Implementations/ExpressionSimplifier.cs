using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class ExpressionSimplifier : PostLSSLayer
    {
        static int bufferNum = 0;
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> expression = extractExpression(findFirstInlinedExpression(ref input), input);
            Console.WriteLine();
            foreach(Token token in expression)
            {
                Console.Write(token.content + " ");
            }
            Console.WriteLine();
            List<Token> simplified = simplify(expression);
            Console.WriteLine();
            foreach (Token token in simplified)
            {
                Console.Write(token.content + " ");
                if(token.type == TokenType.SEMICOLON)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            /*
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            
            while (true)
            {
                
                int[] coords = findFirstInlinedExpression(ref input);
                if(coords[0] == -1)
                {
                    break;
                }
                //add before Expression
                for(int i = 0; i < coords[0]; i++)
                {
                    resultBuffer.Add(result[i]);
                }
                //add expression
                List<Token> expression = extractExpression(coords, result);
                List<Token> simplifiedExpressions = simplify(expression);
                resultBuffer.AddRange(simplifiedExpressions);
                //add after expression
                for(int i = 0; i < coords[1]; i++)
                {
                    resultBuffer.Add(result[i]);
                }
                //copy
                result.Clear();
                result.AddRange(resultBuffer);
            }
            return result;
            */
            return input;
        }

        private List<Token> simplify(List<Token> expression)
        {
            List<Token> reversePolish = shuntingYard(expression);
            Console.WriteLine(reversePolish.Count);
            Console.WriteLine();
            foreach (Token token in reversePolish)
            {
                Console.Write(token.content + " ");
            }
            Console.WriteLine();
            return expression;
        }

        private List<Token> shuntingYard(List<Token> expression)
        {
            expression.RemoveAt(0);
            expression.RemoveAt(0);
            List<Token> result = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            foreach(Token token in expression)
            {
                if(token.type == TokenType.NUMBER || token.type == TokenType.NAME)
                {
                    result.Add(token);
                }
                if (isOperator(token))
                {
                    while(
                       stack.Count != 0 &&
                       isOperator(stack.Peek()) &&
                       prescedence(token) <= prescedence(stack.Peek())
                    ){
                        result.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                if(token.type == TokenType.BRACE_LEFT)
                {
                    stack.Push(token);
                }
                if (token.type == TokenType.BRACE_RIGHT)
                {
                    while(stack.Peek().type != TokenType.BRACE_LEFT)
                    {
                        result.Add(stack.Pop());
                    }
                    if(stack.Peek().type != TokenType.BRACE_LEFT)
                    {
                        stack.Pop();
                    }
                }
            }
            while(stack.Count != 0)
            {
                if(stack.Peek().type != TokenType.BRACE_RIGHT && stack.Peek().type != TokenType.BRACE_LEFT)
                {
                    result.Add(stack.Pop());
                }
                else
                {
                    stack.Pop();
                }
            }
            return result;
        }

        private int prescedence(Token token)
        {
            switch (token.type)
            {
                case TokenType.OPERATOR_PLUS:
                    return 11;
                case TokenType.OPERATOR_MINUS:
                    return 11;
                case TokenType.OPERATOR_DIVIDE:
                    return 12;
                case TokenType.OPERATOR_MULTIPLY:
                    return 12;
                case TokenType.OPERATOR_NOT:
                    return 13;
                case TokenType.OPERATOR_XOR:
                    return 6;
                case TokenType.OPERATOR_MODULO:
                    return 12;
                case TokenType.OPERATOR_SINGLE_OR:
                    return 5;
                case TokenType.OPERATOR_SINGLE_AND:
                    return 7;
                case TokenType.OPERATOR_DOUBLE_EQUALS:
                    return 8;
                case TokenType.OPERATOR_DOUBLE_OR:
                    return 3;
                case TokenType.OPERATOR_DOUBLE_AND:
                    return 4;
                case TokenType.OPERATOR_LESS:
                    return 9;
                case TokenType.OPERATOR_MORE:
                    return 9;
                default:
                    return -1;
            }
        }

        private List<Token> extractExpression(int[] coords, List<Token> input)
        {
            List<Token> result = new List<Token>();
            for(int i = coords[0]; i < coords[1]; i++)
            {
                result.Add(input[i]);
            }
            return result;
        }

        private int[] findFirstInlinedExpression(ref List<Token> input)
        {
            int[] result = new int[2];
            bool foundExpression = false;
            List<List<Token>> lines = splitToLines(ref input);
            //find expression in lines
            int counter = 0;
            foreach(List<Token> line in lines)
            {
                if(
                    doesContainMoreThanOneOperatorOtherThanSingleEquals(line) &&
                    doesContainSingleEquals(line) &&
                    doesntContainKeyWord(line)
                )
                {
                    foundExpression = true;
                    break;
                }
                counter++;
            }
            if (foundExpression)
            {
                //set result[0] to number of tokens before expression + 1
                result[0] = 0;
                for (int i = 0; i < counter; i++)
                {
                    result[0] += lines[i].Count;
                }
                //set result[1] to result[0] + expression.Count
                result[1] = result[0] + lines[counter].Count;

                return result;
            }
            else
            {
                return new int[] { -1, -1 };
            }
            
        }

        private bool doesntContainKeyWord(List<Token> line)
        {
            foreach(Token token in line)
            {
                if(token.type == TokenType.KEYWORD)
                {
                    return false;
                }
            }
            return true;
        }

        private bool doesContainSingleEquals(List<Token> line)
        {
            foreach (Token token in line)
            {
                if (token.type == TokenType.OPERATOR_SINGLE_EQUALS)
                {
                    return true;
                }
            }
            return false;
        }

        private bool doesContainMoreThanOneOperatorOtherThanSingleEquals(List<Token> line)
        {
            int counter = 0;
            foreach(Token token in line)
            {
                if (isOperator(token))
                {
                    counter++;
                }
            }
            return counter > 1;
        }

        private bool isOperator(Token token)
        {
            TokenType[] tokentypes = new TokenType[]
            {
                TokenType.OPERATOR_PLUS,
                TokenType.OPERATOR_MINUS,
                TokenType.OPERATOR_DIVIDE,
                TokenType.OPERATOR_MULTIPLY,
                TokenType.OPERATOR_NOT,
                TokenType.OPERATOR_XOR,
                TokenType.OPERATOR_MODULO,
                TokenType.OPERATOR_SINGLE_EQUALS,
                TokenType.OPERATOR_SINGLE_OR,
                TokenType.OPERATOR_SINGLE_AND,
                TokenType.OPERATOR_DOUBLE_EQUALS,
                TokenType.OPERATOR_DOUBLE_OR,
                TokenType.OPERATOR_DOUBLE_AND,
                TokenType.OPERATOR_LESS,
                TokenType.OPERATOR_MORE
            };
            return tokentypes.Contains(token.type);
        }

        private List<List<Token>> splitToLines(ref List<Token> input)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<Token> currentLine = new List<Token>();

            foreach(Token token in input)
            {
                currentLine.Add(token);
                if(token.content == ";" || token.content == "{" || token.content == "}")
                {
                    result.Add(new List<Token>());
                    result[result.Count - 1].AddRange(currentLine);
                    currentLine.Clear();
                }
            }
            return result;
        }
    }
}
