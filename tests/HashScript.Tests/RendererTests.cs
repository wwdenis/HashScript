using FluentAssertions;
using Xunit;
using HashScript.Tests.Infrastructure;
using HashScript.Providers;
using HashScript.Tests.Scenarios.Renderer;

namespace HashScript.Tests
{
    public class RendererTests
    {
        [Fact]
        public void Should_Generate()
        {
            var reader = new ScenarioReader("Renderer", "NumberList");
            var data = reader.ReadObject<NumberList>();
            var template = reader.ReadFile("hz");
            var expected = reader.ReadFile("txt");

            var subject = new Renderer(template);
            var provider = new ObjectValueProvider(data);
            var result = subject.Generate(provider);

            result
                .Should()
                .BeEquivalentTo(expected);
        }
    }
}
