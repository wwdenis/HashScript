using System;
using System.Collections.Generic;
using HashScript.Providers;

namespace HashScript.Samples
{
    class Program
    {
        const string NumberListTemplate = @"#+Items# #Number# #!.Last# >#!# #+#";

        const string TableDefinitionTemplate = 
@"class #Name# 
{
#+Columns#
    public #Type# #Name#
    {
        get;
        #!ReadOnly#
        set;
        #!#
    }

#+#
}";

        static object NumberListData = new
        {
            Items = new[]
            {
                new { Number = 1 },
                new { Number = 2 },
                new { Number = 3 },
            }
        };

        static object TableDefinitionData = new
        {
            Name = "Customer",
            Columns = new[]
            {
                new { Name = "Id", Type = "int", ReadOnly = true, },
                new { Name = "Name", Type = "Text", ReadOnly = false, },
                new { Name = "BirthDate", Type = "DateTime", ReadOnly = false },
            }
        };

        static void Main(string[] args)
        {
            var samples = new Dictionary<string, object>
            {
                { NumberListTemplate, NumberListData },
                { TableDefinitionTemplate, TableDefinitionData },
            };

            Console.WriteLine(DateTime.Now);

            foreach (var sample in samples)
            {
                var source = sample.Value;
                var data = new ObjectValueProvider(source);
                var writer = new Writer(sample.Key);
                var output = writer.Generate(data);

                Console.WriteLine(new string('-', 20));
                Console.WriteLine("Template:");
                Console.WriteLine(new string('-', 20));
                Console.WriteLine(sample.Key);
                Console.WriteLine(new string('-', 20));
                Console.WriteLine("Output:");
                Console.WriteLine(new string('-', 20));
                Console.WriteLine(output);
                Console.WriteLine();
            }


            Console.WriteLine(new string('-', 20));
            Console.WriteLine(" *** END *** ");
        }
    }
}
