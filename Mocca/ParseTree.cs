using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Mocca.DataType;

namespace Mocca {
    #region ParseTree
    [Serializable]
    public class ParseErrors : List<ParseError> {
    }

    [Serializable]
    public class ParseError {
        private string message;
        private int code;
        private int line;
        private int col;
        private int pos;
        private int length;

        public int Code { get { return code; } }
        public int Line { get { return line; } }
        public int Column { get { return col; } }
        public int Position { get { return pos; } }
        public int Length { get { return length; } }
        public string Message { get { return message; } }

        // just for the sake of serialization
        public ParseError() {
        }

        public ParseError(string message, int code, ParseNode node) : this(message, code, 0, node.Token.StartPos, node.Token.StartPos, node.Token.Length) {
        }

        public ParseError(string message, int code, int line, int col, int pos, int length) {
            this.message = message;
            this.code = code;
            this.line = line;
            this.col = col;
            this.pos = pos;
            this.length = length;
        }
    }

    // rootlevel of the node tree
    [Serializable]
    public partial class ParseTree : ParseNode {
        public ParseErrors Errors;

        public List<Token> Skipped;

        public ParseTree() : base(new Token(), "ParseTree") {
            Token.Type = TokenType.Start;
            Token.Text = "Root";
            Errors = new ParseErrors();
        }

        public string PrintTree() {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            PrintNode(sb, this, indent);
            return sb.ToString();
        }

        private void PrintNode(StringBuilder sb, ParseNode node, int indent) {

            string space = "".PadLeft(indent, ' ');

            sb.Append(space);
            sb.AppendLine(node.Text);

            foreach (ParseNode n in node.Nodes)
                PrintNode(sb, n, indent + 2);
        }

        /// <summary>
        /// this is the entry point for executing and evaluating the parse tree.
        /// </summary>
        /// <param name="paramlist">additional optional input parameters</param>
        /// <returns>the output of the evaluation function</returns>
        public object Eval(params object[] paramlist) {
            return Nodes[0].Eval(this, paramlist);
        }
    }

    [Serializable]
    [XmlInclude(typeof(ParseTree))]
    public partial class ParseNode {
        protected string text;
        protected List<ParseNode> nodes;

        public List<ParseNode> Nodes { get { return nodes; } }

        [XmlIgnore] // avoid circular references when serializing
        public ParseNode Parent;
        public Token Token; // the token/rule

        [XmlIgnore] // skip redundant text (is part of Token)
        public string Text { // text to display in parse tree 
            get { return text; }
            set { text = value; }
        }

        public virtual ParseNode CreateNode(Token token, string text) {
            ParseNode node = new ParseNode(token, text);
            node.Parent = this;
            return node;
        }

        protected ParseNode(Token token, string text) {
            this.Token = token;
            this.text = text;
            this.nodes = new List<ParseNode>();
        }

        protected object GetValue(ParseTree tree, TokenType type, int index) {
            return GetValue(tree, type, ref index);
        }

        protected object GetValue(ParseTree tree, TokenType type, ref int index) {
            object o = null;
            if (index < 0) return o;

            // left to right
            foreach (ParseNode node in nodes) {
                if (node.Token.Type == type) {
                    index--;
                    if (index < 0) {
                        o = node.Eval(tree);
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
        internal object Eval(ParseTree tree, params object[] paramlist) {
            object Value = null;

            switch (Token.Type) {
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
                    Value = Token.Text;
                    break;
            }
            return Value;
        }

        /*
         * this.GetValue(tree, TokenType.MultExpr, i++);
         * $MultExpr[i++]
         * returns List<MoccaBlockGroup>
         */

        protected virtual object EvalStart(ParseTree tree, params object[] paramlist) {
            List<MoccaBlockGroup> ret = new List<MoccaBlockGroup>();
            int i = 0;
            while (this.GetValue(tree, TokenType.Blockgroup, i) != null) {
                ret.Add((MoccaBlockGroup)this.GetValue(tree, TokenType.Blockgroup, i));
                i++;
            }
            return ret;
        }

		/*
		 * blockgroup Params Block
		 * returns MoccaBlockGroup
		 */ 
        protected virtual object EvalBlockgroup(ParseTree tree, params object[] paramlist) {
            MoccaBlockGroup ret = new MoccaBlockGroup();

			// Taking params
            List<object> param = new List<object>();
			var paramsValue = this.GetValue(tree, TokenType.Params, 0);

			// TODO: Taking code block
            List<MoccaSuite> code = new List<MoccaSuite>();

			return ret;
        }

		/*
		 * ( param , param , param )
		 * returns List<object>
		 */ 
        protected virtual object EvalParams(ParseTree tree, params object[] paramlist) {
			return this.GetValue(tree, TokenType.Param, 0);
        }

		/*
		 * Expression, Expression, Expression...
		 * returns List<object>
		 */ 
        protected virtual object EvalParam(ParseTree tree, params object[] paramlist) {
			List<object> ret = new List<object>();
			var i = 0;
			while (this.GetValue(tree, TokenType.Expression, i) != null) {
				ret.Add(this.GetValue(tree, TokenType.Expression, i));
				i++;
			}
			return ret;
        }

		/*
		 * Symbol | Atom
		 * returns object
		 */ 
        protected virtual object EvalExpression(ParseTree tree, params object[] paramlist) {
			if (this.GetValue(tree, TokenType.Symbol, 0) != null) {
				return this.GetValue(tree, TokenType.Symbol, 0);
			} else {
				return this.GetValue(tree, TokenType.Atom, 0);
			}
        }

		/*
		 * IDENTIFIER (Params)*;
		 * returns MoccaCommand
		 */ 
        protected virtual object EvalSymbol(ParseTree tree, params object[] paramlist) {
			string identifier = (string)this.GetValue(tree, TokenType.IDENTIFIER, 0);
			List<object> param = null;

			if (this.GetValue(tree, TokenType.Params, 0) != null) {
				param = (List<object>)this.GetValue(tree, TokenType.Params, 0);
			}

			MoccaCommand cmd = new MoccaCommand();
			cmd.commandName = identifier;
			cmd.commandArgs = param;

			return cmd;
        }

		/*
		 * NUMBER | STRING | Array | Dictionary
		 * returns object
		 */ 
        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist) {
            if(this.GetValue(tree, TokenType.NUMBER, 0) == null &&
               this.GetValue(tree, TokenType.STRING, 0) != null &&
               this.GetValue(tree, TokenType.Array, 0) != null &&
               this.GetValue(tree, TokenType.Dictionary, 0) != null) {
				float i = 0;
				return float.TryParse((string)this.GetValue(tree, TokenType.NUMBER, 0), out i);
            } else if (this.GetValue(tree, TokenType.NUMBER, 0) != null &&
               this.GetValue(tree, TokenType.STRING, 0) == null &&
               this.GetValue(tree, TokenType.Array, 0) != null &&
               this.GetValue(tree, TokenType.Dictionary, 0) != null) {
				return (string)this.GetValue(tree, TokenType.STRING, 0);
            } else if(this.GetValue(tree, TokenType.NUMBER, 0) != null &&
             this.GetValue(tree, TokenType.STRING, 0) != null &&
             this.GetValue(tree, TokenType.Array, 0) == null &&
             this.GetValue(tree, TokenType.Dictionary, 0) != null) {
				return this.GetValue(tree, TokenType.Array, 0);
            } else {
                return this.GetValue(tree, TokenType.Dictionary, 0);
            }
        }

		/*
		 * [ Param ]
		 * returns MoccaArray
		 */ 
		protected virtual object EvalArray(ParseTree tree, params object[] paramlist) {
			List<object> param = (List<object>)this.GetValue(tree, TokenType.Param, 0);
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
        protected virtual object EvalDictionary(ParseTree tree, params object[] paramlist) {
			MoccaDictionary ret = new MoccaDictionary();

			List<object> param = new List<object>();
			var i = 0;
			while (this.GetValue(tree, TokenType.Params, i) != null) {
				param.Add(this.GetValue(tree, TokenType.Params, i));
				i++;
			}

			foreach (object j in param) {
				List<object> cursor = (List<object>)j;
				string key = (string)cursor[0];
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
        protected virtual object EvalBlock(ParseTree tree, params object[] paramlist) {
			List<MoccaSuite> ret = (List<MoccaSuite>)this.GetValue(tree, TokenType.StatementList, 0);
			return ret;
        }

		/*
		 * Statement Statement Statement...
		 * returns List<MoccaSuite>
		 */ 
        protected virtual object EvalStatementList(ParseTree tree, params object[] paramlist) {
			List<MoccaSuite> ret = new List<MoccaSuite>();
			var i = 0;
			while (this.GetValue(tree, TokenType.Statement, i) != null) {
				ret.Add((MoccaSuite)this.GetValue(tree, TokenType.Statement, i));
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
		List<string> parse_expected = new List<string>();
		List<MoccaLogic> temp_logic = new List<MoccaLogic>();
        protected virtual object EvalStatement(ParseTree tree, params object[] paramlist) {
			MoccaSuite ret = new MoccaSuite();

			string identifier = (string)this.GetValue(tree, TokenType.IDENTIFIER, 0);
			List<object> param = (List<object>)this.GetValue(tree, TokenType.Params, 0);
			if (this.GetValue(tree, TokenType.Block, 0) != null) {
				List<MoccaSuite> block = (List<MoccaSuite>)this.GetValue(tree, TokenType.Block, 0);
			}

			switch (identifier) {
				case "if":
					break;
				case "elif":
					break;
				case "else":
					break;
				case "while":
					break;
				case "for":
					break;
				default:
					break;
			}


			return ret;
        }


    }

    #endregion ParseTree
}
