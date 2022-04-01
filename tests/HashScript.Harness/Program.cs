using System;
using System.Collections.Generic;
using System.Linq;
using HashScript.Harness.Scenarios;

namespace HashScript.Harness
{
    class Program
    {
        private const string TemplateFolder = "Templates";

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);

            var results = new Dictionary<string, string>();

            if (args.Contains("-parse"))
            {
                results = TestHarness.ParseAll();
            }
            else
            {
                results = TestHarness.WriteAll();
            }
            
            foreach (var (name, output) in results)
            {
                Console.WriteLine($"File: {name}");
                Console.WriteLine(output);
            }

            Console.WriteLine(" *** END *** ");
        }
    }
}
