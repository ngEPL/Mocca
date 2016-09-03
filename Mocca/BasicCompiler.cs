using System;
using System.Collections.Generic;
using Mocca.DataType;

namespace Mocca.Compiler {
	public abstract class BasicCompiler {
		List<MoccaBlockGroup> codeBase;

		public string Compile() {
			return this.Eval(codeBase);
		}

		public abstract string Eval(List<MoccaBlockGroup> codeBase);
		public abstract string EvalBlockgroup(object codeBase);
		public abstract string EvalParams(object codeBase);
		public abstract string EvalParam(object codeBase);
		public abstract string EvalExpression(object codeBase);
		public abstract string EvalSymbol(object codeBase);
		public abstract string EvalAtom(object codeBase);
		public abstract string EvalArray(object codeBase);
		public abstract string EvalDictionary(object codeBase);
		public abstract string EvalBlock(object codeBase);
		public abstract string EvalStatementList(object codeBase);
		public abstract string EvalStatement(object codeBase);
	}
}

