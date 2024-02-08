using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class FunctionCallNameConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            bool lastTokenWasSemicolon = true;
            foreach(Token token in input)
            {
                if (lastTokenWasSemicolon)
                {
                    token.setContent(token.content.ToUpper());
                }
                if(token.type == TokenType.SEMICOLON)
                {
                    lastTokenWasSemicolon = true;
                }
                else
                {
                    lastTokenWasSemicolon = false;
                }
                result.Add(token);
            }
            return result;
        }
    }
}
