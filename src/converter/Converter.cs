using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LFAF_LABS{
    class Converter{

        private Grammar grammar;
        private Dictionary<string, string> newProdLookUp = new Dictionary<string, string>();
        private static Dictionary<string, List<string>> newProductions = new Dictionary<string, List<string>>();
        private static int count = 0;

        public Converter(Grammar grammar) {
            this.grammar = grammar;
        }

        public void EliminateEProductions(Dictionary<string, List<string>> productions){
            List<string> nullableSymbols = productions.Where(x => x.Value.Contains("e")).Select(x => x.Key).ToList();

            foreach (string symbol in productions.Keys.ToList()){
                List<string> newProductions = new List<string>();

                foreach (string production in productions[symbol]){
                    if (production == "e"){
                        continue;
                    }

                    string buffer = production;

                    foreach (string nullableSymbol in nullableSymbols){
                        buffer = buffer.Replace(nullableSymbol, "");
                    }

                    if (buffer == ""){
                        newProductions.Add("e");
                    }

                    else{
                        newProductions.Add(buffer);
                    }

                    if (newProductions.Last() != production){
                        newProductions.Add(production);
                    }
                }
                productions[symbol] = newProductions;                
            }
        }

        public void EliminateUnitProductions(Dictionary<string, List<string>> productions){
            bool changed = true;

            while (changed){
                changed = false;

                foreach (string symbol in productions.Keys.ToList()){
                    List<string> unitProductions = productions[symbol].Where(p => p.Length == 1 && char.IsUpper(p[0])).ToList();

                    if (unitProductions.Count > 0){
                        changed = true;
                        productions[symbol].RemoveAll(p => p.Length == 1 && char.IsUpper(p[0]));
                        productions[symbol].AddRange(unitProductions.SelectMany(up => productions[up]));
                    }
                }
            }
        }

        public void EliminateNonProductiveSymbols(Dictionary<string, List<string>> productions, Grammar g){
            List<string> productiveSymbols = new List<string>();

            foreach (string symbol in productions.Keys){
                foreach (string production in productions[symbol]){
                    if (production.Length == 1 && char.IsLower(production[0]) || IsProductive(production, productiveSymbols)){
                        if (!productiveSymbols.Contains(symbol)){
                            productiveSymbols.Add(symbol);
                        }
                        break;
                    }
                }
            }

            List<string> nonTerminals = new List<string>(g.GetVN());
            nonTerminals.RemoveAll(symbol => productiveSymbols.Contains(symbol));
            List<string> nonProductiveSymbols = nonTerminals;

            foreach (string symbol in nonProductiveSymbols){
                productions.Remove(symbol);

                foreach (List<string> prods in productions.Values){
                    prods.RemoveAll(production => production.Contains(symbol));
                }
            }
        }

        private bool IsProductive(string production, List<string> productiveSymbols){
            foreach (char c in production){
                if (char.IsUpper(c) && !productiveSymbols.Contains(c.ToString())){
                    return false;
                }
            }

            return true;
        }

        public void EliminateInaccesibleSymbols(Dictionary<string, List<string>> productions){
            List<string> accesibleSymbols = new List<string> { "S" };
            List<string> currentSymbols = new List<string>(accesibleSymbols);

            while (currentSymbols.Count > 0){
                List<string> newSymbols = new List<string>();

                foreach (string symbol in currentSymbols){
                    foreach (string production in productions[symbol]){
                        foreach (char c in production){
                            if (char.IsUpper(c)){
                                string newSymbol = c.ToString();
                                if (!newSymbols.Contains(newSymbol) && !accesibleSymbols.Contains(newSymbol)){
                                    newSymbols.Add(newSymbol);
                                }
                            }
                        }
                    }
                }

                accesibleSymbols.AddRange(newSymbols);
                currentSymbols = new List<string>(newSymbols);

            }

            List<string> inaccesibleSymbols = new List<string>(productions.Keys);
            inaccesibleSymbols.RemoveAll(symbol => accesibleSymbols.Contains(symbol));

            foreach (string inSymbol in inaccesibleSymbols){
                productions.Remove(inSymbol);
                foreach (List<string> prods in productions.Values){
                    prods.RemoveAll(production => production.Contains(inSymbol));
                }
            }
        }

        public void ConvertToCNF(Dictionary<string, List<string>> productions, Grammar g){
            List<string> keys = new List<string>(productions.Keys); 

            foreach (string vt in g.VT){
                string newSymbol = NewLP();
                g.AddProduction(newSymbol, vt);
                g.AddVN(newSymbol);

                foreach (string lhs in keys){
                    List<string> rhs = productions[lhs];

                    for (int i = 0; i < rhs.Count; i++){
                        if (rhs[i].Length > 1){
                            string oldProduction = rhs[i];
                            string newProduction = oldProduction.Replace(vt, newSymbol);
                            rhs[i] = newProduction;
                        }
                    }
                    productions[lhs] = rhs;
                }
            }
        }

        public string NewLP(){
            return "X" + count++;
        }

        public void GetNewRightProd(){
            foreach (string key in grammar.GetP().Keys){
                List<string> values = grammar.GetP()[key];
                values = values.Select(FormProd).ToList();
                grammar.GetP()[key] = values;
            }
            
            foreach (KeyValuePair<string, List<string>> entry in newProductions){
                string key = entry.Key;
                List<string> values = entry.Value;
                grammar.GetP()[key] = values;
            }
        }

        public string FormProd(string prod) {
            int upperCount = prod.Count(char.IsUpper);
            while (upperCount > 2){
                int append = 0;
                string newGroup = "";
                for (int i = 0; i < prod.Length; i++){
                    if (append < 2) {
                        if (prod[i] == 'X'){
                            append++;
                            newGroup += prod.Substring(i, 2);
                            i++;
                        }

                        else{
                            append++;
                            newGroup += prod.Substring(i, 1);
                        }
                    }
                }

                if (newProdLookUp.ContainsKey(newGroup)){
                    prod = prod.Replace(newGroup, newProdLookUp[newGroup]);
                }

                else{
                    string newLP = NewLP();
                    newProductions[newLP] = new List<string>() { newGroup };
                    newProdLookUp[newGroup] = newLP;
                    grammar.AddVN(newLP);
                    prod = prod.Replace(newGroup, newLP);
                }
                upperCount = prod.Count(char.IsUpper);
            }
            return prod;
        }

        public bool IsTerminal(string symbol){
            return symbol.Length == 1 && char.IsLower(symbol[0]);
        }
    }
}