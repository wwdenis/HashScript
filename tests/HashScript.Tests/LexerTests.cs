using FluentAssertions;
using Xunit;
using HashScript.Domain;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class LexerTests
    {
        [Theory]
        [FileData("TokenSimple")]
        public void Can_Read_Single(string content, TokenType expected)
        {
            var expectedTypes = new[]
            {
                new Token(expected),
                new Token(TokenType.EOF),
            };

            var subject = new Lexer(content);
            var result = subject.ReadAll();

            result.Should().BeEquivalentTo(expectedTypes);
        }

        [Theory]
        [FileData("TokenMulti")]
        public void Can_Read_Multi(string content, Token[] expected)
        {
            var subject = new Lexer(content);
            var result = subject.ReadAll();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
