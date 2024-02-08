using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations.HelperClasses
{
    class FunctionCallConverter
    {
        internal List<List<Token>> convertFunctionLine(List<Token> tokens)
        {
            if(containsToken("=", tokens))
            {
                return convertFunctionLineWithEquals(ref tokens);
            }
            else
            {
                return convertFunctionLineWithoutEquals(ref tokens);
            }
        }

        private List<List<Token>> convertFunctionLineWithEquals(ref List<Token> tokens)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<string> argumentNames = getArgumentNames(ref tokens);
            foreach (string argName in argumentNames)
            {
                addPargCall(argName, ref result);
            }
            addCall(tokens[2], ref result);
            addReturnGetCall(tokens[0], ref result);
            return result;
        }

        

        private List<List<Token>> convertFunctionLineWithoutEquals(ref List<Token> tokens)
        {
            List<List<Token>> result = new List<List<Token>>();
            List<string> argumentNames = getArgumentNames(ref tokens);
            foreach (string argName in argumentNames)
            {
                addPargCall(argName, ref result);
            }
            addCall(tokens[0], ref result);

            return result;
        }

        private void addReturnGetCall(Token name, ref List<List<Token>> result)
        {
            List<Token> retGLine = new List<Token>();
            retGLine.Add(new Token("retG", TokenType.NAME, 0));
            retGLine.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            retGLine.Add(name);
            retGLine.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            retGLine.Add(new Token(";", TokenType.SEMICOLON, 0));
            result.Add(retGLine);
        }

        private void addCall(Token name, ref List<List<Token>> result)
        {
            List<Token> callLine = new List<Token>();
            callLine.Add(new Token("call", TokenType.NAME, 0));
            callLine.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            callLine.Add(name);
            callLine.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            callLine.Add(new Token(";", TokenType.SEMICOLON, 0));
            result.Add(callLine);
        }

        private void addPargCall(string argName, ref List<List<Token>> result)
        {
            List<Token> pArgCall = new List<Token>();
            pArgCall.Add(new Token("parg", TokenType.NAME, 0));
            pArgCall.Add(new Token("(", TokenType.BRACE_LEFT, 0));
            pArgCall.Add(new Token(argName, TokenType.NAME, 0));
            pArgCall.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
            pArgCall.Add(new Token(";", TokenType.SEMICOLON, 0));
            result.Add(pArgCall);
        }

        private List<string> getArgumentNames(ref List<Token> tokens)
        {
            int index = 0;
            //run the index until after the first left brace
            while(tokens[index].type != TokenType.BRACE_LEFT)
            {
                index++;
            }
            index++;
            //add every token after the index to the resultBuffer
            List<Token> resultBuffer = new List<Token>();
            for(; index < tokens.Count; index++)
            {
                resultBuffer.Add(tokens[index]);
            }
            //add every token that is a name to resultTokenArray
            List<Token> resultTokenArray = new List<Token>();
            foreach(Token token in resultBuffer)
            {
                if(token.type == TokenType.NAME)
                {
                    resultTokenArray.Add(token);
                }
            }
            //convert to string
            List<string> result = new List<string>();
            foreach(Token token in resultTokenArray)
            {
                result.Add(token.content);
            }
            return result;
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
