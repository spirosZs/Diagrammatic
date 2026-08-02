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

                if (TryGetPropertyIgnoreCase(root, "nodes", out var nodesElement) &&
                    nodesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var n in nodesElement.EnumerateArray())
                    {
                        var id = TryGetPropertyIgnoreCase(n, "id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                            ? idProp.GetString() ?? Guid.NewGuid().ToString()
                            : Guid.NewGuid().ToString();

                        var number = 0;
                        if (TryGetPropertyIgnoreCase(n, "annotations", out var annos) &&
                            annos.ValueKind == JsonValueKind.Array)
                        {
                            var firstAnno = annos.EnumerateArray().FirstOrDefault();
                            if (firstAnno.ValueKind == JsonValueKind.Object &&
                                TryGetPropertyIgnoreCase(firstAnno, "content", out var contentProp) &&
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

                        // The diagram-editor upload flow normalizes a diagram before it's
                        // saved, recording each node's InEdges/OutEdges as connector-id
                        // arrays directly on the node. Read them here when present;
                        // legacy diagrams (e.g. seed data) that don't have them yet fall
                        // through to the sourceID/targetID-based derivation below.
                        if (TryGetPropertyIgnoreCase(n, "inEdges", out var inEdgesEl) &&
                            inEdgesEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var edge in inEdgesEl.EnumerateArray())
                                if (edge.ValueKind == JsonValueKind.String)
                                    node.InEdges.Add(edge.GetString());
                        }

                        if (TryGetPropertyIgnoreCase(n, "outEdges", out var outEdgesEl) &&
                            outEdgesEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var edge in outEdgesEl.EnumerateArray())
                                if (edge.ValueKind == JsonValueKind.String)
                                    node.OutEdges.Add(edge.GetString());
                        }

                        nodes.Add(node);
                        map[id] = node;
                    }
                }

                if (TryGetPropertyIgnoreCase(root, "connectors", out var connElement) &&
                    connElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var c in connElement.EnumerateArray())
                    {
                        var id = TryGetPropertyIgnoreCase(c, "id", out var idProp) && idProp.ValueKind == JsonValueKind.String
                            ? idProp.GetString() ?? Guid.NewGuid().ToString()
                            : Guid.NewGuid().ToString();

                        // Legacy raw diagrams record edges on the connector (sourceID/
                        // targetID) instead of on the nodes. Only used as a fallback —
                        // normalized diagrams already wired their nodes up above.
                        if (TryGetPropertyIgnoreCase(c, "sourceID", out var srcProp) &&
                            srcProp.ValueKind == JsonValueKind.String &&
                            TryGetPropertyIgnoreCase(c, "targetID", out var tgtProp) &&
                            tgtProp.ValueKind == JsonValueKind.String)
                        {
                            var sourceId = srcProp.GetString();
                            var targetId = tgtProp.GetString();

                            if (sourceId != null && map.TryGetValue(sourceId, out var sourceNode) &&
                                !sourceNode.OutEdges.Contains(id))
                                sourceNode.OutEdges.Add(id);

                            if (targetId != null && map.TryGetValue(targetId, out var targetNode) &&
                                !targetNode.InEdges.Contains(id))
                                targetNode.InEdges.Add(id);
                        }

                        connectors.Add(new ConnectorDefinition { Id = id });
                    }
                }
            }

            var definition = new DiagramDefinition();
            foreach (var node in nodes) definition.Nodes.Add(node);
            foreach (var connector in connectors) definition.Connectors.Add(connector);
            return definition;
        }

        private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = prop.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }
    }
}