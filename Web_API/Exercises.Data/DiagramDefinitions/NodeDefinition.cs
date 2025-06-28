using System.Collections.Generic;

namespace Exercises.Data.DiagramDefinitions
{
    public class NodeDefinition
    {
        public string Id { get; set; }
        public int Number { get; set; }
        
        public ICollection<string> InEdges { get; set; } = new List<string>();
        
        public ICollection<string> OutEdges { get; set; } = new List<string>();
    }
}