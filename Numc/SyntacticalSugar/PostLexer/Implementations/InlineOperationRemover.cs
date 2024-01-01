using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class InlineOperationRemover : PostLSSLayer
    {
        static int bufferCounter = 0;

        List<Token> refaBuffer = new List<Token>();
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> lines = splitToLines(ref input);
            List<List<Token>> lineBuffer = new List<List<Token>>();
            int coord = 0;
            while (true)
            {
                coord = findInlineOp(ref lines);
                if(coord == -1)
                {
                    break;
                }
                for(int i = 0; i < coord - 1; i++)
                {
                    lineBuffer.Add(lines[i]);
                }
                List<List<Token>> convertedLines = convertLine(lines[coord], ref lines);
                lineBuffer.AddRange(convertedLines);
                for (int i = coord + 1; i < lines.Count; i++)
                {
                    lineBuffer.Add(lines[i]);
                }
                lines.Clear();
                lines.AddRange(lineBuffer);
                lineBuffer.Clear();
            }
            List<Token> result = new List<Token>();
            foreach(List<Token> line in lines){
                result.AddRange(line);
            }
            return result;
        }

        private List<List<Token>> convertLine(List<Token> tokens, ref List<List<Token>> lines)
        {
            List<List<Token>> arguments = getArguments(ref tokens);
            List<List<Token>> result = new List<List<Token>>();
            List<List<Token>> convertedArguments = new List<List<Token>>();
            Token functionName = tokens[0];
            foreach(List<Token> argument in arguments)
            {
                if (containsOperator(argument))
                {
                    //generate refa call
                    List<Token> refaCall = new List<Token>();
                    string bufferName = "____inline_expr_buffer_" + bufferCounter;
                    int refaType = getHighestType(argument, ref lines);
                    bufferCounter++; 
                    refaCall.Add(new Token("refa", TokenType.NAME, 0));
                    refaCall.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                    refaCall.Add(new Token(bufferName, TokenType.NAME, 0));
                    refaCall.Add(new Token(",", TokenType.COMMA, 0));
                    refaCall.Add(new Token(refaType + "", TokenType.NUMBER, 0));
                    refaCall.Add(new Token(",", TokenType.COMMA, 0));
                    refaCall.Add(new Token("0", TokenType.NUMBER, 0));
                    refaCall.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                    refaCall.Add(new Token(";", TokenType.SEMICOLON, 0));

                    //add refaCall to result
                    result.Add(refaCall);

                    //Add Refa Call to refa buffer
                    refaBuffer.AddRange(refaCall);

                    //generate line (set buffer to expression/argument)
                    List<Token> setExpression = new List<Token>();
                    setExpression.Add(new Token(bufferName, TokenType.NAME, 0));
                    setExpression.Add(new Token("=", TokenType.OPERATOR_SINGLE_EQUALS, 0));
                    setExpression.AddRange(argument);
                    setExpression.Add(new Token(";", TokenType.SEMICOLON, 0));

                    //Add line = (set buffer to expression/argument) to result
                    result.Add(setExpression);

                    //add buffername to converted arguments
                    List < Token > newArgument = new List<Token>();
                    newArgument.Add(new Token(bufferName, TokenType.NAME, 0));
                    convertedArguments.Add(newArgument);

                    //add function call to result
                    List<Token> functionCall = new List<Token>();
                    functionCall.Add(functionName);
                    functionCall.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                    if(arguments.Count > 1)
                    {
                        functionCall.AddRange(convertedArguments[0]);
                        for(int i = 1; i < convertedArguments.Count; i++)
                        {
                            functionCall.Add(new Token(",", TokenType.COMMA, 0));
                            functionCall.AddRange(convertedArguments[i]);
                        }

                        functionCall.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                        functionCall.Add(new Token(";", TokenType.SEMICOLON, 0));
                    }
                    else
                    {

                        functionCall.AddRange(convertedArguments[0]);
                        functionCall.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                        functionCall.Add(new Token(";", TokenType.SEMICOLON, 0));
                    }
                    result.Add(functionCall);
                }
                else
                {
                    convertedArguments.Add(argument);
                }
            }
            return result;
        }

        private int getHighestType(List<Token> argument, ref List<List<Token>> inputLines)
        {
            int highestType = 0;
            List<Token> inputAsTokenList = new List<Token>();
            foreach(List<Token> line in inputLines)
            {
                inputAsTokenList.AddRange(line);
            }

            foreach(Token token in argument)
            {
                if(token.type == TokenType.NUMBER || token.type == TokenType.NAME)
                {
                    highestType = Math.Max(getPrimitiveType(token, ref inputAsTokenList), highestType);
                }
            }
            return highestType;
        }

        private List<List<Token>> getArguments(ref List<Token> line) // gets the line
        {
            List<Token> tokens = new List<Token>();
            tokens.AddRange(line);
            List<List<Token>> result = new List<List<Token>>();
            tokens.RemoveAt(0);//remove funtion name
            tokens.RemoveAt(0);//remove bracket
            tokens.RemoveAt(tokens.Count - 1);//remove semicolon
            tokens.RemoveAt(tokens.Count - 1);//remove bracket

            if(containsComma(ref tokens))
            {
                result = splitAtComma(ref tokens);
            }
            else
            {
                result.Add(tokens);
            }

            return result;
        }

        private List<List<Token>> splitAtComma(ref List<Token> tokens)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<Token> current = new List<Token>();

            foreach(Token token in tokens)
            {
                if(token.type == TokenType.COMMA)
                {
                    result.Add(current);
                    current.Clear();
                }
                else
                {
                    current.Add(token);
                }
            }
            return result;
        }

        private bool containsComma(ref List<Token> tokens)
        {
            foreach(Token token in tokens)
            {
                if(token.type == TokenType.COMMA)
                {
                    return true;
                }
            }
            return false;
        }

        private int findInlineOp(ref List<List<Token>> input)
        {
            int counter = 0;
            foreach(List<Token> line in input)
            {
                if(isFunctionCall(line) && containsOperator(line))
                {
                    return counter;
                }
                counter++;
            }
            return -1;
        }

        private bool containsOperator(List<Token> line)
        {
            foreach(Token token in line)
            {
                if(
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

        private bool isFunctionCall(List<Token> line)
        {
            bool containsBrace = false;
            bool doesntContainEquals = true;
            foreach(Token token in line)
            {
                if(token.type == TokenType.BRACE_LEFT)
                {
                    containsBrace = true;
                }
                if (token.type == TokenType.OPERATOR_SINGLE_EQUALS)
                {
                    doesntContainEquals = false;
                }
            }
            return containsBrace && doesntContainEquals;
        }

        private List<List<Token>> splitToLines(ref List<Token> input)
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
        private int getPrimitiveType(Token token, ref List<Token> input)
        {
            if (token.type == TokenType.NUMBER)
            {
                return token.content.Contains(".") ? 2 : 1;
            }
            else if (token.type == TokenType.NAME && (token.content == "true" || token.content == "false"))
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
            for (int i = 0; i < input.Count; i++)
            {
                if (
                    i + 4 < input.Count - 1 &&
                    input[i].content == "refa" &&
                    input[i + 2].content == varName.content
                )
                {
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
    }
}
