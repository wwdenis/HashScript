using System.Collections.Generic;

namespace HashScript.Nodes
{
    public sealed class FieldNode : Node
    {
        public FieldNode(string name, FieldType fieldType, FunctionType functionType)
        {
            this.Name = name ?? string.Empty;
            this.FieldType = fieldType;
            this.FunctionType = functionType;
        }

        public FieldNode(string name) : this(name, FieldType.Simple, FunctionType.None)
        {
        }

        public FieldNode() : this(null)
        {
        }

        public override NodeType NodeType => NodeType.Field;

        public FieldType FieldType { get; set; }

        public FunctionType FunctionType { get; set; }

        public string Name { get; set; }
    }
}