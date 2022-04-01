using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HashScript.Nodes;

namespace HashScript.Providers
{
    public class ObjectValueProvider : IValueProvider
    {
        private readonly object source;

        public ObjectValueProvider(object source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal ObjectValueProvider(object source, IEnumerable<string> functions)
            : this(source)
        {
            this.Functions = functions ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Functions { get; private set; }

        public object GetValue(FieldNode field)
        {
            if (this.source is null)
            {
                return null;
            }
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.GetProperty;
            var prop = this.source.GetType().GetProperty(field.Name, flags);

            if (prop is null)
            {
                return null;
            }

            return prop.GetValue(this.source);
        }

        public IEnumerable<IValueProvider> GetChildren(FieldNode field)
        {
            var value = this.GetValue(field);
            var data = new List<object>();

            if (value is IEnumerable<object> collection)
            {
                data.AddRange(collection);
            }
            else if (value is not null)
            {
                data.Add(value);
            }

            var result = new List<ObjectValueProvider>();
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

                var provider = new ObjectValueProvider(item, functions);
                result.Add(provider);
            }

            return result;
        }
    }
}
