using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;

namespace HashScript.Tests.Infrastructure
{
    public class FileDataAttribute : DataAttribute
    {
        private const string RootFolder = "Scenarios";

        private readonly string[] pathFragments;

        public FileDataAttribute(params string[] pathFragments)
        {
            this.pathFragments = pathFragments;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            try
            {
                var expectedArg = testMethod
                    .GetParameters()
                    .ElementAt(1);

                var template = ReadTemplate();
                var expected = ReadExpected(expectedArg.ParameterType);
                return new object[][] { new object[] { template, expected } };
            }
            catch (Exception ex)
            {
                var filePath = string.Join('|', pathFragments);
                throw new Exception($"Unable to load scenario for: {filePath}", ex);
            }
        }

        private string ReadTemplate()
        {
            return this.ReadFile("hs");
        }

        private object ReadExpected(Type expectedType)
        {
            var json = this.ReadFile("json");

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(), new NodeConverter() },
                WriteIndented = true
            };

            return JsonSerializer.Deserialize(json, expectedType, options);
        }

        private string ReadFile(string fileExtension)
        {
            var fullPath = RootFolder;

            foreach (var dir in this.pathFragments)
            {
                fullPath = Path.Combine(fullPath, dir);
            }

            if (!Path.HasExtension(fullPath))
            {
                fullPath = Path.ChangeExtension(fullPath, fileExtension);
            }

            return File.ReadAllText(fullPath);
        }
    }
}