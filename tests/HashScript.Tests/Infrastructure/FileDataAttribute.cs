using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace HashScript.Tests.Infrastructure
{
    public class FileDataAttribute : DataAttribute
    {
        private readonly string fileName;
        private readonly IScenarioReader reader;

        internal FileDataAttribute(IScenarioReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public FileDataAttribute(string fileName) : this(new FileScenarioReader())
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            this.fileName = fileName;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            try
            {
                var contents = this.reader.Read(this.fileName);
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

            var scenarios = DeserializeScenarios(contents);
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
                    var value = parameter.Value;

                    if (value is JToken)
                    {
                        var text = value.ToString();
                        value = DeserializeValue(text, type, out error);
                    }
                    else if (type.IsEnum && Enum.TryParse(type, (string)value, out var enumValue))
                    {
                        value = enumValue;
                    }

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

        private static List<Dictionary<string, object>> DeserializeScenarios(string contents)
        {
            var type = typeof(List<Dictionary<string, object>>);
            var result = DeserializeValue(contents, type, out _);
            return result as List<Dictionary<string, object>>;
        }

        private static object DeserializeValue(string contents, Type type, out bool hasErrors)
        {
            var parsingErrors = false;

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                Error = (sender, args) => {
                    parsingErrors = true;
                    args.ErrorContext.Handled = true;
                },
            };

            var value = JsonConvert.DeserializeObject(contents, type, settings);

            hasErrors = parsingErrors;

            return value;
        }
    }
}