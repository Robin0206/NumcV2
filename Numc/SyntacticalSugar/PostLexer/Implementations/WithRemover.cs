using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    //Assumes refa of form refa(name, type, length)
    class WithRemover : PostLSSLayer
    {
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();
            List<Token> withBlock = getWithBlock(input);
            List<Token> refaCalls = new List<Token>();
            int line = 2;
            foreach(List<Token> variableList in getVariableLists(withBlock))
            {
                for(int i = 1; i < variableList.Count; i++)
                {
                    addRefaCall(variableList[i], variableList[0], ref refaCalls, line);
                    line++;
                }
            }
            //add until with
            foreach(Token t in input)
            {
                if(t.content == "with")
                {
                    break;
                }
                result.Add(t);
            }
            //add starting {
            result.Add(new Token("{", TokenType.CURLY_BRACE_LEFT, 1));
            //add refa calls
            result.AddRange(refaCalls);
            //add rest
            int firstLeftBraceIndex = 0;
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].content == "{")
                {
                    firstLeftBraceIndex = i;
                    break;
                }
            }
            int lineOffset = result[result.Count - 1].line - (firstLeftBraceIndex + 1);
            for(int i = firstLeftBraceIndex+1; i < input.Count; i++)
            {
                input[i].setLine(input[i].line + lineOffset);
                result.Add(input[i]);
            }
            return result;
        }

        private void addRefaCall(Token varName, Token type, ref List<Token> refaCalls, int line)
        {
            string typeNum = "0";
            switch (type.content)
            {
                case "bool":
                    typeNum = "0";
                    break;
                case "int":
                    typeNum = "1";
                    break;
                case "real":
                    typeNum = "2";
                    break;
                default:
                    typeNum = "3";
                    break;
            }
            refaCalls.Add(new Token("refa", TokenType.NAME, line));
            refaCalls.Add(new Token("(", TokenType.BRACE_LEFT, line));
            refaCalls.Add(new Token(varName.content, TokenType.NAME, line));
            refaCalls.Add(new Token(",", TokenType.COMMA, line));
            refaCalls.Add(new Token(typeNum, TokenType.NUMBER, line));
            refaCalls.Add(new Token(",", TokenType.COMMA, line));
            refaCalls.Add(new Token("0", TokenType.NUMBER, line));
            refaCalls.Add(new Token(")", TokenType.BRACE_RIGHT, line));
            refaCalls.Add(new Token(";", TokenType.SEMICOLON, line));
        }

        private List<List<Token>> getVariableLists(List<Token> withBlock)
        {
            List<List<Token>> lines = new List<List<Token>>();
            List<Token> currentLine = new List<Token>();
            int lastLine = 0;
            foreach(Token t in withBlock)
            {
                if(lastLine != t.line)
                {
                    lastLine = t.line;
                    lines.Add(currentLine);
                    currentLine = new List<Token>();
                }
                if(t.content != "," && t.content != "(" && t.content != ")")
                {
                    currentLine.Add(t);
                }
            }
            lines.Add(currentLine);
            return lines;
        }

        private List<Token> getWithBlock(List<Token> input)
        {
            List<Token> result = new List<Token>();
            int firstTokenInBlockIndex = 0;
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].content == "with")
                {
                    firstTokenInBlockIndex = i + 2;
                    break;
                }
            }
            for (int i = firstTokenInBlockIndex; i < input.Count; i++)
            {
                if(input[i+1].content == "{")
                {
                    break;
                }
                result.Add(input[i]);
            }
            return result;
        }
    }
}
