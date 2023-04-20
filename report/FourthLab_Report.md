# Topic: Chomsky Normal Form

### Course: Formal Languages & Finite Automata

### Author: Anastasia Cunev

---

## Theory

A grammar is in Chomsky Normal Form if all its production rules are of the form:

`A → BC or A → a`
meaning that there are either two non-terminal symbols or one terminal symbol.

To obtain grammar in CNF we have to follow these 5 steps:

1. Eliminate ε productions.
2. Eliminate unit productions.
3. Eliminate inaccessible symbols.
4. Eliminate the non productive symbols.
5. Obtain the Chomsky Normal Form.

## Objectives:

1. Learn about Chomsky Normal Form (CNF).
2. Get familiar with the approaches of normalizing a grammar.
3. Implement a method for normalizing an input grammar by the rules of CNF.
   1. The implementation needs to be encapsulated in a method with an appropriate signature (also ideally in an appropriate class/type).
   2. The implemented functionality needs executed and tested.
   3. A BONUS point will be given for the student who will have unit tests that validate the functionality of the project.
   4. Also, another BONUS point would be given if the student will make the aforementioned function to accept any grammar, not only the one from the student's variant.

## Implementation description

The first step in performing the conversion in CNF is to eliminate the epsilon productions. `EliminateEProductions` id the method that removes empty (epsilon) productions from a grammar represented by a dictionary called productions. It first identifies nullable symbols and then iterates over each symbol in the dictionary, modifying each production by removing nullable symbols and adding resulting productions to a new list. Finally, it updates the dictionary with the new list of productions for each symbol. The result is a grammar without empty productions.

```c#
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
```

The second step in performing the conversion in CNF is to eliminate the unit productions. `EliminateUnitProductions` removes unit productions from a grammar represented by a dictionary called productions. It does this by repeatedly iterating over each symbol in the dictionary and replacing any unit productions with the productions of the non-terminal symbols they produce. This process continues until no more unit productions can be removed.

```c#
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
```

The third step in performing the conversion in CNF is to eliminate the non-productive symbols. `EliminateNonProductiveSymbols` method removes non-productive symbols from a grammar represented by a dictionary called productions. It first identifies the set of productive symbols by iterating over each production and checking if it contains a terminal symbol or only productive symbols. Then, it removes any non-productive symbols from the dictionary and productions, and updates the grammar accordingly.

```c#
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
```

The previous function uses the `IsProductive` method, that checks if a production is productive given a set of productive symbols. It does this by iterating over each character in the production and checking if it is an uppercase non-terminal symbol that is not in the set of productive symbols. If such a character is found, the method returns false. Otherwise, it returns true.

```c#
private bool IsProductive(string production, List<string> productiveSymbols){
    foreach (char c in production){
        if (char.IsUpper(c) && !productiveSymbols.Contains(c.ToString())){
            return false;
        }
    }

    return true;
}
```

The fourth step is to eliminate the inaccesible symbols. `EliminateInaccesibleSymbols` method eliminates inaccessible symbols from a dictionary of productions. It starts by creating a list of accessible symbols that includes only the start symbol, "S". Then, it iteratively adds all symbols reachable from the current set of accessible symbols to the list of accessible symbols. Finally, it removes all symbols that are not in the list of accessible symbols from the dictionary of productions.

```c#
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
```

The following function is an intermidiate step in converting a grammar to Chomsky Normal Form. It introduces new productions and symbols. For each terminal symbol, it creates a new non-terminal symbol and replaces any occurrence of that terminal symbol in the grammar with the new non-terminal symbol.

```c#
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
```

The following function takes a production string and modifies it. It counts the number of uppercase characters in the input string and if it's greater than 2, it creates a new group of at most 2 uppercase characters and replaces the original group with a new non-terminal symbol. If the new group already exists in a dictionary, it replaces the group with the corresponding non-terminal symbol. If not, it creates a new non-terminal symbol and adds it to the dictionary. The function returns the modified production string.

```c#
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
```

The last function updates the right-hand side productions of a grammar by applying the `FormProd` function to each production. It also updates the right-hand side productions with any new productions generated during the conversion process.

```c#
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
```

## Conclusions / Screenshots / Results

After performing this laboratory work I learned how to make the conversion of CFG in CNF. In this work I implemented separate funcions to perform all the requered steps and see the intermidiate results.

After running the project I got the following results:

```
Original Grammar
S -> bA | AC
A -> bS | BC | AbAa
B -> BbaA | a | bSa
C -> e
D -> AB
--------------------------
Eliminated epsilon productions
S -> bA | A | AC
A -> bS | B | BC | AbAa
B -> BbaA | a | bSa
C ->
D -> AB
--------------------------
Eliminated unit productions
S -> bA | AC | bS | BC | AbAa | BbaA | a | bSa
A -> bS | BC | AbAa | BbaA | a | bSa
B -> BbaA | a | bSa
C ->
D -> AB
--------------------------
Eliminated non-productive symbols
S -> bA | bS | AbAa | BbaA | a | bSa
A -> bS | AbAa | BbaA | a | bSa
B -> BbaA | a | bSa
D -> AB
--------------------------
Eliminated inaccessible symbols
S -> bA | bS | AbAa | BbaA | a | bSa
A -> bS | AbAa | BbaA | a | bSa
B -> BbaA | a | bSa
--------------------------
Intermidiate result
S -> X1A | X1S | AX1AX0 | BX1X0A | a | X1SX0
A -> X1S | AX1AX0 | BX1X0A | a | X1SX0
B -> BX1X0A | a | X1SX0
X1 -> b
X0 -> a
--------------------------
Final Result
S -> X1A | X1S | X3X0 | X5A | a | X6X0
A -> X1S | X3X0 | X5A | a | X6X0
B -> X5A | a | X6X0
X1 -> b
X0 -> a
X2 -> AX1
X3 -> X2A
X4 -> BX1
X5 -> X4X0
X6 -> X1S
```

## References

1. https://github.com/DrVasile/FLFA-Labs
2. https://else.fcim.utm.md/pluginfile.php/66784/mod_resource/content/1/LabN3exemplu_engl.pdf
3. https://en.wikipedia.org/wiki/Chomsky_normal_form
