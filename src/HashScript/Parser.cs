using System;
using System.Collections;
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

            var hashStarted = false;

            while (tokens.Any())
            {
                var current = tokens.Dequeue();
                var next = tokens.Peek();

                var appentToken = !queue.Any();

                switch (current.Type)
                {
                    case TokenType.Text:
                        break;
                    case TokenType.NewLine:
                        break;
                    case TokenType.Tab:
                        break;
                    case TokenType.Space:
                        break;
                    case TokenType.Hash:
                        hashStarted = !hashStarted;
                        break;
                    case TokenType.Complex:
                        break;
                    case TokenType.Dot:
                        break;
                    case TokenType.Condition:
                        break;
                    case TokenType.Negate:
                        break;
                    case TokenType.Reference:
                        break;
                }

                if (appentToken)
                {
                    queue.Enqueue(current);
                }
                
                if (next.Type != current.Type)
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

        private static Node BuildNode(NodeType nodeType, Queue<Token> nodeStack)
        {
            switch (nodeType)
            {
                case NodeType.Text:
                    var content = string.Join("", nodeStack.Select(i => i.Content));
                    return new TextNode(content);
                case NodeType.Field:
                    return new FieldNode();
                default:
                    throw new ArgumentException($"Invalid NodeType: {nodeType}", nameof(nodeType));
            }
        }
    }
}