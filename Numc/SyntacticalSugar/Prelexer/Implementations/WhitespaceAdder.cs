using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.SyntacticalSugar.Prelexer.Implementations
{
    class WhitespaceAdder : PreLSSLayer
    {
        static char[] symbols = "{}();%+-/*=<>|&!^,".ToCharArray();
        //Adds whitespace between operators, names, braces etc.... to make tokenization easier
        public string[] removeSugar(string[] input)
        {
            List<String> resultBuffer = new List<string>();
            foreach(string line in input)
            {
                resultBuffer.Add(addSugarToLine(line));
            }

            return resultBuffer.ToArray();
        }

        private string addSugarToLine(string line)
        {
            StringBuilder result = new StringBuilder("");
            for(int i = 0; i < line.Length; i++)
            {
                if(i != line.Length - 1)
                {
                    if(symbols.Contains(line[i]) && line[i] == line[i + 1] && !(line[i] == '(' || line[i] == ')' || line[i] == '{' || line[i] == '}'))
                    {
                        result.Append("  " + line[i] + line[i + 1] + "  ");
                        i++;
                    }
                    else if (symbols.Contains(line[i]))
                    {
                        result.Append("  " + line[i] + "  ");
                    }
                    else
                    {
                        result.Append(line[i]);
                    }
                }
                else
                {
                    if (symbols.Contains(line[i]))
                    {
                        result.Append("  " + line[i] + "  ");
                    }
                    else
                    {
                        result.Append(line[i]);
                    }
                }
                
            }
            return result.ToString();
        }
    }
}
