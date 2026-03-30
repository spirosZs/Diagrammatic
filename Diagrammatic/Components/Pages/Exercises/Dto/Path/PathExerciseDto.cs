using Diagrammatic_test.Components.Pages.Exercises.Dto.Diagram;

namespace Diagrammatic_test.Components.Pages.Exercises.Dto.Path
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
