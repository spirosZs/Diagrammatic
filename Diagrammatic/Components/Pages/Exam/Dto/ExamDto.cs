using System.Collections.Generic;
using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Exam.Dto
{
    public class ExamDto : EntityDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }

        /// <summary>
        /// Date the game started, null if not started yet.
        /// </summary>
        public DateTime? DateStarted { get; set; }

        /// <summary>
        /// Date the game ended, null if not ended yet.
        /// </summary>
        public DateTime? DateEnded { get; set; }

        /// <summary>
        /// The code the students have to enter in order to participate in a game.
        /// </summary>
        public string ParticipationCode { get; set; }

        /// <summary>
        /// Exam participants.
        /// </summary>
        public ExamParticipantsDto Participants { get; set; }

        /// <summary>
        /// Total time for this exam in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// Total number of exercises.
        /// </summary>
        public int TotalExercises { get; set; }
    }
}
