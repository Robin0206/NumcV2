using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class ReturnConverter : PostLSSLayer
    {
        
        public List<Token> removeSugar(List<Token> input)
        {
            bool inReturnLine = false;
            List<Token> result = new List<Token>();
            foreach(Token token in input)
            {
                if (inReturnLine)
                {
                    if (token.type == TokenType.SEMICOLON)
                    {
                        inReturnLine = false;
                        result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                    }
                    result.Add(token);
                }
                else
                {
                    result.Add(token);
                    if (token.content == "return")
                    {
                        inReturnLine = true;
                        result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                    }
                }
            }

            return result;
        }

    }
}
