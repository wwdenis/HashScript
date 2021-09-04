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

            while (tokens.Any())
            {
                if (TryParseField(tokens, out var fieldNode))
                {
                    nodes.Add(fieldNode);
                }
                else if (TryParseText(tokens, out var textNode))
                {
                    nodes.Add(textNode);
                }
                else 
                {
                    break;
                }
            }

            var doc = new DocumentNode(nodes);

            return doc;
        }

        private bool TryParseField(Queue<Token> tokens, out FieldNode node)
        {
            node = null;
            return false;
        }

        private bool TryParseText(Queue<Token> tokens, out TextNode node)
        {
            var queue = new Queue<Token>();
            var content = string.Empty;

            while (tokens.Any())
            {
                var current = tokens.Dequeue();
                var type = MapType(current);
                
                if (type != NodeType.Text)
                {
                    content = BuildTextContent(queue);
                    break;
                }

                queue.Enqueue(current);
            }

            if (string.IsNullOrEmpty(content))
            {
                node = null;
                return false;
            }

            node = new TextNode(content);
            return true;
        }

        private static NodeType MapType(Token token)
        {
            return token.Type switch 
            {
                TokenType.Text  => NodeType.Text,
                TokenType.Space => NodeType.Text,
                TokenType.NewLine => NodeType.Text,
                TokenType.EOF => NodeType.None,
                _ => throw new ArgumentException($"Invalid TokenType: {token.Type}", nameof(token)),
            };
        }

        private static string BuildTextContent(Queue<Token> tokens)
        {
            if (!tokens.Any())
            {
                return null;
            }

            var allContent = from i in tokens
                             let tokenChar = (char)i.Type
                             let content = string.IsNullOrEmpty(i.Content) ? new string(tokenChar, i.Length) : i.Content
                             select content;
            return string.Join("", allContent);
        }
    }
}