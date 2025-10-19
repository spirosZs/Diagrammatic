namespace Diagrammatic_test.Components.Pages.Exercises.Dto.DiagramDto
{
    public class DiagramExerciseCreateDto : ExerciseCreateDto
    {
        /// <summary>
        /// The diagram solution for this Exercise.
        /// </summary>
        //public DiagramDto Diagram { get; set; }

        /// <summary>
        /// A url string of an image for this diagram.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A json definition of this diagram.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// The code that will be presented to the students when asked to find the correct diagram.
        /// </summary>
        public string Code { get; set; }
    }
}
