using System.Collections.Generic;

namespace LFAF_LABS{
    public class ASTNode{
        public string Value { get; }
        public List<ASTNode> Children { get; }

        public ASTNode(string value){
            Value = value;
            Children = new List<ASTNode>();
        }

        public void AddChild(ASTNode child){
            Children.Add(child);
        }

        public void Visualize(int indentLevel = 0, bool isLastChild = true){
            string indent = GetIndentation(indentLevel);
            string connector = isLastChild ? "└── " : "├── ";

            Console.Write(indent + connector + Value);

            if (Children != null){
                Console.WriteLine();

                for (int i = 0; i < Children.Count; i++){
                    bool isLast = i == Children.Count - 1;
                    Children[i].Visualize(indentLevel + 1, isLast);
                }
            }

            else{
                Console.WriteLine();
            }
        }

        private string GetIndentation(int indentLevel){
            const int spacesPerIndent = 4;
            int spaces = indentLevel * spacesPerIndent;

            return new string(' ', spaces);
        }
    }
}
