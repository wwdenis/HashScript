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
            var errors = new List<string>();
            var nodes = new List<Node>();
            var content = this.lexer.ReadAll();
            var tokens = new Queue<Token>(content);

            var queue = new Queue<Token>();

            while (tokens.Any())
            {
                Node node = null;

                if ((node = ParseField(tokens, errors)) is not null)
                {
                    nodes.Add(node);
                }
                else if ((node = ParseText(tokens)) is not null)
                {
                    nodes.Add(node);
                }
                else
                {
                    break;
                }
            }

            var doc = new DocumentNode(nodes, errors);

            return doc;
        }

        private FieldNode ParseField(Queue<Token> tokens, List<string> errors)
        {
            if (!NextContains(tokens, NodeType.Field))
            {
                return null;
            }

            var buffer = new Queue<Token>();

            var error = string.Empty;
            Token current = null;
            
            var hasStart = false;
            var hasEnd = false;
            var hasInvalid = false;

            while (tokens.Any())
            {
                current = tokens.Dequeue();
                hasInvalid = false;

                if (current.Type == TokenType.Hash)
                {
                    if (hasStart)
                    {
                        hasEnd = true;
                    }
                    else
                    {
                        hasStart = true;
                    }
                }
                else if (current.Type == TokenType.Text)
                {
                    if (HasValidName(current))
                    {
                        buffer.Enqueue(current);
                    }
                    else
                    {
                        hasInvalid = true;
                    }
                }
                else
                {
                    hasInvalid = true;
                }
                
                if (hasInvalid || hasEnd)
                {
                    break;   
                }
            }

            if (hasInvalid)
            {
                error = $"Field contains an invalid character: {GetTokenContent(current)}";
            }
            else if (!buffer.Any())
            {
                error = "Field must contain a valid name";
            }
            else if (!hasEnd)
            {
                error = "Field does not contains a close Hash";
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                errors.Add(error);
                return null;
            }

            var name = BuildContent(buffer);
            var node = new FieldNode(name);
            return node;
        }

        private TextNode ParseText(Queue<Token> tokens)
        {
            var buffer = new Queue<Token>();
            var content = string.Empty;

            while (tokens.Any())
            {
                if (!NextContains(tokens, NodeType.Text))
                {
                    content = BuildContent(buffer);
                    break;
                }

                var current = tokens.Dequeue();
                buffer.Enqueue(current);
            }

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var node = new TextNode(content);
            return node;
        }

        private static bool NextContains(Queue<Token> tokens, NodeType expectedType)
        {
            var mappings = new Dictionary<TokenType, NodeType> 
            {
                { TokenType.Text, NodeType.Text },
                { TokenType.Space, NodeType.Text },
                { TokenType.Tab, NodeType.Text },
                { TokenType.NewLine, NodeType.Text },
                { TokenType.Hash, NodeType.Field },
            };

            if (!tokens.Any())
            {
                return false;
            }

            var next = tokens.Peek();

            if (mappings.TryGetValue(next.Type, out var nodeType))
            {
                return nodeType == expectedType;
            }

            return false;
        }

        private static string BuildContent(Queue<Token> tokens)
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

        private string GetTokenContent(Token token)
        {
            var type = token.Type;
            
            return type switch
            {
                TokenType.Space => type.ToString(),
                TokenType.Tab => type.ToString(),
                TokenType.NewLine => type.ToString(),
                TokenType.EOF => type.ToString(),
                TokenType.Text => token.Content,
                _ => $"{(char)type}"
            };
        }

        private bool HasValidName(Token token)
        {
            return token.Content?.All(i => Char.IsLetterOrDigit(i)) ?? false;
        }
    }
}