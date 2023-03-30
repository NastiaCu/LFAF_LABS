using System;
using static System.Text.Json.JsonSerializer;
using System.Linq;
using System.Collections.Generic;

namespace LFAF_LABS{
    class Program{
        public static void Main(String[] args){
            // //LAB_1
            // Grammar grammar = new Grammar();
            // grammar.AddVN("S");
            // grammar.AddVN("B");
            // grammar.AddVN("D");
            // grammar.AddVT("a");
            // grammar.AddVT("b");
            // grammar.AddVT("c");
            // grammar.AddProduction("S", "aB");
            // grammar.AddProduction("S", "bB");
            // grammar.AddProduction("B", "bD");
            // grammar.AddProduction("B", "cB");
            // grammar.AddProduction("B", "aS");
            // grammar.AddProduction("D", "aD");
            // grammar.AddProduction("D", "b");
            // grammar.AddStart("S");

            // grammar.GenerateString();
            // grammar.GenerateString();
            // grammar.GenerateString();
            // grammar.GenerateString();
            // grammar.GenerateString();            
            // FiniteAutomaton Automaton = grammar.ToFiniteAutomaton();
            
            // string s = "acbab";

            // if (Automaton.isValid(s)){
            //     Console.WriteLine(s + " is a valid string");
            // }
            // else Console.WriteLine(s + "is an invalid string");
            // Console.WriteLine();

            // //LAB_2
            // grammar.typeDefinition();
            // FiniteAutomaton automaton = new FiniteAutomaton();
            // automaton.AddState("q0");
            // automaton.AddState("q1");
            // automaton.AddState("q2");
            // automaton.AddState("q3");
            // automaton.AddAlphabet("a");
            // automaton.AddAlphabet("b");
            // automaton.AddAlphabet("c");
            // automaton.AddTransition("q0", "aq1");
            // automaton.AddTransition("q1", "bq2");
            // automaton.AddTransition("q2", "cq0");
            // automaton.AddTransition("q1", "aq3");
            // automaton.AddTransition("q0", "bq2");
            // automaton.AddTransition("q2", "cq3");
            // automaton.AddInitialState("q0");
            // automaton.AddFinalState("q3");

            // Console.WriteLine("Convertion of a FA to a Regular Grammar");
            // Grammar RG = automaton.ToGrammar();
            // Console.WriteLine("Set of non-terminal symbols: " + Serialize(RG.VN));
            // Console.WriteLine("Set of terminal symbols: " + Serialize(RG.VT));
            // Console.WriteLine("Set of productions: " + Serialize(RG.P));
            // Console.WriteLine();
            // automaton.isDeterministic();

            // FiniteAutomaton a = new FiniteAutomaton();
            // a.AddState("q0");
            // a.AddState("q1");
            // a.AddState("q2");
            // a.AddState("q3");
            // a.AddAlphabet("a");
            // a.AddAlphabet("b");
            // a.AddAlphabet("c");
            // a.AddTransition2("q0", "a", "q1");
            // a.AddTransition2("q1", "b", "q2");
            // a.AddTransition2("q2", "c", "q0");
            // a.AddTransition2("q1", "a", "q3");
            // a.AddTransition2("q0", "b", "q2");
            // a.AddTransition2("q2", "c", "q3");
            // a.AddInitialState("q0");
            // a.AddFinalState("q3");
            // Console.WriteLine("The NFA has the following set of transitions: " + Serialize(a.transitions2 ) );
            // Console.WriteLine();

            // Console.WriteLine("Convertion of the NFA to DFA");
            // a.ConvertToDFA(a);
            // a.ToGraph("fa.dot");       

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
           
            // //LAB_3
            string program = @"
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

            ";

            Lexer l = new Lexer();

            List<(string, string)> tokens = l.Tokenize(program);

            foreach ((string tokenName, string tokenValue) in tokens){
                Console.WriteLine($"Token: ({tokenName} : {tokenValue})"); 
            }
        }
    }
}