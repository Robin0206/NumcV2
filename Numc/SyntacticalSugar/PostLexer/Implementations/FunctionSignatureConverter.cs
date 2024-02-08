using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class FunctionSignatureConverter : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            //add the first line
            if(input[1].content != "main")
            {
                result.Add(new Token("func", TokenType.KEYWORD, 0));
                result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                result.Add(input[1]);
                result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                result.Add(new Token(";", TokenType.SEMICOLON, 0));
            }
            else
            {
                result.Add(new Token("main", TokenType.KEYWORD, 0));
                result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                result.Add(new Token(";", TokenType.SEMICOLON, 0));
            }
            //search for the first { and set the index to its position + 1
            int index = 0;
            while(input[index].type != TokenType.CURLY_BRACE_LEFT)
            {
                index++;
            }
            index++;
            //add the lines in between
            for(int i = index; i < input.Count - 1; i++)
            {
                result.Add(input[i]);
            }
            //add the last lines
            if (input[1].content != "main")
            {
                result.Add(new Token("fend", TokenType.KEYWORD, 0));
                result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                result.Add(new Token(";", TokenType.SEMICOLON, 0));
            }
            else
            {
                result.Add(new Token("mend", TokenType.KEYWORD, 0));
                result.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                result.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                result.Add(new Token(";", TokenType.SEMICOLON, 0));
            }
            
            return result;
        }
    }
}
