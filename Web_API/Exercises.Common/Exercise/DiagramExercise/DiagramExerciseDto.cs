using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class DiagramExerciseDto : ExerciseDto
    {        
        /// <summary>
        /// The code that will be presented to the students when asked to find the correct diagram.
        /// </summary>
        public string Code { get; set; }
    }
}