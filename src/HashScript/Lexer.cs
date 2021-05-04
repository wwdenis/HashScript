using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            var result = new List<Token>();
            Token current;
            
            while ((current = ReadNext()) is not null)
            {
                result.Add(current);
            }

            return result;
        }

        public Token ReadNext()
        {
            var token = ReadSpecial();
            if (token is not null)
            {
                return token;
            }

            var content = new StringBuilder();
            TokenType nextType;

            do
            {
                var current = ReadChar();
                if (current is null)
                {
                    return null;
                }

                var next = PeekChar();
                nextType = ParseTokenType(next ?? CharSpace);
                content.Append(current);
            }
            while (nextType == TokenType.Text);

            return new Token(TokenType.Text, content.ToString());
        }

        private static TokenType ParseTokenType(char content)
        {
            var mappings = new Dictionary<char, TokenType>
            {
                { CharSpace, TokenType.Space },
                { CharTab, TokenType.Tab },
                { CharNewLine, TokenType.NewLine },
                { CharReturn, TokenType.NewLine },
                { CharHash, TokenType.Hash },
            };
            
            if (mappings.TryGetValue(content, out var type))
            {
                return type;
            }

            return TokenType.Text;
        }

        private Token ReadSpecial()
        {
            var next = PeekChar();
            if (next is null)
            {
                return null;
            }

            var type = ParseTokenType(next.Value);
            if (type == TokenType.Text)
            {
                return null;
            }

            var current = ReadChar();
            var content = type == TokenType.NewLine ? Environment.NewLine : current.ToString();
            return new Token(type, content);
        }

        private char? PeekChar()
        {
            var next = reader.Peek();
            return next >= 0 ? (char)next : null;
        }

        private char? ReadChar()
        {
            var current = reader.Read();

            if (current == -1)
            {
                return null;
            }

            var result = (char)current;

            if (result == CharReturn && PeekChar() == CharNewLine)
            {
                reader.Read();
                return CharNewLine;
            }

            return result;
        }
    }
}