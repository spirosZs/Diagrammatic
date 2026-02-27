namespace Diagrammatic_test.Components.Shared.Dto
{
    public class DiagramDefinitionDto
    {
        public ICollection<NodeDefinitionDto> Nodes { get; set; } = new List<NodeDefinitionDto>();
        public ICollection<ConnectorDefinitionDto> Connectors { get; set; } = new List<ConnectorDefinitionDto>();
    }
}
