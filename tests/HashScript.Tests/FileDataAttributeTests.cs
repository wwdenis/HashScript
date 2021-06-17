using FluentAssertions;
using HashScript.Domain;
using HashScript.Tests.Infrastructure;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace HashScript.Tests
{
    public class FileDataAttributeTests
    {
        public static readonly TheoryData ValueScenarios = new TheoryData<Delegate, string, object[][]>
        {
            {
                new Action<string, int, MyEnum>((text, number, enumeration) => { }),
                @"[{
                    'text': 'ABC',
                    'number': 10,
                    'enumeration': 'First'
                }]",
                new object[][]
                {
                    new object[]{ "ABC", 10, MyEnum.First },
                }
            },
        };

        public static readonly TheoryData ArrayScenarios = new TheoryData<Delegate, string, object[][]>
        {
            {
                new Action<string[], int[]>((strings, numbers) => { }),
                @"[{
                    'strings': [ 'A', 'B', 'C' ],
                    'numbers': [ 1, 2, 3 ]
                }]",
                new object[][]
                {
                    new object[]{ new[] { "A", "B", "C" } , new[] { 1, 2, 3 } },
                }
            },
        };

        public static readonly TheoryData ObjectScenarios = new TheoryData<Delegate, string, object[][]>
        {
            {
                new Action<string, FakeNode>((content, expected) => { }),
                @"[{
                    'content': 'ABC',
                    'expected': {
                        '$id': '1',
                        'Content': 'Parent',
                        'Type': 'Document',
                        'Children': [
                            { 'Content': 'One', 'Type': 'Field', 'Parent': { '$ref': '1' } },
                            { 'Content': 'Two', 'Type': 'Field', 'Parent': { '$ref': '1' } },
                            { 'Content': 'Three', 'Type': 'Field', 'Parent': { '$ref': '1' } }
                        ]
                    }
                }]",
                new object[][]
                {
                    new object[]
                    {
                        "ABC",
                        new FakeNode("Parent", NodeType.Document)
                        {
                            Children = new List<FakeNode>
                            {
                                new FakeNode("One", NodeType.Field),
                                new FakeNode("Two", NodeType.Field),
                                new FakeNode("Three", NodeType.Field),
                            }
                        }
                    },
                }
            }
        };

        public enum MyEnum
        {
            First = 1,
            Second = 2,
        }

        [Theory]
        [MemberData(nameof(ValueScenarios))]
        [MemberData(nameof(ArrayScenarios))]
        [MemberData(nameof(ObjectScenarios))]
        public void Can_Parse_Objects(Delegate action, string scenario, object[][] expected)
        {
            var reader = BuildReader(scenario);
            
            var subject = new FileDataAttribute(reader);

            var result = subject.GetData(action.Method);

            result
                .Should()
                .BeEquivalentTo(
                    expected,
                    opts => opts.IgnoringCyclicReferences());
        }

        private static IScenarioReader BuildReader(string scenario)
        {
            scenario = scenario.Replace(@"'", @"""");

            var reader = Substitute.For<IScenarioReader>();
            reader.Read().ReturnsForAnyArgs(scenario);
            return reader;
        }
    }
}
