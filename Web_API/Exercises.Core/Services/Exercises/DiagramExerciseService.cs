using Exercises.Common.Abstractions;
using Exercises.Common.Exercise;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;

namespace Exercises.Core.Services
{
    public class DiagramExerciseService
        : ResourceBase<DiagramExercise, ExerciseFilter, DiagramExerciseCreateDto>, IDiagramExerciseService
    {
        public DiagramExerciseService(ExercisesContext context, IPropertyMappingService propertyMappingService,
            IHttpContextAccessor httpContextAccessor)
            : base(context, propertyMappingService, httpContextAccessor)
        {
        }


        protected override void OnBeforeCreate(DiagramExercise diagramExercise)
        {
            base.OnBeforeCreate(diagramExercise);
            diagramExercise.StampOwnedDiagram(diagramExercise.Diagram);
            _context
                .AddExerciseToReferencedCollection(diagramExercise);
        }
    }
}