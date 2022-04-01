using System;
using System.Collections.Generic;
using System.Linq;
using HashScript.Nodes;
using HashScript.Tokens;

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

        private IEnumerable<Node> ParseChildren(Queue<Token> tokens, List<string> errors, FieldNode parent = null)
        {
            var nodes = new List<Node>();
            var hasCloseNode = false;
            
            while (tokens.Any())
            {
                var textNode = ParseText(tokens);

                if (textNode is not null)
                {
                    nodes.Add(textNode);
                }
                else
                {
                    var fieldNode = ParseField(tokens, errors, parent, out hasCloseNode);
                    var emptyText = NormalizeSpace(nodes, fieldNode, hasCloseNode);

                    if (emptyText is not null)
                    {
                        nodes.Remove(emptyText);
                    }

                    if (fieldNode is not null)
                    {
                        nodes.Add(fieldNode);
                    }
                    else
                    {
                        break;
                    }

                }
            }

            if (!hasCloseNode && parent is not null)
            {
                var error = $"Field '{parent.Name}' does not contains a close Node";
                errors.Add(error);
            }

            return nodes;
        }

        private FieldNode ParseField(Queue<Token> tokens, List<string> errors, FieldNode parent, out bool isClose)
        {
            isClose = false;

            if (!tokens.Any() || tokens.Peek().Type != TokenType.Hash)
            {
                return null;
            }

            var nameBuffer = new Queue<Token>();
            var errorText = string.Empty;
            Token currentToken = null;
            
            var hasStart = false;
            var hasEnd = false;
            var hasInvalid = false;
            var hasFunction = false;
            var hasNested = false;

            var fieldType = FieldType.Simple;

            while (tokens.Any())
            {
                currentToken = tokens.Dequeue();
                hasInvalid = false;

                switch (currentToken.Type)
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
                    case TokenType.Question:
                    case TokenType.Negate:
                        if (!hasStart || nameBuffer.Any())
                        {
                            hasInvalid = true;
                        }
                        else
                        {
                            hasNested = true;
                            fieldType = ParseFieldType(currentToken);
                        }
                        break;
                    case TokenType.Dot:
                        if (!hasNested || !hasStart || nameBuffer.Any())
                        {
                            hasInvalid = true;
                        }
                        else
                        {
                            hasFunction = true;
                        }
                        break;
                    case TokenType.Text:
                        if (HasValidName(currentToken))
                        {
                            nameBuffer.Enqueue(currentToken);
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

            var fieldName = ParseContent(nameBuffer);
            var node = new FieldNode(fieldName, fieldType, hasFunction);

            if (hasInvalid)
            {
                errorText = $"Field contains an invalid character: {ParseContent(currentToken)}";
            }
            else if (!hasEnd)
            {
                errorText = "Field does not contains a close Hash";
            }
            else if (string.IsNullOrEmpty(fieldName))
            {
                if (fieldType == FieldType.Simple || parent is null)
                {
                    errorText = "Field must contain a valid name";
                }
                else if (fieldType == parent.FieldType)
                {
                    isClose = true;
                    return null;
                }
            }

            if (!string.IsNullOrWhiteSpace(errorText))
            {
                errors.Add(errorText);
                return null;
            }

            if (fieldType != FieldType.Simple)
            {
                var children = ParseChildren(tokens, errors, node);
                node.Children.AddRange(children);
            }

            return node;
        }

        private static TextNode ParseText(Queue<Token> tokens)
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

            var content = ParseContent(buffer);
            var node = new TextNode(content);
            return node;
        }

        private static string ParseContent(Queue<Token> tokens)
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

        private static string ParseContent(Token token)
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

        private static FieldType ParseFieldType(Token token)
        {
            var type = token.Type;
            
            return type switch
            {
                TokenType.Complex => FieldType.Complex,
                TokenType.Question => FieldType.Question,
                TokenType.Negate => FieldType.Negate,
                _ => FieldType.Simple
            };
        }

        private static bool HasValidName(Token token)
        {
            return token.Content?.All(i => char.IsLetterOrDigit(i)) ?? false;
        }

        private TextNode NormalizeSpace(List<Node> nodes, FieldNode field, bool isClose)
        {
            var nestedTypes = new FieldType?[] { FieldType.Complex, FieldType.Question, FieldType.Negate };
            var isOpen = nestedTypes.Contains(field?.FieldType);
            var last = nodes.LastOrDefault() as TextNode;

            if (last is null || (!isClose && !isOpen))
            {
                return null;
            }

            var content = last.Content;
            var index = content.LastIndexOf((char)TokenType.NewLine);
            if (index >= 0)
            {
                var hasText = content.Substring(index).TrimEnd().Any();
                if (!hasText)
                {
                    content = content.Remove(index);
                }
                if (string.IsNullOrEmpty(content))
                {
                    return last;
                }
            }
            
            last.Content = content;
            return null;
        }
    }
}