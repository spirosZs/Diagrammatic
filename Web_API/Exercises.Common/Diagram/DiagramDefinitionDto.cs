using System.Collections.Generic;

namespace Exercises.Common.Diagram
{
    public class DiagramDefinitionDto
    {
        public ICollection<NodeDefinitionDto> Nodes = new List<NodeDefinitionDto>(); 
        public ICollection<ConnectorDefinitionDto> Connectors = new List<ConnectorDefinitionDto>(); 
    }
}