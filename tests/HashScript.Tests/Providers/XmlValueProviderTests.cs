using System;
using System.Xml;
using FluentAssertions;
using HashScript.Providers;
using Xunit;

namespace HashScript.Providers.Tests
{
    public class XmlValueProviderTests
    {
        [Fact]
        public void Should_Parse()
        {
            const string Xml =
                @"<Info>
                    <Name>Denis</Name>
                    <City>POA</City>
                 </Info>";

            var doc = new XmlDocument();
            doc.LoadXml(Xml);

            var subject = new XmlValueProvider(doc);

            var result = subject.GetValue("City");

            result
                .Should()
                .BeEquivalentTo("POA");
        }
    }
}
