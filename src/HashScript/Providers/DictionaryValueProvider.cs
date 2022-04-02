using System;
using System.Collections.Generic;
using System.Linq;
using HashScript.Nodes;

namespace HashScript.Providers
{
    public class DictionaryValueProvider : IValueProvider
    {
        private readonly IDictionary<string, object> source;

        public DictionaryValueProvider(IDictionary<string, object> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal DictionaryValueProvider(IDictionary<string, object> source, IEnumerable<string> functions)
            : this(source)
        {
            this.Functions = functions ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Functions { get; private set; }

        public object GetValue(FieldNode field)
        {
            if (this.source.TryGetValue(field.Name, out var value))
            {
                return value;
            }

            return null;
        }

        public IEnumerable<IValueProvider> GetChildren(FieldNode field)
        {
            var value = this.GetValue(field);
            var data = new List<Dictionary<string, object>>();

            if (value is IEnumerable<Dictionary<string, object>> collection)
            {
                data.AddRange(collection);
            }
            else if (value is Dictionary<string, object> single)
            {
                data.Add(single);
            }
            else if (value is not null)
            {
                var empty = new Dictionary<string, object>
                {
                    { "", value },
                };
                data.Add(empty);
            }

            var result = new List<DictionaryValueProvider>();
            var pos = 0;

            foreach (var item in data)
            {
                pos++;
                var functions = new List<string>();

                if (pos == 1)
                {
                    functions.Add("First");
                }

                if (pos == data.Count)
                {
                    functions.Add("Last");
                }

                var provider = new DictionaryValueProvider(item, functions);
                result.Add(provider);
            }

            return result;
        }
    }
}
