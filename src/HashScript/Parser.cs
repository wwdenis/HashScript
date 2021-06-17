using HashScript.Domain;

namespace HashScript
{
    public class Parser
    {
        public DocumentNode Parse(string template)
        {
            using var lexer = new Lexer(template);
            var result = new DocumentNode();
            return result;
        }
    }
}