﻿using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Compiler;

namespace Mocca.Compiler.Language {
	public class PythonCompiler : BasicCompiler {
		ParseTree tree;
		ParseNode nodes;

		public object generate(ParseTree tree) {
			this.tree = tree;
			this.nodes = tree.Nodes[0];
			return this.Eval(tree);
		}

		public object GetValue(ParseTree tree, TokenType type, int index) {
			return GetValue(tree, type, ref index);
		}

		public object GetValue(ParseTree tree, TokenType type, ref int index) {
			object o = null;
			if (index < 0) return o;

			// left to right
			foreach (ParseNode node in nodes.nodes) {
				if (node.Token.Type == type) {
					index--;
					if (index < 0) {
						o = this.Eval(tree);
						break;
					}
				}
			}
			return o;
		}

		/// <summary>
		/// this implements the evaluation functionality, cannot be used directly
		/// </summary>
		/// <param name="tree">the parsetree itself</param>
		/// <param name="paramlist">optional input parameters</param>
		/// <returns>a partial result of the evaluation</returns>
		public object Eval(ParseTree tree, params object[] paramlist) {
			object Value = null;

			switch (nodes.Token.Type) {
				case TokenType.Start:
					Value = EvalStart(tree, paramlist);
					break;
				case TokenType.Blockgroup:
					Value = EvalBlockgroup(tree, paramlist);
					break;
				case TokenType.Params:
					Value = EvalParams(tree, paramlist);
					break;
				case TokenType.Param:
					Value = EvalParam(tree, paramlist);
					break;
				case TokenType.Expression:
					Value = EvalExpression(tree, paramlist);
					break;
				case TokenType.Symbol:
					Value = EvalSymbol(tree, paramlist);
					break;
				case TokenType.Atom:
					Value = EvalAtom(tree, paramlist);
					break;
				case TokenType.Array:
					Value = EvalArray(tree, paramlist);
					break;
				case TokenType.Dictionary:
					Value = EvalDictionary(tree, paramlist);
					break;
				case TokenType.Block:
					Value = EvalBlock(tree, paramlist);
					break;
				case TokenType.StatementList:
					Value = EvalStatementList(tree, paramlist);
					break;
				case TokenType.Statement:
					Value = EvalStatement(tree, paramlist);
					break;

				default:
					Value = nodes.Token.Text;
					break;
			}
			return Value;
		}

		/*
		 * nodes.GetValue(tree, TokenType.MultExpr, i++);
		 * $MultExpr[i++]
		 * returns List<MoccaBlockGroup>
		 */

		public virtual object EvalStart(ParseTree tree, params object[] paramlist) {
			throw new NotImplementedException();
		}

		/*
		 * blockgroup Params Block
		 * returns MoccaBlockGroup
		 */
		public virtual object EvalBlockgroup(ParseTree tree, params object[] paramlist) {
			var ret = new MoccaBlockGroup();

			// Taking params
			var param = (List<object>)nodes.GetValue(tree, TokenType.Params, 0);

			var code = (List<MoccaSuite>)nodes.GetValue(tree, TokenType.Block, 0);

			ret.name = (string)param[0];

			Int32.TryParse(param[1].ToString(), out ret.x);
			Int32.TryParse(param[2].ToString(), out ret.y);

			ret.suite = code;

			return ret;
		}

		/*
		 * ( Param )
		 * returns List<object>
		 */
		public virtual object EvalParams(ParseTree tree, params object[] paramlist) {
			return nodes.GetValue(tree, TokenType.Param, 0);
		}

		/*
		 * Expression, Expression, Expression...
		 * returns List<object>
		 */
		public virtual object EvalParam(ParseTree tree, params object[] paramlist) {
			List<object> ret = new List<object>();

			int i = 0;
			while (nodes.GetValue(tree, TokenType.Expression, i) != null) {
				ret.Add(nodes.GetValue(tree, TokenType.Expression, i));
				i = i + 1;
			}

			return ret;
		}

		/*
		 * Symbol | Atom
		 * returns object
		 */
		public virtual object EvalExpression(ParseTree tree, params object[] paramlist) {
			if (nodes.GetValue(tree, TokenType.Symbol, 0) != null) {
				return nodes.GetValue(tree, TokenType.Symbol, 0);
			} else {
				return nodes.GetValue(tree, TokenType.Atom, 0);
			}
		}

		/*
		 * IDENTIFIER (Params)*;
		 * returns MoccaCommand
		 */
		public virtual object EvalSymbol(ParseTree tree, params object[] paramlist) {
			string identifier = (string)nodes.GetValue(tree, TokenType.IDENTIFIER, 0);
			List<object> param = null;

			if (nodes.GetValue(tree, TokenType.Params, 0) != null) {
				param = (List<object>)nodes.GetValue(tree, TokenType.Params, 0);

				switch (identifier) {
					case "logic_compare":
						return new MoccaExpression(param[0], param[1], param[2].ToString());
					case "eq":
						return new MoccaEquation(param[0], param[1], param[2].ToString());
					default:
						return new MoccaCommand(identifier, param);
				}
			} else {
				return identifier;
			}
		}

		/*
		 * NUMBER | STRING | Array | Dictionary
		 * returns object
		 */
		public virtual object EvalAtom(ParseTree tree, params object[] paramlist) {
			object ret;

			if (nodes.GetValue(tree, TokenType.NUMBER, 0) != null &&
			   nodes.GetValue(tree, TokenType.STRING, 0) == null &&
			   nodes.GetValue(tree, TokenType.Array, 0) == null &&
			   nodes.GetValue(tree, TokenType.Dictionary, 0) == null) {
				float i = 0f;
				if (float.TryParse((string)nodes.GetValue(tree, TokenType.NUMBER, 0), out i)) {
					ret = i;
				} else {
					throw new Exception();
				}
			} else if (nodes.GetValue(tree, TokenType.NUMBER, 0) == null &&
			   nodes.GetValue(tree, TokenType.STRING, 0) != null &&
			   nodes.GetValue(tree, TokenType.Array, 0) == null &&
			   nodes.GetValue(tree, TokenType.Dictionary, 0) == null) {
				ret = (string)nodes.GetValue(tree, TokenType.STRING, 0);
			} else if (nodes.GetValue(tree, TokenType.NUMBER, 0) == null &&
			 nodes.GetValue(tree, TokenType.STRING, 0) == null &&
			 nodes.GetValue(tree, TokenType.Array, 0) != null &&
			 nodes.GetValue(tree, TokenType.Dictionary, 0) == null) {
				ret = nodes.GetValue(tree, TokenType.Array, 0);
			} else {
				ret = nodes.GetValue(tree, TokenType.Dictionary, 0);
			}

			return ret;
		}

		/*
		 * [ Param ]
		 * returns MoccaArray
		 */
		public virtual object EvalArray(ParseTree tree, params object[] paramlist) {
			List<object> param = (List<object>)nodes.GetValue(tree, TokenType.Param, 0);
			List<object> variables = new List<object>();
			foreach (var i in param) {
				Type cursor = i.GetType();
				if (cursor.Equals(typeof(string))) {
					variables.Add(new MoccaVariable() { name = "__none", value = i, type = MoccaType.STRING });
				} else if (cursor.Equals(typeof(float))) {
					variables.Add(new MoccaVariable() { name = "__none", value = i, type = MoccaType.NUMBER });
				} else if (cursor.Equals(typeof(MoccaArray))) {
					variables.Add((MoccaArray)i);
				} else if (cursor.Equals(typeof(MoccaDictionary))) {
					variables.Add((MoccaDictionary)i);
				} else if (cursor.Equals(typeof(MoccaCommand))) {
					variables.Add((MoccaCommand)i);
				} else {
					// TODO: Throws MoccaException when i is not compromised with Mocca
					// For now just pass away
					variables.Add(i);
				}
			}

			MoccaArray ret = new MoccaArray();
			ret.name = "__ready";
			ret.value = param;
			return param;
		}

		/*
		 * { Params , Params , Params(List<object>... }
		 * returns MoccaDictionary
		 */
		public virtual object EvalDictionary(ParseTree tree, params object[] paramlist) {
			MoccaDictionary ret = new MoccaDictionary();

			List<object> param = new List<object>();
			var i = 0;
			while (nodes.GetValue(tree, TokenType.Params, i) != null) {
				param.Add(nodes.GetValue(tree, TokenType.Params, i));
				i++;
			}

			foreach (object j in param) {
				List<object> cursor = (List<object>)j;
				string key = cursor[0].ToString();
				object value = cursor[1];
				ret.value.Add(new MoccaTuple(key, value));
			}

			return ret;
		}

		/*
		 * { StatementList }
		 * or
		 * { }
		 * returns List<MoccaSuite>
		 */
		public virtual object EvalBlock(ParseTree tree, params object[] paramlist) {
			List<MoccaSuite> ret = (List<MoccaSuite>)nodes.GetValue(tree, TokenType.StatementList, 0);
			return ret;
		}

		/*
		 * Statement Statement Statement...
		 * returns List<MoccaSuite>
		 */
		public virtual object EvalStatementList(ParseTree tree, params object[] paramlist) {
			List<MoccaSuite> ret = new List<MoccaSuite>();
			var i = 0;
			while (nodes.GetValue(tree, TokenType.Statement, i) != null) {
				ret.Add((MoccaSuite)nodes.GetValue(tree, TokenType.Statement, i));
				i++;
			}
			return ret;
		}

		/*
		 * IDENTIFIER Params SEMICOLON
		 * or
		 * IDENTIFIER Params Block
		 * * SEMICOLON after Block will be allowed
		 * returns MoccaSuite::Something
		 */
		public virtual object EvalStatement(ParseTree tree, params object[] paramlist) {
			string identifier = (string)nodes.GetValue(tree, TokenType.IDENTIFIER, 0);
			List<object> param = (List<object>)nodes.GetValue(tree, TokenType.Params, 0);
			List<MoccaSuite> block = null;
			if (nodes.GetValue(tree, TokenType.Block, 0) != null) {
				block = (List<MoccaSuite>)nodes.GetValue(tree, TokenType.Block, 0);
			}

			switch (identifier) {
				case "if":
					MoccaLogic a = new MoccaLogic();
					a.keyword = "if";
					a.expression = (MoccaExpression)param[0];
					a.cmd_list = block;
					return a;
				case "elif":
					MoccaLogic b = new MoccaLogic();
					b.keyword = "elif";
					b.expression = (MoccaExpression)param[0];
					b.cmd_list = block;
					return b;
				case "else":
					MoccaLogic c = new MoccaLogic();
					c.keyword = "else";
					c.cmd_list = block;
					return c;
				case "while":
					MoccaWhile d = new MoccaWhile();
					d.expression = (MoccaExpression)param[0];
					d.cmd_list = block;
					return d;
				case "for":
					MoccaFor e = new MoccaFor();
					e.iter = param[0];
					e.cmd_list = block;
					return e;
				default:
					MoccaCommand f = new MoccaCommand(identifier, param);
					return f;
			}
		}
	}
}
