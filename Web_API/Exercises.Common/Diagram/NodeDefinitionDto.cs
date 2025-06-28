using System.Collections.Generic;

namespace Exercises.Common.Diagram
{
    public class NodeDefinitionDto
    {
        public string Id { get; set; }
        
        public ICollection<NodeAnnotationDto> Annotations = new List<NodeAnnotationDto>();
        
        public ICollection<string> InEdges { get; set; } = new List<string>();
        public ICollection<string> OutEdges { get; set; } = new List<string>();
    }
}