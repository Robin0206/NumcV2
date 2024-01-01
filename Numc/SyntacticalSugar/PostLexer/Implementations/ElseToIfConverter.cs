using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class ElseToIfConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {

            List<Token> result = new List<Token>();
            List<Token> resultBuffer = new List<Token>();
            result.AddRange(input);
            bool foundIfWithElse = true;

            while (foundIfWithElse)
            {
                int[] coords = findIfWithElse(ref result);
                if (coords[0] == -1)
                {
                    foundIfWithElse = false;
                    return result;
                }

                List<Token> transformedElseBody = transformElseBody(ref result, coords);
                for(int i = 0; i < coords[1]; i++)
                {
                    resultBuffer.Add(result[i]);
                }
                resultBuffer.AddRange(transformedElseBody);
                for (int i = coords[1] + 1; i < result.Count; i++)
                {
                    resultBuffer.Add(result[i]);
                }

                result.Clear();
                result.AddRange(resultBuffer);
                resultBuffer.Clear();

            }

            return result;
        }

        private List<Token> transformElseBody(ref List<Token> input, int[] coords)
        {
            List<Token> expression = new List<Token>();
            List<Token> result = new List<Token>();
            int index = coords[0] + 1;

            while(input[index].type != TokenType.CURLY_BRACE_LEFT)
            {
                expression.Add(input[index]);
                index++;
            }

            result.Add(new Token("if", TokenType.KEYWORD, 0));
            result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            result.AddRange(expression);

            result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));

            result.Add(new Token("==", TokenType.OPERATOR_DOUBLE_EQUALS, 0));
            result.Add(new Token("false", TokenType.KEYWORD, 0));

            return result;
        }

        private int[] findIfWithElse(ref List<Token> input)
        {
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].content == "if")
                {
                    int elsePos = hasElse(ref input, i);
                    if (elsePos != -1)
                    {
                        return new int[] { i, elsePos };
                    }
                }
            }
            return new int[] { -1, -1 };
        }

        private int hasElse(ref List<Token> input, int i)
        {
            int bracketCounter = 0;
            bool inBody = false;
            int index = 0;
            foreach (Token token in input)
            {
                if(index < i)
                {
                    index++;
                    continue;
                }
                if(!inBody && token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    inBody = true;
                    bracketCounter++;
                }
                else if(inBody && token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    bracketCounter++;
                }else if (inBody && token.type == TokenType.CURLY_BRACE_RIGHT)
                {
                    bracketCounter--;
                }
                else if(inBody && bracketCounter == 0)
                {
                    return token.content == "else" ? index : -1;
                }
                index++;
            }
            return -1;
        }
    }
}
