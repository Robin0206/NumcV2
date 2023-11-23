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
        List<Token> removeSugar(List<Token> input);
    }
}
