using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer.Implementations
{
    class ArgumentRemover : PostLSSLayer
    {
        static int bufferNum = 0;
        public List<Token> removeSugar(List<Token> input)
        {
            List<Token> result = new List<Token>();

            //split argumentList and Program and return the input if the argumentList is empty
            List<Token> rawArgumentList = new List<Token>();
            List<Token> programWithoutArgumentList = new List<Token>();
            splitArgumentListAndProgram(ref input, ref programWithoutArgumentList, ref rawArgumentList);
            if(rawArgumentList.Count == 0)
            {
                return input;
            }

            //Get the argument structs
            List<Argument> argStructs = convertRawArgListToStructs(ref rawArgumentList);

            //generate the refas
            List<Token> newRefas = new List<Token>();
            foreach (Argument arg in argStructs)
            {
                newRefas.Add(new Token("refa", TokenType.NAME, 0));
                newRefas.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                newRefas.Add(new Token(arg.name, TokenType.NAME, 0));
                newRefas.Add(new Token(",", TokenType.COMMA, 0));
                newRefas.Add(new Token( ""+arg.type, TokenType.NUMBER, 0));
                newRefas.Add(new Token(",", TokenType.COMMA, 0));
                newRefas.Add(new Token("0", TokenType.NUMBER, 0));
                newRefas.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                newRefas.Add(new Token(";", TokenType.SEMICOLON, 0));
                bufferNum++;
            }

            //generate the gArg Calls
            List<Token> gArgCalls = new List<Token>();
            bufferNum = 0;
            foreach (Argument arg in argStructs)
            {
                gArgCalls.Add(new Token("gArg", TokenType.NAME, 0));
                gArgCalls.Add(new Token("(", TokenType.BRACE_LEFT, 0));
                gArgCalls.Add(new Token(arg.name, TokenType.NAME, 0));
                gArgCalls.Add(new Token(",", TokenType.COMMA, 0));
                gArgCalls.Add(new Token(bufferNum + "", TokenType.NUMBER, 0));
                gArgCalls.Add(new Token(")", TokenType.BRACE_RIGHT, 0));
                gArgCalls.Add(new Token(";", TokenType.SEMICOLON, 0));
                bufferNum++;
            }

            //split the program without argumentList to beforeRefas, refas, afterRefas
            List<Token> beforeRefas= new List<Token>();
            List<Token> oldRefas = new List<Token>();
            List<Token> afterRefas = new List<Token>();
            splitRawProgram(ref programWithoutArgumentList, ref beforeRefas, ref oldRefas, ref afterRefas);

            //write to result beforeRefas, oldRefas, newRefas, gArgCalls, afterRefas
            result.AddRange(beforeRefas);
            result.AddRange(oldRefas);
            result.AddRange(newRefas);
            result.AddRange(gArgCalls);
            result.AddRange(afterRefas);
            return result;
        }

        private void splitRawProgram(ref List<Token> programWithoutArgumentList, ref List<Token> beforeRefas, ref List<Token> oldRefas, ref List<Token> afterRefas)
        {
            ARGUMENTREMOVERSTATE state = ARGUMENTREMOVERSTATE.BEFOREREFAS;
            Token lastToken = programWithoutArgumentList[0];
            if (!noRefaCalls(ref programWithoutArgumentList))
            {
                foreach(Token token in programWithoutArgumentList)
                {
                    switch (state)
                    {
                        case ARGUMENTREMOVERSTATE.BEFOREREFAS:
                            if(token.type == TokenType.CURLY_BRACE_LEFT)
                            {
                                beforeRefas.Add(token);
                                state = ARGUMENTREMOVERSTATE.INREFAS;
                                lastToken = token;
                            }
                            else
                            {
                                beforeRefas.Add(token);
                                lastToken = token;
                            }
                            break;
                        case ARGUMENTREMOVERSTATE.INREFAS:
                            if(token.content != "refa" && lastToken.type == TokenType.SEMICOLON)
                            {
                                afterRefas.Add(token);
                                state = ARGUMENTREMOVERSTATE.AFTERREFAS;
                            }
                            else
                            {
                                oldRefas.Add(token);
                                lastToken = token;
                            }
                            break;
                        case ARGUMENTREMOVERSTATE.AFTERREFAS:
                            afterRefas.Add(token);
                            break;
                    }
                }
            }
            else
            {
                foreach (Token token in programWithoutArgumentList)
                {
                    switch (state)
                    {
                        case ARGUMENTREMOVERSTATE.BEFOREREFAS:
                            if (token.type == TokenType.CURLY_BRACE_LEFT)
                            {
                                state = ARGUMENTREMOVERSTATE.AFTERREFAS;
                            }
                            beforeRefas.Add(token);
                            break;
                        case ARGUMENTREMOVERSTATE.AFTERREFAS:
                            afterRefas.Add(token);
                            break;
                    }
                }
            }
        }

        private bool noRefaCalls(ref List<Token> programWithoutArgumentList)
        {
            foreach(Token token in programWithoutArgumentList)
            {
                if(token.content == "refa")
                {
                    return true;
                }
            }
            return false;
        }

        private List<Argument> convertRawArgListToStructs(ref List<Token> rawArgumentList)
        {
            List<Argument> result = new List<Argument>();
            for(int i = 0; i < rawArgumentList.Count; i++)
            {
                if (rawArgumentList[i].content == "bool" || rawArgumentList[i].content == "int" || rawArgumentList[i].content == "real")
                {
                    result.Add(new Argument(rawArgumentList[i], rawArgumentList[i + 1]));
                }
            }
            return result;
        }

        private void splitArgumentListAndProgram(ref List<Token> input, ref List<Token> programWithoutArgumentList, ref List<Token> rawArgumentList)
        {
            bool foundArgumentList = false;
            ARGUMENTREMOVERSTATE state = ARGUMENTREMOVERSTATE.INPROGRAM;
            foreach(Token token in input)
            {
                //return without doing anything if there is no argument list
                if(!foundArgumentList && token.type == TokenType.CURLY_BRACE_LEFT)
                {
                    return;
                }
                switch (state)
                {
                    case ARGUMENTREMOVERSTATE.INPROGRAM:
                        if(!foundArgumentList && token.type == TokenType.BRACE_LEFT)
                        {
                            state = ARGUMENTREMOVERSTATE.INARGUMENTLIST;
                            foundArgumentList = true;
                        }
                        else
                        {
                            programWithoutArgumentList.Add(token);
                        }
                        break;
                    case ARGUMENTREMOVERSTATE.INARGUMENTLIST:
                        if (token.type == TokenType.BRACE_RIGHT)
                        {
                            state = ARGUMENTREMOVERSTATE.INPROGRAM;
                        }
                        else
                        {
                            rawArgumentList.Add(token);
                        }
                        break;
                }
            }
        }
        struct Argument
        {
            internal int type;
            internal string name;
            internal  Argument(Token type, Token name)
            {
                if(type.content == "bool")
                {
                    this.type = 0;
                }
                else if(type.content == "int"){
                    this.type = 1;
                }
                else
                {
                    this.type = 2;
                }
                this.name = name.content;
            }

            internal void print()
            {
                string typeName = "bool";
                switch (this.type)
                {
                    case 1:
                        typeName = "int";
                        break;
                    case 2:
                        typeName = "real";
                        break;

                }
                Console.WriteLine(typeName + ": " + name);
            }
            
        }
        enum ARGUMENTREMOVERSTATE
        {
            INPROGRAM,
            INARGUMENTLIST,
            BEFOREREFAS,
            INREFAS,
            AFTERREFAS
        }
    }
}
