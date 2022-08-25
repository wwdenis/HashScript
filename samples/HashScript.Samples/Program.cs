using System;

namespace HashScript.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var samples = ObjectSamples.GetAll();
            
            foreach (var sample in samples)
            {
                var renderer = new Renderer(sample.Key);
                var output = renderer.Generate(sample.Value);

                var lines = new[] { "Template:", sample.Key, "Output:", output };

                foreach (var line in lines)
                {
                    Console.WriteLine(new string('-', 20));
                    Console.WriteLine(line);
                }
            }
        }
    }
}
