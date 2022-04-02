using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public object GetValue(string fieldName)
        {
            if (source is null)
            {
                return null;
            }

            if (this.source.TryGetValue(fieldName, out var value))
            {
                return value;
            }

            return null;
        }

        public IEnumerable<IValueProvider> GetChildren(string fieldName)
        {
            var value = this.GetValue(fieldName);
            var data = new List<Dictionary<string, object>>();

            if (value is IEnumerable<Dictionary<string, object>> collection)
            {
                data.AddRange(collection);
            }
            else if (value is IEnumerable list && value is not string)
            {
                foreach (var item in list)
                {
                    var empty = CreateEmpty(item);
                    data.Add(empty);
                }
            }
            else if (value is Dictionary<string, object> single)
            {
                data.Add(single);
            }
            else if (value is not null)
            {
                var empty = CreateEmpty(value);
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

        private static Dictionary<string, object> CreateEmpty(object value)
        {
            return new Dictionary<string, object>
            {
                { "", value },
            };
        }
    }
}
