using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace HashScript.Tests.Infrastructure
{
    public class FileDataAttribute : DataAttribute
    {
        private readonly ScenarioReader reader;

        public FileDataAttribute(params string[] pathFragments)
        {
            this.reader = new ScenarioReader(pathFragments);
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            try
            {
                var expectedArg = testMethod
                    .GetParameters()
                    .ElementAt(1);

                var template = this.reader.ReadTemplate();
                var expected = this.reader.ReadObject(expectedArg.ParameterType);
                return new object[][] { new object[] { template, expected } };
            }
            catch (Exception ex)
            {
                var filePath = string.Join('|', reader.Fragments);
                throw new Exception($"Unable to load scenario for: {filePath}", ex);
            }
        }
    }
}