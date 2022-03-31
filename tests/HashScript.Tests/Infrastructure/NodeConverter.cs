using HashScript.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace HashScript.Tests.Infrastructure
{
    internal class NodeConverter : JsonConverter<Node>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(Node) == type;
        }

        public override void Write(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override Node Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out var doc))
            {
                throw new JsonException();
            }

            var root = doc.RootElement;
            var nodeType = NodeType.None;

            if (root.TryGetProperty("NodeType", out var nodeTypeElement))
            {
                var nodeTypeName = nodeTypeElement.GetString();
                _ = Enum.TryParse(nodeTypeName, out nodeType);
            }

            var type = nodeType switch
            {
                NodeType.Document => typeof(DocumentNode),
                NodeType.Field => typeof(FieldNode),
                NodeType.Text => typeof(TextNode),
                _ => throw new JsonException()
            };

            var json = root.GetRawText();
            var node = JsonSerializer.Deserialize(json, type, options) as Node;

            return node;
        }
    }
}
