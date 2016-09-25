using System;
using System.Collections.Generic;
using Mocca.DataType;

namespace Mocca.Compiler {
	public class PythonCompiler : BasicCompiler {
		public PythonCompiler(List<MoccaBlockGroup> codeBase) {
			this.codeBase = codeBase;
		}

		#region GeneralCompiler

		int globalIndent = 0;
		bool isMicrobit = false;
		List<string> modules = new List<string>();
		string variableDefinition = "\n# 변수 선언 부분입니다.\n\n";

		public override string Compile() {
			return this.EvalStart(codeBase);
		}

		public override string EvalStart(List<MoccaBlockGroup> codeBase) {
			List<string> groupName = new List<string>();

			string blockgroups = "";
			foreach (MoccaBlockGroup i in codeBase) {
				var evalBlockgroup = EvalBlockgroup(i);
				if (!evalBlockgroup.Equals("")) {
					blockgroups += evalBlockgroup + "\n";
					groupName.Add(i.name);
				}
			}

			string moduleImport = "";

			if (isMicrobit) {
				moduleImport += "from microbit import *\n";
			}

			foreach (string i in modules) {
				moduleImport += "import " + i + "\n";
			}

			string mainBase = "# -*- coding: utf-8 -*-\n# 이 Python 파일은 Mocca에 의해 생성되었습니다.\n# 생성 시각 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "\n\n");

			if (variableDefinition.Equals("\n# 변수 선언 부분입니다.\n\n")) {
				variableDefinition = "";
			}

			mainBase += moduleImport + variableDefinition + "\n" + blockgroups;

			mainBase = mainBase.Replace("\t", "    ");
			return mainBase;
		}

		public override string EvalBlockgroup(MoccaBlockGroup codeBase) {
			string ret = "# " + codeBase.name + " 부분입니다.\n\n";
			string block = "";

			foreach (MoccaSuite i in codeBase.suite) {
				block += Indentation() + EvalSuite(i);
			}

			var temp = block.Replace("\t", "");
			if (temp.Equals("")) {
				return "";
			}
			return ret + block;
		}

		public override string EvalSuite(MoccaSuite codeBase) {
			var type = codeBase.GetType();
			if (type.Equals(typeof(MoccaCommand))) {
				string evalCommand = EvalCommand((MoccaCommand)codeBase);
				if (!evalCommand.Equals("")) {
					return evalCommand;
				} else {
					return "";
				}
			} else if (type.Equals(typeof(MoccaLogic))) {
				return EvalLogic((MoccaLogic)codeBase);
			} else if (type.Equals(typeof(MoccaWhile))) {
				return EvalWhile((MoccaWhile)codeBase);
			} else if (type.Equals(typeof(MoccaEvent))) {
				return EvalEvent((MoccaEvent)codeBase);
			} else if (type.Equals(typeof(MoccaFor))) {
				return EvalFor((MoccaFor)codeBase);
			} else {
				throw new FormatException();
			}
		}

		public override string EvalEvent(MoccaEvent codeBase) {
			switch (codeBase.type) {
				case MoccaEventType.onProgramStart:
					return "# 프로그램이 시작될 때 호출됩니다.\n";
				case MoccaEventType.onKeyPressed:
					return "# " + codeBase.option + " 키가 눌렸을 때 호출됩니다.\n";
				case MoccaEventType.onFunctionCall:
					return "# " + codeBase.option + " 함수입니다.\n";
				case MoccaEventType.onSignalCall:
					return "# " + codeBase.option + " 신호가 사용되면 호출됩니다.\n";
				default:
					throw new FormatException();
			}
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
			} else if (codeBase.GetType().Equals(typeof(MoccaEquation))) {
				return EvalEquation((MoccaEquation)codeBase);
			} else if (codeBase.GetType().Equals(typeof(MoccaCommand))) {
				if (((MoccaCommand)codeBase).name != "set") {
					return EvalCommand((MoccaCommand)codeBase);
				} else {
					throw new FormatException();
				}
			} else {
				var i = 0.0;
				//if (Double.TryParse(codeBase.ToString(), out i) || codeBase.ToString().Substring(0, 1).Equals("\"")) {
					return codeBase.ToString();
				//} else {
				//	return "str(" + codeBase.ToString() + ")";
				//}
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
					ret = codeBase.args[0] + "(" + EvalAtom(codeBase.args[1]) + ")" + "\n";
					break;
				case CommandType.Def:
					variableDefinition += codeBase.args[0].ToString() + " = " + EvalAtom(codeBase.args[1]) + "\n";
					break;
				case CommandType.Set:
					ret = codeBase.args[0].ToString() + " = " + EvalAtom(codeBase.args[1]) + "\n";
					break;
				case CommandType.Textgen:
					foreach (object i in codeBase.args) {
						ret += EvalAtom(i) + ", ";
					}
					ret = ret.Substring(0, ret.Length - 2);
					break;
				case CommandType.Modcall:
					ModuleCheck(codeBase.args[0].ToString());
					ret = codeBase.args[0].ToString() + "." + (codeBase.args[1].ToString()).Substring(1, codeBase.args[1].ToString().Length - 2);
					break;
				case CommandType.Physical:
					PhysicalDeviceCheck(codeBase.args[0].ToString());
					ret = codeBase.args[1].ToString().Substring(1, codeBase.args[1].ToString().Length - 2) + "\n";
					break;
				case CommandType.Unknown:
					throw new FormatException();
			}
			return ret;
		}

		public override string EvalLogic(MoccaLogic codeBase) {
			string ret = "";
			if (!codeBase.keyword.Equals("else")) {
				ret += codeBase.keyword + " " + EvalExpression(codeBase.expression) + ":\n";
			} else {
				ret += codeBase.keyword + ":\n";
			}
			globalIndent++;
			foreach (MoccaSuite i in codeBase.cmd_list) {
				ret += Indentation() + EvalSuite(i);
			}
			globalIndent--;
			return ret + "\n";
		}

		public override string EvalWhile(MoccaWhile codeBase) {
			string ret = "while ";
			ret += EvalExpression(codeBase.expression) + ":\n";
			globalIndent++;
			foreach (MoccaSuite i in codeBase.cmd_list) {
				ret += Indentation() + EvalSuite(i);
			}
			globalIndent--;
			return ret + "\n";

		}

		public override string EvalFor(MoccaFor codeBase) {
			string ret = "for " + GenerateLoopCounter() + " in " + codeBase.iter + ":\n";
			globalIndent++;
			foreach (MoccaSuite i in codeBase.cmd_list) {
				ret += Indentation() + EvalSuite(i);
			}
			globalIndent--;
			ret = ret.Replace("__iterator", GenerateLoopCounter());
			return ret + "\n";
		}

		public override string EvalExpression(MoccaExpression codeBase) {
			if (codeBase.atom_evaluation != null) {
				return codeBase.atom_evaluation;
			}
			string comparer = "?";
			switch (codeBase.logic_op) {
				case "EQUAL":
					comparer = "==";
					break;
				case "NOT_EQUAL":
					comparer = "!=";
					break;
				case "LEFT_BIG":
					comparer = ">";
					break;
				case "LEFT_BIG_EQUAL":
					comparer = ">=";
					break;
				case "RIGHT_BIG":
					comparer = "<";
					break;
				case "RIGHT_BIG_EQUAL":
					comparer = "<=";
					break;
				case "AND":
					comparer = "&&";
					break;
				case "OR":
					comparer = "||";
					break;
			}

			return codeBase.a.ToString() + " " + comparer + " " + codeBase.b.ToString();
		}

		public override string EvalEquation(MoccaEquation codeBase) {
			string op = "";
			switch (codeBase.op) {
				case "ADD":
					op = "+";
					break;
				case "SUB":
					op = "-";
					break;
				case "MUL":
					op = "*";
					break;
				case "DIV":
					op = "/";
					break;
				case "MOD":
					op = "%";
					break;
			}

			return codeBase.a.ToString() + " " + op + " " + codeBase.b.ToString();
		}

		#endregion GeneralCompiler

		#region CustomFunction

		public enum CommandType {
			Def,
			Set,
			Cmd,
			Textgen,
			Modcall,
			Physical,
			Event,
			Unknown

		}

		public string Indentation() {
			string temp = "";
			for (int i = 0; i < globalIndent; i++) {
				temp += "\t";
			}
			return temp;
		}

		public string GenerateLoopCounter() {
			switch (globalIndent) {
				case 0:
					return "i";
				case 1:
					return "j";
				case 2:
					return "k";
				case 3:
					return "l";
				case 4:
					return "m";
				case 5:
					return "n";
				case 6:
					return "o";
				default:
					return "__loopcnt" + globalIndent;
			}
		}

		public void ModuleCheck(string module) {
			foreach (string i in modules) {
				if (i.Equals(module)) {
					return;
				}
			}
			modules.Add(module);
		}

		public void PhysicalDeviceCheck(string device) {
			switch (device) {
				case "microbit":
					isMicrobit = true;
					break;
			}
		}

		public CommandType RecognizeCommandType(string name) {
			switch (name) {
				case "def":
					return CommandType.Def;
				case "set":
					return CommandType.Set;
				case "cmd":
					return CommandType.Cmd;
				case "textgen":
					return CommandType.Textgen;
				case "modcall":
					return CommandType.Modcall;
				case "physical":
					return CommandType.Physical;
				case "event":
					return CommandType.Event;
				default:
					return CommandType.Unknown;
			}
		}

		#endregion CustomFunction
	}
}