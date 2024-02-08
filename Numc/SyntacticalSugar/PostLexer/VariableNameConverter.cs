using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer
{
    class VariableNameConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            int counter = 0;
            List<Token> result = new List<Token>();
            result.AddRange(input);
            for (int i = 1; i < input.Count; i++)
            {
                if(input[i].type == TokenType.NAME && input[i-1].content == "REFA")
                {
                    substituteVarName(ref result, input[i].content, counter);
                    counter++;
                }
            }
            return result;
        }

        private void substituteVarName(ref List<Token> result, string name, int counter)
        {
            for(int i = 0; i < result.Count; i++)
            {
                if(result[i].content == name)
                {
                    result[i] = new Token(counter + "", TokenType.NUMBER, 0);
                }
            }
        }
    }
}
