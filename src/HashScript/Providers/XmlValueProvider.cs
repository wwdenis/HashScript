using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace HashScript.Providers
{
    public class XmlValueProvider : IValueProvider
    {
        private readonly XmlElement source;

        public XmlValueProvider(XmlElement source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public XmlValueProvider(XmlDocument source) : this(source?.DocumentElement)
        {
        }

        private XmlValueProvider(XmlElement source, IEnumerable<string> functions)
            : this(source)
        {
            this.Functions = functions ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Functions { get; private set; }

        public object GetValue()
        {
            return this.source.InnerText;
        }

        public object GetValue(string fieldName)
        {
            if (this.source is null)
            {
                return null;
            }


            if (string.Equals(fieldName, this.source.Name))
            {
                return this.source.InnerText;
            }

            return this
                .source
                .GetElementsByTagName(fieldName)
                .OfType<XmlElement>()
                .SingleOrDefault()?
                .InnerText;
        }

        public IEnumerable<IValueProvider> GetChildren(string fieldName)
        {
            var result = new List<XmlValueProvider>();
            var pos = 0;

            var children = this.source.ChildNodes;
            var data = children.OfType<XmlElement>();

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

                var provider = new XmlValueProvider(item, functions);
                result.Add(provider);
            }

            return result;
        }
    }
}
