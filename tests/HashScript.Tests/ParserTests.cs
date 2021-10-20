using FluentAssertions;
using Xunit;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class ParserTests
    {
        [Theory]
        [FileData("TextParse")]
        [FileData("SimpleFieldParse")]
        public void Can_Parse_Success(string template, FakeNode expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(
                    expected,
                    opts => opts
                        .ExcludingMissingMembers()
                        .IgnoringCyclicReferences());
        }

        [Theory]
        [FileData("SimpleFieldError")]
        public void Can_Parse_Errors(string template, FakeNode expected)
        {
            var subject = new Parser(template);
            var result = subject.Parse();

            result
                .Should()
                .BeEquivalentTo(
                    expected,
                    opts => opts
                        .ExcludingMissingMembers()
                        .IgnoringCyclicReferences());
        }
    }
}
