using System.Collections.Generic;
using System.Linq;

namespace HashScript.Domain
{
    public sealed class DocumentNode : Node
    {
        public DocumentNode()
        {
            this.Type = NodeType.Document;
        }
    }
}