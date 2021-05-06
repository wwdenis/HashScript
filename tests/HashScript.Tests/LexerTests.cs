using System;
using FluentAssertions;
using Xunit;
using HashScript.Domain;

namespace HashScript.Tests
{
    public class LexerTests
    {
        public static TheoryData SingleScenarios = new TheoryData<string, TokenType>
        {
            { " ", TokenType.Space },
            { "\t", TokenType.Tab },
            { "\n", TokenType.NewLine },
            { "\r\n", TokenType.NewLine },
            { "#", TokenType.Hash },
            { "[", TokenType.OpenBracket },
            { "]", TokenType.CloseBracket },
            { "(", TokenType.OpenParentheses },
            { ")", TokenType.CloseParentheses },
        };

        public static TheoryData MultiScenarios = new TheoryData<string, Token[]>
        {
            {
                "##",
                new[]
                {
                    new Token(TokenType.Hash),
                    new Token(TokenType.Hash),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "[[[",
                new[]
                {
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.OpenBracket),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "(())",
                new[]
                {
                    new Token(TokenType.OpenParentheses),
                    new Token(TokenType.OpenParentheses),
                    new Token(TokenType.CloseParentheses),
                    new Token(TokenType.CloseParentheses),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "abc",
                new[]
                {
                    new Token("abc"),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "first second",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.Space),
                    new Token("second"),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "first\tsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.Tab),
                    new Token("second"),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "first\nsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.NewLine),
                    new Token("second"),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "first\r\nsecond",
                new[]
                {
                    new Token("first"),
                    new Token(TokenType.NewLine),
                    new Token("second"),
                    new Token(TokenType.EndOfStream),
                }
            },
            {
                "##first##",
                new[]
                {
                    new Token(TokenType.Hash),
                    new Token(TokenType.Hash),
                    new Token("first"),
                    new Token(TokenType.Hash),
                    new Token(TokenType.Hash),
                    new Token(TokenType.EndOfStream),
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
                    new Token(TokenType.NewLine),
                    new Token(TokenType.NewLine),
                    new Token(TokenType.NewLine),
                    new Token("Today"),
                    new Token(TokenType.Space),
                    new Token(TokenType.Space),
                    new Token("is"),
                    new Token(TokenType.Tab),
                    new Token(TokenType.Tab),
                    new Token(TokenType.Hash),
                    new Token("CurrentDate"),
                    new Token(TokenType.Hash),
                    new Token(TokenType.EndOfStream),
                }
            },
        };


        [Theory]
        [MemberData(nameof(SingleScenarios))]
        public void Should_Read_Single(string content, TokenType expectedType)
        {
            var subject = new Lexer(content);
            var result = subject.ReadAll();

            var expected = new[]
            {
                new Token(expectedType),
                new Token(TokenType.EndOfStream),
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(MultiScenarios))]
        public void Should_Read_Multi(string content, Token[] expected)
        {
            var subject = new Lexer(content);
            var result = subject.ReadAll();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
