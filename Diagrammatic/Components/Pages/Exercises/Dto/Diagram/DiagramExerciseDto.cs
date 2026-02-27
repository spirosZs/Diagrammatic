namespace Diagrammatic_test.Components.Pages.Exercises.Dto.Diagram
{
    public class DiagramExerciseDto : ExerciseDto
    {
        /// <summary>
        /// The diagram solution for this Exercise.
        /// </summary>
        public DiagramDto Diagram { get; set; }

        /// <summary>
        /// The code that will be presented to the students when asked to find the correct diagram.
        /// </summary>
        public string Code { get; set; }
    }
}
