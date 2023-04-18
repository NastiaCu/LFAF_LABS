### Course: Formal Languages & Finite Automata
### Laboratory Works
> Cunev Anastasia

>FAF-211

# Variant 11

# Lab 1 

Variant 11:
```
VN={S, B, D}, 
VT={a, b, c}, 
P={ 
    S → aB
    S → bB
    B → bD
    D → b
    D → aD
    B → cB
    B → aS
}
```
# Lab 2 

Variant 11
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
# Lab 4

Variant 11
```
G = {Vn, Vt, S, P},
Vt = {a,b},
Vn = {S, A, B, C, D},
S -> bA | AC
A -> bS | BC | AbAa
B -> BbaA | a | bSa
C -> e
D -> AB
```
