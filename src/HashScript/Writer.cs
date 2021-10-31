using System;
using System.Collections;
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

            return RenderChildren(doc, data);
        }

        private static string RenderChildren(Node parent, Dictionary<string, object> data)
        {
            var builder = new StringBuilder();

            foreach (var child in parent.Children)
            {
                if (child is TextNode text)
                {
                    builder.Append(text.Content);
                }
                else if (child is FieldNode field)
                {
                    var content = RenderField(field, data);
                    builder.Append(content);
                }
            }

            return builder.ToString();
        }

        private static string RenderField(FieldNode field, Dictionary<string, object> data)
        {
            var builder = new StringBuilder();

            var rawValue = GetRawValue(data, field);
            var contition = GetCondition(rawValue);

            var renderChild = false;
            var renderData = Enumerable.Empty<Dictionary<string, object>>();

            switch (field.FieldType)
            {
                case FieldType.Simple:
                    builder.Append(rawValue ?? $"#{field.Name}#");
                    break;
                case FieldType.Question:
                    renderChild = contition;
                    renderData = new[]{ data };
                    break;
                case FieldType.Negate:
                    renderChild = !contition;
                    renderData = new[]{ data };
                    break;
                case FieldType.Complex:
                    renderChild = true;
                    renderData = GetTreeData(rawValue);;
                    break;
            }

            if (renderChild)
            {
                foreach (var dataItem in renderData)
                {
                    var content = RenderChildren(field, dataItem);
                    builder.Append(content);
                }
            }

            return builder.ToString();
        }

        private static object GetRawValue(Dictionary<string, object> data, FieldNode fieldNode)
        {
            if (data.TryGetValue(fieldNode.Name, out var value))
            {
                return value;
            }

            return null;
        }

        private static IEnumerable<Dictionary<string, object>> GetTreeData(object value)
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
            else if (value is not null)
            {
                var empty = new Dictionary<string, object>
                {
                    { "", value },
                };
                result.Add(empty);
            }

            var pos = 0;

            foreach (var item in result)
            {
                pos++;
                item.TryAdd(".First", pos == 1);
                item.TryAdd(".Last", pos == result.Count);
            }

            return result;
        }

        private static bool GetCondition(object value)
        {
            if (value is bool contition)
            {
                return contition;
            }
            else if (value is double decNumber)
            {
                return decNumber > 0;
            }
            else if (value is long intNumber)
            {
                return intNumber > 0;
            }
            else if (value is string text)
            {
                return !string.IsNullOrEmpty(text);
            }
            else if (value is IEnumerable collection)
            {
                return collection.OfType<object>().Any();
            }

            return false;
        }
    }
}