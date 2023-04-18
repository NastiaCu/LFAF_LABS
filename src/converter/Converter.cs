using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LFAF_LABS{
    class Converter{

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
            List<string> existingSymbols = new List<string>(productions.Keys);
            List<string> keys = new List<string>(productions.Keys); 
            foreach (string vt in g.VT){
                string newSymbol = GenerateNewSymbol(existingSymbols);
                g.AddProduction(newSymbol, vt);
                existingSymbols.Add(newSymbol);
                g.AddVN(newSymbol);
                foreach (string lhs in keys){
                    List<string> rhs = productions[lhs];
                    for (int i = 0; i < rhs.Count; i++){
                        if (rhs[i].Length > 1){
                        string oldProduction = rhs[i];
                        string newProduction = oldProduction.Replace(vt, newSymbol);
                        rhs[i] = newProduction;
                        }
                    
                        else if (rhs[i].Length > 2) {
                            string oldProduction = rhs[i];
                            string[] nonterminals = oldProduction.Split(new[] {"X"}, StringSplitOptions.RemoveEmptyEntries);
                            string newProduction = nonterminals[0] + "X" + nonterminals[1];
                            string currentSymbol = newSymbol;
                            
                            for (int j = 2; j < nonterminals.Length; j++) {
                                string nextSymbol = GenerateNewSymbol(existingSymbols);
                                g.AddProduction(currentSymbol, "X" + nonterminals[j-1] + nextSymbol);
                                existingSymbols.Add(nextSymbol);
                                g.AddVN(nextSymbol);
                                currentSymbol = nextSymbol;
                                }
                            }
                        }
                
                    productions[lhs] = rhs;
                }
            }  
        }

        public bool IsTerminal(string symbol){
            return symbol.Length == 1 && char.IsLower(symbol[0]);
        }

        public string GenerateNewSymbol(List<string> existingSymbols){
            int i = 1;

            while (true){
                string newSymbol = "X" + i.ToString();

                if (!existingSymbols.Contains(newSymbol)){
                    return newSymbol;
                }

                i++;
            }
        }
    }
}
      