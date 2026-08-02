using System;
using Exercises.Common.User;

namespace Exercises.Common.Submission
{
    public class SubmissionDto : EntityDtoBase
    {
        /// <summary>
        /// The Exam id reference for this submission.
        /// </summary>
        public Guid ExamId { get; set; }

        /// <summary>
        /// The Exercise id reference for this submission.
        /// </summary>
        public Guid ExerciseId { get; set; }

        /// <summary>
        /// The contents of the submission data.
        /// </summary>
        public object Data { get; set; }
        
        /// <summary>
        /// The score on this exercise.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The user that made this submission.
        /// </summary>
        public UserDtoBase User { get; set; }
    }
}