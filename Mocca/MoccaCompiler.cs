using System;
using System.Collections.Generic;
using System.Text;
using Mocca.DataType;

namespace Mocca {
	public enum TargetLanguage {
		Python,		// Now available
		Java,		// NOT available
		Javascript,	// NOT available
		C_sharp,	// NOT available
		Swift		// NOT Available
	}

	public interface Compiler {
		string compile(List<MoccaBlockGroup> source);
	}

	public class MoccaCompiler {
		List<MoccaBlockGroup> source;
		TargetLanguage lang;

		public MoccaCompiler(List<MoccaBlockGroup> source, TargetLanguage lang) {
			this.source = source;
			this.lang = lang;
		}
	}
}

