using System;

namespace Mocca.Compiler {
	public class MoccaCompiler {
		ParseTree tree;
		BasicCompiler compiler;

		public MoccaCompiler(ParseTree tree, BasicCompiler compiler) {
			this.tree = tree;
			this.compiler = compiler;
		}

		public object compile() {
			return compiler.generate(tree);
		}
	}
}