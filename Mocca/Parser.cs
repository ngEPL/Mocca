using System;
using System.Collections.Generic;

namespace Mocca
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public ParseTree Parse(string input)
        {
            tree = new ParseTree();
            return Parse(input, tree);
        }

        public ParseTree Parse(string input, ParseTree tree)
        {
            scanner.Init(input);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.BLOCKGROUP);
            while (tok.Type == TokenType.BLOCKGROUP)
            {
                ParseBlockgroup(node);
            tok = scanner.LookAhead(TokenType.BLOCKGROUP);
            }

            
            tok = scanner.Scan(TokenType.EOF);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.EOF) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseBlockgroup(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Blockgroup), "Blockgroup");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.BLOCKGROUP);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BLOCKGROUP) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BLOCKGROUP.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            ParseParams(node);

            
            ParseBlock(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParams(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Params), "Params");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.BRACKETOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            ParseParam(node);

            
            tok = scanner.Scan(TokenType.BRACKETCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParam(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Param), "Param");
            parent.Nodes.Add(node);


            
            ParseExpression(node);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.COMMA) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }

                
                ParseExpression(node);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseExpression(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Expression), "Expression");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.NUMBER, TokenType.STRING, TokenType.SQUAREOPEN, TokenType.MIDDLEOPEN);
            switch (tok.Type)
            {
                case TokenType.IDENTIFIER:
                    ParseSymbol(node);
                    break;
                case TokenType.NUMBER:
                case TokenType.STRING:
                case TokenType.SQUAREOPEN:
                case TokenType.MIDDLEOPEN:
                    ParseAtom(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSymbol(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Symbol), "Symbol");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.BRACKETOPEN);
            while (tok.Type == TokenType.BRACKETOPEN)
            {
                ParseParams(node);
            tok = scanner.LookAhead(TokenType.BRACKETOPEN);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAtom(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Atom), "Atom");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.NUMBER, TokenType.STRING, TokenType.SQUAREOPEN, TokenType.MIDDLEOPEN);
            switch (tok.Type)
            {
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.NUMBER) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.STRING) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.SQUAREOPEN:
                    ParseArray(node);
                    break;
                case TokenType.MIDDLEOPEN:
                    ParseDictionary(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseArray(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Array), "Array");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SQUAREOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SQUAREOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SQUAREOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            ParseParam(node);

            
            tok = scanner.Scan(TokenType.SQUARECLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SQUARECLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SQUARECLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseDictionary(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Dictionary), "Dictionary");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.MIDDLEOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.MIDDLEOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MIDDLEOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            ParseParams(node);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.COMMA) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }

                
                ParseParams(node);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            
            tok = scanner.Scan(TokenType.MIDDLECLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.MIDDLECLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MIDDLECLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseBlock(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Block), "Block");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.MIDDLEOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.MIDDLEOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MIDDLEOPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            while (tok.Type == TokenType.IDENTIFIER)
            {
                ParseStatementList(node);
            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            }

            
            tok = scanner.Scan(TokenType.MIDDLECLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.MIDDLECLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MIDDLECLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseStatementList(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.StatementList), "StatementList");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            while (tok.Type == TokenType.IDENTIFIER)
            {
                ParseStatement(node);
            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseStatement(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Statement), "Statement");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            
            ParseParams(node);

            
            tok = scanner.LookAhead(TokenType.MIDDLEOPEN);
            while (tok.Type == TokenType.MIDDLEOPEN)
            {
                ParseBlock(node);
            tok = scanner.LookAhead(TokenType.MIDDLEOPEN);
            }

            
            tok = scanner.LookAhead(TokenType.SEMICOLON);
            while (tok.Type == TokenType.SEMICOLON)
            {
                tok = scanner.Scan(TokenType.SEMICOLON);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.SEMICOLON) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOLON.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }
            tok = scanner.LookAhead(TokenType.SEMICOLON);
            }

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}
