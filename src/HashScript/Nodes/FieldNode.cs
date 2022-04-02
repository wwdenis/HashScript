using System.Collections.Generic;

namespace HashScript.Nodes
{
    public sealed class FieldNode : Node
    {
        public FieldNode(string name, FieldType fieldType, bool isFunction, bool isValue)
        {
            this.Name = name ?? string.Empty;
            this.FieldType = fieldType;
            this.IsFunction = isFunction;
            this.IsValue = isValue;
        }

        public FieldNode(string name) : this(name, FieldType.Simple, false, false)
        {
        }

        public FieldNode() : this(null)
        {
        }

        public override NodeType NodeType => NodeType.Field;

        public FieldType FieldType { get; set; }

        public bool IsFunction { get; set; }

        public bool IsValue { get; set; }

        public string Name { get; set; }
    }
}