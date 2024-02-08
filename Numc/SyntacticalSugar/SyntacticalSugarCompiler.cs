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
        Lexer lexer = new Lexer();
        PreLSSLayer[] preLexerConversionChain = new PreLSSLayer[]
        {
            new SemicolonAdder(),
            new WhitespaceAdder()
        };
        PostLSSLayer[] postLexerConversionChain = new PostLSSLayer[]
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
            new VariableAssignmentConverter(),
            new InlineNumberConverter(),
            new OperationConverter(),
            new RefaUpPuller(),
            new FunctionSignatureConverter(),
            new CommaAndBraceRemover(),
            new FunctionCallNameConverter()
        };
        public string[] compileFunction(string[] lines)
        {
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
    }
}
