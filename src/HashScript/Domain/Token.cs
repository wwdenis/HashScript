using System;

namespace HashScript.Domain
{
    public class Token
    {
        public Token(TokenType type, string content, int length)
        {
            Type = type;
            Content = content;
            Length = length;
        }

        public Token(TokenType type) : this(type, string.Empty, 1)
        {
        }

        public Token() : this(TokenType.Text, string.Empty, 0)
        {
        }

        public TokenType Type { get; set; }
        
        public string Content { get; set; }

        public int Length { get; set; }

        public override string ToString()
        {
            return $"{Type}";
        }
    }
}