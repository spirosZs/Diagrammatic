using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.Abstractions;
using Exercises.Common.ExerciseCollection;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Core.Services
{
    public class ExerciseCollectionService
        : ResourceBase<ExerciseCollection, ExerciseCollectionFilter, ExerciseCollectionCreateDto>,
            IExerciseCollectionService
    {
        public ExerciseCollectionService(ExercisesContext context, IPropertyMappingService propertyMappingService, IHttpContextAccessor httpContextAccessor)
            : base(context, propertyMappingService, httpContextAccessor)
        {
        }

        protected override void OnGetSingle(ref IQueryable<ExerciseCollection> query)
        {
            base.OnGetSingle(ref query);
            query = query
                .IsNotExam()
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram));
        }

        protected override void OnGet(ExerciseCollectionFilter filter, ref IQueryable<ExerciseCollection> query)
        {
            base.OnGet(filter, ref query);
            query = query
                .IsNotExam()
                .Include(c => c.Exercises);

            if (filter.Category != null)
            {
                query = query
                    .Where(d => d.Category.StartsWith(filter.Category));
            }
        }

        public async Task<ExerciseCollection> AddExerciseToCollection(Guid id, Guid exerciseId,
            CancellationToken token = default)
        {
            var collection = await _dbset.GetExerciseCollectionById(id, token);
            if (collection == null)
            {
                throw new Exception($"exercise collection with id {id} doesn't exist.");
            }
            
            var exercise = await _context.Exercises.FindAsync(exerciseId);

            if (exercise == null)
            {
                throw new Exception($"exercise with id {exerciseId} doesn't exist.");
            }

            if (collection.Exercises.Any(e => e.Id == exerciseId))
            {
                return collection;
            }

            collection.Exercises.Add(exercise);
            await _context.SaveChangesAsync(token);
            return collection;
        }

        public async Task<ExerciseCollection> RemoveExerciseFromCollection(Guid id, Guid exerciseId,
            CancellationToken token = default)
        {
            var collection = await _dbset.GetExerciseCollectionById(id, token);
            if (collection == null)
            {
                throw new Exception($"exercise collection with id {id} doesn't exist.");
            }
            
            var exercise = await _context.Exercises.FindAsync(exerciseId);
            
            if (exercise == null)
            {
                throw new Exception($"exercise with id {exerciseId} doesn't exist.");
            }

            if (collection.Exercises.Any(e => e.Id == exerciseId))
            {
                collection.Exercises.Remove(exercise);
                await _context.SaveChangesAsync(token);
                return collection;
            }

            return collection;
        }
    }
}