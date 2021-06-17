using FluentAssertions;
using Xunit;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class ParserTests
    {
        [Theory]
        [FileData("ParserSimple")]
        public void Can_Parse_Simple(string template, FakeNode expected)
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
