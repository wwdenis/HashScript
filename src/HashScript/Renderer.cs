using System;
using System.Collections;
using System.Linq;
using System.Text;
using HashScript.Nodes;
using HashScript.Providers;

namespace HashScript
{
    public sealed class Renderer
    {
        readonly string template;

        public Renderer(string template)
        {
            this.template = template;
        }

        public string Generate(IValueProvider data)
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

        private static string RenderChildren(Node parent, IValueProvider data)
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

        private static string RenderField(FieldNode field, IValueProvider data)
        {
            var builder = new StringBuilder();

            if (field.IsValue)
            {
                return $"{data.GetValue()}";
            }

            var rawValue = data.GetValue(field.Name);
            var contition = GetCondition(field, data);

            var renderChild = false;
            var renderData = Enumerable.Empty<IValueProvider>();

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
                    renderData = data.GetChildren(field.Name);
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

        private static bool GetCondition(FieldNode field, IValueProvider data)
        {
            if (field.IsFunction)
            {
                return data.Functions.Contains(field.Name);
            }

            var value = data.GetValue(field.Name);

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