using HashScript.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HashScript.Tests.Infrastructure
{
    [JsonObject(IsReference = true)]
    public class FakeNode
    {
        public FakeNode()
        {
            this.Children = new List<FakeNode>();
        }

        public FakeNode(string content, NodeType type) : this()
        {
            this.Content = content;
            this.Type = type;
        }

        public FakeNode(string content, NodeType type, FakeNode parent) : this(content, type)
        {
            this.Parent = parent;
        }

        public FakeNode(string content, NodeType type, IEnumerable<FakeNode> children) : this(content, type)
        {
            if (children is not null)
            {
                foreach (var item in children)
                {
                    item.Parent = this;
                }
                this.Children = children.ToList();
            }
        }

        public string Content { get; set; }

        public NodeType Type { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public FakeNode Parent { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public List<FakeNode> Children { get; set; }

        public override string ToString()
        {
            return $"{this.Type}";
        }
    }
}
