using System.Collections.Generic;
using Exercises.Common.Abstractions;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DiagramDefinitions;

namespace Exercises.Core.Services
{
    public class DiagramService : IDiagramService
    {
        public DiagramService()
        {
        }

        public ICollection<string> GetSuggestedPaths(Diagram diagram)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<string> GetSuggestedPaths(DiagramDefinition diagramDefinition)
        {
            return diagramDefinition
                .ToGraph()
                .GetAllPaths();
        }
    }
}