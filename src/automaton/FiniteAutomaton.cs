using System;
using static System.Text.Json.JsonSerializer;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace LFAF_LABS{

    class FiniteAutomaton{

        public List<string> states = new List<string>();
        public List<string> alphabet = new List<string>();
        public Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>();
        private string initialState = "";
        private List<string> finalStates = new List<string>();
        public Dictionary<string, Dictionary<string, List<string>>> transitions2 = new Dictionary<string, Dictionary<string, List<string>>>();

        public List<string> GetStates(){
            return states;
        }

        public List<string> GetAlphabet(){
            return alphabet;
        }

        public Dictionary<string, List<string>> GetTransitions(){
            return transitions;
        }

        public string GetInitialState(){
            return initialState;
        }

        public List<string> GetFinalStates(){
            return finalStates;
        }

        public void AddState(string state){
            states.Add(state);
        }

        public void AddAlphabet(string alph){
            alphabet.Add(alph);
        }

        public void AddInitialState(String initialState) {
            this.initialState = initialState;
        }

        public void AddFinalState(String finalState) {
            finalStates.Add(finalState);
        }

        public void AddTransition(string fromState, string alph){
        
            if (!transitions.ContainsKey(fromState)){
                transitions[fromState] = new List<string>();
            }

            transitions[fromState].Add(alph);
        }

        public void AddTransition2(string fromState, string symbol, string toState){
            if (!transitions2.ContainsKey(fromState)){
                transitions2.Add(fromState, new Dictionary<string, List<string>>());
            }
            if (!transitions2[fromState].ContainsKey(symbol)){
                transitions2[fromState].Add(symbol, new List<string>());
            }
            if (!states.Contains(toState)){
                states.Add(toState);
            }
            transitions2[fromState][symbol].Add(toState);
        }  

        public bool isValid(string input){
            
            string current = initialState;

            foreach (char symbol in input){
                if (!alphabet.Contains(symbol.ToString())){
                    return false;
                }

                bool found = true;
                foreach (KeyValuePair<string, List<string>> transition in transitions){
                    if (transition.Key == current && transition.Value.Contains(symbol.ToString())){
                        current = transition.Key;
                        found = true;
                        break;
                    }
                }

                if (!found){
                    return false;
                }
            }

            return true;
        }

        public Grammar ToGrammar(){
            Grammar grammar = new Grammar();
            
            foreach (string symbol in states){
                grammar.AddVN(symbol);
            }

            foreach (string symbol in alphabet){
                grammar.AddVT(symbol);
            }

            foreach (KeyValuePair<string, List<string>> transition in transitions){
                
                foreach (string value in transition.Value){
                    grammar.AddProduction(transition.Key, value);
                }
            }

            grammar.AddStart(initialState);

            return grammar;
        }

        public void isDeterministic(){
            bool isNFA = false;
            foreach (KeyValuePair<string, List<string>> transition in transitions){
                List<string> t = transition.Value;
                List<char> processed = new List<char>();
               
                foreach (string w in t){
                    char alphabet = w[0];

                    if (processed.Contains(alphabet)){
                        isNFA = true;
                    }
        
                    processed.Add(alphabet);
                }
            }

            if (isNFA) Console.WriteLine("The FA is a NFA");
            else Console.WriteLine("The FA is a DFA");
        }

        public FiniteAutomaton ConvertToDFA(FiniteAutomaton nfa){
            FiniteAutomaton dfa = new FiniteAutomaton();

            dfa.AddState(string.Join(",", initialState));
            dfa.AddInitialState(string.Join(",", initialState));

            Queue<string> unprocessedDfaStates = new Queue<string>();
            unprocessedDfaStates.Enqueue(string.Join(",", initialState));

            while (unprocessedDfaStates.Count > 0){
                string dfaState = unprocessedDfaStates.Dequeue();

                foreach (string finalState in nfa.finalStates){
                    if (dfaState.Split(',').Contains(finalState)){
                        dfa.AddFinalState(dfaState);
                        break;
                    }
                }

                foreach (string symbol in alphabet){
                    HashSet<string> nfaStates = new HashSet<string>();
                    foreach (string state in dfaState.Split(',')){
                        if (nfa.transitions2.ContainsKey(state) && nfa.transitions2[state].ContainsKey(symbol)){
                            nfaStates.UnionWith(nfa.transitions2[state][symbol]);
                        }
                    }

                    if (nfaStates.Count == 0){
                        continue;
                    }

                    string newDfaState = string.Join(",", nfaStates);

                    if (!dfa.GetStates().Contains(newDfaState)){
                        dfa.AddState(newDfaState);
                        unprocessedDfaStates.Enqueue(newDfaState);
                    }

                    dfa.AddTransition2(dfaState, symbol, newDfaState);
                }
            }

            Console.WriteLine("Set of states for the new DFA" + Serialize(dfa.states));
            Console.WriteLine("Set of final states for the new DFA" + Serialize(dfa.finalStates));
            Console.WriteLine("Set of transitions for the new DFA" + Serialize(dfa.transitions2));

            dfa.ToGraph("dfa.dot");

            return dfa;
        }

        public void ToGraph(string filename){
            using (StreamWriter writer = new StreamWriter(filename)){
                writer.WriteLine("digraph FA {");

                writer.WriteLine("\trankdir=LR;");
                writer.WriteLine("\tsize=\"8,5\"");
                writer.WriteLine("\tnode [shape = circle];");

                writer.WriteLine($"\t\"\" -> \"{initialState}\";");

                foreach (string state in states){
                    writer.Write($"\t\"{state}\"");
                    if (finalStates.Contains(state))
                    {
                        writer.Write(" [shape = doublecircle]");
                    }
                    writer.WriteLine(";");
                }

                foreach (KeyValuePair<string, Dictionary<string, List<string>>> entry in transitions2){
                    string fromState = entry.Key;
                    foreach (KeyValuePair<string, List<string>> subentry in entry.Value){
                        string alph = subentry.Key;
                        foreach (string toState in subentry.Value){
                            writer.WriteLine($"\t\"{fromState}\" -> \"{toState}\" [label = \"{alph}\"];");
                        }
                    }
                }
                writer.WriteLine("}");
            }
        }
    }
}
