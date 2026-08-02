using Diagrammatic_test.Components.Pages.Exercises.Dto.Diagram;
using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Game.Dto
{
    /// <summary>
    /// Response of GET /api/game/{id}/exercise — the exercise for the game's current round.
    /// </summary>
    public class GameExerciseDto : EntityDtoBase
    {
        /// <summary>
        /// The problem type identifies the type of the exercise.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The weight attribute is used to sort exercises inside an Exercise Collection.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// The time in which a student has to complete the exercise expressed in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// A string representing a category label for this exercise.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// If the exercise is a Diagram exercise, its solution diagram.
        /// </summary>
        public DiagramDto Diagram { get; set; }

        public Guid? DiagramId { get; set; }
    }
}
