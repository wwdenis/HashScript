using System.Collections.Generic;
using HashScript.Nodes;

namespace HashScript.Providers
{
    public interface IValueProvider
    {
        IEnumerable<string> Functions { get; }

        object GetValue();

        object GetValue(string fieldName);

        IEnumerable<IValueProvider> GetChildren(string fieldName);
    }
}