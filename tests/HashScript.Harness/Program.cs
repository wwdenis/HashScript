using System;
using System.IO;
using System.Collections.Generic;
using HashScript.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HashScript.Harness
{
    class Program
    {
        private const string TemplateFolder = "Templates";

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);

            var templates = ReadTemplates();
            
            foreach (var (name, contents) in templates)
            {
                var parser = new Parser(contents);
                var result = parser.Parse();
                var output = Serialize(result);

                Console.WriteLine($"File: {name}");
                Console.WriteLine(output);
            }

            Console.WriteLine(" *** END *** ");
        }

        static Dictionary<string, string> ReadTemplates()
        {
            var result = new Dictionary<string, string>();
            var dir = new DirectoryInfo(TemplateFolder);
            var files = dir.EnumerateFiles("*.txt");
            
            foreach (var file in files)
            {
                var contents = File.ReadAllText(file.FullName);
                result.Add(file.Name, contents);
            }

            return result;
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
