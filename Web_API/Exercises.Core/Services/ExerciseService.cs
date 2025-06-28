using System.Linq;
using Exercises.Common.Abstractions;
using Exercises.Common.Exercise;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Core.Services
{
    public class ExerciseService
        : ResourceBase<Exercise, ExerciseFilter, ExerciseCreateDto>, IExerciseService
    {
        public ExerciseService(ExercisesContext context, IPropertyMappingService propertyMappingService,
            IHttpContextAccessor httpContextAccessor)
            : base(context, propertyMappingService, httpContextAccessor)
        {
        }

        protected override void OnGetSingle(ref IQueryable<Exercise> query)
        {
            base.OnGetSingle(ref query);
            query = query
                .Include(exercise => ((exercise as PathExercise).Diagram))
                .Include(exercise => ((exercise as DiagramExercise).Diagram));
        }

        protected override void OnGet(ExerciseFilter filter, ref IQueryable<Exercise> query)
        {
            query = query
                .Include(exercise => ((exercise as PathExercise).Diagram))
                .Include(exercise => ((exercise as DiagramExercise).Diagram));
        }

        protected override void OnBeforeCreate(Exercise resource)
        {
            base.OnBeforeCreate(resource);
            if (resource.TimeToComplete == 0)
            {
                resource.TimeToComplete = Constants.DEFAULT_TIME_TO_COMPLETE_EXERCISE;
            }
        }
    }
}