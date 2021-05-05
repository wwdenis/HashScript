using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashScript.Extensions;

namespace HashScript
{
    public sealed class Lexer : IDisposable
    {
        readonly StringReader reader;

        public Lexer(string content)
        {
            Content = content;
            reader = new StringReader(content);
        }

        public string Content { get; }

        public void Dispose()
        {
            reader.Dispose();
        }

        public IEnumerable<Token> ReadAll()
        {
            if (reader.Peek() < 0)
            {
                return Enumerable.Empty<Token>();
            }

            var position = 0;
            var buffer = new List<(int Pos, char Content, TokenType Type)>();
            var currentIndex = reader.Peek();
            var currentChar = (char)currentIndex;
            var currentType = currentChar.BuildType();

            while ((currentIndex = reader.Read()) >= 0)
            {
                currentChar = (char)currentIndex;
                if (currentType != currentChar.BuildType())
                {
                    currentType = currentChar.BuildType();
                    position++;
                }
                buffer.Add((position, currentChar, currentType));
            }

            var result = new List<Token>();
            var maxPos = buffer.Max(i => i.Pos);

            for (int pos = 0; pos <= maxPos; pos++)
            {
                var group = buffer.Where(i => i.Pos == pos);
                var type = group.First().Type;
                var size = group.Count();

                var content = new StringBuilder();
                content.Append(group.Select(i => i.Content).ToArray());

                Token escapedToken = null;
                Token token = null;

                switch (type)
                {
                    case TokenType.Text:
                        token = new Token(type, size, content.ToString());
                        break;
                    case TokenType.NewLine:
                        content.Replace("\r\n", "\n");
                        token = new Token(type, content.Length);
                        break;
                    case TokenType.Hash:
                        if (size > 1)
                        {
                            var escapedChar = type.BuildChar();
                            var escapedSize = size / 2;
                            var escapedContent = new string(escapedChar, escapedSize);
                            escapedToken = new Token(TokenType.Text, escapedSize, escapedContent);
                        }
                        if (size % 2 > 0)
                        {
                            token = new Token(type, 1);
                        }
                        break;
                    default:
                        token = new Token(type, size);
                        break;
                }

                if (escapedToken is not null)
                {
                    result.Add(escapedToken);
                }

                if (token is not null)
                {
                    result.Add(token);
                }
            }

            return result;
        }
    }
}