using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocca.Compiler.Language;

namespace Mocca {
    class Program {
        public static void Main(string[] args) {
            var c = new MoccaParser("../../../Example/middle_lang.mocca", CompileMode.FILE_PASS);
            var tree = c.Parse();
			var compiler = new PythonCompiler();
			Console.WriteLine(compiler.Eval(tree.Nodes[0], tree));
        }
    }
}
