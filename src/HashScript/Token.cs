using System;

namespace HashScript
{
    public class Token
    {
        public const char CharSpace = ' ';
        public const char CharTab = '	';
        public const char CharHash = '#';
        public const char CharNewLine = '\n';
        public const char CharReturn = '\r';

        public Token(TokenType type, string content)
        {
            Type = type;
            Content = content;
        }

        public Token(TokenType type) : this(type, BuildContent(type))
        {
        }

        public Token(string content) : this(TokenType.Text, content)
        {
        }

        public TokenType Type { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return Type == TokenType.Text ? $"{Type}: {Content}" : $"{Type}";
        }

        private static string BuildContent(TokenType type)
        {
            return type switch
            {
                TokenType.Space => $"{CharSpace}",
                TokenType.Tab => $"{CharTab}",
                TokenType.Hash => $"{CharHash}",
                TokenType.NewLine => Environment.NewLine,
                _ => string.Empty,
            };
        }
    }
}