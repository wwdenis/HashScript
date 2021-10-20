using HashScript.Domain;
using System.Collections.Generic;
using System.Linq;

namespace HashScript.Tests.Infrastructure
{
    public class FakeNode
    {
        private List<FakeNode> children = new();

        public FakeNode()
        {
            this.Children = new List<FakeNode>();
        }

        public FakeNode(string content, NodeType type) : this()
        {
            this.Content = content;
            this.NodeType = type;
        }

        public FakeNode(string content, NodeType type, FakeNode parent) : this(content, type)
        {
            this.Parent = parent;
        }

        public FakeNode(string content, NodeType type, IEnumerable<FakeNode> children) : this(content, type)
        {
            if (children is not null)
            {
                this.Children = children.ToList();
            }
        }

        public string Content { get; set; }

        public NodeType NodeType { get; set; }

        public FakeNode Parent { get; set; }

        public List<string> Errors { get; set; }

        public List<FakeNode> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value ?? new List<FakeNode>();
                children.ForEach(i => i.Parent = this);
            }
        }

        public override string ToString()
        {
            return $"{this.NodeType}";
        }
    }
}
