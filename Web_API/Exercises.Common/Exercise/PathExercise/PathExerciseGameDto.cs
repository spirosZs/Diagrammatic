using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class PathExerciseGameDto : ExerciseGameDto
    {
        /// <summary>
        /// A diagram definition for this Exercise.
        /// </summary>
        public DiagramDto Diagram { get; set; }
        
    }
}