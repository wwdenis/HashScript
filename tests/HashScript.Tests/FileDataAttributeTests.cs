using FluentAssertions;
using HashScript.Domain;
using HashScript.Tests.Infrastructure;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace HashScript.Tests
{
    public class FileDataAttributeTests
    {
        [Fact]
        public void Can_GetData()
        {
            Action<string, FakeNode> lambda = (content, expected) => { };

            var children = new[]
            {
                new FakeNode("One", NodeType.Field),
                new FakeNode("Two", NodeType.Field),
                new FakeNode("Three", NodeType.Field),
            };

            var root = new FakeNode("Parent", NodeType.Document, children);

            var scenarios = new[]
            {
                new Dictionary<string, object>
                {
                    { "content", "ABC" },
                    { "expected", root },
                }
            };

            var expected = new object[][]
            {
                new object[] { "ABC", root }
            };

            var reader = BuildReader(scenarios);
            
            var subject = new FileDataAttribute(reader);

            var result = subject.GetData(lambda.Method);

            result
                .Should()
                .BeEquivalentTo(
                    expected,
                    opts => opts.IgnoringCyclicReferences());
        }

        private static IScenarioReader BuildReader(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            var contents = JsonConvert.SerializeObject(value, settings);
            
            var reader = Substitute.For<IScenarioReader>();
            reader.Read(default).ReturnsForAnyArgs(contents);
            return reader;
        }
    }
}
