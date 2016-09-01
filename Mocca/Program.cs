using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocca.Compiler;
using Mocca.Compiler.Language;

namespace Mocca {
    class Program {
        public static void Main(string[] args) {
            var parser = new MoccaParser("../../../Example/middle_lang.mocca", CompileMode.FILE_PASS);
            var tree = parser.Parse();
			var compiler = new MoccaCompiler(tree, new PythonCompiler());
			var code = compiler.compile();
			Console.WriteLine(code);
        }
    }
}
