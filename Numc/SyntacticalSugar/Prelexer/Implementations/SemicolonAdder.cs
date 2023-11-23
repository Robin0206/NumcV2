using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.Prelexer.Implementations
{
    class SemicolonAdder : PreLSSLayer
    {
        public string[] removeSugar(string[] input)
        {
            //find first curlybrace
            int firstCurlyBraceLine = 0;
            int counter = 0;
            foreach(string i in input)
            {
                if(i.Contains("}") || i.Contains("{"))
                {
                    firstCurlyBraceLine = counter;
                    break;
                }
                counter++;
            }
            //add semicolon to end of every line that doesnt contain a curly brace
            for(int i = firstCurlyBraceLine+1; i < input.Length; i++)
            {
                if(!(input[i].Contains("}") || input[i].Contains("{")))
                {
                    input[i] += ";";
                }
            }
            return input;
        }
    }
}
