using System;

class Grammar{

    private List<string> VN = new List<string> { "S", "B", "D" };
    private List<string> VT = new List<string> { "a", "b", "c" };
    private Dictionary<string, List<string>> P = new Dictionary<string, List<string>>();

    public Grammar(){
        P["S"] = new List<string> { "aB", "bB" };
        P["B"] = new List<string> { "bD", "cB", "aS" };
        P["D"] = new List<string> { "b", "aD" };
    }

    public List<string> GetVN(){
        return VN;
    }

    public List<string> GetVT(){
        return VT;
    }

    public Dictionary<string, List<string>> GetP(){
        return P;
    }

    public void GenerateString(){
        
        Random random = new Random();
        string s = "S";
        
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
}