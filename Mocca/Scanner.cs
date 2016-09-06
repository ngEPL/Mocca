using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Mocca
{
	namespace Compiler {
		#region Scanner

		public partial class Scanner {
			public string Input;
			public int StartPos = 0;
			public int EndPos = 0;
			public int CurrentLine;
			public int CurrentColumn;
			public int CurrentPosition;
			public List<Token> Skipped; // tokens that were skipped
			public Dictionary<TokenType, Regex> Patterns;

			private Token LookAheadToken;
			private List<TokenType> Tokens;
			private List<TokenType> SkipList; // tokens to be skipped

			public Scanner() {
				Regex regex;
				Patterns = new Dictionary<TokenType, Regex>();
				Tokens = new List<TokenType>();
				LookAheadToken = null;
				Skipped = new List<Token>();

				SkipList = new List<TokenType>();
				SkipList.Add(TokenType.WHITESPACE);
				SkipList.Add(TokenType.COMMENTLINE);
				SkipList.Add(TokenType.COMMENTBLOCK);

				regex = new Regex(@"\(", RegexOptions.Compiled);
				Patterns.Add(TokenType.BRACKETOPEN, regex);
				Tokens.Add(TokenType.BRACKETOPEN);

				regex = new Regex(@"\)", RegexOptions.Compiled);
				Patterns.Add(TokenType.BRACKETCLOSE, regex);
				Tokens.Add(TokenType.BRACKETCLOSE);

				regex = new Regex(@"\{", RegexOptions.Compiled);
				Patterns.Add(TokenType.MIDDLEOPEN, regex);
				Tokens.Add(TokenType.MIDDLEOPEN);

				regex = new Regex(@"\}", RegexOptions.Compiled);
				Patterns.Add(TokenType.MIDDLECLOSE, regex);
				Tokens.Add(TokenType.MIDDLECLOSE);

				regex = new Regex(@"\,", RegexOptions.Compiled);
				Patterns.Add(TokenType.COMMA, regex);
				Tokens.Add(TokenType.COMMA);

				regex = new Regex(@"\[", RegexOptions.Compiled);
				Patterns.Add(TokenType.SQUAREOPEN, regex);
				Tokens.Add(TokenType.SQUAREOPEN);

				regex = new Regex(@"\]", RegexOptions.Compiled);
				Patterns.Add(TokenType.SQUARECLOSE, regex);
				Tokens.Add(TokenType.SQUARECLOSE);

				regex = new Regex(@"=", RegexOptions.Compiled);
				Patterns.Add(TokenType.ASSIGN, regex);
				Tokens.Add(TokenType.ASSIGN);

				regex = new Regex(@";", RegexOptions.Compiled);
				Patterns.Add(TokenType.SEMICOLON, regex);
				Tokens.Add(TokenType.SEMICOLON);

				regex = new Regex(@"(\*|\+|\?)", RegexOptions.Compiled);
				Patterns.Add(TokenType.UNARYOPER, regex);
				Tokens.Add(TokenType.UNARYOPER);

				regex = new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*", RegexOptions.Compiled);
				Patterns.Add(TokenType.IDENTIFIER, regex);
				Tokens.Add(TokenType.IDENTIFIER);

				// TODO: Need float, double support here
				regex = new Regex(@"[0-9]+", RegexOptions.Compiled);
				Patterns.Add(TokenType.NUMBER, regex);
				Tokens.Add(TokenType.NUMBER);

				regex = new Regex(@"^$", RegexOptions.Compiled);
				Patterns.Add(TokenType.EOF, regex);
				Tokens.Add(TokenType.EOF);

				regex = new Regex(@"@?\""(\""\""|[^\""])*\""", RegexOptions.Compiled);
				Patterns.Add(TokenType.STRING, regex);
				Tokens.Add(TokenType.STRING);

				regex = new Regex(@"if", RegexOptions.Compiled);
				Patterns.Add(TokenType.IF, regex);
				Tokens.Add(TokenType.IF);

				regex = new Regex(@"elif", RegexOptions.Compiled);
				Patterns.Add(TokenType.ELIF, regex);
				Tokens.Add(TokenType.ELIF);

				regex = new Regex(@"else", RegexOptions.Compiled);
				Patterns.Add(TokenType.ELSE, regex);
				Tokens.Add(TokenType.ELSE);

				regex = new Regex(@"blockgroup", RegexOptions.Compiled);
				Patterns.Add(TokenType.BLOCKGROUP, regex);
				Tokens.Add(TokenType.BLOCKGROUP);

				regex = new Regex(@"for", RegexOptions.Compiled);
				Patterns.Add(TokenType.FOR, regex);
				Tokens.Add(TokenType.FOR);

				regex = new Regex(@"while", RegexOptions.Compiled);
				Patterns.Add(TokenType.WHILE, regex);
				Tokens.Add(TokenType.WHILE);

				regex = new Regex(@"\s+", RegexOptions.Compiled);
				Patterns.Add(TokenType.WHITESPACE, regex);
				Tokens.Add(TokenType.WHITESPACE);

				regex = new Regex(@"//[^\n]*\n?", RegexOptions.Compiled);
				Patterns.Add(TokenType.COMMENTLINE, regex);
				Tokens.Add(TokenType.COMMENTLINE);

				regex = new Regex(@"/\*[^*]*\*+(?:[^/*][^*]*\*+)*/", RegexOptions.Compiled);
				Patterns.Add(TokenType.COMMENTBLOCK, regex);
				Tokens.Add(TokenType.COMMENTBLOCK);


			}

			public void Init(string input) {
				this.Input = input;
				StartPos = 0;
				EndPos = 0;
				CurrentLine = 0;
				CurrentColumn = 0;
				CurrentPosition = 0;
				LookAheadToken = null;
			}

			public Token GetToken(TokenType type) {
				Token t = new Token(this.StartPos, this.EndPos);
				t.Type = type;
				return t;
			}

			/// <summary>
			/// executes a lookahead of the next token
			/// and will advance the scan on the input string
			/// </summary>
			/// <returns></returns>
			public Token Scan(params TokenType[] expectedtokens) {
				Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
				LookAheadToken = null; // reset lookahead token, so scanning will continue
				StartPos = tok.EndPos;
				EndPos = tok.EndPos; // set the tokenizer to the new scan position
				return tok;
			}

			/// <summary>
			/// returns token with longest best match
			/// </summary>
			/// <returns></returns>
			public Token LookAhead(params TokenType[] expectedtokens) {
				int i;
				int startpos = StartPos;
				Token tok = null;
				List<TokenType> scantokens;


				// this prevents double scanning and matching
				// increased performance
				if (LookAheadToken != null
					&& LookAheadToken.Type != TokenType._UNDETERMINED_
					&& LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

				// if no scantokens specified, then scan for all of them (= backward compatible)
				if (expectedtokens.Length == 0)
					scantokens = Tokens;
				else {
					scantokens = new List<TokenType>(expectedtokens);
					scantokens.AddRange(SkipList);
				}

				do {

					int len = -1;
					TokenType index = (TokenType)int.MaxValue;
					string input = Input.Substring(startpos);

					tok = new Token(startpos, EndPos);

					for (i = 0; i < scantokens.Count; i++) {
						Regex r = Patterns[scantokens[i]];
						Match m = r.Match(input);
						if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len))) {
							len = m.Length;
							index = scantokens[i];
						}
					}

					if (index >= 0 && len >= 0) {
						tok.EndPos = startpos + len;
						tok.Text = Input.Substring(tok.StartPos, len);
						tok.Type = index;
					} else if (tok.StartPos < tok.EndPos - 1) {
						tok.Text = Input.Substring(tok.StartPos, 1);
					}

					if (SkipList.Contains(tok.Type)) {
						startpos = tok.EndPos;
						Skipped.Add(tok);
					} else {
						// only assign to non-skipped tokens
						tok.Skipped = Skipped; // assign prior skips to this token
						Skipped = new List<Token>(); //reset skips
					}
				}
				while (SkipList.Contains(tok.Type));

				LookAheadToken = tok;
				return tok;
			}
		}

		#endregion

		#region Token

		public enum TokenType {

			//Non terminal tokens:
			_NONE_ = 0,
			_UNDETERMINED_ = 1,

			//Non terminal tokens:
			Start = 2,
			Blockgroup = 3,
			Params = 4,
			Param = 5,
			Expression = 6,
			Symbol = 7,
			Atom = 8,
			Array = 9,
			Dictionary = 10,
			Block = 11,
			StatementList = 12,
			Statement = 13,

			//Terminal tokens:
			BRACKETOPEN = 14,
			BRACKETCLOSE = 15,
			MIDDLEOPEN = 16,
			MIDDLECLOSE = 17,
			COMMA = 18,
			SQUAREOPEN = 19,
			SQUARECLOSE = 20,
			ASSIGN = 21,
			SEMICOLON = 22,
			UNARYOPER = 23,
			IDENTIFIER = 24,
			NUMBER = 25,
			EOF = 26,
			STRING = 27,
			IF = 28,
			ELIF = 29,
			ELSE = 30,
			BLOCKGROUP = 31,
			FOR = 32,
			WHILE = 33,
			WHITESPACE = 34,
			COMMENTLINE = 35,
			COMMENTBLOCK = 36
		}

		public class Token {
			private int startpos;
			private int endpos;
			private string text;
			private object value;

			// contains all prior skipped symbols
			private List<Token> skipped;

			public int StartPos {
				get { return startpos; }
				set { startpos = value; }
			}

			public int Length {
				get { return endpos - startpos; }
			}

			public int EndPos {
				get { return endpos; }
				set { endpos = value; }
			}

			public string Text {
				get { return text; }
				set { text = value; }
			}

			public List<Token> Skipped {
				get { return skipped; }
				set { skipped = value; }
			}
			public object Value {
				get { return value; }
				set { this.value = value; }
			}

			[XmlAttribute]
			public TokenType Type;

			public Token()
				: this(0, 0) {
			}

			public Token(int start, int end) {
				Type = TokenType._UNDETERMINED_;
				startpos = start;
				endpos = end;
				Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
				Value = null;
			}

			public void UpdateRange(Token token) {
				if (token.StartPos < startpos) startpos = token.StartPos;
				if (token.EndPos > endpos) endpos = token.EndPos;
			}

			public override string ToString() {
				if (Text != null)
					return Type.ToString() + " '" + Text + "'";
				else
					return Type.ToString();
			}
		}

		#endregion
	}
}
