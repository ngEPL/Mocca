using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser("C:\\Users\\hj332\\Workspace\\EPL\\Example\\middle_lang.mocca", ParseMode.SOURCE_FILE);
            p.parse();
        }
    }
}
