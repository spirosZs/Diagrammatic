using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Game.Dto
{
    public class GameInfoDto : EntityDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }

        /// <summary>
        /// A boolean value representing if the game has started.
        /// </summary>
        public bool HasStarted { get; set; }

        /// <summary>
        /// A boolean value representing if the game has ended.
        /// </summary>
        public bool HasEnded { get; set; }

        /// <summary>
        /// Date the game started.
        /// </summary>
        public DateTime? DateStarted { get; set; }

        /// <summary>
        /// Date the game ended.
        /// </summary>
        public DateTime? DateEnded { get; set; }

        /// <summary>
        /// Total number of participants.
        /// </summary>
        public int Participants { get; set; }

        /// <summary>
        /// Total number of exercises.
        /// </summary>
        public int TotalExercises { get; set; }

        /// <summary>
        /// Date the game will end.
        /// </summary>
        public DateTime? DateTimeToEnd { get; set; }

        /// <summary>
        /// Date the game will proceed to the next exercise.
        /// </summary>
        public DateTime? DateTimeToNextExercise { get; set; }
    }
}
