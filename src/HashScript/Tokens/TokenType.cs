using System;

namespace HashScript.Tokens
{
    public enum TokenType : int
    {
        EOF = -1,
        Text = 0,
        NewLine = '\n',
        Tab = '\t',
        Space = ' ',
        Hash = '#',
        Complex = '+',
        Dot = '.',
        Question = '?',
        Negate = '!',
    }
}