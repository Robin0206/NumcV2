using Numc.lexer;
using Numc.SyntacticalSugar.PostLexer.Implementations.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class FunctionCallRemover : PostLSSLayer
    {
        FunctionCallConverter functionCallConverter = new FunctionCallConverter();
        public List<Token> removeSugar(List<Token> input)
        {
            List<List<Token>> lines = convertToLines(ref input);
            while(findFunctionLineIndex(ref lines) != -1)
            {
                convertFunction(ref lines);
            }
            List<Token> result = convertLinesToTokenList(ref lines);
            return result;
        }

        private List<Token> convertLinesToTokenList(ref List<List<Token>> lines)
        {
            List<Token> result = new List<Token>();
            foreach(List<Token> line in lines)
            {
                result.AddRange(line);
            }
            return result;
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

        private void convertFunction(ref List<List<Token>> lines)
        {
            
            List<List<Token>> functionBuffer = new List<List<Token>>();
            int functionLineIndex = findFunctionLineIndex(ref lines);
            List<List<Token>> convertedFunction = functionCallConverter.convertFunctionLine(lines[functionLineIndex]);
            for(int i = 0; i < lines.Count; i++)
            {
                if(i != functionLineIndex)
                {
                    functionBuffer.Add(lines[i]);
                }
                else
                {
                    functionBuffer.AddRange(convertedFunction);
                }
            }
            lines.Clear();
            lines.AddRange(functionBuffer);
            

        }

        

        private int findFunctionLineIndex(ref List<List<Token>> lines)
        {
            
            //return the index of the first line that doesnt contain "define" but braces
            for(int i = 0; i < lines.Count; i++)
            {
                if(
                    !containsToken("func", lines[i]) && 
                    containsToken("(", lines[i]) && 
                    containsToken(")", lines[i]) && 
                    !containsToken("call", lines[i]) &&
                    !containsToken("parg", lines[i]) &&
                    !containsToken("retG", lines[i])
                )
                {
                    return i;
                }
            }
            return -1;
        }

        private bool containsToken(string tokenContent, List<Token> line)
        {
            foreach (Token token in line)
            {
                if (token.content == tokenContent)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
