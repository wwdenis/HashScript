using System.Collections.Generic;

namespace HashScript.Domain
{
    public abstract class Node
    {
        public Node()
        {
            this.Children = new List<Node>();
        }

        public string Content { get; set; }

        public NodeType Type { get; set; }

        public Node Parent { get; set; }

        public List<Node> Children { get; }

        public override string ToString()
        {
            return $"{this.Type}";
        }
    }
}