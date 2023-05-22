using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace LFAF_LABS{
    public enum TokenType{
        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        COMMA,
        ASSIGN_EQUAL,
        SEMICOLON,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        GREATER,
        LESS,
        NOT_EQUAL,
        IF,
        ELSE,
        FOR,
        RETURN,
        BREAK,
        VAR,
        FUNCTION,
        ID,
        NUMBER,
        STRING,
        WHITESPACE,
        COMMENT
    }
}