using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LFAF_LABS{
    public class Token{
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value){
            Type = type;
            Value = value;
        }

        public override string ToString(){
            return $"{Type}({Value})";
        }
    }
}
