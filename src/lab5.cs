using System;
using System.Collections.Generic;

namespace LFAF_LABS{
    class Program{
        static void Main(string[] args){

            var lexer = new Lexer();
            var parser = new Parser();
            
            string program = @"
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
            ";

            List<(TokenType, string)> tokens = lexer.Tokenize(program);
            ASTNode ast = parser.Parse(tokens);
            ast.Visualize();

        }
    }
}
