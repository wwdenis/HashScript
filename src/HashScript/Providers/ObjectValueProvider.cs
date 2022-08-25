using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HashScript.Providers
{
    public class ObjectValueProvider : IValueProvider
    {
        private readonly object source;

        public ObjectValueProvider(object source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private ObjectValueProvider(object source, IEnumerable<string> functions)
            : this(source)
        {
            this.Functions = functions ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Functions { get; private set; }

        public object GetValue()
        {
            return this.source;
        }

        public object GetValue(string fieldName)
        {
            if (this.source is null)
            {
                return null;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.GetProperty;
            var prop = this.source.GetType().GetProperty(fieldName, flags);

            if (prop is null)
            {
                return null;
            }

            return prop.GetValue(this.source);
        }

        public IEnumerable<IValueProvider> GetChildren(string fieldName)
        {
            var value = this.GetValue(fieldName);
            IEnumerable<object> data = null;

            if (value is IEnumerable collection && value is not string)
            {
                data = collection.OfType<object>();
            }
            else if (value is not null)
            {
                data = new[] { value };
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

                if (pos == data.Count())
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
