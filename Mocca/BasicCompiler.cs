using System;
using System.Collections.Generic;
using Mocca.DataType;

namespace Mocca.Compiler {
	public abstract class BasicCompiler {
		List<MoccaBlockGroup> codeBase;

		public abstract string Compile();

		public abstract string EvalStart(List<MoccaBlockGroup> codeBase);
		public abstract string EvalBlockgroup(MoccaBlockGroup codeBase);
		public abstract string EvalAtom(object codeBase);
		public abstract string EvalArray(MoccaArray codeBase);
		public abstract string EvalDictionary(MoccaDictionary codeBase);
		public abstract string EvalTuple(MoccaTuple codeBase);
		public abstract string EvalCommand(MoccaCommand codeBase);
		public abstract string EvalLogic(MoccaLogic codeBase);
		public abstract string EvalWhile(MoccaWhile codeBase);
		public abstract string EvalFor(MoccaFor codeBase);
		public abstract string EvalEquation(MoccaEquation codeBase);
	}
}

