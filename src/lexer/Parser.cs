using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LFAF_LABS{
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
        
        private ASTNode ParseVariableDeclaration(){
            Consume(TokenType.VAR);
            var identifier = Consume(TokenType.ID).Item2;
            Consume(TokenType.ASSIGN_EQUAL);
            var initializer = ParseExpression();
            Consume(TokenType.SEMICOLON);

            var variableDeclarationNode = new ASTNode("VariableDeclaration");
            variableDeclarationNode.AddChild(new ASTNode(identifier));
            variableDeclarationNode.AddChild(initializer);

            return variableDeclarationNode;
        }

        private ASTNode ParseFunctionDeclarationStatement(){
            var functionDeclarationNode = new ASTNode("FunctionDeclaration");
            Consume(TokenType.FUNCTION);
            var identifierToken = Consume(TokenType.ID);
            var identifierNode = new ASTNode(identifierToken.Item2);
            functionDeclarationNode.AddChild(identifierNode);
            Consume(TokenType.LPAREN);
            var parameters = ParseFunctionParameters();
            functionDeclarationNode.Children.AddRange(parameters);
            Consume(TokenType.RPAREN);
            var functionBody = ParseBlock();
            functionDeclarationNode.AddChild(functionBody);
            return functionDeclarationNode;
        }

        private List<ASTNode> ParseFunctionParameters(){
            var parameters = new List<ASTNode>();
            while (Match(TokenType.ID)){
                var identifierToken = Consume(TokenType.ID);
                var identifierNode = new ASTNode(identifierToken.Item2);
                parameters.Add(identifierNode);
                if (!Match(TokenType.RPAREN))
                    Consume(TokenType.COMMA);
            }
            return parameters;
        }

        private ASTNode ParseIfStatement(){
            Consume(TokenType.IF);
            Consume(TokenType.LPAREN);
            var condition = ParseExpression();
            Consume(TokenType.RPAREN);
            var ifBlock = ParseBlock();
            if (Match(TokenType.ELSE)){
                Consume(TokenType.ELSE);
                var elseBlock = ParseBlock();
                var ifElseNode = new ASTNode("IfElseStatement");
                ifElseNode.AddChild(condition);
                ifElseNode.AddChild(ifBlock);
                ifElseNode.AddChild(elseBlock);
                return ifElseNode;
            }

            else{
                var ifNode = new ASTNode("IfStatement");
                ifNode.AddChild(condition);
                ifNode.AddChild(ifBlock);
                return ifNode;
            }
        }


        private ASTNode ParseForStatement(){
            Consume(TokenType.FOR);
            Consume(TokenType.LPAREN);

            var initialization = ParseExpressionStatement();
            Consume(TokenType.SEMICOLON);

            var condition = ParseExpression();
            Consume(TokenType.SEMICOLON);

            var increment = ParseExpression();
            Consume(TokenType.RPAREN);

            var body = ParseBlock();

            var forNode = new ASTNode("ForStatement");
            forNode.AddChild(initialization);
            forNode.AddChild(condition);
            forNode.AddChild(increment);
            forNode.AddChild(body);

            return forNode;
        }


        private ASTNode ParseReturnStatement(){
            var returnNode = new ASTNode("ReturnStatement");
            Consume(TokenType.RETURN);
            if (!Match(TokenType.SEMICOLON)){
                var expression = ParseExpression();
                returnNode.AddChild(expression);
            }
            Consume(TokenType.SEMICOLON);
            return returnNode;
        }

        private ASTNode ParseBreakStatement(){
            Consume(TokenType.BREAK);
            Consume(TokenType.SEMICOLON);
            return new ASTNode("BreakStatement");
        }

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

        private ASTNode ParseEqualityExpression(){
            var left = ParseRelationalExpression();
            while (Match(TokenType.NOT_EQUAL)){
                var operatorToken = Consume(TokenType.NOT_EQUAL);
                var right = ParseRelationalExpression();
                var equalityNode = new ASTNode(operatorToken.Item2);
                equalityNode.AddChild(left);
                equalityNode.AddChild(right);
                left = equalityNode;
            }
            return left;
        }

        private ASTNode ParseRelationalExpression(){
            var left = ParseAdditiveExpression();
            while (Match(TokenType.GREATER) || Match(TokenType.LESS)){
                var operatorToken = Consume(TokenType.GREATER, TokenType.LESS);
                var right = ParseAdditiveExpression();
                var relationalNode = new ASTNode(operatorToken.Item2);
                relationalNode.AddChild(left);
                relationalNode.AddChild(right);
                left = relationalNode;
            }
            return left;
        }

        private ASTNode ParseAdditiveExpression(){
            var left = ParseMultiplicativeExpression();
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS)){
                var operatorToken = Consume(TokenType.PLUS, TokenType.MINUS);
                var right = ParseMultiplicativeExpression();
                var additiveNode = new ASTNode(operatorToken.Item2);
                additiveNode.AddChild(left);
                additiveNode.AddChild(right);
                left = additiveNode;
            }
            return left;
        }

        private ASTNode ParseMultiplicativeExpression(){
            var left = ParsePrimaryExpression();
            while (Match(TokenType.MULTIPLY) || Match(TokenType.DIVIDE)){
                var operatorToken = Consume(TokenType.MULTIPLY, TokenType.DIVIDE);
                var right = ParsePrimaryExpression();
                var multiplicativeNode = new ASTNode(operatorToken.Item2);
                multiplicativeNode.AddChild(left);
                multiplicativeNode.AddChild(right);
                left = multiplicativeNode;
            }
            return left;
        }

        private ASTNode ParsePrimaryExpression(){
            if (Match(TokenType.NUMBER) || Match(TokenType.STRING) || Match(TokenType.ID)){
                var token = Consume(TokenType.NUMBER, TokenType.STRING, TokenType.ID);
                return new ASTNode(token.Item2);
            }

            if (Match(TokenType.LPAREN)){
                Consume(TokenType.LPAREN);
                var expression = ParseExpression();
                Consume(TokenType.RPAREN);
                return expression;
            }

            if (Match(TokenType.LBRACE)){
                return ParseBlock();
            }
            throw new Exception("Invalid syntax: " + tokens[currentTokenIndex].ToString());
        }


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
    }
}
