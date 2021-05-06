using System;

namespace HashScript.Domain
{
    public class Token
    {
        public Token(TokenType type, string content)
        {
            Type = type;
            Content = content;
        }

        public Token(TokenType type) : this(type, string.Empty)
        {
        }

        public Token(string content) : this(TokenType.Text, content)
        {
        }

        public Token()
        {
        }

        public TokenType Type { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return $"{Type}";
        }
    }
}