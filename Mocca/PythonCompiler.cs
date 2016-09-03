using System;
using System.Collections.Generic;
using Mocca.DataType;

namespace Mocca.Compiler {
	public class PythonCompiler : BasicCompiler {
		List<MoccaBlockGroup> codeBase = new List<MoccaBlockGroup>();

		public PythonCompiler(List<MoccaBlockGroup> codeBase) {
			this.codeBase = codeBase;
		}

		#region GeneralCompiler

		int globalIndent = 0;
		List<string> modules = new List<string>();

		public override string Compile() {
			return this.EvalStart(codeBase);
		}

		public override string EvalStart(List<MoccaBlockGroup> codeBase) {
			List<string> groupName = new List<string>();

			string blockgroups = "";
			foreach (MoccaBlockGroup i in codeBase) {
				blockgroups += EvalBlockgroup(i) + "\n";
				groupName.Add(i.name);
			}

			string moduleImport = "";
			foreach (string i in modules) {
				moduleImport += "import " + i + "\n";
			}

			string mainBase = "# 이 Python 파일은 Mocca에 의해 생성되었습니다.\n# 생성 시각 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "\n\n");
			mainBase += moduleImport + "\n" + blockgroups;

			mainBase += "def __main():\n";
			globalIndent++;

			foreach (string i in groupName) {
				mainBase += Indentation() + i + "()\n";
			}

			globalIndent--;

			mainBase += "\n__main()\n";
			return mainBase;
		}

		public override string EvalBlockgroup(MoccaBlockGroup codeBase) {
			string ret = "def " + codeBase.name + "():\n";
			globalIndent++;
			foreach (MoccaSuite i in codeBase.suite) {
				var type = i.GetType();
				if (type.Equals(typeof(MoccaCommand))) {
					ret += Indentation() + EvalCommand((MoccaCommand)i) + "\n";
				} else if (type.Equals(typeof(MoccaLogic))) {
					ret += Indentation() + EvalLogic((MoccaLogic)i) + "\n";
				} else if (type.Equals(typeof(MoccaWhile))) {
					ret += Indentation() + EvalWhile((MoccaWhile)i) + "\n";
				} else if (type.Equals(typeof(MoccaFor))) {
					ret += Indentation() + EvalFor((MoccaFor)i) + "\n";
				} else {
					throw new FormatException();
				}
			}
			globalIndent--;
			return ret;
		}

		public override string EvalAtom(object codeBase) {
			if (codeBase.ToString().Equals("true")) {
				return "True";
			} else if (codeBase.ToString().Equals("false")) {
				return "False";
			} else if (codeBase.GetType().Equals(typeof(MoccaArray))) {
				return EvalArray((MoccaArray)codeBase);
			} else if (codeBase.GetType().Equals(typeof(MoccaDictionary))) {
				return EvalDictionary((MoccaDictionary)codeBase);
			} else if (codeBase.GetType().Equals(typeof(MoccaCommand))) {
				if (((MoccaCommand)codeBase).name != "set") {
					return EvalCommand((MoccaCommand)codeBase);
				} else {
					throw new FormatException();
				}
			} else {
				return codeBase.ToString();
			}
		}

		public override string EvalArray(MoccaArray codeBase) {
			string ret = "[";
			foreach (object i in codeBase.value) {
				ret += EvalAtom(i) + ", ";
			}
			ret = ret.Substring(0, ret.Length - 2) + "]";
			return ret;

		}

		public override string EvalDictionary(MoccaDictionary codeBase) {
			string ret = "{";
			foreach (MoccaTuple i in codeBase.value) {
				ret += EvalTuple(i) + ", ";
			}
			ret = ret.Substring(0, ret.Length - 2) + "}";
			return ret;
		}

		public override string EvalTuple(MoccaTuple codeBase) {
			string ret = "(" + codeBase.key + ", " + EvalAtom(codeBase.value) + ")";
			return ret;
		}

		public override string EvalCommand(MoccaCommand codeBase) {
			string ret = "";
			switch (RecognizeCommandType(codeBase.name)) {
				case CommandType.Cmd:
					ret = codeBase.args[0] + "(" + EvalAtom(codeBase.args[1]) + ")";
					break;
				case CommandType.Set:
					ret = "global " + (string)codeBase.args[0] + " = " + EvalAtom(codeBase.args[1]);
					break;
				case CommandType.Textgen:
					foreach (object i in codeBase.args) {
						ret += EvalAtom(i) + " + ";
					}
					ret = ret.Substring(0, ret.Length - 3);
					break;
				case CommandType.Modcall:
					modules.Add(codeBase.args[0].ToString());
					ret = codeBase.args[0].ToString() + "." + (codeBase.args[1].ToString()).Substring(1, codeBase.args[1].ToString().Length - 2);
					break;
				case CommandType.Unknown:
					throw new FormatException();
			}
			return ret;
		}

		public override string EvalLogic(MoccaLogic codeBase) {
			return "logic";
		}

		public override string EvalWhile(MoccaWhile codeBase) {
			return "while";
		}

		public override string EvalFor(MoccaFor codeBase) {
			return "for";
		}

		public override string EvalEquation(MoccaEquation codeBase) {
			return "eq";
		}

		#endregion GeneralCompiler

		#region CustomFunction

		public enum CommandType {
			Set,
			Cmd,
			Textgen,
			Modcall,
			Unknown

		}

		public string Indentation() {
			string temp = "";
			for (int i = 0; i < globalIndent; i++) {
				temp += "\t";
			}
			return temp;
		}

		public CommandType RecognizeCommandType(string name) {
			switch (name) {
				case "set":
					return CommandType.Set;
				case "cmd":
					return CommandType.Cmd;
				case "textgen":
					return CommandType.Textgen;
				case "modcall":
					return CommandType.Modcall;
				default:
					return CommandType.Unknown;
			}
		}

		#endregion CustomFunction
	}
}

