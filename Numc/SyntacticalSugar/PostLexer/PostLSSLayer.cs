using Numc.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.PostLexer
{
    interface PostLSSLayer
    {
        //Post Lexer Syntactical Sugar Layer
        //Removes syntactical sugar from the tokens
        //should be applied in the following order:
        //1. with remover
        //2. do to for converter
        //3. fortowhileconverter
        List<Token> removeSugar(List<Token> input);
    }
}
