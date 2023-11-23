using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.Prelexer
{
    interface PreLSSLayer
    {
        //Pre Syntactical Sugar Layer
        //Removes syntactical sugar from the raw lines before they are tokenized
        string[] removeSugar(string[] input);
    }
}
