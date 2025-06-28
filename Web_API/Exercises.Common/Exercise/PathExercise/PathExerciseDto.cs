using System.Collections.Generic;
using Exercises.Common.Diagram;

namespace Exercises.Common.Exercise
{
    public class PathExerciseDto : ExerciseDto
    {
        /// <summary>
        /// A diagram definition for this Exercise.
        /// </summary>
        public DiagramDto Diagram { get; set; }
        
        /// <summary>
        /// The correct paths solution for this Exercise as specified by the teacher.
        /// </summary>
        public ICollection<PathDto> Paths { get; set; } = new List<PathDto>();
    }
}