namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionDto : EntityDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string Description { get; set; }

        /// <summary>
        /// Total time for the exercises of this collection in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// Total number of exercises.
        /// </summary>
        public int TotalExercises { get; set; }
    }
}