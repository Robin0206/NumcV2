using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class TrueAndFalseRemover : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            foreach(Token t in input)
            {
                if(t.content == "true")
                {
                    result.Add(new Token("1", TokenType.NUMBER, 0));
                }else if (t.content == "false")
                {
                    result.Add(new Token("0", TokenType.NUMBER, 0));
                }
                else
                {
                    result.Add(t);
                }
            }
            return result;
        }
    }
}
