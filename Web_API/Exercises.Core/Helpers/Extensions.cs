using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Exercises.Common.Diagram;
using Exercises.Data;
using Exercises.Data.DiagramDefinitions;

namespace Exercises.Core.Helpers
{
    public static class Extensions
    {
        public static Graph ToGraph(this Diagram diagram)
        {
            if (diagram == null || string.IsNullOrWhiteSpace(diagram.Definition))
                return new Graph();

            var definition = ParseDiagramDefinition(diagram.Definition);
            return definition.ToGraph();
        }

        private static readonly JsonSerializerOptions DataDeserializerOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true
        };

        public static T GetDataObject<T>(this Submission submission)
        {
            var data = submission.Data;
            return JsonSerializer.Deserialize<T>(data, DataDeserializerOptions);
        }

        private static DiagramDefinition ParseDiagramDefinition(string rawJson)
        {
            var trimmed = rawJson.TrimStart();
            if (trimmed.StartsWith("\""))
            {
                rawJson = JsonSerializer.Deserialize<string>(rawJson) ?? rawJson;
            }

            var nodes = new List<NodeDefinition>();
            var connectors = new List<ConnectorDefinition>();
            var map = new Dictionary<string, NodeDefinition>();

            using (var doc = JsonDocument.Parse(rawJson))
            {
                var root = doc.RootElement;

                if (root.TryGetProperty("nodes", out var nodesElement) &&
                    nodesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var n in nodesElement.EnumerateArray())
                    {
                        var id = n.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                            ? idProp.GetString() ?? Guid.NewGuid().ToString()
                            : Guid.NewGuid().ToString();

                        var number = 0;
                        if (n.TryGetProperty("annotations", out var annos) &&
                            annos.ValueKind == JsonValueKind.Array)
                        {
                            var firstAnno = annos.EnumerateArray().FirstOrDefault();
                            if (firstAnno.ValueKind == JsonValueKind.Object &&
                                firstAnno.TryGetProperty("content", out var contentProp) &&
                                contentProp.ValueKind == JsonValueKind.String)
                            {
                                int.TryParse(contentProp.GetString(), out number);
                            }
                        }

                        var node = new NodeDefinition
                        {
                            Id = id,
                            Number = number
                        };

                        nodes.Add(node);
                        map[id] = node;
                    }
                }

                if (root.TryGetProperty("connectors", out var connElement) &&
                    connElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var c in connElement.EnumerateArray())
                    {
                        var id = c.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                            ? idProp.GetString() ?? Guid.NewGuid().ToString()
                            : Guid.NewGuid().ToString();

                        var sourceId = c.TryGetProperty("sourceID", out var srcProp) && srcProp.ValueKind == JsonValueKind.String
                            ? srcProp.GetString()
                            : null;

                        var targetId = c.TryGetProperty("targetID", out var tgtProp) && tgtProp.ValueKind == JsonValueKind.String
                            ? tgtProp.GetString()
                            : null;

                        if (sourceId != null && map.TryGetValue(sourceId, out var sourceNode))
                            sourceNode.OutEdges.Add(id);

                        if (targetId != null && map.TryGetValue(targetId, out var targetNode))
                            targetNode.InEdges.Add(id);

                        connectors.Add(new ConnectorDefinition { Id = id });
                    }
                }
            }

            var definition = new DiagramDefinition();
            foreach (var node in nodes) definition.Nodes.Add(node);
            foreach (var connector in connectors) definition.Connectors.Add(connector);
            return definition;
        }
    }
}