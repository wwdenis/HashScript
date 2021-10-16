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
            var children = ParseChildren(tokens, errors);

            var doc = new DocumentNode(children, errors);

            return doc;
        }

        public List<Node> ParseChildren(Queue<Token> tokens, List<string> errors)
        {
            var nodes = new List<Node>();
            
            while (tokens.Any())
            {
                FieldNode fieldNode = null;
                TextNode textNode = null;

                if ((fieldNode = ParseField(tokens, errors)) is not null)
                {
                    var isClose = string.IsNullOrEmpty(fieldNode.Name);
                    nodes.Add(fieldNode);

                    if (isClose)
                    {
                        break;
                    }
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

        private FieldNode ParseField(Queue<Token> tokens, List<string> errors)
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
                else if (current.Type == TokenType.Complex)
                {
                    if (!hasStart || buffer.Any())
                    {
                        hasInvalid = true;
                    }
                    else
                    {
                        hasChildren = true;
                        fieldType = FieldType.Complex;
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

            var name = BuildContent(buffer);
            var children = new List<Node>();

            if (hasInvalid)
            {
                error = $"Field contains an invalid character: {GetTokenContent(current)}";
            }
            else if (!hasEnd)
            {
                error = "Field does not contains a close Hash";
            }
            else if (fieldType == FieldType.Simple && string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Simple Field must contain a valid name");
            }
            else if (hasChildren && !string.IsNullOrEmpty(name))
            {
                children = ParseChildren(tokens, errors);
                var last = children.LastOrDefault() as FieldNode;

                if (last is not null && last.FieldType == fieldType && string.IsNullOrEmpty(last.Name))
                {
                    children.Remove(last);
                }
                else
                {
                    error = $"Field '{name}' does not contains a close Node";
                }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                errors.Add(error);
                return null;
            }

            var node = new FieldNode(name, children);
            node.FieldType = fieldType;
            return node;
        }

        private TextNode ParseText(Queue<Token> tokens)
        {
            var invalidTokens = new[] { TokenType.Hash, TokenType.EOF };
            var buffer = new Queue<Token>();

            while (tokens.Any())
            {
                var current = tokens.Dequeue();
                if (invalidTokens.Contains(current.Type))
                {
                    break;
                }
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