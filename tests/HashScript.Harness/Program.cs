using System;
using HashScript.Harness.Scenarios;

namespace HashScript.Harness
{
    class Program
    {
        private const string TemplateFolder = "Templates";

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);

            var templates = WriterScenario.GenerateAll();
            
            foreach (var (name, output) in templates)
            {
                Console.WriteLine($"File: {name}");
                Console.WriteLine(output);
            }

            Console.WriteLine(" *** END *** ");
        }
    }
}
