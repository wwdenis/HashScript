using System;
using System.Collections.Generic;
using System.Linq;
using HashScript.Domain;

namespace HashScript
{
    public sealed class Parser : IDisposable
    {
        readonly Lexer lexer;

        public Parser(string template)
        {
            this.lexer = new Lexer(template);
        }

        public void Dispose()
        {
            this.lexer.Dispose();
        }

        public DocumentNode Parse()
        {
            var nodes = new List<Node>();
            var content = this.lexer.ReadAll();
            var tokens = new Queue<Token>(content);

            var queue = new Queue<Token>();
            var type = NodeType.Text;

            while (tokens.Any())
            {
                var current = tokens.Dequeue();
                var next = tokens.Peek();

                var currentType = MapType(current.Type);
                var nextType = MapType(next.Type);

                queue.Enqueue(current);

                if (nextType != currentType)
                {
                    var node = BuildNode(type, queue);
                    nodes.Add(node);
                    queue.Clear();
                }

                if (next.Type == TokenType.EOF)
                {
                    break;
                }
            }

            var doc = new DocumentNode(nodes);

            return doc;
        }

        private static NodeType MapType(TokenType tokenType)
        {
            return tokenType switch 
            {
                TokenType.Text  => NodeType.Text,
                TokenType.Space => NodeType.Text,
                TokenType.NewLine => NodeType.Text,
                TokenType.EOF => NodeType.None,
                _ => throw new ArgumentException($"Invalid TokenType: {tokenType}", nameof(tokenType)),
            };
        }

        private static Node BuildNode(NodeType nodeType, Queue<Token> tokens)
        {
            switch (nodeType)
            {
                case NodeType.Text:
                    var allContent = from i in tokens
                                     let tokenChar = (char)i.Type
                                     let content = string.IsNullOrEmpty(i.Content) ? new string(tokenChar, i.Length) : i.Content
                                     select content;
                    var textContent = string.Join("", allContent);
                    return new TextNode(textContent);
                case NodeType.Field:
                    return new FieldNode();
                default:
                    throw new ArgumentException($"Invalid NodeType: {nodeType}", nameof(nodeType));
            }
        }
    }
}