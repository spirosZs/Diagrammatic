using Diagrammatic_test.Components.Pages.Exercises.Dto.Diagram;

namespace Diagrammatic_test.Components.Pages.Exercises.Dto.Path
{
    public class PathExerciseCreateDto : ExerciseCreateDto
    {
        /// <summary>
        /// A diagram definition for this Exercise.
        /// </summary>
        public DiagramExerciseCreateDto Diagram { get; set; }

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
