using System;

namespace HashScript.Domain
{
    public enum TokenType
    {
        EndOfStream,
        Text,
        NewLine,
        Space,
        Tab,
        Hash,
        Complex,
        Condition,
        Negate,
        Content,
    }
}