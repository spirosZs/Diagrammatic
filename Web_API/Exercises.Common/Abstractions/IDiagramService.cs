using System.Collections.Generic;
using Exercises.Common.Diagram;
using Exercises.Data.DiagramDefinitions;

namespace Exercises.Common.Abstractions
{
    public interface IDiagramService
        : ICrudService<Data.Diagram, DiagramFilter, DiagramCreateDto>
    {
        ICollection<string> GetSuggestedPaths(Data.Diagram diagram);
        ICollection<string> GetSuggestedPaths(DiagramDefinition diagramDefinition);
    }
}
