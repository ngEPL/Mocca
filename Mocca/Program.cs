using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    class Program {
        public static void Main(string[] args) {
            var c = new MoccaParser("../../../Example/middle_lang.mocca", CompileMode.FILE_PASS);
            var ret = c.Parse();
            Console.WriteLine(ret);
        }
    }
}
