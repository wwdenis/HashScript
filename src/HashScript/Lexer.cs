using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HashScript
{
    public sealed class Lexer : IDisposable
    {
        const char CharSpace = ' ';
        const char CharTab = '	';
        const char CharHash = '#';
        const char CharNewLine = '\n';
        const char CharReturn = '\r';

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

            var buffer = new List<(int Pos, char Content, TokenType Type)>();
            int currentIndex;
            var position = 0;
            var currentType = BuildType((char)reader.Peek());

            while ((currentIndex = reader.Read()) >= 0)
            {
                var currentChar = (char)currentIndex;
                if (currentType != BuildType(currentChar))
                {
                    currentType = BuildType(currentChar);
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
                            var escapedSize = size / 2;
                            var escapedContent = new string(Token.CharHash, escapedSize);
                            escapedToken = new Token(TokenType.Text, escapedSize, escapedContent);
                        }
                        if (size % 2 > 0)
                        {
                            token = new Token(type, size);
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

        private static TokenType BuildType(char content)
        {
            return content switch
            {
                CharSpace => TokenType.Space,
                CharTab => TokenType.Tab,
                CharReturn => TokenType.NewLine,
                CharNewLine => TokenType.NewLine,
                CharHash => TokenType.Hash,
                _ => TokenType.Text,
            };
        }
    }
}