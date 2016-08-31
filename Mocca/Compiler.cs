using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Compiler;

namespace Mocca {
	public interface BasicCompiler {
		object Eval(ParseNode nodes, ParseTree tree, params object[] paramlist);
	}
}

