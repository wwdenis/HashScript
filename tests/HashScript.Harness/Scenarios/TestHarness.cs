using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using HashScript.Domain;
using Newtonsoft.Json.Converters;

namespace HashScript.Harness.Scenarios
{
    internal static class TestHarness
    {
        private const string TemplateFolder = "Templates";
        private const string ContentFolder = "Contents";

        public static Dictionary<string, string> WriteAll()
        {
            var result = new Dictionary<string, string>();
            var scenarios = CreateWriteScenarios();
            
            foreach (var (name, template, input) in scenarios)
            {
                var writer = new Writer(template);
                var output = writer.Generate(input);

                result.Add(name, output);
            }

            return result;
        }

        public static Dictionary<string, string> ParseAll()
        {
            var result = new Dictionary<string, string>();
            var templates = ReadFiles(TemplateFolder, "hs");
            
            foreach (var (name, contents) in templates)
            {
                var parser = new Parser(contents);
                var doc = parser.Parse();
                var output = Serialize(doc);

                result.Add(name, output);
            }

            return result;
        }

        static (string, string, Dictionary<string, object>)[] CreateWriteScenarios()
        {
            var result = new List<(string, string, Dictionary<string, object>)>();

            var templates = ReadFiles(TemplateFolder, "hs");
            var contents = ReadFiles(ContentFolder, "json");

            var dir = new DirectoryInfo(ContentFolder);
            
            foreach (var content in contents)
            {
                var data = Deserialize(content.Value);
                foreach (var template in templates)
                {
                    result.Add(($"{content.Key}/{template.Key}", template.Value, data));
                }
            }

            return result.ToArray();
        }

        static Dictionary<string, string> ReadFiles(string folder, string extension)
        {
            var result = new Dictionary<string, string>();
            var dir = new DirectoryInfo(folder);
            var files = dir.EnumerateFiles($"*.{extension}");
            
            foreach (var file in files)
            {
                var template = File.ReadAllText(file.FullName);
                result.Add(file.Name, template);
            }

            return result;
        }

        static Dictionary<string, object> Deserialize(string contents)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(contents, new NestedObjectConverter());
        }

        static string Serialize(Node node)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new[]
                {
                    new StringEnumConverter()
                },
            };

            return JsonConvert.SerializeObject(node, settings);
        }
    }
}
