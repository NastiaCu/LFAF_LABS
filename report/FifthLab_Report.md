# Topic: Parser & Building an Abstract Syntax Tree

### Course: Formal Languages & Finite Automata

### Author: Anastasia Cunev

---

## Theory

**Abstract Syntax Tree (AST):**
An AST is a data structure commonly used in programming language compilers and interpreters. It represents the structure of source code in a tree-like format, where each node corresponds to a syntactic element of the code. ASTs are useful for analyzing and manipulating code during compilation or interpretation processes.

**Parser:**
A parser is a program or software component that analyzes the structure of a text or code according to a specific grammar or syntax. Its primary purpose is to interpret the input and determine whether it conforms to the specified rules or grammar.

## Objectives:

1. Get familiar with parsing, what it is and how it can be programmed [1].
2. Get familiar with the concept of AST [2].
3. In addition to what has been done in the 3rd lab work do the following:
   1. In case you didn't have a type that denotes the possible types of tokens you need to:
      1. Have a type **_TokenType_** (like an enum) that can be used in the lexical analysis to categorize the tokens.
      2. Please use regular expressions to identify the type of the token.
   2. Implement the necessary data structures for an AST that could be used for the text you have processed in the 3rd lab work.
   3. Implement a simple parser program that could extract the syntactic information from the input text.

## Implementation description

For this laboratory work I added `TokenType` which has all possible token types.

```c#
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
```

For this laboratory work I had to implement the `AST` class. The `ASTNode` class represents a node in an abstract syntax tree. It has a `Value` property to store the value or label of the node, and a `Children` list to store its child nodes. The `AddChild` method adds a child node to the current node. The `Visualize` method is used to print a formatted visualization of the `AST` structure. It recursively traverses the `AST`, printing each node with appropriate indentation. The `GetIndentation` helper method calculates the indentation based on the given indent level.

```c#
public class ASTNode{
    public string Value { get; }
    public List<ASTNode> Children { get; }

    public ASTNode(string value){
        Value = value;
        Children = new List<ASTNode>();
    }

    public void AddChild(ASTNode child){
        Children.Add(child);
    }

    public void Visualize(int indentLevel = 0, bool isLastChild = true){
        string indent = GetIndentation(indentLevel);
        string connector = isLastChild ? "└── " : "├── ";

        Console.Write(indent + connector + Value);

        if (Children != null){
            Console.WriteLine();

            for (int i = 0; i < Children.Count; i++){
                bool isLast = i == Children.Count - 1;
                Children[i].Visualize(indentLevel + 1, isLast);
            }
        }

        else{
            Console.WriteLine();
        }
    }

    private string GetIndentation(int indentLevel){
        const int spacesPerIndent = 4;
        int spaces = indentLevel * spacesPerIndent;

        return new string(' ', spaces);
    }
}
```

Further are presented some methods of the `Parser`. It provides a `Parse` method that takes a list of tokens as input and returns the root `ASTNode` representing the program. The `Parse` method initializes the parser state and calls the `ParseProgram` method.

The `ParseProgram` method creates the root node for the program and iteratively calls the `ParseStatement` method to parse each statement in the program. The parsed statement nodes are added as children to the program node, which is then returned as the result.

The `ParseStatement` method checks the type of the current token and calls the corresponding parsing method based on the token type. If none of the statement types match, it assumes it to be an expression statement and calls `ParseExpressionStatement`.

```c#
public class Parser{
    private List<(TokenType, string)> tokens = new List<(TokenType, string)>();
    private int currentTokenIndex;

    public ASTNode Parse(List<(TokenType, string)> tokens){
        this.tokens = tokens;
        currentTokenIndex = 0;
        return ParseProgram();
    }

    private ASTNode ParseProgram(){
        var programNode = new ASTNode("Program");
        while (!IsEndOfTokens()){
            var statement = ParseStatement();
            programNode.AddChild(statement);
        }
        return programNode;
    }

    private ASTNode ParseStatement(){
        if (Match(TokenType.VAR))
            return ParseVariableDeclarationStatement();
        if (Match(TokenType.FUNCTION))
            return ParseFunctionDeclarationStatement();
        if (Match(TokenType.IF))
            return ParseIfStatement();
        if (Match(TokenType.FOR))
            return ParseForStatement();
        if (Match(TokenType.RETURN))
            return ParseReturnStatement();
        if (Match(TokenType.BREAK))
            return ParseBreakStatement();
        return ParseExpressionStatement();
    }
}
```

`ParseExpressionStatement`: Parses an expression statement. If the current token matches the variable declaration token, it calls `ParseVariableDeclaration` to parse the variable declaration statement. Otherwise, it calls `ParseExpression` to parse a general expression.

`ParseExpression`: Parses an expression. It delegates the parsing to `ParseAssignmentExpression` to handle assignment expressions.

`ParseAssignmentExpression`: Parses an assignment expression. It first parses the left-hand side of the assignment by calling `ParseEqualityExpression`. If the current token matches the assignment operator token, it consumes the token, parses the right-hand side of the assignment recursively by calling `ParseAssignmentExpression`, and creates an `AST` node for the assignment expression. The left and right expressions are added as children to the assignment node. Finally, it returns the assignment node. If there is no assignment operator, it simply returns the left-hand side expression.

```c#
private ASTNode ParseExpressionStatement(){
    if (Match(TokenType.VAR)){
       return ParseVariableDeclaration();
    }

    return ParseExpression();
}


private ASTNode ParseExpression(){
    return ParseAssignmentExpression();
}

private ASTNode ParseAssignmentExpression(){
    var left = ParseEqualityExpression();
    if (Match(TokenType.ASSIGN_EQUAL)){
       Consume(TokenType.ASSIGN_EQUAL);
       var right = ParseAssignmentExpression();
       var assignmentNode = new ASTNode("AssignmentExpression");
       assignmentNode.AddChild(left);
       assignmentNode.AddChild(right);
       return assignmentNode;
    }
    return left;
}
```

The `ParseVariableDeclarationStatement` method parses a variable declaration statement. It creates an `ASTNode` for the variable declaration, consumes the `VAR` token, and parses the identifier and optional expression. The identifier node and the expression node (if present) are added as children to the variable declaration node, and the method returns the variable declaration node.

```c#
private ASTNode ParseVariableDeclarationStatement(){
    var variableDeclarationNode = new ASTNode("VariableDeclaration");
    Consume(TokenType.VAR);
    var identifierToken = Consume(TokenType.ID);
    var identifierNode = new ASTNode(identifierToken.Item2);
    variableDeclarationNode.AddChild(identifierNode);
    if (Match(TokenType.ASSIGN_EQUAL)){
        Consume(TokenType.ASSIGN_EQUAL);
        var expression = ParseExpression();
        variableDeclarationNode.AddChild(expression);
    }
    Consume(TokenType.SEMICOLON);
    return variableDeclarationNode;
}
```

Additional helper methods:

`ParseBlock`: Parses a block of code by iterating through statements within the block using `ParseStatement` and adding them as children to a new "Block" AST node. It expects the block to start with a left brace token and end with a right brace token.

`Consume`: Consumes (matches and moves to the next) tokens from the token stream. It checks if the current token matches any expected token types, returns the matched token, and advances the current token index. If no match is found, it throws an exception for invalid syntax.

`Match`: Checks if the current token matches any expected token types. Returns true if a match is found, otherwise false. It also checks if the end of the token stream has been reached.

`IsEndOfTokens`: Checks if the current token index has exceeded the total number of tokens in the token stream, indicating the end of the token stream.

```c#
private ASTNode ParseBlock(){
    var blockNode = new ASTNode("Block");
    Consume(TokenType.LBRACE);
    while (!Match(TokenType.RBRACE)){
        var statement = ParseStatement();
        blockNode.AddChild(statement);
    }
    Consume(TokenType.RBRACE);
    return blockNode;
}

private (TokenType, string) Consume(params TokenType[] expectedTokenTypes){
    foreach (var tokenType in expectedTokenTypes){
        if (Match(tokenType)){
            var token = tokens[currentTokenIndex++];
            return token;
        }
    }

    throw new Exception("Invalid syntax: " + tokens[currentTokenIndex].ToString());
}

private bool Match(params TokenType[] expectedTokenTypes){
    if (IsEndOfTokens())
        return false;
    return expectedTokenTypes.Contains(tokens[currentTokenIndex].Item1);
}

private bool IsEndOfTokens(){
    return currentTokenIndex >= tokens.Count;
}
```

## Conclusions / Screenshots / Results

After performing this laboratory work I undersood what is an `AST` and a `Parser` and what role they play in the language design and implementation. I implemented the AST, which consrtucts a tree, based on the user input and a parser, which has methods needed to handle different types of statements and their corresponding parsing rules.

For the following piece of code:

```c#
function func(){
    var x = 34;
    var y = 55;

    if (x != y){
        return y;
    }
    //some comment
    else{
        var x = 24;
    }
}
```

The AST will look like that:

```
└── Program
    └── FunctionDeclaration
        ├── func
        └── Block
            ├── VariableDeclaration
                ├── x
                └── 34
            ├── VariableDeclaration
                ├── y
                └── 55
            └── IfElseStatement
                ├── !=
                    ├── x
                    └── y
                ├── Block
                    └── ReturnStatement
                        └── y
                └── Block
                    └── VariableDeclaration
                        ├── x
                        └── 24
```

## References

1. https://github.com/DrVasile/FLFA-Labs

2. https://en.wikipedia.org/wiki/Abstract_syntax_tree

3. https://en.wikipedia.org/wiki/Parsing
