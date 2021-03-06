using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashScript.Tokens;

namespace HashScript
{
    public sealed class Lexer : IDisposable
    {
        const char ReturnChar = '\r';

        private static readonly TokenType[] TextTokens = new[] { TokenType.Text, TokenType.Space, TokenType.Tab, TokenType.NewLine };
        private static readonly TokenType[] SpecialTokens = new[] { TokenType.Hash, TokenType.Complex, TokenType.Dot, TokenType.Question, TokenType.Negate};

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
            var length = 0;
            int current;

            while ((current = reader.Read()) >= 0)
            {
                if (current == ReturnChar)
                {
                    continue;
                }

                var next = reader.Peek();
                var currentType = BuildType(current);
                var nextType = BuildType(next);

                var isText = TextTokens.Contains(currentType);
                var isSpecial = SpecialTokens.Contains(currentType);
                var createToken = false;

                if (isSpecial)
                {
                    length = 1;
                    createToken = true;
                }
                else if (isText)
                {
                    length++;
                    createToken = currentType != nextType;
                    if (currentType == TokenType.Text)
                    {
                        content.Append((char)current);
                    }
                }

                if (createToken)
                {
                    var token = new Token(currentType, content.ToString(), length);
                    result.Add(token);

                    length = 0;
                    content.Clear();
                }
            }

            result.Add(new Token(TokenType.EOF));
           
            return result;
        }

        static TokenType BuildType(int charIndex)
        {
            charIndex = charIndex == '\r' ? '\n' : charIndex;
            var tokeType = (TokenType)charIndex;
            var types = Enum.GetValues(typeof(TokenType)) as TokenType[];
            return types.Contains(tokeType) ? tokeType : TokenType.Text;
        }
    }
}