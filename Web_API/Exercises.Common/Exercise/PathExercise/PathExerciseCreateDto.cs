using System.Collections.Generic;
using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class PathExerciseCreateDto : ExerciseCreateDto
    {
        /// <summary>
        /// A diagram definition for this Exercise.
        /// </summary>
        public DiagramCreateDto Diagram { get; set; }
        
        /// <summary>
        /// The correct paths solution for this Exercise as specified by the teacher.
        /// </summary>
        public ICollection<PathDto> Paths { get; set; } = new List<PathDto>();
        
        /// <summary>
        /// The code that will be presented to the students when asked to find the correct paths.
        /// </summary>
        public string Code { get; set; }
    }
}