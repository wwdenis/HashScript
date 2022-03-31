using System.Collections.Generic;
using System.Linq;

namespace HashScript.Nodes
{
    public sealed class TextNode : Node
    {
        public TextNode()
        {
        }

        public TextNode(string content)
        {
            this.Content = content;
        }

        public override NodeType NodeType => NodeType.Text;

        public string Content { get; set; }
    }
}