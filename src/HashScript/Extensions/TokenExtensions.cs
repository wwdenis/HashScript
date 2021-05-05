using System.Linq;
using System.Collections.Generic;

namespace HashScript.Extensions
{
    public static class TokenExtensions
    {
        static readonly Dictionary<char, TokenType> Mappings = new Dictionary<char, TokenType>
        {
            { ' ' , TokenType.Space },
            { '\t' , TokenType.Tab },
            { '\r' , TokenType.NewLine },
            { '\n' , TokenType.NewLine },
            { '#' , TokenType.Hash },
        };

        public static TokenType BuildType(this char content)
        {
            if (!Mappings.TryGetValue(content, out var type))
            {
                type = TokenType.Text;
            }
            return type;
        }

        public static char BuildChar(this TokenType type)
        {
            if(!Mappings.ContainsValue(type))
            {
                return (char)0;
            }
            return Mappings.Single(i => i.Value == type).Key;
        }
    }
}