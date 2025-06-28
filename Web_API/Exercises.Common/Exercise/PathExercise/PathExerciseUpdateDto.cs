using System.Collections.Generic;
using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class PathExerciseUpdateDto : ExerciseUpdateDto
    {
        /// <summary>
        /// A diagram definition for this Exercise.
        /// </summary>
        public DiagramUpdateDto Diagram { get; set; }

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