using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exercises.Common.Exercise;
using Exercises.Data;

namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionExercisesOperationDto
    {
        /// <summary>
        /// The operation to perform.
        /// </summary>
        [Required]
        public ExerciseCollectionOperation Operation { get; set; }
        
        /// <summary>
        /// The Exercise id to either add or remove to this Exercise Collection.
        /// </summary>
        [Required]
        public Guid ExerciseId {get; set;}
    }
}