using System.Collections.Generic;
using HashScript.Nodes;

namespace HashScript.Providers
{
    public interface IValueProvider
    {
        IEnumerable<string> Functions { get; }

        object GetValue(FieldNode field);

        IEnumerable<IValueProvider> GetChildren(FieldNode field);
    }
}