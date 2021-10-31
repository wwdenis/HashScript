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

        private List<Node> ParseChildren(Queue<Token> tokens, List<string> errors, FieldNode parent = null)
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
                    var fieldNode = ParseField(tokens, errors);
                    hasCloseNode = IsCloseNode(parent, fieldNode);

                    if (fieldNode is null || hasCloseNode)
                    {
                        break;
                    }
                    else if (string.IsNullOrEmpty(fieldNode.Name))
                    {
                        var error = "Field must contain a valid name";
                        errors.Add(error);
                    }
                    else
                    {
                        nodes.Add(fieldNode);
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

        private FieldNode ParseField(Queue<Token> tokens, List<string> errors)
        {
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
            var hasComposite = false;

            var fieldType = FieldType.Simple;
            var functionType = FunctionType.None;

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
                            hasComposite = true;
                            fieldType = ParseFieldType(currentToken);
                        }
                        break;
                    case TokenType.Dot:
                        if (!hasComposite || !hasStart || nameBuffer.Any())
                        {
                            hasInvalid = true;
                        }
                        else
                        {
                            nameBuffer.Enqueue(currentToken);
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
                    case TokenType.EOF:
                        hasInvalid = false;
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

            if (hasFunction && !TryParseFunction(fieldName, out functionType))
            {
                errorText = $"Field contains an invalid function: {fieldName}";
            }

            var node = new FieldNode(fieldName, fieldType, functionType);

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
                if (fieldType == FieldType.Simple)
                {
                    errorText = "Field must contain a valid name";
                }
            }
            else if (fieldType != FieldType.Simple)
            {
                var children = ParseChildren(tokens, errors, node);
                node.Children.AddRange(children);
            }

            if (!string.IsNullOrWhiteSpace(errorText))
            {
                errors.Add(errorText);
                return null;
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

        private static bool TryParseFunction(string name, out FunctionType type)
        {
            var isFunction = name.StartsWith((char)TokenType.Dot);
            
            if (!isFunction || !Enum.TryParse(name.Remove(0, 1), true, out type))
            {
                type = FunctionType.None;
            }

            return type != FunctionType.None;
        }

        private static bool IsCloseNode(FieldNode parent, FieldNode child)
        {
            return
                child is not null &&
                parent is not null && 
                child.FieldType == parent.FieldType &&
                string.IsNullOrEmpty(child.Name);
        }

        private static bool HasValidName(Token token)
        {
            return token.Content?.All(i => Char.IsLetterOrDigit(i)) ?? false;
        }
    }
}