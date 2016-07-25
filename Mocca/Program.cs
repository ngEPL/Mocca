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
            Parser p = new Parser("..\\..\\..\\Example\\middle_lang.mocca", ParseMode.SOURCE_FILE);
            var a = p.Parse(); // List<Token>
            foreach(Token i in a) {
                Console.WriteLine(i.GetType().ToString() + " 타입의 " + i.GetValue().ToString());
            }
            
        }
    }
}
