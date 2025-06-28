using System;
using System.ComponentModel.DataAnnotations;

namespace Exercises.Common.Submission
{
    public class SubmissionCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// The Exam id reference for this submission.
        /// </summary>
        [Required]
        public Guid ExamId { get; set; }

        /// <summary>
        /// The Exercise id reference for this submission.
        /// </summary>
        [Required]
        public Guid ExerciseId { get; set; }

        /// <summary>
        /// The contents of the submission data.
        /// </summary>
        [Required]
        public object Data { get; set; }
    }
}