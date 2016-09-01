using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Compiler;

namespace Mocca {
	public interface BasicCompiler {
		object GetValue(ParseTree tree, TokenType type, int index);
		object GetValue(ParseTree tree, TokenType type, ref int index);
		object generate(ParseTree tree);
		object Eval(ParseTree tree, params object[] paramlist);
	}
}

