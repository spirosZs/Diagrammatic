using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class DiagramExerciseUpdateDto : ExerciseUpdateDto
    {
        /// <summary>
        /// The diagram solution for this Exercise.
        /// </summary>
        public DiagramUpdateDto Diagram { get; set; }

        /// <summary>
        /// The code that will be presented to the students when asked to find the correct diagram.
        /// </summary>
        public string Code { get; set; }
    }
}