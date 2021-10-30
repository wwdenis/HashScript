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

        private string GenerateChildren(Node node, Dictionary<string, object> data)
        {
            var builder = new StringBuilder();

            foreach (var child in node.Children)
            {
                if (child is TextNode text)
                {
                    builder.Append(text.Content);
                }
                else if (child is FieldNode field)
                {
                    var value = GetRawValue(data, field);
                    var treeData = GetTreeData(value);

                    if (value is null)
                    {
                        builder.Append($" ||{field.Name}|| ");
                    }
                    else if (field.FieldType == FieldType.Simple)
                    {
                        builder.Append(value);
                    }

                    foreach (var leafData in treeData)
                    {
                        var content = GenerateChildren(child, leafData);
                        builder.Append(content);
                    }
                }
            }

            return builder.ToString();
        }

        private object GetRawValue(Dictionary<string, object> data, FieldNode fieldNode)
        {
            if (data.TryGetValue(fieldNode.Name, out var value))
            {
                return value;
            }

            return null;
        }

        private static List<Dictionary<string, object>> GetTreeData(object value)
        {
            var result = new List<Dictionary<string, object>>();

            if (value is Dictionary<string, object>[] collection)
            {
                result.AddRange(collection);
            }
            else if (value is Dictionary<string, object> single)
            {
                result.Add(single);
            }

            return result;
        }
    }
}