using FluentAssertions;
using Xunit;
using HashScript.Tests.Infrastructure;

namespace HashScript.Tests
{
    public class ParserTests
    {
        [Theory]
        [FileData("ParserSimple")]
        public void Can_Parse_Simple(string content, FakeNode expected)
        {
            var subject = new Parser();
            var result = subject.Parse(content);

            result
                .Should()
                .BeEquivalentTo(
                    expected,
                    opts => opts.IgnoringCyclicReferences());
        }
    }
}
