using System;

namespace HashScript
{
    public class Token
    {
        public Token(TokenType type, int size, string content)
        {
            Type = type;
            Size = size;
            Content = content;
        }

        public Token(TokenType type, int size) : this(type, size, string.Empty)
        {
        }

        public Token(TokenType type) : this(type, 1)
        {
        }

        public Token(string content) : this(TokenType.Text, content.Length, content)
        {
        }

        public Token()
        {
        }

        public TokenType Type { get; set; }
        public int Size { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $"{Type}:({Size})";
        }
    }
}