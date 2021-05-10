using System.Linq;
using System.Collections.Generic;
using HashScript.Domain;

namespace HashScript.Extensions
{
    public static class TokenExtensions
    {
        static readonly Dictionary<char, TokenType> Mappings = new Dictionary<char, TokenType>
        {
            { (char)0 , TokenType.EndOfStream },
            { ' ' , TokenType.Space },
            { '\t' , TokenType.Tab },
            { '\n' , TokenType.NewLine }, 
            { '\r' , TokenType.NewLine },
            { '#' , TokenType.Hash },
            { '+' , TokenType.Complex },
            { '?' , TokenType.Condition },
            { '!' , TokenType.Negate },
            { '$' , TokenType.Content },
        };

        public static TokenType BuildType(this char content)
        {
            if (!Mappings.TryGetValue(content, out var type))
            {
                type = TokenType.Text;
            }
            return type;
        }
    }
}