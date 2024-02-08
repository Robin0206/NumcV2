using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class CommaAndBraceRemover : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            foreach(Token token in input)
            {
                if(
                    token.type != TokenType.BRACE_LEFT &&
                    token.type != TokenType.BRACE_RIGHT &&
                    token.type != TokenType.COMMA
                )
                {
                    result.Add(token);
                }
            }
            return result;
        }
    }
}
