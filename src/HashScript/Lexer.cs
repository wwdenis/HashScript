using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashScript.Domain;
using HashScript.Extensions;

namespace HashScript
{
    public sealed class Lexer : IDisposable
    {
        readonly StringReader reader;

        public Lexer(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

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

            var result = new List<Token>();
            var content = new StringBuilder();
            var index = -1;
            Token token = null;

            while ((index = reader.Read()) >= 0)
            {
                var current = (char)index;
                if (current == '\r')
                {
                    continue;
                }

                var currentType = current.BuildType();
                if (currentType == TokenType.Text)
                {
                    content.Append(current);
                }
                else
                {
                    token = new Token(currentType);
                }

                var next = (char)Math.Max(reader.Peek(), 0);
                var nextType = next.BuildType();
                if (nextType != TokenType.Text)
                {
                    token = new Token(currentType, content.ToString());
                }

                if (token is not null)
                {
                    result.Add(token);
                    content.Clear();
                    token = null;
                }
            }

            result.Add(new Token(TokenType.EndOfStream));
           
            return result;
        }
    }
}