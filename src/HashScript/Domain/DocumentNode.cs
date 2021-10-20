using System.Collections.Generic;

namespace HashScript.Domain
{
    public sealed class DocumentNode : Node
    {
        public DocumentNode() : base()
        {
        }

        public DocumentNode(IEnumerable<Node> nodes, IEnumerable<string> errors) : base(nodes)
        {
            this.Errors = errors ?? new string[0];
        }

        public override NodeType NodeType => NodeType.Document;

        public IEnumerable<string> Errors { get; } 
    }
}