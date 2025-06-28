using Exercises.Common.Exercise;

namespace Exercises.Common.Abstractions
{
    public interface IDiagramExerciseService : ICrudService<Data.DiagramExercise, ExerciseFilter, DiagramExerciseCreateDto>
    { }
}