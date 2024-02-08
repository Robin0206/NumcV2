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
        List<Token> refaBuffer = new List<Token>();
        string lastBufferName = "";
        string nameBeforeConversion = "";
        public List<Token> removeSugar(List<Token> input)
        {
       
            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            List<Token> debugBuffer = new List<Token>();
            result.AddRange(input);
            
            while (true)
            {
                
                int[] coords = findFirstInlinedExpression(ref result);
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
                List<Token> simplifiedExpressions = simplify(expression, ref input);
                resultBuffer.AddRange(simplifiedExpressions);
                //add after expression
                for(int i = coords[1]; i < result.Count; i++)
                {
                    if(result[i].content != nameBeforeConversion)
                    {
                        resultBuffer.Add(result[i]);
                    }
                    else
                    {
                        resultBuffer.Add(new Token(lastBufferName, TokenType.NAME, 0));
                    }
                }
                //copy
                debugBuffer.Clear();
                debugBuffer.AddRange(resultBuffer);
                result.Clear();
                result.AddRange(resultBuffer);
                resultBuffer.Clear();
            }
            return result;
        }

        private List<Token> simplify(List<Token> expression, ref List<Token> input)
        {
            nameBeforeConversion = expression[0].content;
            List<Token> result = new List<Token>();
            List<Token> reversePolish = shuntingYard(expression);
            Stack<Token> stack = new Stack<Token>();
            List<Token> expressionConversionResult = new List<Token>();

            foreach (Token token in reversePolish)
            {
                if (isOperator(token))
                {
                    Token right = stack.Pop();
                    Token left = stack.Pop();
                    expressionConversionResult.Clear();
                    expressionConversionResult.AddRange(constructExpression(left, right, token, ref input));
                    result.AddRange(new List<Token>(expressionConversionResult));
                    stack.Push(new Token(expressionConversionResult[9].content, expressionConversionResult[9].type, 0));
                }
                else
                {
                    stack.Push(token);
                }
            }
            return result;
        }

        private List<Token> constructExpression(Token left, Token right, Token op, ref List<Token> input)
        {
            List<Token> result = new List<Token>();
            string varName = "____expression_buffer_" + bufferNum;
            bufferNum++;
            lastBufferName = varName;
            int leftType = getPrimitiveType(left, ref input);
            int rightType = getPrimitiveType(right, ref input);
            int resultType = 0;
            if (!opIsAlwaysReturningTrue(op))
            {
                resultType = Math.Max(leftType, rightType);
            }
            

            //add Refa to result
            result.Add(new Token("refa", TokenType.NAME, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.Add(new Token(varName, TokenType.NAME, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token(resultType+"", TokenType.NUMBER, 0));
            result.Add(new Token(",", TokenType.COMMA, 0));
            result.Add(new Token("0", TokenType.NUMBER, 0));
            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            result.Add(new Token(";", TokenType.SEMICOLON, 0));
            //Add Refa to refa buffer
            refaBuffer.Add(new Token("refa", TokenType.NAME, 0));
            refaBuffer.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            refaBuffer.Add(new Token(varName, TokenType.NAME, 0));
            refaBuffer.Add(new Token(",", TokenType.COMMA, 0));
            refaBuffer.Add(new Token(resultType + "", TokenType.NUMBER, 0));
            refaBuffer.Add(new Token(",", TokenType.COMMA, 0));
            refaBuffer.Add(new Token("0", TokenType.NUMBER, 0));
            refaBuffer.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            refaBuffer.Add(new Token(";", TokenType.SEMICOLON, 0));
            //add statement
            result.Add(new Token(varName, TokenType.NAME, 0));
            result.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, 0));
            result.Add(left);
            result.Add(op);
            result.Add(right);
            result.Add(new Token(";", TokenType.SEMICOLON, 0));

            return result;
        }

        private bool opIsAlwaysReturningTrue(Token op)
        {
            return new TokenType[]
            {
                TokenType.OPERATOR_DOUBLE_EQUALS,
                TokenType.OPERATOR_DOUBLE_OR,
                TokenType.OPERATOR_DOUBLE_AND,
                TokenType.OPERATOR_LESS,
                TokenType.OPERATOR_MORE
            }
            .Contains(op.type);
        }

        private int getPrimitiveType(Token token, ref List<Token> input)
        {
            if(token.type == TokenType.NUMBER)
            {
                return token.content.Contains(".") ? 2 : 1;
            }else if(token.type == TokenType.NAME && (token.content == "true" || token.content == "false"))
            {
                return 0;
            }
            else
            {
                return getTypeFromReferenceAllocation(token, ref input);
            }
        }

        private int getTypeFromReferenceAllocation(Token varName, ref List<Token> input)
        {
            //search in input
            for(int i = 0; i < input.Count; i++)
            {
                if(
                    i + 4 < input.Count - 1 &&
                    input[i].content == "refa" &&
                    input[i + 2].content == varName.content
                ){
                    return int.Parse(input[i + 4].content);
                }
            }
            //search in refa buffer
            for (int i = 0; i < refaBuffer.Count; i++)
            {
                if (
                    i + 4 < refaBuffer.Count - 1 &&
                    refaBuffer[i].content == "refa" &&
                    refaBuffer[i + 2].content == varName.content
                )
                {
                    return int.Parse(refaBuffer[i + 4].content);
                }
            }

            //throw exception if not found
            throw new Exception("Exception: Variable " + varName.content + " not found");
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
                    while(stack.Count != 0 && stack.Peek().type != TokenType.BRACE_LEFT)
                    {
                        result.Add(stack.Pop());
                    }
                    if(stack.Count != 0 && stack.Peek().type != TokenType.BRACE_LEFT)
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
                if (isOperator(token) && token.type != TokenType.OPERATOR_SINGLE_EQUALS)
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
