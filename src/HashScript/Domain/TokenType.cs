using System;

namespace HashScript.Domain
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
        IsTrue = '?',
        IsFalse = '!',
        Value = '$',
    }
}