using FluentAssertions;
using Xunit;
using HashScript.Nodes;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class ParserTests
    {
        [Theory]
        [FileData("Parser", "Text", "SingleWord")]
        [FileData("Parser", "Text", "MultiLine")]
        public void Should_Parse_Text(string template, Node expected)
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
        public void Should_Parse_SimpleField(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Errors", "SimpleField", "NoCloseHash")]
        [FileData("Errors", "SimpleField", "NoName")]
        [FileData("Errors", "SimpleField", "NoNameAndClose")]
        [FileData("Errors", "SimpleField", "WithAmperstand")]
        [FileData("Errors", "SimpleField", "WithComplex")]
        [FileData("Errors", "SimpleField", "WithDot")]
        [FileData("Errors", "SimpleField", "WithNegate")]
        [FileData("Errors", "SimpleField", "WithQuestion")]
        [FileData("Errors", "SimpleField", "WithNewLine")]
        [FileData("Errors", "SimpleField", "WithSpace")]
        [FileData("Errors", "SimpleField", "WithTab")]
        [FileData("Errors", "SimpleField", "WithCurrency")]
        public void Should_Fail_SimpleField(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Parser", "ComplexField", "QuestionSingle")]
        [FileData("Parser", "ComplexField", "ComplexSingle")]
        [FileData("Parser", "ComplexField", "ComplexNested")]
        [FileData("Parser", "ComplexField", "NegateSingle")]
        [FileData("Parser", "ComplexField", "NegateAndLast")]
        [FileData("Parser", "ComplexField", "NegateAndFirst")]
        public void Should_Parse_ComplexField(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory]
        [FileData("Errors", "ComplexField", "InvalidFunction")]
        [FileData("Errors", "ComplexField", "NoCloseHash")]
        [FileData("Errors", "ComplexField", "NoCloseNode")]
        [FileData("Errors", "ComplexField", "NoName")]
        [FileData("Errors", "ComplexField", "NoNameAndClose")]
        public void Should_Fail_ComplexField(string template, Node expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(expected);
        }
    }
}
