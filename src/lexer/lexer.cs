using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace LFAF_LABS{
    public class Lexer{
        private static Dictionary<TokenType, string> tokenPatterns = new Dictionary<TokenType, string>(){
            { TokenType.LPAREN, @"\(" },
            { TokenType.RPAREN, @"\)" },
            { TokenType.LBRACE, @"\{" },
            { TokenType.RBRACE, @"\}" },
            { TokenType.COMMA, @"\," },
            { TokenType.ASSIGN_EQUAL, @"\=" },
            { TokenType.SEMICOLON, @"\;" },
            { TokenType.PLUS, @"\+" },
            { TokenType.MINUS, @"\-" },
            { TokenType.MULTIPLY, @"\*" },
            { TokenType.DIVIDE, @"\/" },
            { TokenType.GREATER, @"\>" },
            { TokenType.LESS, @"\<" },
            { TokenType.NOT_EQUAL, @"\!=" },
            { TokenType.IF, @"if" },
            { TokenType.ELSE, @"else" },
            { TokenType.FOR, @"for" },
            { TokenType.RETURN, @"return" },
            { TokenType.BREAK, @"break" },
            { TokenType.VAR, @"var" },
            { TokenType.FUNCTION, @"function" },
            { TokenType.ID, @"[a-zA-Z_]\w*" },
            { TokenType.NUMBER, @"\d+(\.\d+)?" },
            { TokenType.STRING, @"""[^""]*""" },
            { TokenType.WHITESPACE, @"\s" },
            { TokenType.COMMENT, @"\//" }
        };

        private List<(TokenType, Regex)> tokenRegexes = new List<(TokenType, Regex)>();

        public Lexer(){
            foreach (KeyValuePair<TokenType, string> pattern in tokenPatterns){
                tokenRegexes.Add((pattern.Key, new Regex("^" + pattern.Value)));
            }
        }

        public List<(TokenType, string)> Tokenize(string program){
            List<(TokenType, string)> tokens = new List<(TokenType, string)>();

            while (!string.IsNullOrEmpty(program)){
                bool match = false;

                if (IsComment(program)){
                    int endOfLine = program.IndexOf('\n');
                    program = program.Substring(endOfLine + 1);
                    continue;
                }

                foreach ((TokenType tokenType, Regex regex) in tokenRegexes){
                    Match tokenMatch = regex.Match(program);
                    if (tokenMatch.Success){
                        (TokenType, string) token = (tokenType, tokenMatch.Value);
                        if (tokenType != TokenType.WHITESPACE){
                            tokens.Add(token);
                        }

                        program = program.Substring(tokenMatch.Length);
                        match = true;
                        break;
                    }
                }
                
                if (!match){
                    Console.WriteLine("Invalid syntax: " + program);
                }
            }
            return tokens;
        }

        public bool IsComment(string token){
            return token.StartsWith("//");
        }
    }
}