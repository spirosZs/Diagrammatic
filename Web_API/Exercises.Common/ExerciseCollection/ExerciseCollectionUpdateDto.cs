namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionUpdateDto : EntityUpdateDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string Description { get; set; }
        
    }
}