using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class InlineNumberConverter : PostLSSLayer
    {
        HashSet<string> numbers = new HashSet<string>();
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> lines = convertToLines(ref input);
            List<List<Token>> linesWithRemovedNumbers = new List<List<Token>>();

            throw new NotImplementedException();
        }

        public string numberToVarName(int n)
        {
            return "____numBuffer_" + n;
        }
        private List<List<Token>> convertToLines(ref List<Token> input)
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
    }
}
