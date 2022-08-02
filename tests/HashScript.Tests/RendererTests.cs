using System;
using FluentAssertions;
using HashScript.Providers;
using Xunit;

namespace HashScript.Tests
{
    public class RendererTests
    {
        public static readonly TheoryData ObjectScenarios = new TheoryData<string, string, object>
        {
            {
                "#!Value#No value found#!#",
                "No value found",
                new
                {
                    Value = (Type)null
                }
            },
            {
                "#+Numbers##.##+#",
                "0123456789",
                new
                {
                    Numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                }
            },
            {
                "#+Items##Position##!.Last# > #!##+#",
                "1 > 2 > 3",
                new 
                {
                    Items = new[]
                    {
                        new { Position = 1 },
                        new { Position = 2 },
                        new { Position = 3 },
                    }
                }
            }
        };
        
        [Theory]
        [MemberData(nameof(ObjectScenarios))]
        public void Should_Generate(string template, string expected, object data)
        {
            var subject = new Renderer(template);
            var provider = new ObjectValueProvider(data);
            var result = subject.Generate(provider);

            result
                .Should()
                .BeEquivalentTo(expected);
        }
    }
}
