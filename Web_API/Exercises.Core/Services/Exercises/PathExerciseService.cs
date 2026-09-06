using Exercises.Common.Abstractions;
using Exercises.Common.Exercise;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;

namespace Exercises.Core.Services
{
    public class PathExerciseService
        : ResourceBase<PathExercise, ExerciseFilter, PathExerciseCreateDto>, IPathExerciseService
    {
        public PathExerciseService(ExercisesContext context, IPropertyMappingService propertyMappingService, IHttpContextAccessor httpContextAccessor)
            : base(context, propertyMappingService, httpContextAccessor)
        {
        }

        protected override void OnBeforeCreate(PathExercise resource)
        {
            base.OnBeforeCreate(resource);
            resource.StampOwnedDiagram(resource.Diagram);
            _context.AddExerciseToReferencedCollection(resource);
        }
    }
}