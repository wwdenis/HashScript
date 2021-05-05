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
            { '\n' , TokenType.NewLine }, 
            { '\r' , TokenType.NewLine },
            { '#' , TokenType.Hash },
            { '[' , TokenType.OpenBracket },
            { ']' , TokenType.CloseBracket },
            { '(' , TokenType.OpenParentheses },
            { ')' , TokenType.CloseParentheses },
        };

        static readonly TokenType[] SpecialTypes = new[]
        {
            TokenType.Hash,
            TokenType.OpenBracket,
            TokenType.CloseBracket,
            TokenType.OpenParentheses,
            TokenType.CloseParentheses,
        };

        public static bool IsSpecial(this TokenType type)
        {
            return SpecialTypes.Contains(type);
        }

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
            return Mappings.First(i => i.Value == type).Key;
        }

        public static Token TryEscape(this TokenType type, int size)
        {
            if (!type.IsSpecial() || size == 1)
            {
                return null;
            }

            size /= 2;
            var item = type.BuildChar();
            var content = new string(item, size);
            return new Token(TokenType.Text, size, content);
        }
    }
}