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
                var content = string.Empty;

                if (type == TokenType.Text)
                {
                    content = new string(group.Select(i => i.Content).ToArray());
                }
                else if (type == TokenType.NewLine)
                {
                    size = group.Count(i => i.Content != '\r');
                }
                else if (type.IsSpecial())
                {
                    var escapedToken = type.TryEscape(size);
                    if (escapedToken is not null)
                    {
                        result.Add(escapedToken);
                    }
                    size %= 2;
                }

                if (size > 0)
                {
                    var token = new Token(type, size, content);
                    result.Add(token);
                }
            }

            return result;
        }
    }
}