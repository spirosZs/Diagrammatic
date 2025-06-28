using System;

namespace Exercises.Common.Exercise
{
    public class ExerciseGameDto : EntityDtoBase
    {
        /// <summary>
        /// The problem type identifies the type of the exercise.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The id of the exercise collection that contains this exercise.
        /// </summary>
        public Guid? ExerciseCollectionId { get; set; }

        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
    }
}