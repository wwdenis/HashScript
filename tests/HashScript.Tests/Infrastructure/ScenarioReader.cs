using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HashScript.Tests.Infrastructure
{
    internal class ScenarioReader
    {
        private const string RootFolder = "Scenarios";
        private const string DataExtension = "json";
        private const string TemplateExtension = "hz";

        public ScenarioReader(params string[] pathFragments)
        {
            this.Fragments = pathFragments;
        }

        public string[] Fragments { get; }

        public string ReadFile(string fileExtension)
        {
            var fullPath = RootFolder;

            foreach (var dir in this.Fragments)
            {
                fullPath = Path.Combine(fullPath, dir);
            }

            if (!Path.HasExtension(fullPath))
            {
                fullPath = Path.ChangeExtension(fullPath, fileExtension);
            }

            return File.ReadAllText(fullPath);
        }

        public string ReadTemplate()
        {
            return this.ReadFile(TemplateExtension);
        }

        public object ReadObject(Type expectedType, string fileExtension = DataExtension)
        {
            var json = this.ReadFile(fileExtension);

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(), new NodeConverter() },
                WriteIndented = true
            };

            return JsonSerializer.Deserialize(json, expectedType, options);
        }

        public object ReadObject<T>(string fileExtension = DataExtension)
        {
            return this.ReadObject(typeof(T), fileExtension);
        }
    }
}