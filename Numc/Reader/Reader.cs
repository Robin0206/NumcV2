using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Numc.Reader
{
    class Reader
    {
        public static string[] readLinesRaw(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}
