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

        protected virtual object EvalBlockgroup(ParseTree tree, params object[] paramlist) {
            MoccaBlockGroup ret = new MoccaBlockGroup();
            List<object> param = new List<object>();
            int i = 0;
            while (this.GetValue(tree, TokenType.Params, i) != null) {
                param.Add(this.GetValue(tree, TokenType.Params, i));
                i++;
            }
            List<MoccaSuite> code = new List<MoccaSuite>();
            // TODO: 코딩하다 말았다
        }

        protected virtual object EvalParams(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalParam(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalExpression(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalSymbol(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist) {
            if(this.GetValue(tree, TokenType.NUMBER, 0) == null &&
               this.GetValue(tree, TokenType.STRING, 0) != null &&
               this.GetValue(tree, TokenType.Array, 0) != null &&
               this.GetValue(tree, TokenType.Dictionary, 0) != null) {
                return this.GetValue(tree, TokenType.NUMBER, 0);
            } else if (this.GetValue(tree, TokenType.NUMBER, 0) != null &&
               this.GetValue(tree, TokenType.STRING, 0) == null &&
               this.GetValue(tree, TokenType.Array, 0) != null &&
               this.GetValue(tree, TokenType.Dictionary, 0) != null) {
                return this.GetValue(tree, TokenType.STRING, 0);
            } else if(this.GetValue(tree, TokenType.NUMBER, 0) != null &&
             this.GetValue(tree, TokenType.STRING, 0) != null &&
             this.GetValue(tree, TokenType.Array, 0) == null &&
             this.GetValue(tree, TokenType.Dictionary, 0) != null) {
                return this.GetValue(tree, TokenType.Array, 0);
            } else {
                return this.GetValue(tree, TokenType.Dictionary, 0);
            }
        }

        protected virtual object EvalArray(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalDictionary(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalBlock(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalStatementList(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }

        protected virtual object EvalStatement(ParseTree tree, params object[] paramlist) {
            throw new NotImplementedException();
        }


    }

    #endregion ParseTree
}
