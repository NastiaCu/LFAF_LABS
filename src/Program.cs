class Program{
    public static void Main(String[] args){
        
        Grammar grammar = new Grammar();
        FiniteAutomaton automaton = grammar.ToFiniteAutomaton();

        grammar.GenerateString();
        grammar.GenerateString();
        grammar.GenerateString();
        grammar.GenerateString();
        grammar.GenerateString();

        if (automaton.isValid("abab")){
            Console.WriteLine("flse");
        }
    }
}
