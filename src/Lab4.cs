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
            grammar.AddStart("S");

            Console.WriteLine("Original Grammar");
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }
            
            Console.WriteLine("--------------------------");
            Console.WriteLine("Eliminated epsilon productions");
            Converter c = new Converter(grammar);
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
            Console.WriteLine("Intermidiate result");

            c.ConvertToCNF(grammar.P, grammar);
           
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine("Final Result");

            c.GetNewRightProd();
            foreach (KeyValuePair<string, List<string>> entry in grammar.P){
                Console.WriteLine($"{entry.Key} -> {string.Join(" | ", entry.Value)}");
            }            
        }
    }
}