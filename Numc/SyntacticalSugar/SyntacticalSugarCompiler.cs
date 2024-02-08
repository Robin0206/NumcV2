using Numc.SyntacticalSugar.PostLexer;
using Numc.SyntacticalSugar.PostLexer.Implementations;
using Numc.SyntacticalSugar.PostLexer.Implementations.HelperClasses;
using Numc.SyntacticalSugar.Prelexer;
using Numc.SyntacticalSugar.Prelexer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numc.lexer;

namespace Numc.SyntacticalSugar
{
    class SyntacticalSugarCompiler
    {
        private int funcNameCounter = 0;
        private List<string[]> functions = new List<string[]>();
        private Lexer lexer = new Lexer();
        private PreLSSLayer[] preLexerConversionChain = new PreLSSLayer[]
        {
            new SemicolonAdder(),
            new WhitespaceAdder()
        };
        private PostLSSLayer[] postLexerConversionChain = new PostLSSLayer[]
        {
            new FunctionCallRemover(),
            new WithRemover(),
            new DoToForConverter(),
            new ForToWhileConverter(),
            new WhileToIfConverter(),
            new ElseToIfConverter(),
            new IfRemover(),
            new ArgumentRemover(),
            new ReturnConverter(),
            new InlineOperationRemover(),
            new ExpressionSimplifier(),
            new TrueAndFalseRemover(),
            new VariableAssignmentConverter(),
            new InlineNumberConverter(),
            new OperationConverter(),
            new RefaUpPuller(),
            new FunctionSignatureConverter(),
            new CommaAndBraceRemover(),
            new FunctionCallNameConverter(),
            new VariableNameConverter()

        };
        public string[] compileProgram(string[] lines)
        {
            List<List<string>> splittedFunctions = splitFunctions(ref lines);
            foreach (List<string> function in splittedFunctions)
            {
                string[] compiledFunction = compileFunction(function.ToArray());
                functions.Add(compiledFunction);
            }
            List<string> functionList = new List<string>();
            foreach (string[] function in functions)
            {
                foreach (string line in function)
                {
                    functionList.Add(line);
                }
            }
            return functionList.ToArray();
        }

        private List<List<string>> splitFunctions(ref string[] lines)
        {
            List<List<string>> result = new List<List<string>>();
            List<string> currentFunction = new List<string>();
            foreach(string line in lines)
            {
                if (line.Length > 4 && startsWithFunc(line) && currentFunction.Count > 0)
                {
                    result.Add(new List<string>());
                    result[result.Count - 1].AddRange(currentFunction);
                    currentFunction.Clear();
                }
                currentFunction.Add(line);
            }
            result.Add(currentFunction);
            return result;
        }

        private bool startsWithFunc(string line)
        {
            return 
                line[0] == 'f' &&
                line[1] == 'u' &&
                line[2] == 'n' &&
                line[3] == 'c';
        }

        public string[] compileFunction(string[] lines)
        {
            resetConversionChains();
            foreach(PreLSSLayer pre in preLexerConversionChain)
            {
                lines = pre.removeSugar(lines);
            }
            List<Token> tokens = lexer.tokenize(ref lines);
            foreach (PostLSSLayer post in postLexerConversionChain)
            {
                tokens = post.removeSugar(tokens);
            }
            List<string> resultList = new List<string>();
            StringBuilder currentString = new StringBuilder();
            foreach(Token token in tokens)
            {
                if(token.type != TokenType.SEMICOLON)
                {
                    currentString.Append(token.content + " ");
                }
                else
                {
                    resultList.Add(currentString.ToString());
                    currentString.Clear();
                }
            }
            string[] result = new string[resultList.Count];
            for(int i = 0; i < resultList.Count; i++) {
                result[i] = resultList[i];
            }
            return result;
        }

        private void resetConversionChains()
        {
            postLexerConversionChain = new PostLSSLayer[]
            {
                new FunctionCallRemover(),
                new WithRemover(),
                new DoToForConverter(),
                new ForToWhileConverter(),
                new WhileToIfConverter(),
                new ElseToIfConverter(),
                new IfRemover(),
                new ArgumentRemover(),
                new ReturnConverter(),
                new InlineOperationRemover(),
                new ExpressionSimplifier(),
                new TrueAndFalseRemover(),
                new VariableAssignmentConverter(),
                new InlineNumberConverter(),
                new OperationConverter(),
                new RefaUpPuller(),
                new FunctionSignatureConverter(),
                new CommaAndBraceRemover(),
                new FunctionCallNameConverter(),
                new VariableNameConverter()
            };
            preLexerConversionChain = new PreLSSLayer[]
            {
                new SemicolonAdder(),
                new WhitespaceAdder()
            };
        }

        static void printTokens(List<Token> tokens)
        {
            int line = 0;
            int tabLevel = 0;
            Console.Write(line + " ");
            line++;
            foreach (Token token in tokens)
            {
                if (token.content == "{")
                {
                    tabLevel++;
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if (token.content == "}")
                {
                    tabLevel--;
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else if (token.content == ";")
                {
                    Console.Write(token.content);
                    Console.WriteLine();
                    Console.Write(line + " ");
                    line++;
                    for (int i = 0; i < tabLevel; i++)
                    {
                        Console.Write("    ");
                    }
                }
                else
                {
                    Console.Write(token.content + " ");
                }
            }
        }
    }
}

