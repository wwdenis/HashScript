using FluentAssertions;
using Xunit;
using HashScript.Domain;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class LexerTests
    {
        [Theory]
        [FileData("Lexer", "Single", "Complex")]
        [FileData("Lexer", "Single", "Dot")]
        [FileData("Lexer", "Single", "Hash")]
        [FileData("Lexer", "Single", "IsFalse")]
        [FileData("Lexer", "Single", "IsTrue")]
        [FileData("Lexer", "Single", "NewLine")]
        [FileData("Lexer", "Single", "NewLineReturn")]
        [FileData("Lexer", "Single", "Space")]
        [FileData("Lexer", "Single", "Tab")]
        [FileData("Lexer", "Single", "Value")]
        public void Can_Read_Single(string template, Token[] expected)
        {
            var subject = new Lexer(template);
            var result = subject.ReadAll();
            
            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Lexer", "Multi", "MixedSentence")]
        [FileData("Lexer", "Multi", "Complex")]
        [FileData("Lexer", "Multi", "Hash")]
        [FileData("Lexer", "Multi", "Mixed")]
        [FileData("Lexer", "Multi", "NewLine")]
        [FileData("Lexer", "Multi", "OneWord")]
        [FileData("Lexer", "Multi", "OneWordHash")]
        [FileData("Lexer", "Multi", "Space")]
        [FileData("Lexer", "Multi", "Tab")]
        [FileData("Lexer", "Multi", "TwoWordSpace")]
        [FileData("Lexer", "Multi", "TwoWordTab")]
        [FileData("Lexer", "Multi", "TwoWordTabNewLine")]
        public void Can_Read_Multi(string template, Token[] expected)
        {
            var subject = new Lexer(template);
            var result = subject.ReadAll();

            result
                .Should()
                .BeEquivalentTo(expected);
        }
    }
}
