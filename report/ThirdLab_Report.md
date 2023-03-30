# Topic: Lexer & Scanner

### Course: Formal Languages & Finite Automata

### Author: Anastasia Cunev

---

## Theory

A lexet is a tool that takes a string input and splits it on the list of tokens. Tokens are the set of rules/ types of the input string.
For example, if we have such tokens as

```c#
{ "PLUS", @"\+" },
{ "NUMBER", @"\d+(\.\d+)?" }
```

And the input string :

` 3 + 5`

The lexer would match that expression to the existing tokens and would give the following answer:

```
Token: (NUMBER : 3)
Token: (PLUS : +)
Token: (NUMBER : 5)
```

## Objectives:

1. Understand what lexical analysis [1] is.
2. Get familiar with the inner workings of a lexer/scanner/tokenizer.
3. Implement a sample lexer and show how it works.

## Implementation description

First of all we have to define all the tokens in a dictionary.

```c#
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
    { "COMMENT", @"\//"}
};
```

After that I made a constructor for the `lexer` class, which iterates through each item in the dictionary `tokenPatterns` and adds it to the `tokenRegexes` list, making the Regex object from the value in the `tokenPatterns` dictionary.

```c#
private List<(string, Regex)> tokenRegexes = new List<(string, Regex)>();

public Lexer(){
    foreach (KeyValuePair<string, string> pattern in tokenPatterns){
        tokenRegexes.Add((pattern.Key, new Regex("^" + pattern.Value)));
    }
}
```

Function to check if a string is a comment:

```c#
private bool IsComment(string token){
   return token.StartsWith("//");
}
```

The following `Tokenize` function takes a string input and returns a list of tokens (tokenName and its value). After that I defined a list `tokens`, where will be stored matched tokens. In the while loop, which iterates untill the string is not empty, the code first checks if the string is a `COMMENT`, and if it is, it ignores the entire line. After than it goes through each regular expression in the list `tokenRegexes`, where each item in the list is a pair of a string (the name of a token) and a regular expression used to identify that token in the input program. After that we try to match every regular expression to the beginning of the input string using `Match` function. If the match succeeds, and it's not the `WHITESPACE` token, we add it to the list of tokens. After that we take the unprocessed string using `Substring` function. If the code gets an unknown symbol, it prints the error.

```c#
public List<(string, string)> Tokenize(string program){
    List<(string, string)> tokens = new List<(string, string)>();

    while (!string.IsNullOrEmpty(program)){
        bool match = false;

        if (IsComment(program)){
            int endOfLine = program.IndexOf('\n');
            program = program.Substring(endOfLine + 1);
            continue;
        }

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
```

## Conclusions / Screenshots / Results

After performing this laboratory work I undersood what lexical analysis is and got familiar with the inner workings of a tokenizer. I also implemented a lexer using simple tokens, which can define such types as `VAR`, `FUNCTION`, different operators, numbers and strings.

For the following piece of code:

```c#
function func(){
    var x = 34;
    var y = 55;
    var string = ""hello world"";

    if (x != y){
        return y;
    }
    //some comment

    else{
        for (int i = 0; i < 10; i++){
            x++;
        }
        break;
    }
}
```

The lexer will produce the following result:

```
Token: (FUNCTION : function)
Token: (ID : func)
Token: (LPAREN : ()
Token: (RPAREN : ))
Token: (LBRACE : {)
Token: (VAR : var)
Token: (ID : x)
Token: (ASSIGN_EQUAL : =)
Token: (NUMBER : 34)
Token: (SEMICOLON : ;)
Token: (VAR : var)
Token: (ID : y)
Token: (ASSIGN_EQUAL : =)
Token: (NUMBER : 55)
Token: (SEMICOLON : ;)
Token: (VAR : var)
Token: (ID : string)
Token: (ASSIGN_EQUAL : =)
Token: (STRING : "hello world")
Token: (SEMICOLON : ;)
Token: (IF : if)
Token: (LPAREN : ()
Token: (ID : x)
Token: (NOT_EQUAL : !=)
Token: (ID : y)
Token: (RPAREN : ))
Token: (LBRACE : {)
Token: (RETURN : return)
Token: (ID : y)
Token: (SEMICOLON : ;)
Token: (RBRACE : })
Token: (ELSE : else)
Token: (LBRACE : {)
Token: (FOR : for)
Token: (LPAREN : ()
Token: (ID : int)
Token: (ID : i)
Token: (ASSIGN_EQUAL : =)
Token: (NUMBER : 0)
Token: (SEMICOLON : ;)
Token: (ID : i)
Token: (LESS : <)
Token: (NUMBER : 10)
Token: (SEMICOLON : ;)
Token: (ID : i)
Token: (PLUS : +)
Token: (PLUS : +)
Token: (RPAREN : ))
Token: (LBRACE : {)
Token: (ID : x)
Token: (PLUS : +)
Token: (PLUS : +)
Token: (SEMICOLON : ;)
Token: (RBRACE : })
Token: (BREAK : break)
Token: (SEMICOLON : ;)
Token: (RBRACE : })
```

## References

1. https://github.com/DrVasile/FLFA-Labs

2. https://en.wikipedia.org/wiki/Lexical_analysis

3. https://else.fcim.utm.md/pluginfile.php/65934/mod_resource/content/1/Lexer%20_%20Scanner.pdf
