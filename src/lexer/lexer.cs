using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace LFAF_LABS{
    class Lexer{
        private static Dictionary<string, string> tokenPatterns = new Dictionary<string, string>(){
            { "LPAREN", @"\(" },
            { "RPAREN", @"\)" },
            { "LBRACE", @"\{" },
            { "RBRACE", @"\}" },
            { "COMMA", @"\," },
            { "ASSIGN_EQUAL", @"\=" },
            { "SEMICOLON", @"\;" },
            { "PLUS", @"\+" },
            { "MINUS", @"\-" },
            { "MULTIPLY", @"\*" },
            { "DIVIDE", @"\/" },
            { "GREATER", @"\>" },
            { "LESS", @"\<" },
            { "NOT_EQUAL", @"\!=" },
            { "IF", @"if" },
            { "ELSE", @"else" },
            { "FOR", @"for" },
            { "RETURN", @"return" },
            { "BREAK", @"break" },
            { "VAR", @"var" },
            { "FUNCTION", @"function" },
            { "ID", @"[a-zA-Z_]\w*" },
            { "NUMBER", @"\d+(\.\d+)?" },
            { "STRING", @"""[^""]*""" },
            { "WHITESPACE", @"\s" },
        };

        private List<(string, Regex)> tokenRegexes = new List<(string, Regex)>();

        public Lexer(){
            foreach (KeyValuePair<string, string> pattern in tokenPatterns){
                tokenRegexes.Add((pattern.Key, new Regex("^" + pattern.Value)));
            }
        }

        public List<(string, string)> Tokenize(string program){
            List<(string, string)> tokens = new List<(string, string)>();

            while (!string.IsNullOrEmpty(program)){
                bool match = false;
                foreach ((string tokenName, Regex regex) in tokenRegexes){
                    Match tokenMatch = regex.Match(program);
                    if (tokenMatch.Success){
                        (string, string) token = (tokenName, tokenMatch.Value);
                        if (tokenName != "WHITESPACE"){
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
    }
}