using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class RefaUpPuller : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> lines = splitToLines(ref input);
            List<Token> signature = lines[0];
            List<Token> refas = getRefas(ref lines);
            List<Token> bodyWithoutRefas = getBodyWithoutRefas(ref lines);
            List<Token> result = new List<Token>();
            result.AddRange(signature);
            result.AddRange(refas);
            result.AddRange(bodyWithoutRefas);
            return result;
        }

        private List<Token> getBodyWithoutRefas(ref List<List<Token>> lines)
        {
            List<Token> result = new List<Token>();
            int counter = 0;
            foreach (List<Token> line in lines)
            {
                if (!(line[0].content == "refa") && counter != 0)
                {
                    result.AddRange(line);
                }
                counter++;
            }
            return result;
        }

        private List<Token> getRefas(ref List<List<Token>> lines)
        {
            List<Token> result = new List<Token>();
            foreach(List<Token> line in lines)
            {
                if(line[0].content == "refa")
                {
                    result.AddRange(line);
                }
            }
            return result;
        }

        private List<List<Token>> splitToLines(ref List<Token> input)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<Token> currentLine = new List<Token>();
            foreach(Token token in input)
            {
                currentLine.Add(token);
                if(token.content == ";" || token.content == "{" || token.content == "}"){
                    result.Add(new List<Token>());
                    foreach(Token lineToken in currentLine)
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
