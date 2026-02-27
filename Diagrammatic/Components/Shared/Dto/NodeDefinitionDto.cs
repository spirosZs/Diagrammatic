namespace Diagrammatic_test.Components.Shared.Dto
{
    public class NodeDefinitionDto
    {
        public string Id { get; set; } = string.Empty;

        public ICollection<NodeAnnotationDto> Annotations { get; set; } = new List<NodeAnnotationDto>();

        public ICollection<string> InEdges { get; set; } = new List<string>();
        public ICollection<string> OutEdges { get; set; } = new List<string>();
    }
}
