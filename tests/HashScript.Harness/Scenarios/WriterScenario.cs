using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HashScript.Harness.Scenarios
{
    internal static class WriterScenario
    {
        private const string TemplateFolder = "Templates";
        private const string ContentFolder = "Contents";

        public static Dictionary<string, string> GenerateAll()
        {
            var result = new Dictionary<string, string>();
            var scenarios = ReadScenarios();
            
            foreach (var (name, template, input) in scenarios)
            {
                var writer = new Writer(template);
                var output = writer.Generate(input);

                result.Add(name, output);
            }

            return result;
        }

        static (string, string, Dictionary<string, object>)[] ReadScenarios()
        {
            var result = new List<(string, string, Dictionary<string, object>)>();

            var templates = ReadFiles(TemplateFolder, "hs");
            var contents = ReadFiles(ContentFolder, "json");

            var dir = new DirectoryInfo(ContentFolder);
            
            foreach (var content in contents)
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content.Value, new NestedObjectConverter());
                foreach (var template in templates)
                {
                    result.Add((content.Key, template.Value, data));
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
    }
}
