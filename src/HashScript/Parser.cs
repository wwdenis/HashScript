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
            var content = this.lexer.ReadAll();
            var tokens = new Queue<Token>(content);

            var errors = new List<string>();
            var children = ParseChildren(tokens, errors, parent: null);
            var doc = new DocumentNode(children, errors);

            return doc;
        }

        public List<Node> ParseChildren(Queue<Token> tokens, List<string> errors, FieldNode parent)
        {
            var nodes = new List<Node>();
            
            while (tokens.Any())
            {
                FieldNode fieldNode = null;
                TextNode textNode = null;

                if ((fieldNode = ParseField(tokens, errors, parent)) is not null)
                {
                    nodes.Add(fieldNode);
                }
                else if ((textNode = ParseText(tokens)) is not null)
                {
                    nodes.Add(textNode);
                }
                else
                {
                    break;
                }
            }

            return nodes;
        }

        private FieldNode ParseField(Queue<Token> tokens, List<string> errors, FieldNode parent)
        {
            if (!tokens.Any() || tokens.Peek().Type != TokenType.Hash)
            {
                return null;
            }

            var buffer = new Queue<Token>();

            var error = string.Empty;
            Token current = null;
            
            var hasStart = false;
            var hasEnd = false;
            var hasInvalid = false;
            var hasChildren = false;
            var fieldType = FieldType.Simple;

            while (tokens.Any())
            {
                current = tokens.Dequeue();
                hasInvalid = false;

                switch (current.Type)
                {
                    case TokenType.Hash:
                        if (hasStart)
                        {
                            hasEnd = true;
                        }
                        else
                        {
                            hasStart = true;
                        }
                        break;
                    case TokenType.Complex:
                        if (!hasStart || buffer.Any())
                        {
                            hasInvalid = true;
                        }
                        else
                        {
                            hasChildren = true;
                            fieldType = GetFieldType(current);
                        }
                        break;
                    case TokenType.Text:
                        if (HasValidName(current))
                        {
                            buffer.Enqueue(current);
                        }
                        else
                        {
                            hasInvalid = true;
                        }
                        break;
                    default:
                        hasInvalid = true;
                        break;
                }
                
                if (hasInvalid || hasEnd)
                {
                    break;   
                }
            }

            var name = BuildContent(buffer);
            var node = new FieldNode(name);
            node.FieldType = fieldType;

            if (hasInvalid)
            {
                error = $"Field contains an invalid character: {GetTokenContent(current)}";
            }
            else if (string.IsNullOrEmpty(name))
            {
                if (parent?.FieldType == fieldType)
                {
                    return null;
                }
                else
                {
                    error = "Field must contain a valid name";
                }
            }
            else if (!hasEnd)
            {
                error = "Field does not contains a close Hash";
            }
            else if (hasChildren)
            {
                var children = ParseChildren(tokens, errors, node);
                node.Children.AddRange(children);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                errors.Add(error);
                return null;
            }

            return node;
        }

        private TextNode ParseText(Queue<Token> tokens)
        {
            var invalidTokens = new[] { TokenType.Hash, TokenType.EOF };
            var buffer = new Queue<Token>();

            while (tokens.Any())
            {
                var next = tokens.Peek();
                if (invalidTokens.Contains(next.Type))
                {
                    break;
                }
                var current = tokens.Dequeue();
                buffer.Enqueue(current);
            }

            if (!buffer.Any())
            {
                return null;
            }

            var content = BuildContent(buffer);
            var node = new TextNode(content);
            return node;
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

        private static string GetTokenContent(Token token)
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

        private static FieldType GetFieldType(Token token)
        {
            var type = token.Type;
            
            return type switch
            {
                TokenType.Complex => FieldType.Complex,
                _ => FieldType.Simple
            };
        }

        private static bool HasValidName(Token token)
        {
            return token.Content?.All(i => Char.IsLetterOrDigit(i)) ?? false;
        }
    }
}