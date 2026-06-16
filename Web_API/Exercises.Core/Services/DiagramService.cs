using System.Collections.Generic;
using Exercises.Common.Abstractions;
using Exercises.Common.Diagram;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DbContext;
using Exercises.Data.DiagramDefinitions;
using Microsoft.AspNetCore.Http;

namespace Exercises.Core.Services
{
    public class DiagramService
        : ResourceBase<Diagram, DiagramFilter, DiagramCreateDto>, IDiagramService
    {
        public DiagramService(
            ExercisesContext context,
            IPropertyMappingService propertyMappingService,
            IHttpContextAccessor httpContextAccessor
        )
            : base(context, propertyMappingService, httpContextAccessor)
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
