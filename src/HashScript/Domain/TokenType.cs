using System;

namespace HashScript.Domain
{
    public enum TokenType : int
    {
        EndOfStream = -1,
        Text = 0,
        NewLine = '\n',
        Tab = '\t',
        Space = ' ',
        Hash = '#',
        Complex = '+',
        Condition = '?',
        Negate = '!',
        Content = '$',
    }
}