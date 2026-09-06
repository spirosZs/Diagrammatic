using System;

namespace Exercises.Common.Exam.Game
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

        /// <summary>
        /// The exercise this info describes. Clients that read the deadline here and the
        /// exercise from GET /api/game/{id}/exercise must check the two agree — the round
        /// can advance between the two requests, and a deadline paired with the wrong
        /// exercise reads as "already expired".
        /// </summary>
        public Guid? CurrentExerciseId { get; set; }

        /// <summary>
        /// Zero-based position of the current round, or -1 when no round is active.
        /// </summary>
        public int CurrentExerciseIndex { get; set; }

        /// <summary>
        /// True when the current round is the last one of the exam, so its expiry ends
        /// the game instead of advancing to another exercise. Clients should prefer this
        /// over comparing DateTimeToNextExercise with DateTimeToEnd.
        /// </summary>
        public bool IsOnLastExercise { get; set; }
    }
}
