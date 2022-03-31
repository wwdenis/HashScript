using System.Collections.Generic;
using System.Linq;

namespace HashScript.Nodes
{
    public abstract class Node
    {
        public Node()
        {
        }

        public Node(IEnumerable<Node> nodes)
        {
            this.Children = nodes?.ToList() ?? new List<Node>();
        }

        public abstract NodeType NodeType { get; }

        public List<Node> Children { get; set; } = new List<Node>();

        public override string ToString()
        {
            return $"{this.NodeType}";
        }
    }
}