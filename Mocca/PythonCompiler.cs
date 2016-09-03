using System;
using System.Collections.Generic;
using Mocca.DataType;

namespace Mocca.Compiler {
	public class PythonCompiler : BasicCompiler {
		List<MoccaBlockGroup> codeBase;

		public PythonCompiler(List<MoccaBlockGroup> codeBase) {
			this.codeBase = codeBase;
		}

		public override string Eval(List<MoccaBlockGroup> codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalArray(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalAtom(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalBlock(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalBlockgroup(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalDictionary(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalExpression(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalParam(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalParams(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalStatement(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalStatementList(object codeBase) {
			throw new NotImplementedException();
		}

		public override string EvalSymbol(object codeBase) {
			throw new NotImplementedException();
		}
	}
}

