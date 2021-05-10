using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashScript.Domain;

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
            var current = -1;
            Token token = null;

            while ((current = reader.Read()) >= 0)
            {
                if (current == '\r')
                {
                    continue;
                }

                var next = reader.Peek();
                var currentType = BuildType(current);
                var nextType = BuildType(next);

                if (currentType == TokenType.Text)
                {
                    content.Append((char)current);
                }
                else
                {
                    token = new Token(currentType);
                }

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

        static TokenType BuildType(int charIndex)
        {
            charIndex = charIndex == '\r' ? '\n' : charIndex;
            return Enum.IsDefined((TokenType)charIndex) ? (TokenType)charIndex :TokenType.Text;
        }
    }
}