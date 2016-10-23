using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocca.Compiler;
using Mocca.Simulator.Arduino;

namespace Mocca {
    class Program {
        public static void Main(string[] args) {
            var parser = new MoccaParser("../../../Example/microbit.mocca", CompileMode.FILE_PASS);
			var tree = parser.Parse();
			var eval = tree.Eval();
			//Console.WriteLine(tree.PrintTree());
			//var compiler = new PythonCompiler(eval);
			//Console.WriteLine(compiler.Compile());
			var portDic = new Dictionary<Port, Accessory>();
			portDic.Add(Port.Digital13, Accessory.LED);
			portDic.Add(Port.Digital12, Accessory.LED);

			var arduino = new ArduinoSimulator(portDic);
			arduino.setCode(eval);
			arduino.run();
        }
    }
}