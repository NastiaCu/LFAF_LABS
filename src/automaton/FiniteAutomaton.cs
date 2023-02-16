using System;

namespace LFAF_LABS{

    class FiniteAutomaton{

        private List<string> states = new List<string>();
        private List<string> alphabet = new List<string>();
        private Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>();
        private string initialState = "";
        private List<string> finalStates = new List<string>();

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
    }
}