using System.Collections.Generic;
using System.Linq;

namespace HashScript.Domain
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

        public Node Parent { get; set; }

        public List<Node> Children { get; } = new List<Node>();

        public override string ToString()
        {
            return $"{this.NodeType}";
        }
    }
}