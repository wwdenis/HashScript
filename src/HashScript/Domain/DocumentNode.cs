using System.Collections.Generic;

namespace HashScript.Domain
{
    public sealed class DocumentNode : Node
    {
        public DocumentNode() : base()
        {
        }

        public DocumentNode(IEnumerable<Node> nodes) : base(nodes)
        {
        }

        public override NodeType Type => NodeType.Document;
    }
}