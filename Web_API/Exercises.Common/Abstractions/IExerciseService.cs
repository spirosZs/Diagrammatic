using Exercises.Common.Exercise;

namespace Exercises.Common.Abstractions
{
    public interface IExerciseService : ICrudService<Data.Exercise, ExerciseFilter, ExerciseCreateDto>
    { }
}