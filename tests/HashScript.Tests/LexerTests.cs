using System;
using FluentAssertions;
using Xunit;

namespace HashScript.Tests
{
    public class LexerTests
    {
        public static TheoryData ReadScenarios = new TheoryData<string, Token[]>
        {
            {
                string.Empty,
                new Token[0]
            },
            {
                " ",
                new[]
                {
                    new Token(TokenType.Space),
                }
            },
            {
                "\t",
                new[]
                {
                    new Token(TokenType.Tab),
                }
            },
            {
                "\n",
                new[]
                {
                    new Token(TokenType.NewLine),
                }
            },
            {
                "\r\n",
                new[]
                {
                    new Token(TokenType.NewLine),
                }
            },
            {
                "#",
                new[]
                {
                    new Token(TokenType.Hash),
                }
            },
            {
                "[",
                new[]
                {
                    new Token(TokenType.OpenBracket),
                }
            },
            {
                "]",
                new[]
                {
                    new Token(TokenType.CloseBracket),
                }
            },
            {
                "(",
                new[]
                {
                    new Token(TokenType.OpenParentheses),
                }
            },
            {
                ")",
                new[]
                {
                    new Token(TokenType.CloseParentheses),
                }
            },
            {
                "##",
                new[]
                {
                    new Token("#"),
                }
            },
            {
                "[[[",
                new[]
                {
                    new Token("["),
                    new Token(TokenType.OpenBracket),
                }
            },
            {
                "((()))",
                new[]
                {
                    new Token("("),
                    new Token(TokenType.OpenParentheses),
                    new Token(")"),
                    new Token(TokenType.CloseParentheses),
                }
            },
            {
                "abc",
                new[]
                {
                    new Token("abc"),
                }
            },
            {
                "first second",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.Space),
                    new Token("second"),
                }
            },
            {
                "first\tsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.Tab),
                    new Token("second"),
                }
            },
            {
                "first\nsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.NewLine),
                    new Token("second"),
                }
            },
            {
                "first\r\nsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.NewLine),
                    new Token("second"),
                }
            },
            {
                "##escaped##",
                new[]
                {
                    new Token("#"),
                    new Token("escaped"),
                    new Token("#"),
                }
            },
            {
                
                "Good Morning Mr. #UserName#!!!\n\n\nToday  is\t\t#CurrentDate#",
                new[]
                {
                    new Token("Good"),
                    new Token(TokenType.Space),
                    new Token("Morning"),
                    new Token(TokenType.Space),
                    new Token("Mr."),
                    new Token(TokenType.Space),
                    new Token(TokenType.Hash),
                    new Token("UserName"),
                    new Token(TokenType.Hash),
                    new Token("!!!"),
                    new Token(TokenType.NewLine, 3),
                    new Token("Today"),
                    new Token(TokenType.Space, 2),
                    new Token("is"),
                    new Token(TokenType.Tab, 2),
                    new Token(TokenType.Hash),
                    new Token("CurrentDate"),
                    new Token(TokenType.Hash),
                }
            },
        };


        [Theory]
        [MemberData(nameof(ReadScenarios))]
        public void Should_Read_All(string content, Token[] expected)
        {
            var subject = new Lexer(content);
            var result = subject.ReadAll();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
