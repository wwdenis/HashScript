using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HashScript.Harness.Scenarios
{
    internal static class WriterScenario
    {
        private const string TemplateFolder = "Templates";

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
            var dir = new DirectoryInfo(TemplateFolder);
            var files = dir.EnumerateFiles("*.hs");
            
            foreach (var file in files)
            {
                var template = File.ReadAllText(file.FullName);
                var contents = Deserialize(file.FullName);
                result.Add((file.Name, template, contents));
            }

            return result.ToArray();
        }

        static Dictionary<string, object> Deserialize(string templateFile)
        {
            var contentFile = Path.ChangeExtension(templateFile, ".json");
            var contents = File.ReadAllText(contentFile);
            
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(contents, new NestedJsonConverter());
        }
    }

    class NestedJsonConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        public override IDictionary<string, object> Create(Type objectType)
        {
            return new Dictionary<string, object>();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize(reader, typeof(Dictionary<string, object>[]));
            }

            return serializer.Deserialize(reader);
        
        }
    }
}
