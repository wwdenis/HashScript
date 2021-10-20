using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HashScript.Domain;

namespace HashScript
{
    public sealed class Writer
    {
        readonly string template;

        public Writer(string template)
        {
            this.template = template;
        }

        public string Generate(Dictionary<string, object> data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var parser = new Parser(template);
            var doc = parser.Parse();

            if (doc.Errors.Any())
            {
                throw new ApplicationException("There are errors in the template syntax");
            }

            return GenerateChildren(doc, data);
        }

        private string GenerateChildren(Node parent, Dictionary<string, object> data)
        {
            var builder = new StringBuilder();

            foreach (var node in parent.Children)
            {
                if (node is TextNode textNode)
                {
                    builder.Append(textNode.Content);
                }
                else if (node is FieldNode fieldNode)
                {
                    var content = string.Empty;
                    var value = data.ContainsKey(fieldNode.Name) ? data[fieldNode.Name] : null;

                    if (fieldNode.FieldType == FieldType.Simple)
                    {
                        var simpleContent = value?.ToString() ?? "##NOT FOUND##";
                        builder.Append(simpleContent);
                    }
                    else
                    {
                        var list = value as Dictionary<string, object>[];
                        foreach (var item in list)
                        {
                            var complexContent = GenerateChildren(node, item);
                            builder.Append(complexContent);
                        }
                    }

                }
            }

            return builder.ToString();
        }
    }
}