# Topic: Determinism in Finite Automata. Conversion from NDFA 2 DFA. Chomsky Hierarchy.

### Course: Formal Languages & Finite Automata
### Author: Anastasia Cunev 

----

## Theory

**Chomsky Hierarchy**
There are 4 types of the Grammar according to the Chomsky Hierarchy:
* Type 0, Unrestricted 
  The prodcution has no restrictions, can be of the form:
  ```
  AB → aB | b 
  aB → Bb
  ```
* Type 1, Context Free
  The left side of the production has to be shorter than the right side.
  Can be of the form:
  ```
  AB → aBaB | a
  ```
* Type 2, Context Sensitive
  The left side of the production has to be a single non-terminal symbol.
  Can be of the form:
  ```
  A → abaB | a
  ```
* Type 3, Regular Grammar
  The most strict one, can be of the form:
  ```
  A → aB | a
  ```


**NFA vs DFA**
The NFA is a Finite automaton, which has the repetitions in the transitions.
For example:
```
Q = {q0,q1,q2,q3},
∑ = {a,b,c},
F = {q3},
δ(q0,a) = q1,
δ(q1,b) = q2,
δ(q2,c) = q0,
δ(q1,a) = q3,
δ(q0,b) = q2,
δ(q2,c) = q3.
```
This FA is a NFA, because from the state `q2` with the `c` input string we can go in both `q0` and `q3`.

To convert the NFA to DFA, we have to add new states so that the transitions won't repeat.
  
## Objectives:
1. Understand what an automaton is and what it can be used for.

2. Continuing the work in the same repository and the same project, the following need to be added:
    a. Provide a function in your grammar type/class that could classify the grammar based on Chomsky hierarchy.

    b. For this you can use the variant from the previous lab.

3. According to your variant number (by universal convention it is register ID), get the finite automaton definition and do the following tasks:

    a. Implement conversion of a finite automaton to a regular grammar.

    b. Determine whether your FA is deterministic or non-deterministic.

    c. Implement some functionality that would convert an NDFA to a DFA.
    
    d. Represent the finite automaton graphically (Optional, and can be considered as a __*bonus point*__):
      
    - You can use external libraries, tools or APIs to generate the figures/diagrams.
        
    - Your program needs to gather and send the data about the automaton and the lib/tool/API return the visual representation.

## Implementation description

To add the new functionality to the `Grammar` class to determine the type of that `Grammar`, I implemended 3 functions for the first, the second and the third types of the Chomsky Hierarchy and called them in the function `typeDefinintion`. As an example, the boolean function `Type3` is used to determine wheather the `Grammar` is a `Regular Grammar` or not. This function takes the productions of the Grammar and checks if the right side of the production is bigger than 1, the left side is bigger than 2 or is just a non-terminal symbol, the left side is an empty string. If the function returns a false value, then it goes further to check weather the grammar is `Context Free`, `Context Sensitive` or `Unrestricted`.

```c#
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
```

The following method converts an object of type `Finite Automaton` to type `Grammar`. To do so we have to transforms the initial variables from the `Finite Automaton` to the ones in the `Grammar`. We assign the initial state and the final state. `states` set from the `Finite Automaton` is the `Vn` set in `Grammar` and `alphabet` set is the `Vt` set. The transitions from the set `transitions` from the `Finite Automaton` are added to the productions `P` set from the `Grammar`.

```c#
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
```

For the Finite Futomaton we have an `isDeterministic` function, which checks if the given FA ia deterministic or not. The function takes the transitions of the Finite Automaton and checks if there are reprtitions of the transitions, meaning that a FA is deterministic if it can't go from one state to the other states using the same input string. 

```c#
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
```
The following function converts an NFA to a DFA. Firstly, it creates a new `Finite Automaton` object, which will be our `DFA`. Then the initial state is added to the new DFA `initialState`. We then create a queue to keep track of the unprocessed states. Then the method processes all the unprocessed states and checks if it is a final state or not and adds it to the `finalStates` set. After that the method iterates through the alphabet and finds the set of NFA states that the current `DFA` state can transition to on that symbol. Finally, it adds the `states` and `transitions`.
```c#
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
```
The following function makes a graphical representation of the `Finite Automaton`. It makes the `.dot` file which then can be converted to a png file using `Graphviz` with the following statement: 
```
dot -Tpng -o dfa.png dfa.dot
```
It takes the attributes of the FA and draws initial state, states, final states in a double circles and adds labels for the transitions.
```c#
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
```

## Conclusions / Screenshots / Results
After performing this laboratory work I studied the types of the `Grammar` according to the `Chomsky Hierarchy`. I also understood the difference between an NFA and a DFA and implemented the function which determines whether a FA is deterministic or not   

After running the project we get the following results:
```
Type 3, Regular Grammar
Convertion of a FA to a Regular Grammar
Set of non-terminal symbols: ["q0","q1","q2","q3"]
Set of terminal symbols: ["a","b","c"]
Set of productions: {"q0":["aq1","bq2"],"q1":["bq2","aq3"],"q2":["cq0","cq3"]}

The FA is a NFA
The NFA has the following set of transitions: {"q0":{"a":["q1"],"b":["q2"]},"q1":{"b":["q2"],"a":["q3"]},"q2":{"c":["q0","q3"]}}

Convertion of the NFA to DFA
Set of states for the new DFA["q0","q1","q2","q3","q0,q3"]
Set of final states for the new DFA["q3","q0,q3"]
Set of transitions for the new DFA{"q0":{"a":["q1"],"b":["q2"]},"q1":{"a":["q3"],"b":["q2"]},"q2":{"c":["q0,q3"]},"q0,q3":{"a":["q1"],"b":["q2"]}}
```

## References 

1. https://github.com/DrVasile/FLFA-Labs
2. https://github.com/DrVasile/FLFA-Labs-Examples
3. https://else.fcim.utm.md/pluginfile.php/110458/mod_resource/content/0/LFPC_Guide.pdf
4. https://www.tutorialspoint.com/automata_theory/chomsky_classification_of_grammars.htm
