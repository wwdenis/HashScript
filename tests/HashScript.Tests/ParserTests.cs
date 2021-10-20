using FluentAssertions;
using Xunit;
using HashScript.Domain;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class ParserTests
    {
        [Theory]
        [FileData("Parser", "Text", "SingleWord")]
        [FileData("Parser", "Text", "MultiLine")]
        public void Can_Parse_Text(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Parser", "SimpleField", "OneField")]
        [FileData("Parser", "SimpleField", "TwoFields")]
        public void Can_Parse_SimpleField(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Errors", "SimpleField", "NoClose")]
        [FileData("Errors", "SimpleField", "NoName")]
        [FileData("Errors", "SimpleField", "NoNameAndClose")]
        [FileData("Errors", "SimpleField", "WithAmperstand")]
        [FileData("Errors", "SimpleField", "WithComplex")]
        [FileData("Errors", "SimpleField", "WithDot")]
        [FileData("Errors", "SimpleField", "WithIsFalse")]
        [FileData("Errors", "SimpleField", "WithIsTrue")]
        [FileData("Errors", "SimpleField", "WithNewLine")]
        [FileData("Errors", "SimpleField", "WithSpace")]
        [FileData("Errors", "SimpleField", "WithTab")]
        [FileData("Errors", "SimpleField", "WithValueSign")]
        public void Can_Parse_Errors(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }
    }
}
