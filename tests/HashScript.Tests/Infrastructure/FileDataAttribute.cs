using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace HashScript.Tests.Infrastructure
{
    public class FileDataAttribute : DataAttribute
    {
        private const string RootFolder = "Scenarios";

        private const string DefaultExtension = ".json";

        private readonly string fileName;

        public FileDataAttribute(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            this.fileName = fileName;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod is null)
            {
                throw new ArgumentNullException(nameof(testMethod));
            }

            var contents = ReadContents(this.fileName);
            var result = ParseTestArguments(contents, testMethod);

            return result;
        }

        private static string ReadContents(string fileName)
        {
            var fullPath = Path.Combine(RootFolder, fileName);

            if (!Path.HasExtension(fullPath))
            {
                fullPath = Path.ChangeExtension(fullPath, DefaultExtension);
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Scenario file does not exist.", fullPath);
            }

            var contents = File.ReadAllText(fullPath);
            return contents;
        }

        private static List<object[]> ParseTestArguments(string contents, MethodInfo testMethod)
        {
            var result = new List<object[]>();

            var scenarios = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(contents);
            var args = testMethod.GetParameters().ToDictionary(k => k.Name, v => v.ParameterType);

            foreach (var item in scenarios)
            {
                var values = new List<object>();

                foreach (var arg in item)
                {
                    var type = args[arg.Key];
                    var value = arg.Value;

                    if (value is JContainer obj)
                    {
                        value = obj.ToObject(type);
                    }
                    else if (type.IsEnum && Enum.TryParse(type, (string)value, out var enumValue))
                    {
                        value = enumValue;
                    }

                    values.Add(value);
                }

                result.Add(values.ToArray());
            }

            return result;
        }
    }
}