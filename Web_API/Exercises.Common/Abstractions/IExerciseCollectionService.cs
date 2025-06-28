using System;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.ExerciseCollection;

namespace Exercises.Common.Abstractions
{
    public interface IExerciseCollectionService
        : ICrudService<Data.ExerciseCollection, ExerciseCollectionFilter, ExerciseCollectionCreateDto>
    {
        Task<Data.ExerciseCollection> AddExerciseToCollection(
            Guid id,
            Guid exerciseId,
            CancellationToken token = default
        );

        Task<Data.ExerciseCollection> RemoveExerciseFromCollection(
            Guid id,
            Guid exerciseId,
            CancellationToken token = default
        );
    }
}