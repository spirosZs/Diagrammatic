using System;

namespace Exercises.Common.Exam.Game
{
    public class UpdateGameDto  
    {
        /// <summary>
        /// Set this flag to true in the request body to update the participation code for this game.
        /// </summary>
        public bool ResetParticipationCodeFlag { get; set; }

        /// <summary>
        /// Set this flag to true in the request body to restart this game.
        /// Participations will not be removed.
        /// </summary>
        public bool RestartGameFlag { get; set; }
    }
}