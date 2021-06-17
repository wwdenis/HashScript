using System.IO;

namespace HashScript.Tests.Infrastructure
{
    public class FileScenarioReader : IScenarioReader
    {
        private const string RootFolder = "Scenarios";

        private const string DefaultExtension = ".json";
        private readonly string fileName;

        public FileScenarioReader(string fileName)
        {
            this.fileName = fileName;
        }

        public string Read()
        {
            var fullPath = Path.Combine(RootFolder, this.fileName);

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
    }
}