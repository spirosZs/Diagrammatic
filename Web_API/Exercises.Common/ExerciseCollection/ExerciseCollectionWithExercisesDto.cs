using System.Collections.Generic;
using Exercises.Common.Exercise;

namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionWithExercisesDto : EntityDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
        
        /// <summary>
        /// A list with all the exercises inside this Exercise Collection.
        /// </summary> 
        public ICollection<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();

        /// <summary>
        /// Total time for this Exercise Collection.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// Total number of exercises.
        /// </summary>
        public int TotalExercises { get; set; }
    }
}