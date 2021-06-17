using System.Collections.Generic;
using System.Linq;

namespace HashScript.Domain
{
    public sealed class FieldNode : Node
    {
        public FieldNode()
        {
        }

        public FieldNode(IEnumerable<Node> nodes) : base(nodes)
        {
        }

        public FieldNode(string name)
        {
            this.Name = name;
        }

        public override NodeType Type => NodeType.Field;

        public string Name { get; set; }
    }
}