﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocca.Compiler;

namespace Mocca {
    class Program {
        public static void Main(string[] args) {
            var parser = new MoccaParser("../../../Example/middle_lang.mocca", CompileMode.FILE_PASS);
			var tree = parser.Parse();
			var eval = tree.Eval();
			Console.WriteLine(tree.PrintTree());
			var compiler = new PythonCompiler(eval);
			Console.WriteLine(compiler.Compile());
        }
    }
}