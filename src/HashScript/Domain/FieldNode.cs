using System.Collections.Generic;

namespace HashScript.Domain
{
    public sealed class FieldNode : Node
    {
        public FieldNode(string name, IEnumerable<Node> nodes) : base(nodes)
        {
            this.Name = name ?? string.Empty;
            this.FieldType = FieldType.Simple;
            this.FieldFunction = FieldFunction.None;
        }

        public FieldNode(string name) : this(name, null)
        {
        }

        public FieldNode() : this(null)
        {
        }

        public override NodeType NodeType => NodeType.Field;

        public FieldType FieldType { get; set; }

        public FieldFunction FieldFunction { get; set; }

        public string Name { get; set; }
    }
}