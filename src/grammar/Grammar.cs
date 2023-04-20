using System;

namespace LFAF_LABS{

    class Grammar{

        private string start = "";
        public List<string> VN = new List<string>();
        public List<string> VT = new List<string>();
        public Dictionary<string, List<string>> P = new Dictionary<string, List<string>>();

        public List<string> GetVN(){
            return VN;
        }

        public List<string> GetVT(){
            return VT;
        }

        public Dictionary<string, List<string>> GetP(){
            // Console.WriteLine(P);
            return P;
        }

        public string GetStart(){
            return start;
        }

        public void AddVN(string vn){
            VN.Add(vn);
        }

        public void AddVT(string vt){
            VT.Add(vt);
        }

        public void AddStart(String start){
            this.start = start;
        }

        public void AddProduction(string VN, string production){
            if (!P.ContainsKey(VN)){
                P[VN] = new List<string>();
            }

            P[VN].Add(production);
        }

        public void GenerateString(){
            
            Random random = new Random();
            string s = this.start;
            
            while (true){
                string word = "";
                
                for (int i = 0; i < s.Length; i++){
                    string symbol = s[i].ToString();
                    if (VN.Contains(symbol)){
                        List<string> options = P[symbol];
                        int optionIndex = random.Next(options.Count);
                        word += options[optionIndex];
                    }
                
                    else{
                        word += symbol;
                    }
                }

                if (!IsTerminal(word)){
                    Console.WriteLine(word);
                    s = word;
                }

                else{
                    Console.WriteLine("Final result: " + word);
                    break;
                }
            }
        }

        private bool IsTerminal(string s){
            
            for (int i = 0; i < s.Length; i++){
                if (!VT.Contains(s[i].ToString())){
                    return false;
                }
            }

            return true;
        }

        public FiniteAutomaton ToFiniteAutomaton(){

            FiniteAutomaton automaton = new FiniteAutomaton();
            string initialState = "S";
            List<string> finalStates = new List<string>{ "D" };
            
            foreach (string symbol in VN){
                automaton.AddState(symbol);
            }

            foreach (string symbol in VT){
                automaton.AddAlphabet(symbol);
            }

            foreach (KeyValuePair<string, List<string>> production in P){
                
                foreach (string value in production.Value){
                    automaton.AddTransition(production.Key, value);
                }
            }

            automaton.AddInitialState(initialState);

            foreach (string finalState in finalStates){
                automaton.AddFinalState(finalState);
            }

            return automaton;
        }

        public bool Type3(){
            foreach(KeyValuePair<string, List<string>> production in P){
                if(production.Key.Length > 1){
                    return false;
                }
                foreach(string p in production.Value){
                    if (p.Length > 2 || (p.Length == 1 && char.IsUpper(p[0]))){
                        return false;
                    }

                    else if (p.Length == 0 || (p.Length == 1 && char.IsUpper(p[0]))){
                        return false;
                    }

                    else if (p.Length == 2 && char.IsUpper(p[0]) && char.IsLower(p[1])){
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Type2(){
            foreach(KeyValuePair<string, List<string>> production in P){
                if(production.Key.Length > 1){
                    return false;
                }
                foreach(string p in production.Value){
                    if (p.Length == 0 || (p.Length == 1 && char.IsUpper(p[0]))){
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Type1(){
            foreach (KeyValuePair<string, List<string>> production in P){
                foreach(string p in production.Value){
                    if(production.Key.Length >= p.Length){
                        return false;
                    }
                }
            }
            return true;
        }
   
        public void typeDefinition(){
            if (Type3())
                Console.WriteLine("Type 3, Regular Grammar");
            
            else if (Type2())
                Console.WriteLine("Type 2, Context Free");
            
            else if (Type1())
                Console.WriteLine("Type 1, Context Sensitive");
            
            else 
                Console.WriteLine("Type 0, Unrestricted");
        }
    }
}