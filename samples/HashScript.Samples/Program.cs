using System;
using System.Collections.Generic;
using HashScript.Providers;

namespace HashScript.Samples
{
    class Program
    {
        static readonly Dictionary<string, object> Scenarios = new Dictionary<string, object>
        {
            {
                @"#+Items# #.# #!.Last# >#!# #+#",
                new
                {
                    Items = new[] { 1, 2, 3 },
                }
            },
            {
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
}",
                new
                {
                    Name = "Customer",
                    Columns = new[]
                    {
                        new { Name = "Id", Type = "int", ReadOnly = true, },
                        new { Name = "Name", Type = "Text", ReadOnly = false, },
                        new { Name = "BirthDate", Type = "DateTime", ReadOnly = false },
                    }
                }
            }
        };

        static void Main(string[] args)
        {
            var separator = new string('-', 20);

            foreach (var scenario in Scenarios)
            {
                var source = scenario.Value;
                var data = new ObjectValueProvider(source);
                var renderer = new Renderer(scenario.Key);
                var output = renderer.Generate(data);

                var lines = new[] { "Template:", scenario.Key, "Output:", output };

                foreach (var line in lines)
                {
                    Console.WriteLine(separator);
                    Console.WriteLine(line);
                }
            }
        }
    }
}
