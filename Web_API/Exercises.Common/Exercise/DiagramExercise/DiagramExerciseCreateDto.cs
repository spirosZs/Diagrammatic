using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class DiagramExerciseCreateDto : ExerciseCreateDto
    {
        /// <summary>
        /// The diagram solution for this Exercise.
        /// </summary>
        public DiagramCreateDto Diagram { get; set; }
        
        /// <summary>
        /// The code that will be presented to the students when asked to find the correct diagram.
        /// </summary>
        public string Code { get; set; }
    }
}