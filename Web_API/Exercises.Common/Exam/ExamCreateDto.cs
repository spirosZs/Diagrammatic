using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exercises.Common.Exercise;

namespace Exercises.Common.Exam
{
    /// <summary>
    /// An object to g
    /// </summary>
    public class ExamCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// The exercise collection Guid to create an exam from
        /// </summary>
        [Required] 
        public Guid ExerciseCollectionId { get; set; }
        
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
    }
}