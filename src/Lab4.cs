using System;
using static System.Text.Json.JsonSerializer;
using System.Linq;
using System.Collections.Generic;

namespace LFAF_LABS{
    class Program{
        public static void Main(String[] args){
            Grammar grammar = new Grammar();
            grammar.AddVN("S");
            grammar.AddVN("A");
            grammar.AddVN("B");
            grammar.AddVN("C");
            grammar.AddVN("D");
            grammar.AddVT("a");
            grammar.AddVT("b");
            grammar.AddProduction("S", "bA");
            grammar.AddProduction("S", "AC");
            grammar.AddProduction("A", "bS");
            grammar.AddProduction("A", "BC");
            grammar.AddProduction("A", "AbAa");
            grammar.AddProduction("B", "BbaA");
            grammar.AddProduction("B", "a");
            grammar.AddProduction("B", "bSa");
            grammar.AddProduction("C", "e");
            grammar.AddProduction("D", "AB");
            // grammar.AddProduction("S", "dB");
            // grammar.AddProduction("S", "A");
            // grammar.AddProduction("A", "d");
            // grammar.AddProduction("A", "dS");
            // grammar.AddProduction("A", "aBdAB");
            // grammar.AddProduction("B", "a");
            // grammar.AddProduction("B", "dA");
            // grammar.AddProduction("B", "A");
            // grammar.AddProduction("B", "e");
            // grammar.AddProduction("C", "Aa");
            grammar.AddStart("S");

            Console.WriteLine("Original Grammar");
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }
            
            Console.WriteLine("--------------------------");
            Console.WriteLine("Eliminated epsilon productions");
            Converter c = new Converter();
            c.EliminateEProductions(grammar.P);

            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine("Eliminated unit productions");
            c.EliminateUnitProductions(grammar.P);
           
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }
        
            Console.WriteLine("--------------------------");
            Console.WriteLine("Eliminated non-productive symbols");
            c.EliminateNonProductiveSymbols(grammar.P, grammar);
           
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine("Eliminated inaccessible symbols");
            c.EliminateInaccesibleSymbols(grammar.P);
           
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine("CNF grammar");
            // c.ConvertToTwoNonterminalsOrTerminal(grammar.P, grammar);
            // c.ConvertToTwoNonterminals(grammar.P, grammar);
            c.ConvertToCNF(grammar.P, grammar);
           
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }
            foreach (string s in grammar.VN){
                Console.WriteLine(s);
            }
        }
    }
}