using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;

namespace HashScript.Tests.Infrastructure
{
    public class FileDataAttribute : DataAttribute
    {
        private readonly IScenarioReader reader;

        internal FileDataAttribute(IScenarioReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public FileDataAttribute(string fileName) : this(new FileScenarioReader(fileName))
        {
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            try
            {
                var contents = this.reader.Read();
                var result = ParseTestArguments(contents, testMethod);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private static List<object[]> ParseTestArguments(string contents, MethodInfo testMethod)
        {
            var result = new List<object[]>();
            
            var args = testMethod
                .GetParameters()
                .ToDictionary(k => k.Name, v => v.ParameterType);

            var scenarios = Deserialize<List<Dictionary<string, JsonElement>>>(contents);
            var fileError = scenarios is null || scenarios.Any(i => i.Keys.Except(args.Keys).Any());

            if (fileError)
            {
                return result;
            }

            foreach (var scenario in scenarios)
            {
                var values = new List<object>();
                
                foreach (var parameter in scenario)
                {
                    var error = false;
                    var type = args[parameter.Key];
                    var element = parameter.Value;

                    var value = ReadElement(element, type, out error);

                    if (error)
                    {
                        values = null;
                        continue;
                    }

                    values.Add(value);
                }

                if (values is not null)
                {
                    result.Add(values.ToArray());
                }
            }

            return result;
        }

        private static object ReadElement(JsonElement element, Type type, out bool error)
        {
            error = false;
            object value = null;
            
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    value = element.GetString();
                    break;
                case JsonValueKind.Number:
                    value = element.GetDouble();
                    break;
                case JsonValueKind.False:
                    value = false;
                    break;
                case JsonValueKind.True:
                    value = true;
                    break;
                case JsonValueKind.Object:
                    var json = element.GetRawText();
                    value = Deserialize(json, type, out error);
                    break;
                case JsonValueKind.Array:
                    value = ReadArray(element, type, out error);
                    break;
            }

            if (type.IsEnum && Enum.TryParse(type, (string)value, out var enumValue))
            {
                value = enumValue;
            }

            return value;
        }

        private static IEnumerable<object> ReadArray(JsonElement element, Type type, out bool error)
        {
            error = false;

            if (!type.IsArray)
            {
                error = true;
                return null;
            }

            var itemType = type.GetElementType();
            var source = element.EnumerateArray();
            var target = new List<object>();

            foreach (var item in source)
            {
                var sourceItem = ReadElement(item, itemType, out error);
                if (error)
                {
                    return null;
                }
                target.Add(sourceItem);
            }

            return target;
        }

        private static T Deserialize<T>(string contents)
        {
            return (T)Deserialize(contents, typeof(T), out _);
        }

        private static object Deserialize(string contents, Type type, out bool hasErrors)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            options.Converters.Add(new JsonStringEnumConverter());

            object value = null;

            try
            {
                hasErrors = false;
                value = JsonSerializer.Deserialize(contents, type, options);
            }
            catch (Exception)
            {
                hasErrors = true;
            }

            return value;
        }
    }
}