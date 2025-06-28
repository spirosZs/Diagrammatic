using System.Collections.Generic;

namespace Exercises.Data.DiagramDefinitions
{
    public class DiagramDefinition
    {
        public ICollection<NodeDefinition> Nodes = new List<NodeDefinition>(); 
        public ICollection<ConnectorDefinition> Connectors = new List<ConnectorDefinition>(); 
    }
}