using System;

namespace LFAF_LABS{

    class Program{
        public static void Main(String[] args){
            
            Grammar grammar = new Grammar();
            FiniteAutomaton automaton = grammar.ToFiniteAutomaton();

            grammar.GenerateString();
            grammar.GenerateString();
            grammar.GenerateString();
            grammar.GenerateString();
            grammar.GenerateString();

            string s = "acbab";

            if (automaton.isValid(s)){
                Console.WriteLine(s + " is a valid string");
            }
            else Console.WriteLine(s + "is an invalid string");
        }
    }
}