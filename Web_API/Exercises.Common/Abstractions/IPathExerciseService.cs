using Exercises.Common.Exercise;

namespace Exercises.Common.Abstractions
{
    public interface IPathExerciseService : ICrudService<Data.PathExercise, ExerciseFilter, PathExerciseCreateDto>
    { }
}