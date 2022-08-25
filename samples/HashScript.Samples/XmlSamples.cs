using System;
using System.Collections.Generic;
using System.Xml;
using HashScript.Providers;

namespace HashScript.Samples
{
    class XmlSamples
    {
        static readonly Dictionary<string, string> Samples = new Dictionary<string, string>
        {
            {
                @"#+Items# #Number# #!.Last# >#!# #+#",
                @"<Items>
                    <Number>1</Number>
                    <Number>2</Number>
                    <Number>3</Number>
                </Items>"
            }
        };

        public static IDictionary<string, IValueProvider> GetAll()
        {
            var samples = new Dictionary<string, IValueProvider>();

            foreach (var sample in Samples)
            {
                var doc = new XmlDocument();
                doc.LoadXml(sample.Value);
                var data = new XmlValueProvider(doc);

                samples.Add(sample.Key, data);
            }

            return samples;
        }
    }
}
